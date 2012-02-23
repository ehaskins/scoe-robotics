using System;
using System.Collections.Generic;
using System.Linq;
using EHaskins.Utilities.Binary;
using Scoe.Shared.Model;

namespace Scoe.Communication.DataSections
{
    public class StateSection : Scoe.Communication.DataSection
    {
        RobotState _state;
        public StateSection(RobotState state, bool isDsLink = true)
            : base(100)
        {
            _IsDSLink = isDsLink;
            _state = state;
        }

        private bool _IsDSLink;
        public bool IsDSLink
        {
            get { return _IsDSLink; }
            set
            {
                if (_IsDSLink == value)
                    return;
                _IsDSLink = value;
                RaisePropertyChanged("IsDSLink");
            }
        }
        public override DataSectionData GetData()
        {
            var bits = new BitField8();
            bits[0] = _state.IsEStopped;
            bits[1] = _state.IsDSConnected;
            bits[2] = _state.IsEnabled;
            bits[3] = _state.IsAutonomous;
            bits[4] = _state.IsIODeviceConnected;
            return new DataSectionData() { SectionId = SectionId, Data = new byte[] { bits.RawValue } };
        }

        public override void Update(DataSectionData sectionData)
        {
            var data = sectionData.Data;
            var offset = 0;

            var bits = new BitField8(data[offset++]);
            _state.IsEStopped = bits[0];
            if (!IsDSLink)//Don't apply DS state when communicating over the DS link. 
                _state.IsDSConnected = bits[1];
            _state.IsEnabled = bits[2];
            _state.IsAutonomous = bits[3];
            _state.IsIODeviceConnected = bits[4];
        }
    }
}
