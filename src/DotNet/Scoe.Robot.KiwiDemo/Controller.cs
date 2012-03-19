using System;
using System.Collections.Generic;
using System.Linq;
using Scoe.Shared.Model;
using Scoe.Shared.Controller;
using EHaskins.Utilities.Extensions;
using Scoe.Shared.Model.Pid;
using Scoe.Communication;
using Scoe.Communication.Arduino;
using Scoe.Communication.Udp;

namespace Scoe.Robot.KiwiDemo
{
    public class Controller : IterativeControllerBase
    {
        bool enablePid = false;

        KiwiModel model;
        double maxSpeed = 3.0; //max speed in m/s

        public VelocityPid WestPid;
        public VelocityPid EastPid;
        public VelocityPid SouthPid;

        public Controller() : base()
        {
            var port = Environment.OSVersion.VersionString.Contains("Windows") ? "COM10" : "/dev/serial/by-id/usb-Arduino__www.arduino.cc__Arduino_Mega_2560_6493234363835131E111-if00";
            var baudRate = 115200;
            //Build IO interfaces
            var IOInterface = new ArduinoInterface(port, baudRate);
            var ControlInterface = new ServerInterface(new UdpProtocol(1150, 1110));

            this.RequiredConnectecions.Add(ControlInterface);
            model = new KiwiModel(State, IOInterface, ControlInterface);

            if (enablePid)
            {
                var i = 0;
                var p = 0;
                var d = 5;
                WestPid = new VelocityPid() { Source = model.WestEncoder, Output = model.WestMotor, P = p, I = i, D = d, OutputMax = 1, OutputMin = -1 };
                EastPid = new VelocityPid() { Source = model.EastEncoder, Output = model.EastMotor, P = p, I = i, D = d, OutputMax = 1, OutputMin = -1 };
                SouthPid = new VelocityPid() { Source = model.SouthEncoder, Output = model.SouthMotor, P = p, I = i, D = d, OutputMax = 1, OutputMin = -1 };
            }
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

                double xboxDeadband = 0.05;
                var x = model.DriverController.Axes[2].Deadband(0.0, 1.0, xboxDeadband);
                var y = -model.DriverController.Axes[1].Deadband(0.0, 1.0, xboxDeadband);
                var z = -model.DriverController.Axes[0].Deadband(0.0, 1.0, xboxDeadband);

                double velocity = enablePid ? maxSpeed : 1; //Desired maximum robot velocity in meters/second

                x *= velocity;
                y *= velocity;
                z *= 0.3;

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

                if (enablePid)
                {
                    WestPid.Value = west;
                    EastPid.Value = east;
                    SouthPid.Value = south;
                }
                else
                {
                    model.WestMotor.Value = west;
                    model.EastMotor.Value = east;
                    model.SouthMotor.Value = south;
                }


            }
            model.WriteVelocities();
            Console.WriteLine(State.PrimaryState + model.UltraSonicChannel.Value);
        }

        protected override void DisabledInit()
        {
            if (enablePid)
            {
                WestPid.Value = 0;
                EastPid.Value = 0;
                SouthPid.Value = 0;
            }

            foreach (var motor in model.Motors)
            {
                motor.IsEnabled = false;
            }
        }

        double dimmerDir = 0.001;
        double dimmerMin = 0.0;
        double dimmerMax = 0.5;
        double dimmerPos = 0;

        protected override void DisabledLoop()
        {

            if (dimmerPos > dimmerMax || dimmerPos < dimmerMin)
                dimmerDir *= -1;

            dimmerPos += dimmerDir;
            model.LedDimmer.Value = dimmerPos;

            Console.WriteLine(dimmerPos.ToString("f2"));
            model.WriteVelocities();
        }


        double motorChangeDir = 0.01;
        double motorMax = 1;
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
