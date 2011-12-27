using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Scoe.Beagle.Shared;
using Scoe.Beagle.Shared.RobotModel;

namespace Scoe.Beagle.MecanumDemoBot
{
    public class Startup
    {
        public static void Main(string[] args)
        {
            var robot = new MecanumDemoBot(new FrcConnection(6), new ArduinoConnection("COM3"));
            robot.Run();
        }
    }
    public class MecanumDemoBot : GroundRobotModel
    {
        ulong teleopCount = 0;
        public override void TeleopInit()
        {
            teleopCount = 0;
        }

        public override void TeleopLoop()
        {

        }

        ulong autonomousCount = 0;
        public override void AutonomousInit()
        {
            autonomousCount = 0;
        }
        public override void AutonomousLoop()
        {

        }

        ulong disabledCount = 0;
        public override void DisabledInit()
        {
            disabledCount = 0;
        }
        public override void DisabledLoop()
        {

        }
    }
}
