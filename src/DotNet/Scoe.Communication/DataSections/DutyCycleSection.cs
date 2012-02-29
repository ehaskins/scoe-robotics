using System;
using System.Collections.Generic;
using System.Linq;
using Scoe.Shared.Model;
using EHaskins.Utilities.Extensions;
using System.IO;

namespace Scoe.Communication.DataSections
{
    public class DutyCycleSection : DataSection
    {
        public DutyCycleSection(IList<DutyCyclePwm> pwms)
            : base(4)
        {
            Pwms = pwms;
        }

        public IList<DutyCyclePwm> Pwms { get; private set; }

        public override DataSectionData GetCommandData()
        {
            using (var stream = new MemoryStream())
            {
                stream.WriteByte((byte)Pwms.Count);
                foreach (var pwm in Pwms)
                {
                    stream.WriteByte(pwm.IsEnabled ? pwm.ID : (byte)0);
                    stream.WriteByte((byte)(pwm.Value * 255).Limit(0, byte.MaxValue));
                }

                return new DataSectionData() { SectionId = SectionId, Data = stream.ToArray() };
            }
        }

    }
}
