using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Collections.ObjectModel;
using System.Net;
using System.Net.Sockets;

namespace Scoe.Communication.Udp
{
    public class UdpServer : Scoe.Communication.Interface, IDisposable
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
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
                if (safteyTimer != null)
                {
                    safteyTimer.Dispose();
                    safteyTimer = null;
                }
        }
        ~UdpServer()
        {
            Dispose(false);
        }

        public int ListenPort { get; set; }
        public int DestinationPot { get; set; }

        public override void Start()
        {
            _isEnabled = true;
            _client = new UdpClient(ListenPort);

            _receieveThread = new Thread((ThreadStart)this.ReceiveDataSync) { Name = "UDP receive thread" };
            _receieveThread.Start();
            safteyTimer.Start();
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
            IsConnected = true;
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