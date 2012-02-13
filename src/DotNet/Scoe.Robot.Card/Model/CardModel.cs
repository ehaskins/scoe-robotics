using System;
using System.Collections.Generic;
using System.Linq;

namespace Scoe.Shared.Model
{
    public class CardModelBase : RobotModel
    {
        public CardModelBase() { State = new RobotState(); }

        private RobotState _state;
        public RobotState State
        {
            get
            {
                return _state;
            }
            set
            {
                if (_state == value)
                    return;
                _state = value;
                RaisePropertyChanged("State");
            }
        }
    }
}