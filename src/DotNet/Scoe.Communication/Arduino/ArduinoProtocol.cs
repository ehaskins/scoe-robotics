using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO.Ports;
using System.Threading;
using System.IO;
using System.Diagnostics;

namespace Scoe.Communication.Arduino
{
    public class ArduinoProtocol : Protocol, IDisposable
    {
        public SerialPort serialPort;

        bool isStopped = false;
        Boolean isWriting = false;
        object isWritingSemaphore = new object();
        Thread readThread;

        bool _isWaiting = true;
        byte _lastByte = 0;

        public ArduinoProtocol(string port, int baud)
        {
            serialPort = new SerialPort(port, baud);
        }

        public override void Start()
        {
            IsEnabled = true;
            serialPort.Open();
            readThread = new Thread(ReadWorker);
            readThread.Name = "Serial read worker";
            readThread.Start();
        }

        public override void Stop()
        {
            IsEnabled = false;
            SpinWait.SpinUntil(() => isStopped, 500); //TODO:What is the timeout behavior? Exception?
            serialPort.Close();
        }

        public override void Transmit(PacketV3 packet)
        {
            lock (isWritingSemaphore)
            {
                if (isWriting)
                    return;
                isWriting = true;
            }
            var data = packet.GetData();
            Write(data, 0, data.Length);
            isWriting = false;
        }

        int _length = 0;

        int _sizeBufferOffset = 0;
        byte[] _sizeBuffer = new byte[2];
        int _receiveBufferOffset = 0;
        byte[] _receiveBuffer;
        List<byte> debugBuffer = new List<byte>();
        private bool ParseData()
        {
            while (serialPort.BytesToRead > 0)
            {
                var thisByte = (byte)serialPort.ReadByte();
                debugBuffer.Add(thisByte);
                //Debug.Write((char)thisByte);
                if (!_isWaiting)
                {
                    if (thisByte == (byte)SpecialChars.Escape && _lastByte != (byte)SpecialChars.Escape)
                    {
                        //Wait until next loop
                    }
                    else if (_sizeBufferOffset < 2)
                    {
                        _sizeBuffer[_sizeBufferOffset++] = thisByte;
                        if (_sizeBufferOffset == 2)
                        {
                            _length = BitConverter.ToUInt16(_sizeBuffer, 0);
                            _receiveBuffer = new byte[_length];
                        }
                    }
                    else if (_receiveBufferOffset < _length && _length > 12)
                    {
                        _receiveBuffer[_receiveBufferOffset++] = thisByte;
                        if (_receiveBufferOffset == _length)
                        {
                            try
                            {
                                Received(new PacketV3(_receiveBuffer));
                            }
                            catch (Exception ex)
                            {
                                Debug.WriteLine(ex.Message);

                                var builder = new StringBuilder();
                                for (int i = 9; i < _receiveBuffer.Length; i++)
                                {
                                    if (_receiveBuffer[i] != 0)
                                        builder.AppendFormat("testData[{0}] = {1};", i - 9, _receiveBuffer[i]);
                                }
                                Debug.WriteLine(builder.ToString());
                            }
                            finally
                            {
                                debugBuffer.Clear();
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
                    _sizeBufferOffset = 0;
                    _receiveBufferOffset = 0;
                    _isWaiting = false;
                }

                _lastByte = thisByte;
            }

            return false;
        }

        private void Write(byte[] data, int offset, int length)
        {
            var writer = new BinaryWriter(serialPort.BaseStream);
            writer.Write(new byte[] { (byte)SpecialChars.Command, (byte)SpecialChars.NewPacket });
            var sizeBytes = BitConverter.GetBytes((ushort)data.Length);
            for (int i = 0; i < sizeBytes.Length; i++)
            {
                if (sizeBytes[i] >= 254)
                    writer.Write((byte)SpecialChars.Escape);
                writer.Write(sizeBytes[i]);
            }
            for (int i = offset; i < length; i++)
            {
                if (data[i] >= 254)
                    writer.Write((byte)SpecialChars.Escape);
                writer.Write(data[i]);
            }
            writer.Flush();
        }

        private void ReadWorker()
        {
            isStopped = false;
            while (IsEnabled)
            {
                SpinWait.SpinUntil(() => serialPort.BytesToRead > 0 || !IsEnabled);
                if (IsEnabled)
                {
                    ParseData();
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
                if (serialPort != null)
                {
                    serialPort.Dispose();
                    serialPort = null;
                }
        }
        ~ArduinoProtocol()
        {
            Dispose(false);
        }
    }
}
