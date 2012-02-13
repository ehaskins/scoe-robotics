using System;
using System.Collections.Generic;
using System.Linq;
using Scoe.Shared.Model;

namespace Scoe.Robot.Interface.Arduino
{
    public class RslModelSection : ArduinoDataSection
    {
        private RobotState _State;
        public bool _updated = false;
        public RslModelSection(RobotState state)
            : base(0)
        {
            State = state;
        }
        public RobotState State
        {
            get
            {
                return _State;
            }
            set
            {
                if (_State == value)
                    return;
                _State = value;
                RaisePropertyChanged("State");
            }
        }

        public override void GetData(ref byte[] data, ref int offset)
        {
            RslMode state = 0;

            if (State.IsEStopped)
                state = RslMode.EStopped;
            else if (!State.IsDSConnected)
                state = RslMode.NoFrcCommunication;
            else if (State.IsAutonomous)
                state = RslMode.Autonomous;
            else if (!State.IsEnabled)
                state = RslMode.Disabled;
            else
                state = RslMode.Enabled;

            data[offset++] = (byte)state;
        }
        public override void Update(byte[] data, int offset)
        {
            _updated = true;
        }

        private enum RslMode : byte
        {
            NoFrcCommunication = 0,
            Enabled = 1,
            Disabled = 2,
            Autonomous = 3,
            EStopped = 4,
            NoState = 255
        }
    }
}
