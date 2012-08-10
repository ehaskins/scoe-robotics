using System;
using System.Collections.Generic;
using System.Linq;
using Scoe.Shared.Model;

namespace Scoe.Robot.KiwiDemo
{
    public class AnalogAccelerometer : Accelerometer
    {
        private AnalogAccelerometerChannel _ZChannel;
        private AnalogAccelerometerChannel _YChannel;
        private AnalogAccelerometerChannel _XChannel;
        public AnalogAccelerometerChannel XChannel
        {
            get
            {
                return _XChannel;
            }
            set
            {
                if (_XChannel == value)
                    return;
                if (_XChannel != null)
                    _XChannel.ValueChanged -= UpdateX;
                _XChannel = value;
                if (_XChannel != null)
                    _XChannel.ValueChanged += UpdateX;
            }
        }
        private void UpdateX(object sender, ChannelValueChangedEventArgs<double> e)
        {
            XAccel = e.NewValue;
        }
        public AnalogAccelerometerChannel YChannel
        {
            get
            {
                return _YChannel;
            }
            set
            {
                if (_YChannel == value)
                    return;
                if (_YChannel != null)
                    _YChannel.ValueChanged -= UpdateY;
                _YChannel = value;
                if (_YChannel != null)
                    _YChannel.ValueChanged += UpdateY;
            }
        }
        private void UpdateY(object sender, ChannelValueChangedEventArgs<double> e)
        {
            YAccel = e.NewValue;
        }
        public AnalogAccelerometerChannel ZChannel
        {
            get
            {
                return _ZChannel;
            }
            set
            {
                if (_ZChannel == value)
                    return;
                if (_ZChannel != null)
                    _ZChannel.ValueChanged -= UpdateZ;
                _ZChannel = value;
                if (_ZChannel != null)
                    _ZChannel.ValueChanged += UpdateZ;
            }
        }
        private void UpdateZ(object sender, ChannelValueChangedEventArgs<double> e)
        {
            ZAccel = e.NewValue;
        }

        /// <summary>
        /// Initializes a new instance of the AnalogAccelerometer class.
        /// </summary>
        /// <param name="xChannel"></param>
        /// <param name="yChannel"></param>
        /// <param name="zChannel"></param>
        public AnalogAccelerometer(AnalogAccelerometerChannel xChannel, AnalogAccelerometerChannel yChannel, AnalogAccelerometerChannel zChannel)
        {
            _ZChannel = zChannel;
            _YChannel = yChannel;
            _XChannel = xChannel;
        }
    }
}
