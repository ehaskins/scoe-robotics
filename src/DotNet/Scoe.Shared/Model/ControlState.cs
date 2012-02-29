using System;
using System.Collections.Generic;
using System.Linq;

namespace Scoe.Shared.Model
{
    public enum ControlState
    {
        Disabled = 0,
        Teleop = 1,
        Autonomous = 2,
        EStopped = 3,
        NoDriverStation = 4
    }
}
