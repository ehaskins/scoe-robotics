using System;
using System.Collections.Generic;
using System.Linq;
using Scoe.Robot.Interface.Arduino;
using Scoe.Communication.Udp;
using Scoe.Shared.Model;
using Scoe.Shared.Controller;
using EHaskins.Utilities.NumericExtensions;

namespace Scoe.Robot.Controller.MecanumDemoBot
{
    public class MecanumDemoModel : CardModelBase
    {
        public MecanumDemoModel()
        {
            //Build objects
            NWMotor = new PwmOutput(1);
            NEMotor = new PwmOutput(2);
            SWMotor = new PwmOutput(3);
            SEMotor = new PwmOutput(4);

            UltraSonicChannel = new AnalogInput(0);

            DriverController = new XBoxController();

            //Initialize collections
            PwmOutputs = new List<PwmOutput>();
            AnalogInputs = new List<AnalogInput>();

            //Add objects to collections
            Joysticks = new List<Joystick>(new Joystick[] { DriverController });
            PwmOutputs.AddRange(new PwmOutput[] { NWMotor, NEMotor, SWMotor, SEMotor });
            AnalogInputs.Add(UltraSonicChannel);
        }

        public Joystick DriverController { get; private set; }

        public PwmOutput NWMotor { get; private set; }
        public PwmOutput NEMotor { get; private set; }
        public PwmOutput SWMotor { get; private set; }
        public PwmOutput SEMotor { get; private set; }

        public AnalogInput UltraSonicChannel { get; private set; }

        public List<Joystick> Joysticks { get; private set; }
        public List<AnalogInput> AnalogInputs { get; private set; }
        public List<PwmOutput> PwmOutputs { get; private set; }
    }

    public class Controller : IterativeControllerBase<MecanumDemoModel>
    {
        public Controller()
        {
            //Create robotmodel
            Robot = new MecanumDemoModel();

            //Build IO interfaces
            var ioInt = new ArduinoInterface("COM7", 115200, 20);
            ioInt.Sections.Add(new RslModelSection(Robot.State));
            ioInt.Sections.Add(new AnalogIODataSection(Robot.AnalogInputs));
            ioInt.Sections.Add(new PwmDataSection(Robot.PwmOutputs));

            var ctrlInt = new UdpServer(1150, 1110);
            ctrlInt.Sections.Add(new StateSection(Robot.State) { PrimaryInterface = ctrlInt });
            ctrlInt.Sections.Add(new JoystickSection(Robot.Joysticks));

            ioInt.Start();
            ctrlInt.Start();
        }


        protected override void EnabledLoop()
        {
            var x = Robot.DriverController.Axes[0];
            var y = Robot.DriverController.Axes[1];
            var z = Robot.DriverController.Axes[2];

            var nw = y + x + z;
            var ne = y - x - z;
            var sw = y - x + z;
            var se = y + x - z;

            Robot.NWMotor.Value = (byte)nw.Map(-1, 1, 0, 255);
            Robot.NEMotor.Value = (byte)ne.Map(-1, 1, 0, 255); ;
            Robot.SWMotor.Value = (byte)sw.Map(-1, 1, 0, 255); ;
            Robot.SEMotor.Value = (byte)se.Map(-1, 1, 0, 255); ;

            Console.WriteLine("E " + Robot.UltraSonicChannel.Value);
        }

        protected override void DisabledLoop()
        {
            Console.WriteLine("D " + Robot.UltraSonicChannel.Value);
        }
    }
}
