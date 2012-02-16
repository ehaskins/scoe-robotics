using System;
using System.Collections.Generic;
using System.Linq;
using Scoe.Communication.Udp;
using System.Net;
using Scoe.Shared.Model;

namespace Scoe.DSTestConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            var state = new RobotState();
            var stick = new Joystick();
            stick.Axes.Add(0.0);

            var ep = new IPEndPoint(IPAddress.Loopback, 1150);
            var udp = new DSUdpClient(ep, 1110);

            udp.Sections.Add(new StateSection(state));
            udp.Sections.Add(new JoystickSection(new List<Joystick>(new Joystick[] { stick })));
            udp.Start();
            bool done = false;
            while (!done)
            {
                var key = Console.ReadKey();
                switch (key.Key)
                {
                    case ConsoleKey.E:
                        state.IsEStopped = !state.IsEStopped;
                        Console.WriteLine("IsEStopped:" + state.IsEStopped);
                        break;
                    case ConsoleKey.D:
                        state.IsEnabled = !state.IsEnabled;
                        Console.WriteLine("IsEnabled:" + state.IsEnabled);
                        break;
                    case ConsoleKey.A:
                        state.IsAutonomous = !state.IsAutonomous;
                        Console.WriteLine("IsAutonomous:" + state.IsAutonomous);
                        break;
                    case ConsoleKey.C:
                        state.IsDSConnected = !state.IsDSConnected;
                        Console.WriteLine("IsDSConnected:" + state.IsDSConnected);
                        break;
                    case ConsoleKey.X:
                        done = true;
                        break;
                    case ConsoleKey.UpArrow:
                        stick.Axes[0] += 0.2;
                        Console.WriteLine("Stick up:" + stick.Axes[0]);
                        break;
                    case ConsoleKey.DownArrow:
                        stick.Axes[0] -= 0.2;
                        Console.WriteLine("Stick down:" + stick.Axes[0]);
                        break;

                }
            }
        }
    }
}
