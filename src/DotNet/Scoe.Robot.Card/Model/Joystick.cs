using System;
using System.ComponentModel;
using System.Collections.ObjectModel;

namespace Scoe.Shared.Model
{
    public class Joystick
    {
        public const int NUM_BYTES = 8;
        public const int NUM_AXES = 6;

        public Joystick()
        {
            Buttons = new ObservableCollection<bool>();
            for (int i = 0; i < NUM_AXES; i++)
            {
                _analogInputs[i] = 0;
            }
        }


        double[] _analogInputs = new double[NUM_AXES];

        public double[] Axes
        {
            get { return _analogInputs; }
            set
            {
                if (_analogInputs == value)
                    return;
                _analogInputs = value;

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
