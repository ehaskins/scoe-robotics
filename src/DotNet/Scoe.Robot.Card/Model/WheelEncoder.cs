using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;

namespace Scoe.Shared.Model
{
    public class WheelEncoder : Encoder
    {
        private double _Velocity;
        private int _TicksPerMeter;
        private bool _IsInverted;
        Stopwatch stopwatch = new Stopwatch();
        double lastElapsed = 0;
        long lastTicks = 0;

        public WheelEncoder(byte pinA, byte pinB, int ticksPerMeter = 1, bool invert = false)
            : base(pinA, pinB)
        {

            TicksPerMeter = ticksPerMeter;
            IsInverted = invert;
            stopwatch.Start();
        }
        public override void Update(int ticks)
        {
            var elapsed = (double)stopwatch.ElapsedTicks / Stopwatch.Frequency;
            var diffElapsed = elapsed - lastElapsed;

            if (IsInverted)
                ticks *= -1;

            var diffTicks = ticks - lastTicks;
            var ticksPerSec = diffTicks / diffElapsed;

            Velocity = ticksPerSec / TicksPerMeter;

            base.Update(ticks);
            lastElapsed = elapsed;
            lastTicks = ticks;
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
                if (_Velocity == value)
                    return;
                _Velocity = value;
                RaisePropertyChanged("Velocity");
            }
        }
    }
}
