using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections.ObjectModel;
using Scoe.Robot.Interface;

namespace Scoe.Robot.Model
{
    public class CardModelBase : Scoe.Robot.Model.RobotModel
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