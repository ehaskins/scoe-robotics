using System;
using System.Collections.Generic;
using System.Linq;
using Scoe.Shared.Model;
using EHaskins.Utilities.Extensions;
namespace Scoe.Communication.Udp
{
    public class JoystickSection : UdpDataSection
    {
        public JoystickSection(IList<Joystick> joysticks) : base(1)
        {
            Joysticks = joysticks;
        }
        public IList<Joystick> Joysticks { get; set; }
        public override void GetData(ref byte[] data, ref int offset)
        {
            var count = Joysticks.Count.Limit(0, byte.MaxValue);

            data[offset++] = (byte)count;
            for (int i = 0; i < count; i++)
            {
                var stick = Joysticks[i];
                int axisCount = stick.Axes.Count.Limit(0, byte.MaxValue);
                data[offset++] = (byte)axisCount;
                for (int iAxis = 0; iAxis < axisCount; iAxis++)
                {
                    data[offset++] = (byte)(((stick.Axes[iAxis] + 1) * 127).Limit(0, byte.MaxValue));
                }
            }
        }

        public override void Update(byte[] data, int offset)
        {
            var count = data[offset++];
            for (int i = 0; i < count; i++)
            {
                Joystick stick;
                if (Joysticks.Count < i+1){
                    stick = new Joystick();
                    Joysticks.Add(stick);
                }
                else
                    stick = Joysticks[i];

                var axes = data[offset++];
                for (int iAxis = 0; iAxis < axes; iAxis++)
                {
                    var value = (data[offset++] / 127.0) - 1.0;
                    if (stick.Axes.Count < iAxis + 1)
                    {
                        stick.Axes.Add(value);
                    }
                    stick.Axes[i] = value;
                }
            }
        }
    }
}
