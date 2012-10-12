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
        public DataSectionData(byte sectionId, Byte[] data)
        {
            SectionId = sectionId;
            Data = data;
        }
        public DataSectionData()
        {
        }

        public byte SectionId { get; set; }
        public byte[] Data { get; set; }
    }

    public abstract class Packet
    {
        public Packet(byte version)
        {
            Version = version;
            Sections = new List<DataSectionData>();
        }

        public ushort PacketIndex { get; set; }
        public byte Version { get; protected set; }
        public PacketType Type { get; set; }
        public List<DataSectionData> Sections { get; set; }

        public abstract byte[] GetData();
        public abstract void Parse(byte[] data);

    }
}