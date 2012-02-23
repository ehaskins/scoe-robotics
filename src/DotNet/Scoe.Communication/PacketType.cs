using System;
using System.Collections.Generic;
using System.Linq;

namespace Scoe.Communication
{
    public enum PacketType : byte
    {
        Probe = 0,
        Echo = 1,
        Command = 2,
        Status = 3
    }
}
