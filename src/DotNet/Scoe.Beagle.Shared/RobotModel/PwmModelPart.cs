using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections.ObjectModel;

namespace Scoe.Beagle.Shared.RobotModel
{
    public class PwmModelPart : RobotModelPart
    {
        public PwmModelPart()
            : base(1)
        {
            _PwmOutputs = new ObservableCollection<PwmOutput>();
        }

        public override void GetData(ref byte[] data, ref int offset)
        {
            data[offset++] = (byte)PwmOutputs.Count;
            foreach (PwmOutput pwmOutput in PwmOutputs)
            {
                data[offset++] = pwmOutput.Pin;
                data[offset++] = pwmOutput.Value;
            }
        }

        public override void Update(byte[] data, int offset)
        {
            //Pwm model part doesn't get any updates
        }

        private ObservableCollection<PwmOutput> _PwmOutputs;

        public ObservableCollection<PwmOutput> PwmOutputs
        {
            get { return _PwmOutputs; }
            protected set
            {
                if (_PwmOutputs == value)
                    return;
                _PwmOutputs = value;
                RaisePropertyChanged("PwmOutputs");
            }
        }
    }
}
