using System;
using System.Collections.Generic;
using System.Linq;
using EHaskins.Utilities.Extensions;

namespace Scoe.Shared.Model
{
    public class AnalogInput : Channel<byte, ushort>
    {
        public AnalogInput() { }
        public AnalogInput(byte pin, ushort minValue = 0, ushort maxValue = 1024, double maxVolts = 5.0)
        {
            ID = pin;
            MinValue = minValue;
            MaxValue = maxValue;
            MaxVolts = maxVolts;
        }
        protected override void OnUpdated(ushort oldValue, ushort newValue)
        {
            RaisePropertyChanged("Volts");
            base.OnUpdated(oldValue, newValue);
        }
        public double Volts
        {
            get
            {
                return ((double)Value).Map(MinValue, MaxValue, 0, MaxVolts);
            }
        }
        public ushort MinValue { get; set; }
        public ushort MaxValue { get; set; }
        public double MaxVolts { get; set; }
    }
}
