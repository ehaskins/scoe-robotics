using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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

        public override void GetData(ref byte[] data, ref int offset)
        {
            var rnd = new Random();
            for (int i = 0; i < Length; i++)
            {
                data[offset++] = (byte)rnd.Next(255);
            }
        }
        public override void Update(byte[] data, int offset)
        {
           
        }
    }
}
