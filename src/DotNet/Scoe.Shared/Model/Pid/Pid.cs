using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;
using EHaskins.Utilities.Extensions;
namespace Scoe.Shared.Model.Pid
{
    public class VelocityPid
    {
        double _lastError = double.NaN;
        double _i = 0.0;
        Stopwatch stopwatch;
        double lastTicks;
        public VelocityPid()
        {
            stopwatch = new Stopwatch();
        }

        private void SourceUpdated(object sender, PidSourceUpdatedEventArgs e)
        {
            var elapsed = 0.0;
            if (!stopwatch.IsRunning)
            {
                stopwatch.Start();
            }
            else
            {
                var ticks = stopwatch.ElapsedTicks;
                elapsed = (double)(ticks - lastTicks) / Stopwatch.Frequency;
                lastTicks = ticks;
            }
            DoPid(e.Value, elapsed);
        }

        private void DoPid(double currentValue, double elapsedSeconds)
        {
            var error = Value - currentValue;

            var p = error * P * elapsedSeconds;
            _i += error * I * elapsedSeconds;
            var d = 0.0;
            if (!double.IsNaN(_lastError))
                d = (error - _lastError) * D * elapsedSeconds;

            CurrentOutput = p + _i + d;
            CurrentOutput = CurrentOutput.Limit(OutputMin, OutputMax);
            Output.Value = CurrentOutput;
            _lastError = error;
        }

        public IPidOutput Output { get; set; }
        private IPidSource _Source;
        public IPidSource Source
        {
            get
            {
                return _Source;
            }
            set
            {
                if (_Source == value)
                    return;
                if (_Source != null)
                    _Source.Updated -= SourceUpdated;
                _Source = value;
                _Source.Updated += SourceUpdated;
            }
        }

        public double OutputMax { get; set; }
        public double OutputMin { get; set; }
        public double CurrentOutput { get; set; }
        public double Value { get; set; }
        public double P { get; set; }
        public double I { get; set; }
        public double D { get; set; }
        public double IMax { get; set; }
        public double IMin { get; set; }
    }
}
