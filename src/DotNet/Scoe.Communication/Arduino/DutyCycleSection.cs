using System;
using System.Collections.Generic;
using System.Linq;
using Scoe.Shared.Model;
using EHaskins.Utilities.Extensions;

namespace Scoe.Communication.Arduino
{
    public class DutyCycleSection : DataSection
    {
        public DutyCycleSection(IList<DutyCyclePwm> pwms)
            : base(4)
        {
            Pwms = pwms;
        }

        public IList<DutyCyclePwm> Pwms { get; private set; }

        public override void GetData(ref byte[] data, ref int offset)
        {
            data[offset++] = (byte)Pwms.Count;
            foreach (var pwm in Pwms)
            {
                data[offset++] = pwm.IsEnabled ? pwm.ID : (byte)0;
                data[offset++] = (byte)(pwm.Value * 255).Limit(0, byte.MaxValue);
            }
        }

        public override void Update(byte[] data, int offset)
        {
            //Pwm model part doesn't get any updates
        }

    }
}
