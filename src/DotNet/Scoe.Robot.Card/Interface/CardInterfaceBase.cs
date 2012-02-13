using System;
using System.Collections.Generic;
using System.Linq;
using Scoe.Shared.Model;

namespace Scoe.Shared.Interface
{
    public abstract class CardInterfaceBase : RobotInterfaceBase
    {
        private RobotState _State;
        public RobotState State
        {
            get
            {
                return _State;
            }
            set
            {
                if (_State == value)
                    return;
                _State = value;
                RaisePropertyChanged("State");
            }
        }
    }
}
