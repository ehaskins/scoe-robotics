using System;
using System.Collections.Generic;
using System.Linq;
using EHaskins.Utilities;

namespace Scoe.Shared.Model
{
    public class Encoder : NotifyObject
    {
        public Encoder(byte pinA, byte pinB)
        {
            ChannelAPin = pinA;
            ChannelBPin = pinB;
        }
        public void Update(int ticks)
        {
            Ticks = ticks;
        }
        private bool _Stalled;
        private float _Velocity;
        private long _Ticks;
        private short _TicksPerMeter;
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
        public short TicksPerMeter
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
        public float Velocity
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
