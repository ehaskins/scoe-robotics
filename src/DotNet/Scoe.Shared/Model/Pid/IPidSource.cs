using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Scoe.Shared.Model.Pid
{
    public interface IPidSource
    {
        /// <summary>
        /// Called when PidSource value is updated.
        /// </summary>
        event PidSourceUpdatedEventHandler Updated;
    }
}