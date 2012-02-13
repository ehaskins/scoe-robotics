using System;
using System.Collections.Generic;
using System.Linq;
using Scoe.Communication.Udp;
using Scoe.Shared.Model;

namespace Scoe.Robot.Controller.MecanumDemoBot
{
    public class JoystickSection : UdpDataSection
    {
        public JoystickSection(List<Joystick> joysticks) : base(1)
        {
            Joysticks = joysticks;
        }
        public List<Joystick> Joysticks { get; set; }
        public override void GetData(ref byte[] data, ref int offset)
        {
            throw new NotImplementedException();
        }
        public override void Update(byte[] data, int offset)
        {
            throw new NotImplementedException();
        }
    }
}
