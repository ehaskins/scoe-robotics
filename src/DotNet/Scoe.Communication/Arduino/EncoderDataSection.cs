using System;
using System.Collections.Generic;
using System.Linq;
using Scoe.Shared.Model;

namespace Scoe.Communication.Arduino
{
    public class EncoderDataSection : DataSection
    {
        public EncoderDataSection(IList<Encoder> encoders)
            : base(5)
        {
            Encoders = encoders;
        }

        public IList<Encoder> Encoders { get; private set; }

        public override void GetData(ref byte[] data, ref int offset)
        {
            int count = Encoders.Count < byte.MaxValue ? Encoders.Count : byte.MaxValue;
            data[offset++] = (byte)count;
            for (int i = 0; i < count; i++)
            {
                data[offset++] = Encoders[i].ChannelAPin;
                data[offset++] = Encoders[i].ChannelBPin;
            }
        }

        public override void Update(byte[] data, int offset)
        {
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
