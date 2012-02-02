using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Scoe.Robot.Controller.Host.MecanumDemoBot
{
    class Program
    {
        static void Main(string[] args)
        {
            (new Scoe.Robot.Controller.MecanumDemoBot.Controller()).Run();
        }
    }
}
