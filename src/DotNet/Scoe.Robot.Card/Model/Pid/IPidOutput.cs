using System;
using System.Collections.Generic;
using System.Linq;

namespace Scoe.Shared.Model.Pid
{
    public class Pid
    {

        private void SourceUpdated(object sender, PidSourceUpdatedEventArgs e)
        {

        }

        public IPidOutput Output { get; set; }
        private IPidSource _Source;
        public IPidSource Source
        {
            get
            {
                return _Source;
            }
            set
            {
                if (_Source == value)
                    return;
                _Source.Updated -= SourceUpdated;
                _Source = value;
                _Source.Updated += SourceUpdated;
            }
        }
        public double Value { get; set; }
        public double P { get; set; }
        public double I { get; set; }
        public double D { get; set; }
        public double IMax { get; set; }

    }
    public interface IPidOutput
    {
        /// <summary>
        /// Output value. 
        /// </summary>
        double Value { get; }
    }
}
