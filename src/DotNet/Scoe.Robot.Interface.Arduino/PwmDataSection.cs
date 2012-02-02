using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections.ObjectModel;
using Scoe.Robot.Model;

namespace Scoe.Robot.Interface.Arduino
{
    public class PwmDataSection : ArduinoDataSection
    {
        public PwmDataSection(IList<PwmOutput> pwmOutputs)
            : base(1)
        {
            _PwmOutputs = pwmOutputs;
        }

        public override void GetData(ref byte[] data, ref int offset)
        {
            data[offset++] = (byte)PwmOutputs.Count;
            foreach (PwmOutput pwmOutput in PwmOutputs)
            {
                data[offset++] = pwmOutput.Enabled ? pwmOutput.Pin : (byte)0;
                data[offset++] = pwmOutput.Value;
            }
        }

        public override void Update(byte[] data, int offset)
        {
            //Pwm model part doesn't get any updates
        }

        private IList<PwmOutput> _PwmOutputs;

        public IList<PwmOutput> PwmOutputs
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
