using System;
using System.Collections.Generic;
using System.Linq;

namespace Scoe.Robot.Shared.RobotModel
{
    public class DigitalIO : Channel<bool>
    {
        private DigitalIOMode _Mode;
        public DigitalIOMode Mode
        {
            get
            {
                return _Mode;
            }
            set
            {
                if (_Mode == value)
                    return;
                _Mode = value;
                RaisePropertyChanged("DigitalIO");
            }
        }
    }
}
