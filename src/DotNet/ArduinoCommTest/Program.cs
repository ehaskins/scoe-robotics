using System;
using System.Collections.Generic;
using System.Linq;
using Scoe.Robot.Interface.Arduino;
using Scoe.Shared.Model;
using System.Threading;

namespace ArduinoCommTest
{
    class Program
    {
        private static List<PwmOutput> _Pwms;
        static void Main(string[] args)
        {
            var arduino = new ArduinoInterface("COM7", 115200, 20);
            var model = new CardModelBase();
            _Pwms = new List<PwmOutput>();
            _Pwms.Add(new PwmOutput(6));
            _Pwms.Add(new PwmOutput(9));
            _Pwms.Add(new PwmOutput(10));
            _Pwms.Add(new PwmOutput(11));

            var thread = new Thread(UpdatePwm);
            thread.Start();
            arduino.Sections.Add(new PwmDataSection(_Pwms));
            arduino.Sections.Add(new RslModelSection(model.State));
            arduino.Start();

            foreach (var pwm in _Pwms)
            {
                pwm.Value = 127;
                pwm.IsEnabled = true;
            }

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
                    case ConsoleKey.Spacebar:
                        if (position == 127)
                            position = 110;
                        else
                            position = 127;


                        foreach (var pwm in _Pwms)
                        {
                            pwm.IsEnabled = !pwm.IsEnabled;
                        }
                        break;

                }
            }
            arduino.Stop();
        }

        static int direction = 1;
        static int position = 127;
        static int min = 127 - 20;
        static int max = 127 + 20;
        static void UpdatePwm()
        {
            while (true)
            {
                if (position < min || position > max)
                    direction *= -1;
                position += direction;

                foreach (var pwm in _Pwms)
                {
                    pwm.Value = (byte)position;
                }

                Thread.Sleep(75);
            }
        }
    }
}
