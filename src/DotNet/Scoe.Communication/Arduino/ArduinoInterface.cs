using System;
using System.Collections.Generic;
using System.Linq;
using System.IO.Ports;
using System.Threading;
using System.Collections.ObjectModel;
using System.Diagnostics;
using Scoe.Shared.Interface;
using System.IO;

namespace Scoe.Communication.Arduino
{
    public class ArduinoInterface : CardInterfaceBase, IDisposable, IInterface
    {
        private uint _packetCrc;
        private int _packetDataLength;
        private Byte[] _contentData;
        public SerialPort serialPort;
        System.Timers.Timer transmitTimer;
        bool isEnabled = false;
        bool isStopped = false;
        Boolean isWriting = false;
        object isWritingSemaphore = new object();
        Thread readThread;

        bool _isWaiting = true;
        byte _lastByte = 0;
        byte[] _receiveBuffer = new byte[100];
        List<byte> _debugBuffer = new List<byte>();
        int _receiveBufferPosition = 0;

        public ArduinoInterface(string port, int baud = 9600, int interval = 20)
        {
            Sections = new ObservableCollection<DataSection>();
            serialPort = new SerialPort(port, baud);
            transmitTimer = new System.Timers.Timer(interval);
            transmitTimer.Elapsed += TransmitTimerElapsed;
        }

        public bool IsConnected
        {
            get
            {
                //TODO: IMplement me!
                return true;
            }
        }

        private ObservableCollection<DataSection> _Sections;
        public ObservableCollection<DataSection> Sections
        {
            get { return _Sections; }
            protected set
            {
                if (_Sections == value)
                    return;
                _Sections = value;
            }
        }

        public override void Start()
        {
            isEnabled = true;
            serialPort.DtrEnable = true;
            serialPort.Open();
            Thread.Sleep(5000);
            transmitTimer.Start();
            readThread = new Thread(ReadWorker);
            readThread.Name = "ArduinoInterafaceRead";
            readThread.Start();
        }

        public override void Stop()
        {
            transmitTimer.Stop();
            isEnabled = false;
            SpinWait.SpinUntil(() => isStopped, 500); //TODO:What is the timeout behavior? Exception?
            serialPort.Close();
        }

        private enum Mode
        {
            Normal = 0,
            Escape = 1
        }
        Mode mode;
        private bool ParseData()
        {
            while (serialPort.BytesToRead > 0)
            {
                var thisByte = (byte)serialPort.ReadByte();
                _debugBuffer.Add(thisByte);

                if (!_isWaiting)
                {
                    if (thisByte == (byte)SpecialChars.Escape && mode != Mode.Escape)
                    {
                        mode = Mode.Escape;
                    }
                    else if ((thisByte < 254 || mode == Mode.Escape) && _receiveBufferPosition < _receiveBuffer.Length - 6)
                    {
                        mode = Mode.Normal;
                        _receiveBuffer[_receiveBufferPosition++] = thisByte;
                        if (_receiveBufferPosition == 2)
                        {
                            _packetDataLength = BitConverter.ToUInt16(_receiveBuffer, 0);
                            if (_packetDataLength > (_receiveBuffer.Length - 6))
                            {
                                _isWaiting = true;
                            }
                        }
                        else if (_receiveBufferPosition == 6)
                        {
                            _packetCrc = BitConverter.ToUInt32(_receiveBuffer, 2);
                        }
                        else if (_receiveBufferPosition == _packetDataLength + 6)
                        {
                            _isWaiting = true;

                            var crcData = new byte[_packetDataLength];
                            Array.ConstrainedCopy(_receiveBuffer, 6, crcData, 0, _packetDataLength);
                            _contentData = crcData;
                            var calculatedCrc = Crc32.Compute(crcData);
                            if (calculatedCrc == _packetCrc)
                            {
                                return true;
                            }
                            else
                            {
                                Debug.WriteLine("Bad CRC");
                            }
                        }

                    }
                    else
                    {
                        _isWaiting = true;
                    }
                }

                if (_lastByte == (byte)SpecialChars.Command && thisByte == (byte)SpecialChars.NewPacket)
                {
                    _isWaiting = false;
                    _receiveBufferPosition = 0;

                    _debugBuffer.Clear();
                }

                _lastByte = thisByte;
            }

            return false;
        }

        private DataSectionData[] ProcessData()
        {
            byte sectionCount = _contentData[0];
            var sections = new DataSectionData[sectionCount];

            int index = 1;
            for (int i = 0; i < sectionCount; i++)
            {
                var sectionId = _contentData[index++];
                var sectionLength = BitConverter.ToUInt16(_contentData, index);
                index += 2;

                var sectionData = new byte[sectionLength];
                Array.ConstrainedCopy(_contentData, index, sectionData, 0, sectionLength);
                sections[i] = new DataSectionData() { SectionId = sectionId, Data = sectionData };
                index += sectionLength;
            }
            return sections;
        }

        public byte[] GetBytes()
        {
            using (var stream = new MemoryStream())
            using (var writer = new BinaryWriter(stream))
            {
                var content = GetContentData();
                var crc = Crc32.Compute(content);

                writer.Write((UInt16)content.Length);
                writer.Write(crc);
                writer.Write(content);

                return stream.ToArray();
            }
        }
        private byte[] GetContentData()
        {
            using (var stream = new MemoryStream())
            using (var writer = new BinaryWriter(stream))
            {
                writer.Write((byte)Sections.Count);

                foreach (var section in Sections)
                {
                    var sectionData = section.GetCommandData();
                    writer.Write(sectionData.SectionId);
                    var length = sectionData.Data != null ? (UInt16)sectionData.Data.Length : (UInt16)0;
                    writer.Write(length);
                    if (sectionData.Data != null)
                        writer.Write(sectionData.Data);
                }
                return stream.ToArray();
            }
        }

        private void TransmitTimerElapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            lock (isWritingSemaphore)
            {
                if (isWriting)
                    return;
                isWriting = true;
            }
            var data = GetBytes();
            Write(data, 0, data.Length);
            isWriting = false;
        }
        private void Write(byte[] data, int offset, int length)
        {
            var outData = new List<byte>();
            outData.AddRange(new byte[] { (byte)SpecialChars.Command, (byte)SpecialChars.NewPacket });

            for (int i = offset; i < length; i++)
            {
                if (data[i] >= 254)
                    outData.Add((byte)SpecialChars.Escape);
                outData.Add(data[i]);
            }
            serialPort.Write(outData.ToArray(), 0, outData.Count);
        }

        private void ReadWorker()
        {
            isStopped = false;
            while (isEnabled)
            {
                SpinWait.SpinUntil(() => serialPort.BytesToRead > 0 || !isEnabled);
                if (isEnabled)
                {
                    if (ParseData())
                    {
                        var sections = ProcessData();

                        foreach (var section in sections)
                        {
                            var modelSection = (from s in Sections where s.SectionId == section.SectionId select s).SingleOrDefault();
                            if (modelSection != null)
                            {
                                modelSection.ParseStatus(section);
                            }
                        }
                    }

                }
            }
            isStopped = true;
        }

        private enum SpecialChars : byte
        {
            Command = 0xff,
            Escape = 0xfe,
            NewPacket = 0xff
        }
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (serialPort != null)
                {
                    serialPort.Dispose();
                    serialPort = null;
                }
                if (transmitTimer != null)
                {
                    transmitTimer.Dispose();
                    transmitTimer = null;
                }
            }
        }
        ~ArduinoInterface()
        {
            Dispose(false);
        }
    }

}
