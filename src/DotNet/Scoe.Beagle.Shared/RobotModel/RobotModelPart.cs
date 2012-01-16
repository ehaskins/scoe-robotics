using System;
using System.Collections.Generic;
using System.Linq;
using EHaskins.Utilities;

namespace Scoe.Robot.Shared.RobotModel
{
    public abstract class RobotModelSection : NotifyObject
    {
        public RobotModelSection(byte sectionId)
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

        public abstract void GetData(ref byte[] data, ref int offset);
        public abstract void Update(byte[] data, int offset);
    }
}
