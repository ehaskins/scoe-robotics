using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Collections.ObjectModel;
using System.Net;
using System.Net.Sockets;

namespace Scoe.Communication.Udp
{
    public class UdpServer : UdpInterface
    {
        System.Timers.Timer safteyTimer;
        Thread _receieveThread;

        public UdpServer(ushort listenPort, ushort destinationPort, int safteyTimeout = 100)
        {
            ListenPort = listenPort;
            DestinationPot = destinationPort;
            safteyTimer = new System.Timers.Timer(safteyTimeout / 2);
            safteyTimer.Elapsed += safteyTimerElapsed;
        }
        public int ListenPort { get; set; }
        public int DestinationPot { get; set; }

        public override void Start()
        {
            _isEnabled = true;
            _client = new UdpClient(ListenPort);

            _receieveThread = new Thread((ThreadStart)this.ReceiveDataSync) { Name = "UDP receive thread" };
            _receieveThread.Start();
        }
        public override void Stop()
        {
            _isEnabled = false;
            if (_client != null)
                _client.Close();
            _client = null;
            SpinWait.SpinUntil(() => _isStopped, 100);
        }

        public override void DataProcessed(IPEndPoint endPoint)
        {
            SendData(endPoint, LastPacketIndex);
        }

        ushort safteyLastPacketId = 0;
        private void safteyTimerElapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            if (safteyLastPacketId == LastPacketIndex)
                IsConnected = false;
            safteyLastPacketId = LastPacketIndex;
        }
    }
}