using System;
using System.Collections.Generic;
using System.Linq;
using SlimDX.DirectInput;
using System.Diagnostics;
namespace Scoe.DriverStation.Input
{
    public class JoystickUpdater
    {
        public JoystickUpdater(Scoe.Shared.Model.Joystick stick, SlimDX.DirectInput.Joystick sdxStick)
        {
            Joystick = stick;
            PhysicalJoystick = sdxStick;
        }

        public Scoe.Shared.Model.Joystick Joystick { get; private set; }
        public SlimDX.DirectInput.Joystick PhysicalJoystick { get; private set; }

        public void Update()
        {
            double[] physicalAxes = null;
            bool[] physicalButtons = null;
            if (PhysicalJoystick != null)
            {
                try
                {
                    if (PhysicalJoystick.Acquire().IsSuccess && PhysicalJoystick.Poll().IsSuccess && SlimDX.Result.Last.IsSuccess)
                    {
                        JoystickState state = PhysicalJoystick.GetCurrentState();
                        physicalButtons = state.GetButtons();

                        var sliders = state.GetSliders();
                        physicalAxes = new double[6 + sliders.Length];

                        physicalAxes[0] = state.X / 1000f;
                        physicalAxes[1] = state.Y / 1000f;
                        physicalAxes[2] = state.Z / 1000f;
                        physicalAxes[3] = state.RotationX / 1000f;
                        physicalAxes[4] = state.RotationY / 1000f;
                        physicalAxes[5] = state.RotationZ / 1000f;
                        for (int i = 0; i < sliders.Length; i++)
                        {
                            physicalAxes[6 + i] = sliders[i] / 1000f;
                        }
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                    //Joystick disconnected disable it.
                    //Joystick = null;
                }
            }
            if (physicalAxes == null)
                physicalAxes = new double[0];
            if (physicalButtons == null)
                physicalButtons = new bool[0];

            for (int i = 0; i < physicalAxes.Length; i++)
            {
                if (Joystick.Axes.Count < i + 1)
                    Joystick.Axes.Add(physicalAxes[i]);
                Joystick.Axes[i] = physicalAxes[i];
            }
            for (int i = physicalAxes.Length; i < Joystick.Axes.Count; i++)
            {
                Joystick.Axes.RemoveAt(i);
            }

            for (int i = 0; i < physicalButtons.Length; i++)
            {
                if (Joystick.Buttons.Count < i + 1)
                    Joystick.Buttons.Add(physicalButtons[i]);
                Joystick.Buttons[i] = physicalButtons[i];
            }

            for (int i = physicalButtons.Length; i < Joystick.Buttons.Count; i++)
            {
                Joystick.Buttons.RemoveAt(i);
            }
        }
    }
}
