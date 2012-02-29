using System;
using System.Collections.Generic;
using System.Linq;
using Scoe.Communication.Udp;
using Scoe.Shared.Model;
using Scoe.Communication.Arduino;
using Scoe.Communication;
using Scoe.Communication.DataSections;
using Scoe.Shared.Model.Pid;

namespace Scoe.Robot.KiwiDemo
{
    public class KiwiModel
    {
        RobotState State;
        public KiwiModel(RobotState state, string port, int baudRate)
        {
            State = state;
            SetupModel();
            SetupIO(port, baudRate);
        }

        public Joystick DriverController;

        public Motor WestMotor;
        public Motor EastMotor;
        public Motor SouthMotor;

        public VelocityPid WestPid;
        public VelocityPid EastPid;
        public VelocityPid SouthPid;

        public WheelEncoder WestEncoder;
        public WheelEncoder EastEncoder;
        public WheelEncoder SouthEncoder;

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
            var antiBuzz = 0.15;
            WestMotor = new Motor(2, true, controllerDeadband: antiBuzz);
            EastMotor = new Motor(3, false, controllerDeadband: antiBuzz);
            SouthMotor = new Motor(4, true, controllerDeadband: antiBuzz);

            var wheelDia = 0.1524; //diameter(6 inches) in meters
            int ticks360 = (int)((360 * 4) / (Math.PI * wheelDia));
            int ticks250 = (int)((250 * 4) / (Math.PI * wheelDia));

            WestEncoder = new WheelEncoder(21, 17, ticks360, true);
            EastEncoder = new WheelEncoder(20, 16, ticks360);
            SouthEncoder = new WheelEncoder(19, 15, ticks250, true);

            var i = 0;
            var p = 1;
            var d = 0;
            WestPid = new VelocityPid() { Source = WestEncoder, Output = WestMotor, P = p, I = i, D = d, OutputMax = 1, OutputMin = -1 };
            EastPid = new VelocityPid() { Source = EastEncoder, Output = EastMotor, P = p, I = i, D = d, OutputMax = 1, OutputMin = -1 };
            SouthPid = new VelocityPid() { Source = SouthEncoder, Output = SouthMotor, P = p, I = i, D = d, OutputMax = 1, OutputMin = -1 };

            UltraSonicChannel = new AnalogInput(0);

            DriverController = new Joystick();

            JSDimmer = new DutyCyclePwm(11);
            LedDimmer = new DutyCyclePwm(12);

            //Initialize collections
            Motors = new List<Motor>(new Motor[] { WestMotor, EastMotor, SouthMotor });
            AnalogInputs = new List<AnalogInput>(new AnalogInput[] { UltraSonicChannel });
            DutyCyclePwms = new List<DutyCyclePwm>(new DutyCyclePwm[] { LedDimmer, JSDimmer });
            Joysticks = new List<Joystick>(new Joystick[] { DriverController });
            Encoders = new List<Encoder>(new Encoder[] { WestEncoder, EastEncoder, SouthEncoder });
        }

        private void SetupIO(string port, int baudRate)
        {
            //Build IO interfaces
            var ioInt = new ArduinoInterface(port, baudRate, 20);
            ioInt.Sections.Add(new RslModelSection(State));
            ioInt.Sections.Add(new AnalogInputSection(AnalogInputs));
            ioInt.Sections.Add(new MotorSection(Motors));
            ioInt.Sections.Add(new DutyCycleSection(DutyCyclePwms));
            ioInt.Sections.Add(new EncoderSection(Encoders));
            ioInt.Sections.Add(new DummySection(50));


            ioInt.Start();

            var ctrlInt = new ServerInterface(new UdpProtocol(1150, 1110, null));
            ctrlInt.Connected += (source, e) => State.IsDSConnected = true;
            ctrlInt.Disconnected += (source, e) => State.IsDSConnected = false;

            ctrlInt.Sections.Add(new StateSection(State));
            ctrlInt.Sections.Add(new JoystickSection(Joysticks));

            ctrlInt.Start();
        }
        public void WriteVelocities()
        {
            for (int i = 0; i < Encoders.Count; i++)
            {
                var enc = Encoders[i] as WheelEncoder;
                Console.WriteLine(String.Format("Encoder {0:f0} : {1:f2}", i, enc.Velocity));
            }
        }
    }
}
