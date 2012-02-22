using System;
using System.Collections.Generic;
using System.Linq;
using EHaskins.Utilities;
using System.Diagnostics;

namespace Scoe.Shared.Model
{
    public class Encoder : NotifyObject
    {

        public Encoder(byte pinA, byte pinB)
        {
            ChannelAPin = pinA;
            ChannelBPin = pinB;
        }
        public virtual void Update(int ticks)
        {
            Ticks = ticks;
        }

        private byte _ChannelBPin;
        private byte _ChannelAPin;
        private long _Ticks;
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

    }
}
