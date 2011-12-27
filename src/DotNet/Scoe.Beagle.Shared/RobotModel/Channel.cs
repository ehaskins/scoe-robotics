using System;
using System.Collections.Generic;
using System.Linq;

namespace Scoe.Beagle.Shared.RobotModel
{
    public abstract class Channel<TValue> : NotifyObject
    {
        private TValue _Value;
        private ushort _Pin;

        public ushort Pin
        {
            get
            {
                return _Pin;
            }
            set
            {
                if (_Pin == value)
                    return;
                _Pin = value;
                RaisePropertyChanged("Pin");
            }
        }

        public TValue Value
        {
            get
            {
                return _Value;
            }
            set
            {
                _Value = value;
                RaisePropertyChanged("Value");
            }
        }
    }
}
