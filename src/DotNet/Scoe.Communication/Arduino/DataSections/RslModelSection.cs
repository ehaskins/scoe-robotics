using System;
using System.Collections.Generic;
using System.Linq;
using Scoe.Shared.Model;
using Scoe.Communication.Udp;

namespace Scoe.Communication.Arduino
{
    public class RslModelSection : Scoe.Communication.DataSection
    {
        private RobotState _State;
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

        public override DataSectionData GetData()
        {
            RslMode state = 0;

            if (State.IsEStopped)
                state = RslMode.EStopped;
            else if (!State.IsDSConnected)
                state = RslMode.NoFrcCommunication;
            else if (!State.IsEnabled)
                state = RslMode.Disabled;
            else if (State.IsAutonomous)
                state = RslMode.Autonomous;
            else
                state = RslMode.Enabled;

            return new DataSectionData() { SectionId = SectionId, Data = new byte[] { (byte)state} };
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
