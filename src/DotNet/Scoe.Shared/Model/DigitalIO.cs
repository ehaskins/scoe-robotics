using System;
using System.Collections.Generic;
using System.Linq;

namespace Scoe.Shared.Model
{
    public class DigitalIO : Channel<byte, bool>
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
