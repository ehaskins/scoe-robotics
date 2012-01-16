using System;
using System.Collections.Generic;
using System.Linq;

namespace Scoe.Robot.Shared.RobotModel
{
    public class PwmOutput : Channel<byte>
    {
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
