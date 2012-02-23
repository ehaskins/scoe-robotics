using System;
using System.Collections.Generic;
using System.Linq;
using System.Timers;

namespace Scoe.Communication
{
    public class ClientInterface : Interface, IDisposable
    {
        Timer _transmitTimer;

        public ClientInterface(Protocol protocol, double frequency = 50) : base(protocol)
        {
            TransmitFrequency = frequency;
            Mode = InterfaceMode.Client;
        }
        private void TransmitTimerElapsed(object sender, ElapsedEventArgs e)
        {
            Transmit();
        }
        public override void Start()
        {
            base.Start();

            if (_transmitTimer != null)
                _transmitTimer.Dispose();
            _transmitTimer = new Timer(50 / TransmitFrequency);
            _transmitTimer.Elapsed += TransmitTimerElapsed;
            _transmitTimer.Start();
        }
        public double TransmitFrequency { get; private set; }


        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
                if (_transmitTimer != null)
                {
                    _transmitTimer.Dispose();
                    _transmitTimer = null;
                }
        }
        ~ClientInterface()
        {
            Dispose(false);
        }

    }
}
