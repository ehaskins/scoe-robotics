using System;
using System.Collections.Generic;
using System.Linq;
using EHaskins.Utilities;
using System.Diagnostics;

namespace Scoe.Shared.Model
{
    public class Encoder : NotifyObject
    {
        private bool _IsInverted;
        Stopwatch stopwatch = new Stopwatch();
        double lastElapsed = 0;
        long lastTicks = 0;
        public Encoder(byte pinA, byte pinB, int ticksPerMeter = 1, bool invert = false)
        {
            ChannelAPin = pinA;
            ChannelBPin = pinB;
            TicksPerMeter = ticksPerMeter;
            IsInverted = invert;
            stopwatch.Start();
        }
        public void Update(int ticks)
        {
            var elapsed = (double)stopwatch.ElapsedTicks / Stopwatch.Frequency;
            var diffElapsed = elapsed - lastElapsed;

            if (IsInverted)
                ticks *= -1;

            var diffTicks = ticks - lastTicks;
            var ticksPerSec = diffTicks / diffElapsed;

            Velocity = ticksPerSec / TicksPerMeter;
            Ticks = ticks;
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
        private double _Velocity;
        private long _Ticks;
        private int _TicksPerMeter;
        private byte _ChannelBPin;
        private byte _ChannelAPin;
        public byte ChannelAPin
        {
            get
            {
                return _ChannelAPin;
            }
            set
            {
                if (_ChannelAPin == value)
                    return;
                _ChannelAPin = value;
                RaisePropertyChanged("ChannelAPin");
            }
        }
        public byte ChannelBPin
        {
            get
            {
                return _ChannelBPin;
            }
            set
            {
                if (_ChannelBPin == value)
                    return;
                _ChannelBPin = value;
                RaisePropertyChanged("ChannelBPin");
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
        public long Ticks
        {
            get
            {
                return _Ticks;
            }
            set
            {
                if (_Ticks == value)
                    return;
                _Ticks = value;
                RaisePropertyChanged("Ticks");
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
