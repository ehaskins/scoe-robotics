using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;
using Scoe.Shared.Model.Pid;

namespace Scoe.Shared.Model
{
    public class WheelEncoder : Encoder, IPidSource
    {
        private double _Velocity;
        private int _TicksPerMeter;
        private bool _IsInverted;
        Stopwatch stopwatch = new Stopwatch();
        long lastElapsedTicks = 0;
        long lastTicks = 0;
        bool hasReceivedFirstTicks = false;

        public WheelEncoder(byte pinA, byte pinB, int ticksPerMeter = 1, bool invert = false)
            : base(pinA, pinB)
        {

            TicksPerMeter = ticksPerMeter;
            IsInverted = invert;
            stopwatch.Start();
        }
        public override void Update(int ticks)
        {
            var elapsedTicks = stopwatch.ElapsedTicks;
            var diffElapsed = (double)(elapsedTicks - lastElapsedTicks) / Stopwatch.Frequency;

            if (IsInverted)
                ticks *= -1;
            if (hasReceivedFirstTicks)
            {
                var diffTicks = ticks - lastTicks;
                var ticksPerSec = diffTicks / diffElapsed;

                Velocity = ticksPerSec / TicksPerMeter;
            }
            else
            {
                hasReceivedFirstTicks = true;
            }
            lastElapsedTicks = elapsedTicks;
            lastTicks = ticks;
            base.Update(ticks);
        }

        public bool IsInverted
        {
            get
            {
                return _IsInverted;
            }
            set
            {
                if (_IsInverted == value)
                    return;
                _IsInverted = value;
                RaisePropertyChanged("IsInverted");
            }
        }

        public int TicksPerMeter
        {
            get
            {
                return _TicksPerMeter;
            }
            set
            {
                if (_TicksPerMeter == value)
                    return;
                _TicksPerMeter = value;
                RaisePropertyChanged("TicksPerMeter");
            }
        }
        public double Velocity
        {
            get
            {
                return _Velocity;
            }
            set
            {
                RaiseUpdated();
                if (_Velocity == value)
                    return;
                _Velocity = value;
                RaisePropertyChanged("Velocity");
            }
        }

        public event PidSourceUpdatedEventHandler Updated;
        protected void RaiseUpdated()
        {
            if (Updated != null)
            {
                Updated(this, new PidSourceUpdatedEventArgs(Velocity));
            }
        }
    }
}
