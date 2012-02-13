using System;
using System.Collections.Generic;
using System.Linq;
using EHaskins.Utilities.Binary;
using Scoe.Shared.Model;

namespace Scoe.Communication.Udp
{
    public class StateSection : UdpDataSection
    {
        RobotState _state;
        UdpInterface _primaryInterface;
        public StateSection(RobotState state)
            : base(0)
        {
            _state = state;
        }

        public UdpInterface PrimaryInterface
        {
            get
            {
                return _primaryInterface;
            }
            set
            {
                if (_primaryInterface == value)
                    return;
                _primaryInterface = value;
            }
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

        public override void ConnectionStateChanged(UdpInterface sender, bool isConnected)
        {
            if (_primaryInterface != null && sender == _primaryInterface)
                _state.IsDSConnected = isConnected;
        }
    }
}
