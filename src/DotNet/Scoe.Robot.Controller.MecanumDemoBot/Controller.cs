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
        public Joystick DriverController;

        public Motor NWMotor;
        public Motor NEMotor;
        public Motor SWMotor;
        public Motor SEMotor;

        public Encoder NWEncoder;
        public Encoder NEEncoder;
        public Encoder SWEncoder;
        public Encoder SEEncoder;

        public AnalogInput UltraSonicChannel;

        public DutyCyclePwm LedDimmer;
        public DutyCyclePwm JSDimmer;

        public List<Joystick> Joysticks;
        public List<AnalogInput> AnalogInputs;
        public List<Motor> Motors;
        public List<DutyCyclePwm> DutyCyclePwms;
        public List<Encoder> Encoders;

        private void SetupModel()
        {
            //Build objects
            NWMotor = new Motor(2);
            NEMotor = new Motor(3, true);
            SWMotor = new Motor(4);
            SEMotor = new Motor(5, true);

            NWEncoder = new Encoder(18, 14);
            NEEncoder = new Encoder(19, 15);
            SWEncoder = new Encoder(20, 16);
            SEEncoder = new Encoder(21, 17);

            UltraSonicChannel = new AnalogInput(0);

            DriverController = new Joystick();

            JSDimmer = new DutyCyclePwm(11);
            LedDimmer = new DutyCyclePwm(12);

            //Initialize collections
            Motors = new List<Motor>(new Motor[] { NWMotor, NEMotor, SWMotor, SEMotor });
            AnalogInputs = new List<AnalogInput>(new AnalogInput[] { UltraSonicChannel });
            DutyCyclePwms = new List<DutyCyclePwm>(new DutyCyclePwm[] { LedDimmer, JSDimmer });
            Joysticks = new List<Joystick>(new Joystick[] { DriverController });
            Encoders = new List<Encoder>(new Encoder[] { NWEncoder, NEEncoder, SWEncoder, SEEncoder });
        }
        private void SetupIO()
        {
            //Build IO interfaces
            var ioInt = new ArduinoInterface("COM5", 115200, 20);
            ioInt.Sections.Add(new RslModelSection(State));
            ioInt.Sections.Add(new AnalogIODataSection(AnalogInputs));
            ioInt.Sections.Add(new MotorDataSection(Motors));
            ioInt.Sections.Add(new DutyCycleSection(DutyCyclePwms));
            ioInt.Sections.Add(new EncoderDataSection(Encoders));
            //ioInt.Sections.Add(new DummySection(30));

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
                var x = DriverController.Axes[0].Deadband(0.0, 1.0, 0.1);
                var y = -DriverController.Axes[1].Deadband(0.0, 1.0, 0.1);
                var z = DriverController.Axes[3].Deadband(0.0, 1.0, 0.1);


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

                NWMotor.Value = nw;
                NEMotor.Value = ne;
                SWMotor.Value = sw;
                SEMotor.Value = se;
            }

            Console.WriteLine(State.PrimaryState.ToString() + UltraSonicChannel.Value);
        }

        protected override void DisabledInit()
        {
            foreach (Motor motor in Motors)
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
            LedDimmer.Value = dimmerPos;

            if (DriverController.Axes.Count >= 1)
            {
                JSDimmer.Value = DriverController.Axes[0];
                Console.WriteLine(State.PrimaryState.ToString() + DriverController.Axes[0]);
            }

            for (int i = 0; i < Encoders.Count; i++)
            {
                Console.WriteLine(String.Format("Encoder {0} : {1}", i, Encoders[i].Ticks));
            }
        }
    }
}
