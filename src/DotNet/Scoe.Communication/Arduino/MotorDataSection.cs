using System;
using System.Collections.Generic;
using System.Linq;
using Scoe.Shared.Model;
using EHaskins.Utilities.Extensions;
namespace Scoe.Communication.Arduino
{
    public class MotorDataSection : DataSection
    {
        public MotorDataSection(IList<Motor> pwmOutputs)
            : base(1)
        {
            _PwmOutputs = pwmOutputs;
        }

        public override void GetData(ref byte[] data, ref int offset)
        {
            data[offset++] = (byte)Motors.Count;
            foreach (Motor motor in Motors)
            {
                data[offset++] = motor.IsEnabled ? motor.ID : (byte)0;

                //Scale motor value from -1 to 1, or 0 to 1, reversible or not, respectively.
                var normalized = motor.GetNormalized();

                if (motor.IsReversible)
                    normalized = (normalized + 1) / 2;

                data[offset++] = (byte)(normalized * 255);
            }
        }

        public override void Update(byte[] data, int offset)
        {
            //Pwm model part doesn't get any updates
        }

        private IList<Motor> _PwmOutputs;

        public IList<Motor> Motors
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
