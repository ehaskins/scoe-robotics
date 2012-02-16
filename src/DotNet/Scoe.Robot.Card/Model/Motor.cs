using System;
using System.Collections.Generic;
using System.Linq;

namespace Scoe.Shared.Model
{
    public class Motor : DisableableChannel<byte, double>
    {
        public Motor()
        {
            IsEnabled = true;
            Scale = 1;
            Reversible = true;
        }
        public Motor(byte id) : this() { ID = id; }

        private double _Scale;
        public double Scale
        {
            get { return _Scale; }
            set
            {
                if (_Scale == value)
                    return;
                _Scale = value;
                RaisePropertyChanged("Scale");
            }
        }
        private bool _Reversible;
        public bool Reversible
        {
            get { return _Reversible; }
            set
            {
                if (_Reversible == value)
                    return;
                _Reversible = value;
                RaisePropertyChanged("Reversible");
            }
        }


    }
}
