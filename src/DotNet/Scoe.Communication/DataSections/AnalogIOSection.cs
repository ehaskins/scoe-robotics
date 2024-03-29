using System;
using System.Collections.Generic;
using System.Linq;
using Scoe.Shared.Model;
using System.IO;

namespace Scoe.Communication.DataSections
{
    public class AnalogInputSection : DataSection
    {
        public AnalogInputSection(IList<AnalogInput> analogInputs)
            : base(3)
        {
            AnalogInputs = analogInputs;
        }

        public override DataSectionData GetCommandData()
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

        public override DataSectionData GetStatusData()
        {
            using (var stream = new MemoryStream())
            using (var writer = new BinaryWriter(stream))
            {
                writer.Write(AnalogInputs.Count);
                foreach (var aio in AnalogInputs)
                {
                    writer.Write(aio.ID);
                    writer.Write(aio.Value);
                }

                return new DataSectionData() { SectionId = SectionId, Data = stream.ToArray() };
            }
        }

        public override void ParseCommand(DataSectionData data)
        {
            using (var stream = new MemoryStream(data.Data))
            using (var reader = new BinaryReader(stream))
            {
                var count = reader.ReadByte();
                for (int i = 0; i < count; i++)
                {
                    AnalogInput aio;
                    if (AnalogInputs.Count <= i)
                    {
                        aio = new AnalogInput();
                        AnalogInputs.Add(aio);
                    }
                    else{
                        aio = AnalogInputs[i];
                    }
                    aio.ID = reader.ReadByte();
                   var samples =  reader.ReadByte(); //TODO: Implement sampleing
                }

                for (int i = count; i < AnalogInputs.Count; i++)
                {
                    AnalogInputs.RemoveAt(count);
                }
            }
        }

        public override void ParseStatus(DataSectionData sectionData)
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
        public IList<AnalogInput> AnalogInputs { get; protected set; }
    }
}
