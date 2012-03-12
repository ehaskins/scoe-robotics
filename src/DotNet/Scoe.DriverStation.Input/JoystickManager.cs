using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SlimDX.DirectInput;

namespace Scoe.DriverStation.Input
{
    public class JoystickManager
    {
        private static DirectInput _input;
        public static DirectInput Input
        {
            get
            {
                if (_input == null)
                    _input = new DirectInput();
                return _input;
            }
            set
            {
                _input = value;
            }
        }
        public static Joystick[] GetSticks()
        {
            var sticks = new List<SlimDX.DirectInput.Joystick>();
            foreach (DeviceInstance device in Input.GetDevices(DeviceClass.GameController, DeviceEnumerationFlags.AttachedOnly))
            {
                // create the device
                try
                {
                    var stick = new SlimDX.DirectInput.Joystick(Input, device.InstanceGuid);
                    stick.Acquire();

                    foreach (DeviceObjectInstance deviceObject in stick.GetObjects())
                    {
                        if ((deviceObject.ObjectType & ObjectDeviceType.Axis) != 0)
                            stick.GetObjectPropertiesById((int)deviceObject.ObjectType).SetRange(-1000, 1000);
                    }

                    sticks.Add(stick);

                    Console.WriteLine(stick.Information.InstanceName);
                }
                catch (DirectInputException)
                {
                }
            }
            return sticks.ToArray();
        }
    }
}
