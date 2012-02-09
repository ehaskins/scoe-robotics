using System;
using System.Collections.Generic;
using System.Linq;

namespace Scoe.Shared.Model
{
    public class PwmOutput : Channel<byte>
    {
        public PwmOutput() { }
        public PwmOutput(byte pin) { Pin = pin; }
        private bool _Enabled;
        public bool Enabled
        {
            get { return _Enabled; }
            set
            {
                if (_Enabled == value)
                    return;
                _Enabled = value;
                RaisePropertyChanged("enabled");
            }
        }
        
    }
}
