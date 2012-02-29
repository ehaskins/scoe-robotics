using System;
using System.Collections.Generic;
using System.Linq;

namespace Scoe.Shared.Model.Pid
{
    public interface IPidOutput
    {
        /// <summary>
        /// Output value. 
        /// </summary>
        double Value { get; set; }
    }
}
