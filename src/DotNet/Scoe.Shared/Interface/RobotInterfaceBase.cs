using System;
using System.Collections.Generic;
using System.Linq;
using EHaskins.Utilities;

namespace Scoe.Shared.Interface
{
    public abstract class RobotInterfaceBase: NotifyObject
    {
        public RobotInterfaceBase()
        {

        }

        public abstract void Start();
        public abstract void Stop();


    }
}
