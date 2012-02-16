using System;

namespace EHaskins.Utilities.Extensions
{
    public static class IntExtensions
    {
        public static int Deadband(this int val, int center, int range, int deadband)
        {
            if (deadband > range)
                throw new ArgumentOutOfRangeException("Deadband must be greater than range.");
            val = val.Limit(center - range, center + range);

            int factor = range / (range - deadband);
            if (val > center + deadband)
                val = (val - deadband) * factor;
            else if (val < center - deadband)
                val = (val + deadband) * factor;
            else
                val = 0;
            return val;
        }

        public static int Limit(this int val, int min, int max)
        {
            if (min > max)
                throw new ArgumentOutOfRangeException("min must be less than max.");
            if (val < min) val = min;
            if (val > max) val = max;
            return val;
        }

        public static int Map(this int val, int inMin, int inMax, int outMin, int outMax)
        {
            if (inMin > inMax)
                throw new ArgumentOutOfRangeException("inMin must be less than inMax.");
            if (outMin > outMax)
                throw new ArgumentOutOfRangeException("outMin must be less than outMax.");

            int factor = (outMax - outMin) / (inMax - inMin);
            val -= inMin;
            val *= factor;
            val += outMin;
            return val;
        }
    }
}
