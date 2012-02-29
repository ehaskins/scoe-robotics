using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Scoe.Communication.Arduino
{
    public class DummySection : DataSection
    {
        public int Length { get; set; }
        public DummySection(int length = 100)
            : base(255)
        {
            Length = length;
        }

        public override DataSectionData GetCommandData()
        {
            return GetData();
        }
        public override DataSectionData GetStatusData()
        {
            return GetData();
        }
        public DataSectionData GetData()
        {
            using (var stream = new MemoryStream())
            {

                var rnd = new Random();
                for (int i = 0; i < Length; i++)
                {
                    stream.WriteByte((byte)rnd.Next(255));
                }

                return new DataSectionData() { SectionId = SectionId, Data = stream.ToArray() };
            }
        }
    }
}
