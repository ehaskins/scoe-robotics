using System;
using System.Collections.Generic;
using System.Linq;
using Scoe.Shared.Model;
using EHaskins.Utilities.Extensions;
using System.IO;
namespace Scoe.Communication.Arduino
{
    public class MotorDataSection : DataSection
    {
        public MotorDataSection(IList<Motor> pwmOutputs)
            : base(1)
        {
            _PwmOutputs = pwmOutputs;
        }

        public override DataSectionData GetData()
        {
            using (var stream = new MemoryStream())
            {

                stream.WriteByte((byte)Motors.Count);
                foreach (Motor motor in Motors)
                {
                    stream.WriteByte(motor.IsEnabled ? motor.ID : (byte)0);

                    //Scale motor value from -1 to 1, or 0 to 1, reversible or not, respectively.
                    var normalized = motor.GetNormalized();

                    if (motor.IsReversible)
                        normalized = (normalized + 1) / 2;

                    stream.WriteByte((byte)(normalized * 255));
                }

                return new DataSectionData() { SectionId = SectionId, Data = stream.ToArray() };
            }

        }

        public override void Update(DataSectionData sectionData)
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
