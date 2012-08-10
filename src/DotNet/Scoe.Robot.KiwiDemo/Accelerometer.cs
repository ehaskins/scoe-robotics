using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Scoe.Shared.Model;

namespace Scoe.Robot.KiwiDemo
{
    public class Accelerometer
    {
        public virtual double XAccel { get; protected set; }
        public virtual double YAccel { get; protected set; }
        public virtual double ZAccel { get; protected set; }
    }
}