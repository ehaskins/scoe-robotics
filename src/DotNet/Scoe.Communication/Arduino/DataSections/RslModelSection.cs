using System;
using System.Collections.Generic;
using System.Linq;
using Scoe.Shared.Model;
using EHaskins.Utilities.Aspects;
namespace Scoe.Communication.Arduino
{
    [NotifyPropertyChanged()]
    public class RslModelSection : Scoe.Communication.DataSection
    {
        public RslModelSection(RobotState state)
            : base(0)
        {
            State = state;
        }
        public RobotState State { get; set; }

        public override DataSectionData GetCommandData()
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
