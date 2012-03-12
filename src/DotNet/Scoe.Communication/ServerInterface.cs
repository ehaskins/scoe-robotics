using System;
using System.Collections.Generic;
using System.Linq;
using System.Timers;

namespace Scoe.Communication
{
    public class ServerInterface : Interface, IDisposable
    {
        System.Timers.Timer _safteyTimer;

        public ServerInterface(Protocol protocol)
            : base(protocol)
        {
            Mode = InterfaceMode.Server;
        }

        public override void Start()
        {
            base.Start();
            _safteyTimer = new Timer();
            _safteyTimer.Elapsed += safteyTimerElapsed;
            _safteyTimer.Start();
        }
        public override void Stop()
        {
            _safteyTimer.Stop();
            base.Stop();
        }
        protected override void Received(Packet packet)
        {
            base.Received(packet);
            Transmit();
        }
        ushort safteyLastPacketId = 0;
        private void safteyTimerElapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            if (safteyLastPacketId == LastPacketIndex)
                IsConnected = false;
            safteyLastPacketId = LastPacketIndex;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
                if (_safteyTimer != null)
                {
                    _safteyTimer.Dispose();
                    _safteyTimer = null;
                }
        }
        ~ServerInterface()
        {
            Dispose(false);
        }
    }
}
