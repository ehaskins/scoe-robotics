using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections.ObjectModel;
using EHaskins.Utilities.Binary;
using Scoe.Shared.Model;
using System.IO;

namespace Scoe.Communication.Arduino
{
    public class DigitalIODataSection : DataSection
    {
        public DigitalIODataSection(ObservableCollection<DigitalIO> digitalInputs)
            : base(2)
        {
            _DigitalInputs = digitalInputs;
        }

        public override DataSectionData GetData()
        {
            using (var stream = new MemoryStream())
            using (var writer = new BinaryWriter(stream))
            {
                var enabledBits = new BitField32();
                var modeBits = new BitField32();
                var stateBits = new BitField32();

                foreach (DigitalIO digitalInput in DigitalInputs)
                {
                    enabledBits[digitalInput.ID] = true;
                    modeBits[digitalInput.ID] = digitalInput.Mode == DigitalIOMode.Output;
                    if (digitalInput.Mode == DigitalIOMode.Output)
                        stateBits[digitalInput.ID] = digitalInput.Value;
                }

                writer.Write(enabledBits.RawValue);
                writer.Write(modeBits.RawValue);
                writer.Write(stateBits.RawValue);

                return new DataSectionData() { SectionId = SectionId, Data = stream.ToArray() };
            }
        }

        public override void Update(DataSectionData sectionData)
        {
            var data = sectionData.Data;
            var offset = 0;

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
                              where d.ID == i
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
