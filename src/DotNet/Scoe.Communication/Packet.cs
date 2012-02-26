using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Diagnostics;

namespace Scoe.Communication
{
    public class DataSectionData
    {
        public DataSectionData()
        {
        }

        public byte SectionId { get; set; }
        public byte[] Data { get; set; }
    }
    public class Packet
    {
        public Packet(byte[] data)
            : this()
        {
            Parse(data);
        }
        public Packet()
        {
            Version = 3;
            Sections = new List<DataSectionData>();
        }
        public byte Version { get; private set; }
        public ushort PacketIndex { get; set; }
        public PacketType Type { get; set; }
        public List<DataSectionData> Sections { get; set; }

        public byte[] GetData()
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
        public byte[] GetContentData()
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
        public void Parse(byte[] data)
        {
            using (var stream = new MemoryStream(data))
            using (var reader = new BinaryReader(stream))
            {
                var crc = reader.ReadUInt32();
                Version = reader.ReadByte();
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
        public void ParseContent(byte[] data)
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
