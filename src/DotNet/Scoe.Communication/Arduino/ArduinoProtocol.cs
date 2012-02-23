using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO.Ports;
using System.Threading;
using System.IO;

namespace Scoe.Communication.Arduino
{
    public class ArduinoProtocol : Protocol
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

        public override void Transmit(Packet packet)
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

        private bool ParseData()
        {
            while (serialPort.BytesToRead > 0)
            {
                var thisByte = (byte)serialPort.ReadByte();

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
                        _receiveBuffer[_receiveBufferOffset] = thisByte;
                        if (_receiveBufferOffset == _length)
                            Transmit(new Packet(_receiveBuffer));
                    }
                    else
                    {
                        _isWaiting = true;
                    }
                }

                if (_lastByte == (byte)SpecialChars.Command && thisByte == (byte)SpecialChars.NewPacket)
                {
                    _isWaiting = false;
                }

                _lastByte = thisByte;
            }

            return false;
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
    }
}
