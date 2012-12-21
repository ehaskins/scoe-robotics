using System;
using System.Collections.Generic;
using System.Linq;
using Scoe.Shared.Model;
using System.Threading;
using Scoe.Communication.Arduino;

namespace ArduinoCommTest
{
    class Program
    {
        private static List<Motor> _Pwms;
        static void Main(string[] args)
        {
            var state = new RobotState();
            var arduino = new ArduinoInterface("COM7", 115200, 20);
            _Pwms = new List<Motor>();
            _Pwms.Add(new Motor(6));
            _Pwms.Add(new Motor(9));
            _Pwms.Add(new Motor(10));
            _Pwms.Add(new Motor(11));

            var thread = new Thread(UpdatePwm);
            thread.Start();
            arduino.Sections.Add(new MotorDataSection(_Pwms));
            arduino.Sections.Add(new RslModelSection(state));
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
