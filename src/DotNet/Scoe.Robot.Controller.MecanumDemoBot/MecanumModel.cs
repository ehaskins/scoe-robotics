using System;
using System.Collections.Generic;
using System.Linq;
using Scoe.Communication.Udp;
using Scoe.Shared.Model;
using Scoe.Communication.Arduino;

namespace Scoe.Robot.MecanumDemoBot
{
    public class MecanumModel
    {
        RobotState State;
        public MecanumModel(RobotState state, string port, int baudRate)
        {
            State = state;
            SetupModel();
            SetupIO(port, baudRate);
        }

        public Joystick DriverController;

        public Motor NWMotor;
        public Motor NEMotor;
        public Motor SWMotor;
        public Motor SEMotor;

        public WheelEncoder NWEncoder;
        public WheelEncoder NEEncoder;
        public WheelEncoder SWEncoder;
        public WheelEncoder SEEncoder;

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
            NWMotor = new Motor(4);
            NEMotor = new Motor(5, true);
            SWMotor = new Motor(2, true);
            SEMotor = new Motor(3);

            var wheelDia = 0.2032; //diameter in meters
            int ticks360 = (int)((360*4) / (Math.PI * wheelDia));
            int ticks250 = (int)((250*4) / (Math.PI * wheelDia));

            NWEncoder = new WheelEncoder(18, 14, ticks360);
            NEEncoder = new WheelEncoder(19, 15, ticks250, true);
            SWEncoder = new WheelEncoder(20, 16, ticks360);
            SEEncoder = new WheelEncoder(21, 17, ticks250, true);

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
        private void SetupIO(string port, int baudRate)
        {
            //Build IO interfaces
            var ioInt = new ArduinoInterface(port, baudRate, 20);
            ioInt.Sections.Add(new RslModelSection(State));
            ioInt.Sections.Add(new AnalogIODataSection(AnalogInputs));
            ioInt.Sections.Add(new MotorDataSection(Motors));
            ioInt.Sections.Add(new DutyCycleSection(DutyCyclePwms));
            ioInt.Sections.Add(new EncoderDataSection(Encoders));
            ioInt.Sections.Add(new DummySection(50));
            //ioInt.Sections.Add(new DummySection(30));

            var ctrlInt = new UdpServer(1150, 1110);
            ctrlInt.Connected += (source, e) => State.IsDSConnected = true;
            ctrlInt.Disconnected += (source, e) => State.IsDSConnected = false;

            ctrlInt.Sections.Add(new StateSection(State));
            ctrlInt.Sections.Add(new JoystickSection(Joysticks));

            ioInt.Start();
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
