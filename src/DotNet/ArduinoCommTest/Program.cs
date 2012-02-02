using System;
using System.Collections.Generic;
using System.Linq;
using Scoe.Robot.Interface.Arduino;
using Scoe.Robot.Model;
namespace ArduinoCommTest
{
    class Program
    {
        static void Main(string[] args)
        {
            var arduino = new ArduinoInterface("COM7", 115200, 20);
            var model = new CardModelBase();
            arduino.Start();

            bool done = false;
            while (!done)
            {
                var key = Console.ReadKey();
                switch (key.Key)
                {
                    case ConsoleKey.E:
                        model.State.IsEStopped = !model.State.IsEStopped;
                        Console.WriteLine("IsEStopped:" + model.State.IsEStopped);
                        break;
                    case ConsoleKey.D:
                        model.State.IsEnabled = !model.State.IsEnabled;
                        Console.WriteLine("IsEnabled:" + model.State.IsEnabled);
                        break;
                    case ConsoleKey.A:
                        model.State.IsAutonomous = !model.State.IsAutonomous;
                        Console.WriteLine("IsAutonomous:" + model.State.IsAutonomous);
                        break;
                    case ConsoleKey.C:
                        model.State.IsDSConnected = !model.State.IsDSConnected;
                        Console.WriteLine("IsDSConnected:" + model.State.IsDSConnected);
                        break;
                    case ConsoleKey.X:
                        done = true;
                        break;
                }
            }
            arduino.Stop();
        }
    }
}
