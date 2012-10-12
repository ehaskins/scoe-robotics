using System;
using System.Collections.Generic;
using System.Linq;

namespace Scoe.Shared.Model
{
    public class DutyCyclePwm : DisableableChannel<byte, double>
    {
        public DutyCyclePwm() { }
        public DutyCyclePwm(byte id)
        {
            ID = id;
            IsEnabled = true;
        }
    }
}
