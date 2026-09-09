using System;

namespace PoliceMP.Core.Shared.Extensions
{
    public static class NumericExtensions
    {
        public static float Truncate(this float value, int digits)
        {
            var mult = Math.Pow(10.0, digits);
            var result = Math.Truncate(mult * value) / mult;
            return (float)result;
        }
    }
}
