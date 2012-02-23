using System;
using System.Collections.Generic;
using System.Linq;
using Scoe.Shared.Model;
using System.IO;

namespace Scoe.Communication.Arduino
{
    public class AnalogIODataSection : DataSection
    {
        public AnalogIODataSection(IList<AnalogInput> analogInputs)
            : base(3)
        {
            _AnalogInputs = analogInputs;
        }

        public override DataSectionData GetData()
        {
            using (var stream = new MemoryStream())
            {

                stream.WriteByte((byte)AnalogInputs.Count);
                foreach (AnalogInput analogInput in AnalogInputs)
                {
                    stream.WriteByte(analogInput.ID);
                    stream.WriteByte(1); //TODO: Implement sample averaging
                }

                return new DataSectionData() { SectionId = SectionId, Data = stream.ToArray() };
            }
        }

        public override void Update(DataSectionData sectionData)
        {
            var data = sectionData.Data;
            var offset = 0;

            if (data.Length > 0)
            {
                byte count = data[offset++];
                for (int i = 0; i < count; i++)
                {
                    byte pin = data[offset++];
                    var ai = (from a in AnalogInputs where a.ID == pin select a).SingleOrDefault();
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
