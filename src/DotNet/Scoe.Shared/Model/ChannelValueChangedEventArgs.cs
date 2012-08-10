using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Scoe.Shared.Model
{
    public class ChannelValueChangedEventArgs<T> : EventArgs
    {
        public T OldValue;
        public T NewValue;

        /// <summary>
        /// Initializes a new instance of the ChannelValueChangedEventArgs class.
        /// </summary>
        /// <param name="oldValue"></param>
        /// <param name="newValue"></param>
        public ChannelValueChangedEventArgs(T oldValue, T newValue)
        {
            OldValue = oldValue;
            NewValue = newValue;
        }
    }
}
