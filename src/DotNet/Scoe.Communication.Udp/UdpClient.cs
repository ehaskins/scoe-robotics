using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Net;
using System.Net.Sockets;

namespace Scoe.Communication.Udp
{
    public class DSUdpClient : UdpInterface
    {
        System.Timers.Timer transmitTimer;
        ushort _lastPacketId;
        Thread _receieveThread;

        public DSUdpClient(IPEndPoint remoteEndpoint, ushort listenPort, int interval = 20)
        {
            RemoteEndpoint = remoteEndpoint;
            ListenPort = listenPort;
            transmitTimer = new System.Timers.Timer(interval);
            transmitTimer.Elapsed += transmitTimerElapsed;
        }

        public IPEndPoint RemoteEndpoint { get; set; }
        public int ListenPort { get; set; }
        public int DestinationPot { get; set; }

        public override void Start()
        {
            _isEnabled = true;
            _client = new UdpClient(ListenPort);

            _receieveThread = new Thread((ThreadStart)this.ReceiveDataSync);
            _receieveThread.Name = "UDP receive thread";
            _receieveThread.Start();

            transmitTimer.Start();
        }
        public override void Stop()
        {
            _isEnabled = false;
            if (_client != null)
                _client.Close();
            _client = null;
            SpinWait.SpinUntil(() => _isStopped, 100);
        }


        private void transmitTimerElapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            SendData(RemoteEndpoint);
        }
    }
}
