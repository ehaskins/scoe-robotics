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
                var old = _Value;
                _Value = value;
                OnUpdated(old, value);
                RaisePropertyChanged("Value");
            }
        }

        public event EventHandler<ChannelValueChangedEventArgs<TValue>> ValueChanged;
        protected virtual void OnUpdated(TValue oldValue, TValue newValue)
        {
            if (ValueChanged != null)
                ValueChanged(this, new ChannelValueChangedEventArgs<TValue>(oldValue, newValue));
        }
    }
}
