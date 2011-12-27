using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel;
using System.Diagnostics;

namespace Scoe.Beagle.Shared
{
    public class NotifyObject
    {

        public bool IsINotifyPropertyChangedEnabled { get; set; }
#if DEBUG
        List<String> verifiedProperties = new List<string>();
#endif
        [DebuggerStepThrough()]
        protected void RaisePropertyChanged(string prop)
        {
            if (IsINotifyPropertyChangedEnabled)
            {
                if (PropertyChanged != null)
                {
                    //Verify the property actually exists on this object.
#if DEBUG
                    if (verifiedProperties.Contains(prop) && this.GetType().GetProperty(prop) != null)
                    {
                        verifiedProperties.Add(prop);
                    }
                    else
                        throw new InvalidOperationException(prop + " is not a property on this object.");
#endif
                    PropertyChanged(this, new PropertyChangedEventArgs(prop));
                }
            }
        }
        public event PropertyChangedEventHandler PropertyChanged;
    }
}
