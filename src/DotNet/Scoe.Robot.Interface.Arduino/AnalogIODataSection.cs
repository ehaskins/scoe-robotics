using System;
using System.Collections.Generic;
using System.Linq;
using Scoe.Shared.Model;

namespace Scoe.Robot.Interface.Arduino
{
    public class AnalogIODataSection : ArduinoDataSection
    {
        public AnalogIODataSection(IList<AnalogInput> analogInputs)
            : base(3)
        {
            _AnalogInputs = analogInputs;
        }

        public override void GetData(ref byte[] data, ref int offset)
        {
            data[offset++] = (byte)AnalogInputs.Count;
            foreach (AnalogInput analogInput in AnalogInputs)
            {
                data[offset++] = analogInput.Pin;
                data[offset++] = 1; //TODO: Implement sample averaging
            }
        }

        public override void Update(byte[] data, int offset)
        {
            if (data.Length > 0)
            {
                byte count = data[offset++];
                for (int i = 0; i < count; i++)
                {
                    byte pin = data[offset++];
                    var ai = (from a in AnalogInputs where a.Pin == pin select a).SingleOrDefault();
                    if (ai != null)
                        ai.Value = BitConverter.ToUInt16(data, offset);
                    offset += 2;
                }
            }
        }


        private IList<AnalogInput> _AnalogInputs;
        public IList<AnalogInput> AnalogInputs
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
    }
}
