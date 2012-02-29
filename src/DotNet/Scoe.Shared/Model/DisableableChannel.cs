using System;
using System.Collections.Generic;
using System.Linq;

namespace Scoe.Shared.Model
{
    public class DisableableChannel<TID, TValue> : Channel<TID, TValue>
    {
        private bool _Enabled;
        public bool IsEnabled
        {
            get
            {
                return _Enabled;
            }
            set
            {
                if (_Enabled == value)
                    return;
                _Enabled = value;
                RaisePropertyChanged("IsEnabled");
            }
        }
    }
}
