using System;
using System.ComponentModel;
using System.Collections.ObjectModel;

namespace Scoe.Shared.Model
{
    public class Joystick
    {
        public Joystick()
        {
            Buttons = new ObservableCollection<bool>();
            Axes = new ObservableCollection<double>();
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

        private new ObservableCollection<bool> _Buttons;
        public new ObservableCollection<bool> Buttons
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

        public event PropertyChangedEventHandler PropertyChanged;

        protected void RaisePropertyChanged(string property)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(property));
        }
    }
}
