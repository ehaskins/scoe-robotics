using System;
using System.Collections.Generic;
using System.Linq;
using Scoe.Communication.Udp;
using Scoe.Shared.Model;
using Scoe.Shared.Controller;
using EHaskins.Utilities.Extensions;
using Scoe.Communication.Arduino;

namespace Scoe.Robot.MecanumDemoBot
{
    public class Controller : IterativeControllerBase
    {
        MecanumModel model;
        public Controller()
        {
            model = new MecanumModel(State, "COM5", 115200);
        }

        protected override void EnabledLoop()
        {
            if (model.DriverController.Axes.Count >= 3)
            {
                var x = model.DriverController.Axes[0].Deadband(0.0, 1.0, 0.1);
                var y = -model.DriverController.Axes[1].Deadband(0.0, 1.0, 0.1);
                var z = model.DriverController.Axes[3].Deadband(0.0, 1.0, 0.1);


                Console.WriteLine(String.Format("Mecanum Drive x:{0:f2} y:{1:f2} z:{2:f2}", x, y, z));

                var nw = -y + x + z;
                var ne = -y - x - z;
                var sw = y - x + z;
                var se = y + x - z;

                var scale = 0.3;
                ne *= scale;
                nw *= scale;
                sw *= scale;
                se *= scale;

                model.NWMotor.Value = nw;
                model.NEMotor.Value = ne;
                model.SWMotor.Value = sw;
                model.SEMotor.Value = se;
            }

            Console.WriteLine(State.PrimaryState.ToString() + model.UltraSonicChannel.Value);
        }

        protected override void DisabledInit()
        {
            foreach (Motor motor in model.Motors)
            {
                motor.Value = 0;
            }
        }

        double dimmerDir = 0.005;
        double dimmerMin = 0;
        double dimmerMax = 0.5;
        double dimmerPos = 0;
        protected override void DisabledLoop()
        {

            if (dimmerPos > dimmerMax || dimmerPos < dimmerMin)
                dimmerDir *= -1;

            dimmerPos += dimmerDir;
            model.LedDimmer.Value = dimmerPos;

            if (model.DriverController.Axes.Count >= 1)
            {
                model.JSDimmer.Value = model.DriverController.Axes[0];
                Console.WriteLine(State.PrimaryState.ToString() + model.DriverController.Axes[0]);
            }

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
