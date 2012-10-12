using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections.ObjectModel;
using System.Timers;
using System.Collections;

namespace Scoe.Communication
{
    public interface IInterface
    {
        void Start();
        void Stop();
        bool IsConnected { get; }
        ObservableCollection<Scoe.Communication.DataSection> Sections { get; }
    }
}
