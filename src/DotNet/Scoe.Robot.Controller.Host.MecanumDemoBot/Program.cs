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
           (new Scoe.Controller.MecanumDemoBot.Controller()).Run();
            var controllerDomain = AppDomain.CreateDomain("ControllerDomain");
            controllerDomain.UnhandledException += controllerCrashHandler;

        }
        private static void controllerCrashHandler(object sender, UnhandledExceptionEventArgs e)
        {
            throw new NotImplementedException();
        }
    }
}
