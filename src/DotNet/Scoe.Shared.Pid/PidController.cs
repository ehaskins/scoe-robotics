using System;
using System.Collections.Generic;
using System.Linq;
using Scoe.Shared.Model;

namespace Scoe.Robot.MecanumDemoBot
{
    public class PidController : Motor
    {
        public Encoder Encoder { get; private set; }
        public double P { get; set; }
        public double I { get; set; }
        public double D { get; set; }

        public override double Value
        {
            get
            {
                //TODO: Implement PID logic
                return base.Value;
            }
            set
            {
                base.Value = value;
            }
        }
    }
}
