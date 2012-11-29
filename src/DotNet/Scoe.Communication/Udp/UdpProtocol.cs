using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Diagnostics;

namespace Scoe.Communication.Udp
{
    public class UdpProtocol : Protocol, IDisposable
    {
        private IPAddress _RemoteAddress;
        protected UdpClient _client;
        protected bool _isStopped = true;
        IPEndPoint endpoint;
        Thread _receieveThread;

        public UdpProtocol(ushort listenPort, ushort remotePort) : this(listenPort, remotePort, null) { }
        public UdpProtocol(ushort listenPort, ushort remotePort, IPAddress remoteAddress)
        {
            RemoteAddress = remoteAddress;
            DestinationPort = remotePort;
            ListenPort = listenPort;
        }

        public IPAddress RemoteAddress
        {
            get
            {
                return _RemoteAddress;
            }
            set
            {
                if (_RemoteAddress == value)
                    return;
                _RemoteAddress = value;
            }
        }
        public int ListenPort { get; set; }
        public int DestinationPort { get; set; }


        public override void Start()
        {
            IsEnabled = true;
            _client = new UdpClient(ListenPort);

            _receieveThread = new Thread((ThreadStart)this.ReceiveDataSync) { Name = "UDP receive thread, v3" };
            _receieveThread.Start();
        }
        public override void Stop()
        {
            IsEnabled = false;
            if (_client != null)
                _client.Close();
            _client = null;
            SpinWait.SpinUntil(() => _isStopped, 100);
        }

        protected override void Write(byte[] data)
        {
            try
            {
                _client.Send(data, data.Length, new IPEndPoint(RemoteAddress, DestinationPort));
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }

        protected void ReceiveDataSync()
        {
            _isStopped = false;
            while (IsEnabled)
            {
                byte[] data;
                try
                { 
                    data = _client.Receive(ref endpoint);
                    if (RemoteAddress == null)
                        RemoteAddress = endpoint.Address;
                    var packet = new PacketV4();
                    packet.Parse(data);
                    Received(packet);
                }
                catch (InvalidOperationException opex)
                {
                    //Bad CRC
                    Debug.WriteLine("Bad CRC");
                }
                catch (SocketException ex)
                {
                    if (ex.ErrorCode != 10054 && IsEnabled)
                        break;
                }
                catch (Exception e)
                {
                    if (IsEnabled)
                        Stop();
                }
            }
            _isStopped = true;
            if (IsEnabled)
                Stop();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
                if (_client != null)
                {
                    _client.Close();
                    _client = null;
                }
        }
        ~UdpProtocol()
        {
            Dispose(false);
        }
    }
}
