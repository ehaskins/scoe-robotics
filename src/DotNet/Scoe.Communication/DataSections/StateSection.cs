using System;
using System.Collections.Generic;
using System.Linq;
using EHaskins.Utilities.Binary;
using Scoe.Shared.Model;

namespace Scoe.Communication.DataSections
{
    public class StateSection : Scoe.Communication.DataSection
    {
        public bool IsDSConnectedUpdated { get; set; }
        RobotState _state;
        public StateSection(RobotState state, bool updateDSConnected = false)
            : base(100)
        {
            _state = state;
            IsDSConnectedUpdated = updateDSConnected;
        }

        public override DataSectionData GetCommandData()
        {
            return GetData();
        }
        public override DataSectionData GetStatusData()
        {
            return GetData();
        }
        private DataSectionData GetData()
        {
            var bits = new BitField8();
            bits[0] = _state.IsEStopped;
            bits[1] = _state.IsDSConnected;
            bits[2] = _state.IsEnabled;
            bits[3] = _state.IsAutonomous;
            bits[4] = _state.IsIODeviceConnected;
            return new DataSectionData() { SectionId = SectionId, Data = new byte[] { bits.RawValue } };
        }
        public override void ParseStatus(DataSectionData sectionData)
        {
            Parse(sectionData);
        }
        public override void ParseCommand(DataSectionData sectionData)
        {
            Parse(sectionData);
        }
        private void Parse(DataSectionData sectionData)
        {
            var data = sectionData.Data;
            var offset = 0;

            var bits = new BitField8(data[offset++]);
            _state.IsEStopped = bits[0];
            if (IsDSConnectedUpdated)
                _state.IsDSConnected = bits[1];
            _state.IsEnabled = bits[2];
            _state.IsAutonomous = bits[3];
            _state.IsIODeviceConnected = bits[4];
        }
    }
}
