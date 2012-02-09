using System;
using System.Collections.Generic;
using System.Linq;
using EHaskins.Utilities;
using Scoe.Robot.Model;
using EHaskins.Utilities.Binary;

namespace Scoe.Communication.Udp
{
    public class StateSection : UdpDataSection
    {
        RobotState _state;
        public StateSection(RobotState state)
            : base(0)
        {
            _state = state;
        }

        public override void GetData(ref byte[] data, ref int offset)
        {
            var bits = new BitField8();
            bits[0] = _state.IsEStopped;
            bits[1] = _state.IsDSConnected;
            bits[2] = _state.IsEnabled;
            bits[3] = _state.IsAutonomous;
            bits[4] = _state.IsIODeviceConnected;
            data[offset++] = bits.RawValue;
        }
        public override void Update(byte[] data, int offset)
        {
            var bits = new BitField8(data[offset++]);
            _state.IsEStopped = bits[0];
            _state.IsDSConnected = bits[1];
            _state.IsEnabled = bits[2];
            _state.IsAutonomous = bits[3];
            _state.IsIODeviceConnected = bits[4];
        }
    }
    public abstract class UdpDataSection : NotifyObject
    {
        public UdpDataSection(byte sectionId)
        {
            SectionId = sectionId;
        }
        private byte _SectionId;
        public byte SectionId
        {
            get { return _SectionId; }
            set
            {
                if (_SectionId == value)
                    return;
                _SectionId = value;
                RaisePropertyChanged("SectionId");
            }
        }

        public virtual void ConnectionStateChanged(bool isConnected) { }
        public abstract void GetData(ref byte[] data, ref int offset);
        public abstract void Update(byte[] data, int offset);
    }
}
