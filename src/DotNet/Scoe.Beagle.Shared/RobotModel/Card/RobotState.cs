using System;
using System.Collections.Generic;
using System.Linq;

namespace Scoe.Beagle.Shared.RobotModel.Card
{
    public class RobotState : NotifyObject
    {
        private bool _IsIODeviceConnected;
        private bool _IsDSConnected;
        private bool _IsEStopped;
        private bool _IsAutonomous;
        private bool _IsEnabled;
        public bool IsEnabled
        {
            get
            {
                return _IsEnabled;
            }
            set
            {
                if (_IsEnabled == value)
                    return;
                _IsEnabled = value;
                RaisePropertyChanged("IsEnabled");
            }
        }
        public bool IsAutonomous
        {
            get
            {
                return _IsAutonomous;
            }
            set
            {
                if (_IsAutonomous == value)
                    return;
                _IsAutonomous = value;
                RaisePropertyChanged("IsAutonomous");
            }
        }
        public bool IsEStopped
        {
            get
            {
                return _IsEStopped;
            }
            set
            {
                if (_IsEStopped == value)
                    return;
                _IsEStopped = value;
                RaisePropertyChanged("IsEStopped");
            }
        }
        public bool IsDSConnected
        {
            get
            {
                return _IsDSConnected;
            }
            set
            {
                if (_IsDSConnected == value)
                    return;
                _IsDSConnected = value;
                RaisePropertyChanged("IsDSConnected");
            }
        }
        public bool IsIODeviceConnected
        {
            get
            {
                return _IsIODeviceConnected;
            }
            set
            {
                if (_IsIODeviceConnected == value)
                    return;
                _IsIODeviceConnected = value;
                RaisePropertyChanged("IsIODeviceConnected");
            }
        }
    }
}
