using System;
using System.Collections.Generic;
using System.Linq;

namespace Scoe.Beagle.Shared.RobotModel
{
    public abstract class RobotModelPart : NotifyObject
    {
        public RobotModelPart(int sectionId)
        {
            ModelSectionId = sectionId;
        }
        private int _ModelSectionId;
        public int ModelSectionId
        {
            get { return _ModelSectionId; }
            set
            {
                if (_ModelSectionId == value)
                    return;
                _ModelSectionId = value;
                RaisePropertyChanged("ModelSectionId");
            }
        }

        public abstract void GetData(ref byte[] data, ref int offset);
        public abstract void Update(byte[] data, int offset);
    }
}
