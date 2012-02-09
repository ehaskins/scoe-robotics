using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Scoe.Communication.Udp;
using System.Net;
using Scoe.Robot.Model;

namespace Scoe.DSTestConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            var state = new RobotState();
            var ep = new IPEndPoint(IPAddress.Loopback, 1150);
            var udp = new DSUdpClient(ep, 1110);

            udp.Sections.Add(new StateSection(state));
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
                }
            }
        }
    }
}
