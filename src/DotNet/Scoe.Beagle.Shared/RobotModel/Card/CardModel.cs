using System;
using System.Collections.Generic;
using System.Linq;

namespace Scoe.Beagle.Shared.RobotModel.Card
{
    public class CardModel : RobotModel
    {
        private RobotState _RobotState;
        public RobotState RobotState
        {
            get
            {
                return _RobotState;
            }
            set
            {
                if (_RobotState == value)
                    return;
                _RobotState = value;
                RaisePropertyChanged("RobotState");
            }
        }
    }
}
