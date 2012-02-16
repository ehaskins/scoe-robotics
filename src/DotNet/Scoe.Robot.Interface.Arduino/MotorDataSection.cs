using System;
using System.Collections.Generic;
using System.Linq;
using Scoe.Shared.Model;
using EHaskins.Utilities.Extensions;
namespace Scoe.Robot.Interface.Arduino
{
    public class MotorDataSection : ArduinoDataSection
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
                var normalized = (motor.Value / motor.Scale);
                normalized = motor.Reversible ? normalized.Limit(-1, 1) : normalized.Limit(0, 1);

                data[offset++] = (byte)(normalized * (motor.Reversible ? 127 : 255));
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
