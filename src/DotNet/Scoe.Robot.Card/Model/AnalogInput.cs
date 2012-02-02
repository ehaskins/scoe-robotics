using System;
using System.Collections.Generic;
using System.Linq;

namespace Scoe.Robot.Model
{
    public class AnalogInput : Channel<ushort> {
        public AnalogInput() { }
        public AnalogInput(byte pin){Pin = pin;}
    }
}
