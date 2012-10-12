using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Scoe.Robot.KiwiDemo
{
    public class Gyro
    {
        public double Rate { get; set; }
        public double Position { get; set; }
        public AngleUnits Units { get; set; }
        public Gyro()
        {
            
        }
    }
}
