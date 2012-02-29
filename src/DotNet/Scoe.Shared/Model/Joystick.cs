using System;
using System.ComponentModel;
using System.Collections.ObjectModel;
using EHaskins.Utilities;

namespace Scoe.Shared.Model
{
    public class Joystick : NotifyObject
    {
        public Joystick()
        {
            Buttons = new ObservableCollection<bool>();
            Axes = new ObservableCollection<double>();
        }

        private string _Name;
        public string Name
        {
            get { return _Name; }
            set
            {
                if (_Name == value)
                    return;
                _Name = value;
                RaisePropertyChanged("Name");
            }
        }
        

        ObservableCollection<double> _axes;
        public ObservableCollection<double> Axes
        {
            get { return _axes; }
            private set
            {
                if (_axes == value)
                    return;
                _axes = value;

                RaisePropertyChanged("Axes");
            }
        }

        private ObservableCollection<bool> _Buttons;
        public ObservableCollection<bool> Buttons
        {
            get { return _Buttons; }
            private set
            {
                if (_Buttons == value)
                    return;
                _Buttons = value;
                RaisePropertyChanged("Buttons");
            }
        }
    }
}
