using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections.ObjectModel;

namespace Scoe.Beagle.Shared.RobotModel
{
    public class GroundRobotModel : RobotModel
    {
        private ObservableCollection<Encoder> _Encoders;
        private int _NumEncoders;
        public int NumEncoders
        {
            get
            {
                return _NumEncoders;
            }
            set
            {
                if (_NumEncoders == value)
                    return;
                _NumEncoders = value;
                RaisePropertyChanged("NumEncoders");
            }
        }
        public ObservableCollection<Encoder> Encoders
        {
            get
            {
                return _Encoders;
            }
            protected set
            {
                if (_Encoders == value)
                    return;
                _Encoders = value;
                RaisePropertyChanged("Encoders");
            }
        }

    }
}
