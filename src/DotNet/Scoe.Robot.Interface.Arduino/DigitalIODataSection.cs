using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections.ObjectModel;
using EHaskins.Utilities.Binary;
using Scoe.Shared.Model;

namespace Scoe.Robot.Interface.Arduino
{
    public class DigitalIODataSection : ArduinoDataSection
    {
        public DigitalIODataSection(ObservableCollection<DigitalIO> digitalInputs)
            : base(2)
        {
            _DigitalInputs = digitalInputs;
        }

        public override void GetData(ref byte[] data, ref int offset)
        {
            var enabledBits = new BitField32();
            var modeBits = new BitField32();
            var stateBits = new BitField32();

            foreach (DigitalIO digitalInput in DigitalInputs)
            {
                enabledBits[digitalInput.Pin] = true;
                modeBits[digitalInput.Pin] = digitalInput.Mode == DigitalIOMode.Output;
                if (digitalInput.Mode == DigitalIOMode.Output)
                    stateBits[digitalInput.Pin] = digitalInput.Value;
            }

            BitConverter.GetBytes(enabledBits.RawValue).CopyTo(data, offset);
            offset += 4;
            BitConverter.GetBytes(modeBits.RawValue).CopyTo(data, offset);
            offset += 4;
            BitConverter.GetBytes(stateBits.RawValue).CopyTo(data, offset);
            offset += 4;
        }

        public override void Update(byte[] data, int offset)
        {
            var enabledBits = new BitField32();
            var modeBits = new BitField32();
            var stateBits = new BitField32();

            enabledBits.RawValue = BitConverter.ToUInt32(data, offset);
            offset += 4;
            modeBits.RawValue = BitConverter.ToUInt32(data, offset);
            offset += 4;
            stateBits.RawValue = BitConverter.ToUInt32(data, offset);
            offset += 4;

            for (int i = 0; i < 32; i++)
            {
                if (enabledBits[i] && !modeBits[i])
                {
                    var di = (from d in DigitalInputs
                              where d.Pin == i
                              select d).SingleOrDefault();
                    if (di != null)
                        di.Value = stateBits[i];
                }
            }
        }


        private ObservableCollection<DigitalIO> _DigitalInputs;

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
    }
}
