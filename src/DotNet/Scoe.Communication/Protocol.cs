using System;
using System.Collections.Generic;
using System.Linq;
using EHaskins.Utilities;
namespace Scoe.Communication
{
    public abstract class Protocol
    {
        public abstract void Start();
        public abstract void Stop();
        public abstract void Transmit(Packet packet);
        public bool IsEnabled { get; protected set; }
        public Method<Packet> Received;
    }
}
