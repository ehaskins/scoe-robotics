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
            //serialPort.
        }

        public override void Stop()
        {
            IsEnabled = false;
            SpinWait.SpinUntil(() => isStopped, 500); //TODO:What is the timeout behavior? Exception?
            serialPort.Close();
        }



        int _length = 0;

        int _sizeBufferOffset = 0;
        byte[] _sizeBuffer = new byte[2];
        int _receiveBufferOffset = 0;
        byte[] _receiveBuffer;
        private bool ParseData()
        {
            while (serialPort.BytesToRead > 0)
            {
                var thisByte = (byte)serialPort.ReadByte();
                debugBuffer.Add(thisByte);

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
                    else if (_receiveBufferOffset < _length)
                    {
                        _receiveBuffer[_receiveBufferOffset++] = thisByte;
                        if (_receiveBufferOffset == _length)
                        {
                            DecodePacket(_receiveBuffer);
                        }
                    }
                    else
                    {
                        _isWaiting = true;
                    }
                }

                if (_lastByte == (byte)SpecialChars.Command && thisByte == (byte)SpecialChars.StartOfPacket)
                {
                    _sizeBufferOffset = 0;
                    _receiveBufferOffset = 0;
                    _isWaiting = false;
                }

                _lastByte = thisByte;
            }

            return false;
        }

        protected override void Write(byte[] data)
        {
            byte[] frame = GetFrame(data);
            serialPort.Write(frame, 0, frame.Length);
        }

        private static byte[] GetFrame(byte[] data)
        {
            MemoryStream stream = new MemoryStream();
            var writer = new BinaryWriter(stream);
            writer.Write(new byte[] { (byte)SpecialChars.Command, (byte)SpecialChars.StartOfPacket });
            var sizeBytes = BitConverter.GetBytes((ushort)data.Length);

            for (int i = 0; i < sizeBytes.Length; i++)
            {
                if (sizeBytes[i] >= 254)
                    writer.Write((byte)SpecialChars.Escape);
                writer.Write(sizeBytes[i]);
            }
            for (int i = 0; i < data.Length; i++)
            {
                if (data[i] >= 254)
                    writer.Write((byte)SpecialChars.Escape);
                writer.Write(data[i]);
            }
            writer.Write(new byte[] { (byte)SpecialChars.Command, (byte)SpecialChars.EndOfPacket });

            var transmitData = stream.ToArray();
            return transmitData;
        }

        private enum SpecialChars : byte
        {
            Command = 0xff,
            Escape = 0xfe,
            StartOfPacket = 0xff,
            EndOfPacket = 0xfe
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
