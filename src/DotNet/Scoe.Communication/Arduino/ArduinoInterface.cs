using System;
using System.Collections.Generic;
using System.Linq;
using System.IO.Ports;
using System.Threading;
using System.Collections.ObjectModel;
using System.Diagnostics;
using Scoe.Shared.Interface;

namespace Scoe.Communication.Arduino
{
    public class ArduinoInterface : CardInterfaceBase, IDisposable
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
        int _receiveBufferPosition = 0;

        public ArduinoInterface(string port, int baud = 9600, int interval = 20)
        {
            Sections = new ObservableCollection<DataSection>();
            serialPort = new SerialPort(port, baud);
            transmitTimer = new System.Timers.Timer(interval);
            transmitTimer.Elapsed += TransmitTimerElapsed;
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
            serialPort.Open();
            transmitTimer.Start();
            readThread = new Thread(ReadWorker);
            readThread.Name = "ArduinoInterafaceRea";
            readThread.Start();
        }

        public override void Stop()
        {
            transmitTimer.Stop();
            isEnabled = false;
            SpinWait.SpinUntil(() => isStopped, 500); //TODO:What is the timeout behavior? Exception?
            serialPort.Close();
        }

        private bool ParseData()
        {
            while (serialPort.BytesToRead > 0)
            {
                var thisByte = (byte)serialPort.ReadByte();
                if (!_isWaiting)
                {
                    if (thisByte == (byte)SpecialChars.Command && _lastByte != (byte)SpecialChars.Escape)
                    {
                        _isWaiting = true;
                    }
                    else if (thisByte == (byte)SpecialChars.Escape && _lastByte != (byte)SpecialChars.Escape)
                    {
                        //Wait until next loop
                    }
                    else if (_receiveBufferPosition < _receiveBuffer.Length - 6)
                    {
                        _receiveBuffer[_receiveBufferPosition++] = thisByte;
                        if (_receiveBufferPosition == 4)
                        {
                            _packetCrc = BitConverter.ToUInt32(_receiveBuffer, 0);
                        }
                        else if (_receiveBufferPosition == 6)
                        {
                            _packetDataLength = BitConverter.ToUInt16(_receiveBuffer, 4);
                            if (_packetDataLength > (_receiveBuffer.Length - 6))
                            {
                                _isWaiting = true;
                            }
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
                else if (_lastByte == (byte)SpecialChars.Command && thisByte == (byte)SpecialChars.NewPacket)
                {
                    _isWaiting = false;
                    _receiveBufferPosition = 0;
                }
                else
                {
                    Debug.Write((char)thisByte);
                }
                _lastByte = thisByte;
            }

            return false;
        }

        private void ProcessData()
        {
            byte sectionCount = _contentData[0];
            int index = 1;
            for (int i = 0; i < sectionCount; i++)
            {
                var sectionId = _contentData[index++];
                var sectionLength = BitConverter.ToUInt16(_contentData, index);
                index += 2;
                var modelSection = (from s in Sections where s.SectionId == sectionId select s).SingleOrDefault();
                if (modelSection != null)
                {
                    var sectionData = new byte[sectionLength];
                    Array.ConstrainedCopy(_contentData, index, sectionData, 0, sectionLength);
                    modelSection.Update(sectionData, 0);
                }
                else
                {
                    Debug.WriteLine("No sectionid #" + sectionId);
                }
                index += sectionLength;
            }
        }

        public byte[] GetBytes()
        {
            var data = new byte[50];
            int index = 6;
            data[index++] = (byte)Sections.Count;
            foreach (var section in Sections)
            {
                data[index++] = section.SectionId;
                index += 2;
                var start = index;

                section.GetData(ref data, ref index);

                //Write length
                var length = index - start;
                var bytes = BitConverter.GetBytes((ushort)length);
                Array.ConstrainedCopy(bytes, 0, data, start - 2, 2);
            }
            var outData = new byte[index];
            int dataLength = index - 6;
            var crcData = new byte[dataLength];
            Array.ConstrainedCopy(data, 6, crcData, 0, dataLength);
            Array.ConstrainedCopy(data, 0, outData, 0, index);
            var crc = Crc32.Compute(crcData);
            var crcBytes = BitConverter.GetBytes(crc);
            Array.ConstrainedCopy(crcBytes, 0, outData, 0, 4);
            var lengthBytes = BitConverter.GetBytes(dataLength);
            Array.ConstrainedCopy(lengthBytes, 0, outData, 4, 2);

            return outData;
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
                if (data[i] > 254)
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
                        ProcessData();

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