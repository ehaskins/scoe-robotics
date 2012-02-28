using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;

namespace Scoe.Communication
{
    public class PacketV3 : Packet
    {
        public PacketV3(byte[] data)
            : this()
        {
            Parse(data);
        }
        public PacketV3() : base(3) { }

        public override byte[] GetData()
        {
            using (var stream = new MemoryStream())
            using (var writer = new BinaryWriter(stream))
            {
                var content = GetContentData();
                var crc = Crc32.Compute(content);

                writer.Write(crc);
                writer.Write(Version);
                writer.Write(PacketIndex);
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
                writer.Write((byte)Type);
                if (Type == PacketType.Command || Type == PacketType.Status)
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
                }
                return stream.ToArray();
            }
        }
        public override void Parse(byte[] data)
        {
            using (var stream = new MemoryStream(data))
            using (var reader = new BinaryReader(stream))
            {
                var crc = reader.ReadUInt32();
                var version = reader.ReadByte();
                if (version != Version)
                    throw new InvalidOperationException("Incompatible protocol version.");
                PacketIndex = reader.ReadUInt16();
                var contentLength = reader.ReadUInt16();
                var content = reader.ReadBytes(contentLength);

                var calcCrc = Crc32.Compute(content);
                if (crc == calcCrc)
                    ParseContent(content);
                else
                {
                    throw new InvalidOperationException("Invalid CRC"); //TODO: use custom exception.
                }

            }
        }
        private void ParseContent(byte[] data)
        {
            using (var stream = new MemoryStream(data))
            using (var reader = new BinaryReader(stream))
            {
                Type = (PacketType)reader.ReadByte();
                if (Type == PacketType.Command || Type == PacketType.Status)
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
}
