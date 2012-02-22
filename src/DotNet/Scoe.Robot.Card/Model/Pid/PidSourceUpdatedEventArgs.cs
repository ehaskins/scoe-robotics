using System;
using System.Collections.Generic;
using System.Linq;

namespace Scoe.Shared.Model.Pid
{
    public class PidSourceUpdatedEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the PidSourceUpdatedEventArgs class.
        /// </summary>
        public PidSourceUpdatedEventArgs(double value)
        {
            Value = value;
        }

        /// <summary>
        /// Current value of the PisSource.
        /// </summary>
        public double Value { get; set; }
    }
}
