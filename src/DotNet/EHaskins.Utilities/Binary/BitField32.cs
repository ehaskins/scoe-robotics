using System;

namespace EHaskins.Utilities.Binary
{
    public class BitField32
    {
        public BitField32()
        {
            Length = 32;
        }
        public BitField32(uint value)
            : this()
        {
            this.RawValue = value;
        }

        private static byte GetMask(int index)
        {
            return (byte)Math.Pow(2, index);
        }
        public bool this[int index]
        {
            get
            {
                return (RawValue & GetMask(index)) == GetMask(index);
            }
            set
            {
                bool current = this[index];
                if (current != value)
                {
                    if (value)
                    {
                        RawValue += GetMask(index);
                    }
                    else
                    {
                        RawValue -= GetMask(index);
                    }
                }
            }
        }

        public virtual uint RawValue { get; set; }
        public int Length { get; protected set; }
    }
}
