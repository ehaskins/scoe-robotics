using System;
using System.Collections.Generic;
using System.Linq;
using Scoe.Robot.Interface.Arduino;
using Scoe.Robot.Model;
using Scoe.Communication.Udp;

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


            //Initialize collections
            PwmOutputs = new List<PwmOutput>();
            AnalogInputs = new List<AnalogInput>();
            //Add objects to collections
            PwmOutputs.AddRange(new PwmOutput[] { NWMotor, NEMotor, SWMotor, SEMotor });
            AnalogInputs.Add(UltraSonicChannel);
        }

        public PwmOutput NWMotor { get; private set; }
        public PwmOutput NEMotor { get; private set; }
        public PwmOutput SWMotor { get; private set; }
        public PwmOutput SEMotor { get; private set; }

        public AnalogInput UltraSonicChannel { get; private set; }

        public List<AnalogInput> AnalogInputs { get; private set; }
        public List<PwmOutput> PwmOutputs { get; private set; }
    }

    public class Controller : IterativeControllerBase<MecanumDemoModel>
    {
        public Controller()
        {
            Robot = new MecanumDemoModel();

            //Build IO interface and robot model
            var ioInt = new ArduinoInterface("COM7", 115200, 20);
            ioInt.Sections.Add(new RslModelSection(Robot.State));
            ioInt.Sections.Add(new AnalogIODataSection(Robot.AnalogInputs));
            ioInt.Sections.Add(new PwmDataSection(Robot.PwmOutputs));

            var ctrlInt = new UdpServer(1150, 1110);
            ctrlInt.Sections.Add(new StateSection(Robot.State));

            Robot.State.IsDSConnected = true;
            Robot.State.IsEnabled = true;
            ioInt.Start();
            ctrlInt.Start();
            //Build driver interface
            //var dsInt = new FrcCommInterface(Robot.DSData);

        }


        protected override void EnabledLoop()
        {
            //TODO:Impletment drive code here!

            Console.WriteLine("E " + Robot.UltraSonicChannel.Value);
        }

        protected override void DisabledLoop()
        {
            Console.WriteLine("D " + Robot.UltraSonicChannel.Value);
        }
    }
}
