using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;

namespace Scoe.Communication.Udp
{
    public abstract class UdpInterface
    {

        public UdpInterface()
        {
            Sections = new ObservableCollection<UdpDataSection>();
        }

        private ObservableCollection<UdpDataSection> _Sections;
        public ObservableCollection<UdpDataSection> Sections
        {
            get { return _Sections; }
            protected set
            {
                if (_Sections == value)
                    return;
                _Sections = value;
            }
        }

        public abstract void Start();

        public abstract void Stop();

        public virtual void DataProcessed(IPEndPoint endPoint) { }

        private byte[] CheckCRC(byte[] data)
        {
            if (data.Length >= 6)
            {
                var length = BitConverter.ToUInt16(data, 0);
                if (data.Length == length + 6)
                {
                    var packetCrc = BitConverter.ToUInt32(data, 2);
                    var crcData = new byte[length];
                    Array.ConstrainedCopy(data, 6, crcData, 0, length);
                    var calculatedCrc = Crc32.Compute(crcData);
                    if (calculatedCrc == packetCrc)
                    {
                        return crcData;
                    }
                }
            }
            return null;
        }
        protected void ProcessData(byte[] data)
        {
            LastPacketIndex = BitConverter.ToUInt16(data, 0);
            byte sectionCount = data[2];
            int index = 3;
            for (int i = 0; i < sectionCount; i++)
            {
                var sectionId = data[index++];
                var sectionLength = BitConverter.ToUInt16(data, index);
                index += 2;
                var modelSection = (from s in Sections where s.SectionId == sectionId select s).SingleOrDefault();
                if (modelSection != null)
                {
                    var sectionData = new byte[sectionLength];
                    Array.ConstrainedCopy(data, index, sectionData, 0, sectionLength);
                    modelSection.Update(sectionData, 0);
                }
                else
                {
                    Debug.WriteLine("No sectionid #" + sectionId);
                }
                index += sectionLength;
            }
        }

        protected byte[] GetData(ushort packetIndex)
        {
            var data = new byte[50];
            int index = 6; //4 bytes crc + 2 bytes length
            BitConverter.GetBytes(packetIndex).CopyTo(data, index);
            index += 2;
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
            Array.ConstrainedCopy(crcBytes, 0, outData, 2, 4);
            var lengthBytes = BitConverter.GetBytes((ushort)dataLength);
            Array.ConstrainedCopy(lengthBytes, 0, outData, 0, 2);

            return outData;
        }

        IPEndPoint endpoint;
        protected void ReceiveDataSync()
        {
            _isStopped = false;
            while (IsEnabled)
            {
                try
                {
                    byte[] data = _client.Receive(ref endpoint);

                    var content = CheckCRC(data);
                    if (content != null)
                    {
                        ProcessData(content);
                        DataProcessed(endpoint);
                    }
                }
                catch (SocketException ex)
                {
                    if (ex.ErrorCode != 10054 && IsEnabled)
                        Stop();
                }
                catch
                {
                    if (IsEnabled)
                        Stop();
                }
            }
            _isStopped = true;
        }

        protected void SendData(IPEndPoint endPoint, ushort packetIndex)
        {
            var data = GetData(packetIndex);
            _client.Send(data, data.Length, endPoint);
        }

        public ushort LastPacketIndex { get; set; }
        protected bool _isStopped = true;
        protected bool _isEnabled = false;
        public bool IsEnabled
        {
            get
            {
                return _isEnabled;
            }
            set
            {
                if (_isEnabled == value)
                    return;
                _isEnabled = value;
                if (_isEnabled)
                    Start();
                else
                    Stop();
            }
        }
        bool _isConnected;
        public bool IsConnected
        {
            get
            {
                return _isConnected;
            }
            set
            {
                if (_isConnected == value)
                    return;
                foreach (var section in Sections)
                {
                    section.ConnectionStateChanged(this, value);
                }
                _isConnected = value;
            }
        }
        protected UdpClient _client;

    }

}
