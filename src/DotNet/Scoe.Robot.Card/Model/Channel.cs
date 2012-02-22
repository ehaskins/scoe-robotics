using System;
using System.Collections.Generic;
using System.Linq;
using EHaskins.Utilities;
using Scoe.Shared.Model.Pid;

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
                OnUpdated();
                RaisePropertyChanged("Value");
            }
        }
        protected virtual void OnUpdated()
        {
        }
    }
}
