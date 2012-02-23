using System;
using System.Collections.Generic;
using System.Linq;
using Scoe.Shared.Model;
using System.IO;

namespace Scoe.Communication.Arduino
{
    public class EncoderDataSection : DataSection
    {
        public EncoderDataSection(List<Encoder> encoders)
            : base(5)
        {
            Encoders = encoders;
        }

        public IList<Encoder> Encoders { get; private set; }

        public override DataSectionData GetData()
        {
            using (var stream = new MemoryStream())
            {
                int count = Encoders.Count < byte.MaxValue ? Encoders.Count : byte.MaxValue;
                stream.WriteByte((byte)count);
                for (int i = 0; i < count; i++)
                {
                    stream.WriteByte(Encoders[i].ChannelAPin);
                    stream.WriteByte(Encoders[i].ChannelBPin);
                }

                return new DataSectionData() { SectionId = SectionId, Data = stream.ToArray() };
            }
        }

        public override void Update(DataSectionData sectionData)
        {
            var data = sectionData.Data;
            var offset = 0;
            var count = data[offset++];

            for (int i = 0; i < count; i++)
            {
                var ticks = BitConverter.ToInt32(data, offset);
                offset += 4;
                var fault = data[offset++] != 0;
                if (!fault)
                    Encoders[i].Update(ticks);
            }
        }
    }
}
