using System;
using System.Collections.Generic;
using System.Linq;
using Scoe.Communication.Udp;
using Scoe.Shared.Model;
using Scoe.Shared.Controller;
using EHaskins.Utilities.Extensions;
using Scoe.Robot.Interface.Arduino;

namespace Scoe.Controller.MecanumDemoBot
{
    public class Controller : IterativeControllerBase
    {
        public Joystick DriverController;

        public Motor NWMotor;
        public Motor NEMotor;
        public Motor SWMotor;
        public Motor SEMotor;

        public AnalogInput UltraSonicChannel;

        public DutyCyclePwm LedDimmer;
        public DutyCyclePwm JSDimmer;

        public List<Joystick> Joysticks;
        public List<AnalogInput> AnalogInputs;
        public List<Motor> PwmOutputs;
        public List<DutyCyclePwm> DutyCyclePwms;


        private void SetupModel()
        {
            //Build objects
            NWMotor = new Motor(1);
            NEMotor = new Motor(2);
            SWMotor = new Motor(3);
            SEMotor = new Motor(4);

            UltraSonicChannel = new AnalogInput(0);

            DriverController = new Joystick();

            JSDimmer = new DutyCyclePwm(11);
            LedDimmer = new DutyCyclePwm(12);

            //Initialize collections
            PwmOutputs = new List<Motor>(new Motor[] { NWMotor, NEMotor, SWMotor, SEMotor });
            AnalogInputs = new List<AnalogInput>(new AnalogInput[] { UltraSonicChannel });
            DutyCyclePwms = new List<DutyCyclePwm>(new DutyCyclePwm[] { LedDimmer, JSDimmer });
            Joysticks = new List<Joystick>(new Joystick[] { DriverController });
        }
        private void SetupIO()
        {
            //Build IO interfaces
            var ioInt = new ArduinoInterface("COM5", 115200, 20);
            ioInt.Sections.Add(new RslModelSection(State));
            ioInt.Sections.Add(new AnalogIODataSection(AnalogInputs));
            ioInt.Sections.Add(new MotorDataSection(PwmOutputs));
            ioInt.Sections.Add(new DutyCycleSection(DutyCyclePwms));

            var ctrlInt = new UdpServer(1150, 1110);
            ctrlInt.Connected += (source, e) => State.IsDSConnected = true;
            ctrlInt.Disconnected += (source, e) => State.IsDSConnected = false;

            ctrlInt.Sections.Add(new StateSection(State));
            ctrlInt.Sections.Add(new JoystickSection(Joysticks));

            ioInt.Start();
            ctrlInt.Start();
        }
        public Controller()
        {
            SetupModel();
            SetupIO();
        }


        protected override void EnabledLoop()
        {
            if (DriverController.Axes.Count >= 3)
            {
                var x = DriverController.Axes[0];
                var y = DriverController.Axes[1];
                var z = DriverController.Axes[2];

                var nw = y + x + z;
                var ne = y - x - z;
                var sw = y - x + z;
                var se = y + x - z;

                NWMotor.Value = (byte)nw.Map(-1, 1, 0, 255);
                NEMotor.Value = (byte)ne.Map(-1, 1, 0, 255);
                SWMotor.Value = (byte)sw.Map(-1, 1, 0, 255);
                SEMotor.Value = (byte)se.Map(-1, 1, 0, 255);
            }

            Console.WriteLine(State.PrimaryState.ToString() + UltraSonicChannel.Value);
        }

        double dimmerDir = 0.01;
        double dimmerMin = 0;
        double dimmerMax = 1;
        double dimmerPos = 0;
        protected override void DisabledLoop()
        {

            if (dimmerPos > dimmerMax || dimmerPos < dimmerMin)
                dimmerDir *= -1;

            dimmerPos += dimmerDir;
            LedDimmer.Value = dimmerPos;

            if (DriverController.Axes.Count >= 1)
                JSDimmer.Value = DriverController.Axes[0];
            Console.WriteLine(State.PrimaryState.ToString() + dimmerPos);
        }
    }
}
