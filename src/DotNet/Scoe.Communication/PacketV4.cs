using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;

namespace Scoe.Communication
{
    public class PacketV4 : Packet
    {
        public PacketV4(byte[] data)
            : this()
        {
            Parse(data);
        }
        public PacketV4() : base(4) { }

        public override byte[] GetData()
        {
            using (var stream = new MemoryStream())
            using (var writer = new BinaryWriter(stream))
            {
                var content = (Type == PacketType.Command || Type == PacketType.Status) ? GetContentData() : new byte[0];

                writer.Write(Version);
                writer.Write(PacketIndex);
                writer.Write((byte)Type);
                writer.Write((UInt16)content.Length);
                writer.Write(content);

                return stream.ToArray();
            }
        }
        private byte[] GetContentData()
        {
            using (var stream = new MemoryStream())
            using (var writer = new BinaryWriter(stream))
            {
                writer.Write((byte)Sections.Count);
                foreach (var section in Sections)
                {
                    writer.Write(section.SectionId);
                    var length = section.Data != null ? (UInt16)section.Data.Length : (UInt16)0;
                    writer.Write(length);
                    if (section.Data != null)
                        writer.Write(section.Data);
                }
                return stream.ToArray();
            }
        }
        public override void Parse(byte[] data)
        {
            using (var stream = new MemoryStream(data))
            using (var reader = new BinaryReader(stream))
            {
                var version = reader.ReadByte();
                if (version != Version)
                    throw new Exception("Incompatible protocol version."); //TODO: DTC & exception
                PacketIndex = reader.ReadUInt16();
                Type = (PacketType)reader.ReadByte();

                var contentLength = reader.ReadUInt16();
                var content = reader.ReadBytes(contentLength);


                if (Type == PacketType.Command || Type == PacketType.Status)
                    ParseContent(content);

            }
        }
        private void ParseContent(byte[] data)
        {
            using (var stream = new MemoryStream(data))
            using (var reader = new BinaryReader(stream))
            {
                var count = reader.ReadByte();

                for (byte i = 0; i < count; i++)
                {
                    var sectionId = reader.ReadByte();
                    var sectionLength = reader.ReadUInt16();
                    var sectionData = reader.ReadBytes(sectionLength);
                    Sections.Add(new DataSectionData() { SectionId = sectionId, Data = sectionData });
                }
            }
        }
    }
}
