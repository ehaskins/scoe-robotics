using System;
using System.Collections.Generic;
using System.Linq;
using Scoe.Shared.Model;
using Scoe.Shared.Controller;
using EHaskins.Utilities.Extensions;

namespace Scoe.Robot.KiwiDemo
{
    public class Controller : IterativeControllerBase
    {
        KiwiModel model;
        public Controller()
        {
            model = new KiwiModel(State, "COM5", 115200);
        }

        protected override void EnabledInit()
        {
            foreach (var motor in model.Motors)
                motor.IsEnabled = true;
        }
        double yFactor = 1.147; // 1/sin(60)
        double xFactor = 2; // 1/cos(60)

        protected override void EnabledLoop()
        {
            if (model.DriverController.Axes.Count >= 3)
            {
                var x = 0;// model.DriverController.Axes[0].Deadband(0.0, 1.0, 0.2);
                var y = -model.DriverController.Axes[1].Deadband(0.0, 1.0, 0.2);
                var z = model.DriverController.Axes[3].Deadband(0.0, 1.0, 0.2);

                var velocity = 3; //Desired maximum robot velocity in meters/second

                x *= velocity;
                y *= velocity;
                z *= velocity;

                Console.WriteLine(String.Format("Kiwi Drive x:{0:f2} y:{1:f2} z:{2:f2}", x, y, z));

                var west = 
                    y * yFactor +
                    x * xFactor +
                    z;
                var east = 
                    y * yFactor +
                    -x * xFactor +
                    -z;
                var south = -x + z;

                model.WestPid.Value = west;
                model.EastPid.Value = east;
                model.SouthPid.Value = south;

                //model.WestMotor.Value = west;
                //model.EastMotor.Value = east;
                //model.SouthMotor.Value = south;

            }
            model.WriteVelocities();
            Console.WriteLine(State.PrimaryState + model.UltraSonicChannel.Value);
        }

        protected override void DisabledInit()
        {
            model.WestPid.Value = 0;
            model.EastPid.Value = 0;
            model.SouthPid.Value = 0;

            foreach (var motor in model.Motors){
                motor.IsEnabled = false;
            }
        }

        double dimmerDir = 0.001;
        double dimmerMin = -0.1;
        double dimmerMax = 0.10;
        double dimmerPos = 0;

        protected override void DisabledLoop()
        {

            //if (dimmerPos > dimmerMax || dimmerPos < dimmerMin)
            //    dimmerDir *= -1;

            //dimmerPos += dimmerDir;
            //model.SouthMotor.Value = dimmerPos;

            //Console.WriteLine(dimmerPos.ToString("f2"));
            model.WriteVelocities();
        }


        double motorChangeDir = -0.01;
        double motorMax = 0.15;
        double motorMin = 0;
        double value = 0.0;
        protected override void AutonomousInit()
        {
            value = 0;
        }
        protected override void AutonomousLoop()
        {
            //if (value >= motorMax || value < motorMin)
            //    motorChangeDir *= -1;

            //value += motorChangeDir;
            if (value < motorMax)
                value += motorChangeDir;

            foreach (var motor in model.Motors)
            {
                motor.Value = value;
            }

            model.WriteVelocities();
        }
    }
}
