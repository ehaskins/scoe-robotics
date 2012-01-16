using System;
using System.Collections.Generic;
using System.Linq;
using Scoe.Robot.Shared.RobotModel;
using Scoe.Robot.Shared.Interface;
using Scoe.Robot.Card.Interface;

namespace Scoe.Robot.Card
{
    public class CardModel: RobotModel
    {
        private CardInterfaceBase _IOInterface;
        RslModelSection _rslSection;

        public CardModel(CardInterfaceBase ioInterface)
        {
            IOInterface = ioInterface;
            State = new RobotState();
            IOInterface.State = State;

            _rslSection = new RslModelSection(State);
            this.Sections.Add(_rslSection);
        }
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


        public CardInterfaceBase IOInterface
        {
            get
            {
                return _IOInterface;
            }
            set
            {
                if (_IOInterface == value)
                    return;
                _IOInterface = value;
                RaisePropertyChanged("IOInterface");
            }
        }
    }
}
