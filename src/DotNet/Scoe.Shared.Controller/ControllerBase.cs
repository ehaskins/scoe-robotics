using System;
using System.Collections.Generic;
using System.Linq;
using EHaskins.Utilities;
using Scoe.Shared.Model;

namespace Scoe.Shared.Controller
{
    public abstract class ControllerBase : NotifyObject
    {
        public abstract void Run();
        public abstract void Stop();

        protected void FeedWatchdog(int timeout)
        {
            //TODO: Implement me!
        }
    }
}