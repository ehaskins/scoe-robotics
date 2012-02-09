using System;
using System.Collections.Generic;
using System.Linq;
using Scoe.Robot.Interface;
using Scoe.Robot.Model;

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
