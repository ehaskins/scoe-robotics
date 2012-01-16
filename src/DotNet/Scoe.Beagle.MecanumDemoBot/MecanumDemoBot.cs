using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Scoe.Robot.Shared;
using Scoe.Robot.Shared.RobotModel;

namespace Scoe.Robot.MecanumDemoBot
{
    public class Startup
    {
        public static void Main(string[] args)
        {
            //var robot = new MecanumDemoBot(new FrcConnection(6), new ArduinoConnection("COM3"));
            //robot.Run();
        }
    }
    public class IterativeRobotBase
    {
        public IterativeRobotBase()
        {

        }

        public void Run()
        {
            //TODO: Implement run
        }

        public virtual void TeleopInit() { }
        public virtual void TeleopLoop() { }
        public virtual void AutonomousInit() { }
        public virtual void AutonomousLoop() { }
        public virtual void DisabledInit() { }
        public virtual void DisabledLoop() { }
    }
    public class MecanumDemoBot : IterativeRobotBase
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
