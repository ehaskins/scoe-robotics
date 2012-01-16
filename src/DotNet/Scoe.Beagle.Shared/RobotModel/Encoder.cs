using System;
using System.Collections.Generic;
using System.Linq;
using EHaskins.Utilities;

namespace Scoe.Robot.Shared.RobotModel
{
    public class Encoder : NotifyObject
    {
        private bool _Stalled;
        private float _Velocity;
        private long _Position;
        private short _TicksPerRotation;
        private ushort _ChannelBPin;
        private ushort _ChannelAPin;
        public ushort ChannelAPin
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
        public ushort ChannelBPin
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
        public short TicksPerRotation
        {
            get
            {
                return _TicksPerRotation;
            }
            set
            {
                if (_TicksPerRotation == value)
                    return;
                _TicksPerRotation = value;
                RaisePropertyChanged("TicksPerRotation");
            }
        }
        public long Position
        {
            get
            {
                return _Position;
            }
            set
            {
                if (_Position == value)
                    return;
                _Position = value;
                RaisePropertyChanged("Position");
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
