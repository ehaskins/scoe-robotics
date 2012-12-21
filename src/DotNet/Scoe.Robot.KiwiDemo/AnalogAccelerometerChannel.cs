using System;
using System.Collections.Generic;
using System.Linq;
using Scoe.Shared.Model;

namespace Scoe.Robot.KiwiDemo
{
    public class AnalogAccelerometerChannel : Channel<string, double>
    {
        private AnalogInput _Input;
        public AnalogInput Input
        {
            get
            {
                return _Input;
            }
            set
            {
                if (_Input == value)
                    return;

                if (_Input != null)
                    _Input.ValueChanged -= this.InputValueChangedHandler;
                if (value != null)
                    value.ValueChanged += this.InputValueChangedHandler;

                _Input = value;
            }
        }

        public double CenterValue { get; set; }
        public double VoltPerG { get; set; }

        private void InputValueChangedHandler(object sender, ChannelValueChangedEventArgs<ushort> e)
        {
            Value = (Input.Volts - CenterValue) / VoltPerG;
        }
    }
}
