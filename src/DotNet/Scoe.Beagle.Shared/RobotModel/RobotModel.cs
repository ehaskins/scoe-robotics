using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EHaskins.Frc.Communication.Robot;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;

namespace Scoe.Beagle.Shared.RobotModel
{
    public class RobotModel : NotifyObject
    {
        public RobotModel()
        {
            _AnalogInputs = new ObservableCollection<AnalogInput>();
            _DigitalInputs = new ObservableCollection<DigitalIO>();
            _PwmOutputs = new ObservableCollection<PwmOutput>();
        }

        private ObservableCollection<AnalogInput> _AnalogInputs;
        private ObservableCollection<DigitalIO> _DigitalInputs;
        private ObservableCollection<PwmOutput> _PwmOutputs;
        private int _NumPwms;
        private int _NumAnalogInputs;
        private int _NumDigitalIO;

        public int NumDigitalIO
        {
            get
            {
                return _NumDigitalIO;
            }
            protected set
            {
                if (_NumDigitalIO == value)
                    return;
                _NumDigitalIO = value;
                RaisePropertyChanged("NumDigitalIO");
            }
        }
        public ObservableCollection<DigitalIO> DigitalInputs
        {
            get
            {
                return _DigitalInputs;
            }
            protected set
            {
                if (_DigitalInputs == value)
                    return;
                _DigitalInputs = value;
                RaisePropertyChanged("DigitalInputs");
            }
        }

        public int NumAnalogInputs
        {
            get
            {
                return _NumAnalogInputs;
            }
            protected set
            {
                if (_NumAnalogInputs == value)
                    return;
                _NumAnalogInputs = value;
                RaisePropertyChanged("NumAnalogInputs");
            }
        }
        public ObservableCollection<AnalogInput> AnalogInputs
        {
            get
            {
                return _AnalogInputs;
            }
            protected set
            {
                if (_AnalogInputs == value)
                    return;
                _AnalogInputs = value;
                RaisePropertyChanged("AnalogInputs");
            }
        }

        public int NumPwms
        {
            get
            {
                return _NumPwms;
            }
            protected set
            {
                if (_NumPwms == value)
                    return;
                _NumPwms = value;
                RaisePropertyChanged("NumPwms");
            }
        }
        public ObservableCollection<PwmOutput> PwmOutputs
        {
            get { return _PwmOutputs; }
            protected set
            {
                if (_PwmOutputs == value)
                    return;
                _PwmOutputs = value;
                RaisePropertyChanged("PwmOutputs");
            }
        }
        
    }
}
