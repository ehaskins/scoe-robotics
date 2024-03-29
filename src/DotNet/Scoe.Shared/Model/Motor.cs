using System;
using System.Collections.Generic;
using System.Linq;
using EHaskins.Utilities.Extensions;
using Scoe.Shared.Model.Pid;
namespace Scoe.Shared.Model
{
    public class Motor : DisableableChannel<byte, double>, IPidOutput
    {
        public event PidSourceUpdatedEventHandler Updated;
        protected void RaiseUpdated()
        {
            if (Updated != null)
                Updated(this, new PidSourceUpdatedEventArgs(Value));
        }

        public Motor()
        {
            IsEnabled = true;
            Scale = 1;
            IsReversible = true;
        }

        public Motor(byte id, bool isInverted = false, double scale = 1.0, bool isReversible = true, double controllerDeadband = 0.0)
            : this()
        {
            ID = id;
            IsInverted = isInverted;
            IsReversible = isReversible;
            Scale = scale;
            ControllerDeadband = controllerDeadband;
        }

        protected override void OnUpdated(double oldValue, double newValue)
        {
            base.OnUpdated(oldValue, newValue);
            RaiseUpdated();
        }

        private double _ControllerDeadband;
        private bool _IsInverted;
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
        public double ControllerDeadband
        {
            get
            {
                return _ControllerDeadband;
            }
            set
            {
                if (_ControllerDeadband == value)
                    return;
                _ControllerDeadband = value;
                RaisePropertyChanged("ControllerDeadband");
            }
        }
        private bool _Reversible;
        public bool IsReversible
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

        public bool IsInverted
        {
            get
            {
                return _IsInverted;
            }
            set
            {
                if (_IsInverted == value)
                    return;
                _IsInverted = value;
                RaisePropertyChanged("IsInverted");
            }
        }

        public double GetNormalized()
        {
            var normalized = Value;
            normalized = IsReversible ? normalized.Limit(-Scale, Scale) : normalized.Limit(0, Scale);

            if (IsInverted)
            {
                if (IsReversible)
                {
                    normalized *= -1;
                }
                else
                {
                    normalized = Scale - normalized;
                }
            }

            normalized /= Scale;

            if (normalized > -ControllerDeadband && normalized < ControllerDeadband)
                normalized = 0;
            return normalized;
        }
    }
}
