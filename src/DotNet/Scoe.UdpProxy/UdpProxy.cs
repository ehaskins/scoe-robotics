using System;
using System.Collections.Generic;
using System.Linq;
using Scoe.Communication;
using Scoe.Communication.Udp;
using Scoe.Shared.Model;
using Scoe.Communication.DataSections;
using Scoe.Communication.Arduino;

namespace Scoe.UdpProxy
{
    public class UdpProxy
    {
        ServerInterface _server;
        ArduinoInterface _ioInt;

        RobotState _state = new RobotState();

        List<Motor> _motors = new List<Motor>();
        List<Encoder> _encoders = new List<Encoder>();
        List<AnalogInput> _analogInputs = new List<AnalogInput>();
        List<DutyCyclePwm> _dutyCyclePwms = new List<DutyCyclePwm>();

        public void Run(string comPort, int baud)
        {
            //Command interface
            _server = new ServerInterface(new UdpProtocol(15000, 15001));

            var commonSections = new DataSection[] { new MotorSection(_motors), new EncoderSection(_encoders), new AnalogInputSection(_analogInputs), new DutyCycleSection(_dutyCyclePwms) };
            _server.Sections.Add(new StateSection(_state, true));
            foreach (var s in commonSections)
                _server.Sections.Add(s);

            _server.Connected += OnConnected;
            _server.Disconnected += OnDisconnected;

            //IO Interface
            _ioInt = new ArduinoInterface(comPort, baud);

            _ioInt.Sections.Add(new RslModelSection(_state));
            foreach (var s in commonSections)
                _ioInt.Sections.Add(s);


        }

        public void OnDisconnected(object sender, EventArgs e)
        {
            foreach (var motor in _motors)
            {
                motor.IsEnabled = false;
            }
            Console.WriteLine("UDP Disconnected");
        }

        public void OnConnected(object sender, EventArgs e)
        {
            Console.WriteLine("UDP Connected");
        }
    }
}
