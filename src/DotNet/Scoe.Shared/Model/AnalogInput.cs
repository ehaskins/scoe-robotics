using System;
using System.Collections.Generic;
using System.Linq;

namespace Scoe.Shared.Model
{
    public class AnalogInput : Channel<byte, ushort> {
        public AnalogInput() { }
        public AnalogInput(byte pin){ID = pin;}
    }
}
