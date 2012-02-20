using System;
using System.Collections.Generic;
using System.Linq;
using EHaskins.Utilities;

namespace Scoe.Shared.Model
{
    public abstract class Channel<TID, TValue> : NotifyObject
    {
        private TValue _Value;
        private TID _Pin;

        public TID ID
        {
            get
            {
                return _Pin;
            }
            set
            {
                _Pin = value;
                RaisePropertyChanged("ID");
            }
        }

        public virtual TValue Value
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
