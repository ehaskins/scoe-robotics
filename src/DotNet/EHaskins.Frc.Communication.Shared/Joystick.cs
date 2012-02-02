using System;
using System.IO;
using System.ComponentModel;
using MiscUtil.IO;
using MiscUtil.Conversion;

namespace EHaskins.Frc.Communication
{
    public class Joystick
    {
        public const int NUM_BYTES = 8;
        public const int NUM_AXES = 6;

        public Joystick()
        {
            Buttons = new BindableBitField16();
            for (int i = 0; i < NUM_AXES; i++)
            {
                _analogInputs[i] = 0;
            }
        }

        int _joystickNumber;

        double[] _analogInputs = new double[NUM_AXES];

        public void Parse(byte[] data, int offset)
        {
            for (int i = 0; i < 6; i++)
            {
                int byteRead = data[offset++];
                double value = byteRead / 128.0 - 1;
                Axes[i] = value;
            }
            Buttons.RawValue = EndianBitConverter.Big.ToUInt16(data, offset); offset += 2;

            RaisePropertyChanged("AnalogInputs");
        }

#if !NETMF
        public byte[] GetBytes()
        {
            Update();
            MemoryStream stream = new MemoryStream();
            BinaryWriter writer = new BinaryWriter(stream);
            for (int i = 0; i < 6; i++)
            {
                double scaled = (Axes[i] + 1) * 128;
                if (scaled > byte.MaxValue)
                    scaled = byte.MaxValue;
                else if (scaled < byte.MinValue)
                    scaled = byte.MinValue;
                writer.Write((byte)scaled);
            }

            writer.Write(Buttons.RawValue);

            var data = stream.ToArray();
            writer.Close();

            return data;
        }
#endif

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
        public int JoystickNumber
        {
            get { return _joystickNumber; }
            set
            {
                if (_joystickNumber == value)
                {
                    return;
                }
                _joystickNumber = value;
                if (PropertyChanged != null)
                {
                    RaisePropertyChanged("JoystickNumber");
                }
            }
        }

        private BitField16 _Buttons;
        public BitField16 Buttons
        {
            get { return _Buttons; }
            set { _Buttons = value; }
        }

        public virtual void Update() { }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void RaisePropertyChanged(string property)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(property));
        }
    }
}