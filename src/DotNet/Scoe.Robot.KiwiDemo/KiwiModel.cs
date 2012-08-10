using System;
using System.Collections.Generic;
using System.Linq;
using Scoe.Communication.Udp;
using Scoe.Shared.Model;
using Scoe.Communication.Arduino;
using Scoe.Communication;
using Scoe.Communication.DataSections;
using Scoe.Shared.Model.Pid;
using System.Net;

namespace Scoe.Robot.KiwiDemo
{
    public class KiwiModel
    {
        RobotState State;
        public KiwiModel(RobotState state, IInterface ioInterface, Interface controlInterface)
        {
            State = state;
            SetupModel();
            IOInterface = ioInterface;
            ControlInterface = controlInterface;
            SetupIO();
        }

        public Joystick DriverController;

        public Motor WestMotor;
        public Motor EastMotor;
        public Motor SouthMotor;

        public WheelEncoder WestEncoder;
        public WheelEncoder EastEncoder;
        public WheelEncoder SouthEncoder;

        public AnalogInput UltraSonicChannel;
        public Accelerometer Accelerometer;
        public Gyro Gyro;

        public DutyCyclePwm LedDimmer;
        public DutyCyclePwm JSDimmer;

        public List<Joystick> Joysticks;
        public List<AnalogInput> AnalogInputs;
        public List<Motor> Motors;
        public List<DutyCyclePwm> DutyCyclePwms;
        public List<Encoder> Encoders;

        public Interface ControlInterface;
        public IInterface IOInterface;

        private void SetupModel()
        {
            //Build objects
            var antiBuzz = 0.10;
            WestMotor = new Motor(2, true, controllerDeadband: antiBuzz);
            EastMotor = new Motor(3, false, controllerDeadband: antiBuzz);
            SouthMotor = new Motor(4, true, controllerDeadband: antiBuzz);

            var wheelDia = 0.1524; //diameter(6 inches) in meters
            int ticks360 = (int)((360 * 4) / (Math.PI * wheelDia));
            int ticks250 = (int)((250 * 4) / (Math.PI * wheelDia));

            WestEncoder = new WheelEncoder(21, 18, ticks360, true);
            EastEncoder = new WheelEncoder(20, 17, ticks360);
            SouthEncoder = new WheelEncoder(19, 16, ticks250, true);

            UltraSonicChannel = new AnalogInput(15);
            var accelXIn = new AnalogInput(0, maxVolts: 3.3);
            var accelYIn = new AnalogInput(1, maxVolts: 3.3);
            var accelZIn = new AnalogInput(2, maxVolts: 3.3);

            Accelerometer = new AnalogAccelerometer(
                new AnalogAccelerometerChannel() { Input = accelXIn, VoltPerG = 1, CenterValue = (3.3 / 2) },
                new AnalogAccelerometerChannel() { Input = accelYIn, VoltPerG = 1, CenterValue = (3.3 / 2) },
                new AnalogAccelerometerChannel() { Input = accelZIn, VoltPerG = 1, CenterValue = (3.3 / 2) }
                );

            DriverController = new Joystick();

            JSDimmer = new DutyCyclePwm(11);
            LedDimmer = new DutyCyclePwm(12);

            //Initialize collections
            Motors = new List<Motor>(new Motor[] { WestMotor, EastMotor, SouthMotor });
            AnalogInputs = new List<AnalogInput>(new AnalogInput[] { UltraSonicChannel, accelXIn, accelYIn, accelZIn });
            DutyCyclePwms = new List<DutyCyclePwm>(new DutyCyclePwm[] { LedDimmer, JSDimmer });
            Joysticks = new List<Joystick>(new Joystick[] { DriverController });
            Encoders = new List<Encoder>(new Encoder[] { WestEncoder, EastEncoder, SouthEncoder });
        }

        private void SetupIO()
        {
            IOInterface.Sections.Add(new RslModelSection(State));
            IOInterface.Sections.Add(new AnalogInputSection(AnalogInputs));
            IOInterface.Sections.Add(new MotorSection(Motors));
            IOInterface.Sections.Add(new DutyCycleSection(DutyCyclePwms));
            IOInterface.Sections.Add(new EncoderSection(Encoders));

            IOInterface.Start();

            ControlInterface.Connected += (source, e) => State.IsDSConnected = true;
            ControlInterface.Disconnected += (source, e) => State.IsDSConnected = false;

            ControlInterface.Sections.Add(new StateSection(State));
            ControlInterface.Sections.Add(new JoystickSection(Joysticks));

            ControlInterface.Start();
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
