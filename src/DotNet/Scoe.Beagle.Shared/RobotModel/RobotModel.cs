using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EHaskins.Frc.Communication.Robot;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using EHaskins.Utilities.Binary;
using EHaskins.Utilities;

namespace Scoe.Robot.Shared.RobotModel
{
    public class RobotModel : NotifyObject
    {
        public RobotModel()
        {
            Sections = new ObservableCollection<ArduinoDataSection>();
        }
        private ObservableCollection<ArduinoDataSection> _Sections;
        public ObservableCollection<ArduinoDataSection> Sections
        {
            get { return _Sections; }
            protected set
            {
                if (_Sections == value)
                    return;
                _Sections = value;
            }
        }
        
    }
}