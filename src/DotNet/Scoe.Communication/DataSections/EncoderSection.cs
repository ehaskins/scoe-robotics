using System;
using System.Collections.Generic;
using System.Linq;
using Scoe.Shared.Model;
using System.IO;

namespace Scoe.Communication.DataSections
{
    public class EncoderSection : DataSection
    {
        public EncoderSection(List<Encoder> encoders)
            : base(5)
        {
            Encoders = encoders;
        }

        public IList<Encoder> Encoders { get; private set; }

        public override DataSectionData GetCommandData()
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

        public override DataSectionData GetStatusData()
        {
            using (var stream = new MemoryStream())
            using (var writer = new BinaryWriter(stream))
            {
                writer.Write((byte)Encoders.Count);

                foreach (var encoder in Encoders)
                {
                    writer.Write(encoder.Ticks);
                    writer.Write(0); //TODO: Impletment fault forwarding
                }

                return new DataSectionData(SectionId, stream.ToArray());
            }
        }
        public override void ParseCommand(DataSectionData data)
        {
            using (var stream = new MemoryStream())
            using (var reader = new BinaryReader(stream))
            {
                var count = reader.ReadByte();

                for (int i = 0; i < count; i++)
                {
                    Encoders[i].ChannelAPin = reader.ReadByte();
                    Encoders[i].ChannelBPin = reader.ReadByte();
                }

                //remove no longer used encoder defs
                for (int i = count; i < Encoders.Count; i++)
                {
                    Encoders.RemoveAt(count);
                }
            }
        }
        public override void ParseStatus(DataSectionData sectionData)
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
