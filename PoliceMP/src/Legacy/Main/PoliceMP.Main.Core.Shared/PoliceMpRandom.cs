using System;

namespace PoliceMP.Main.Core.Shared
{
    public static class PoliceMpRandom
    {
        private static readonly Random _random = new Random();
        private static readonly object _lock = new object();

        public static int Next(int minValue, int maxValue)
        {
            int number;

            lock (_lock) { number = _random.Next(minValue, maxValue); }

            return number;
        }

        public static int Next(int maxValue)
        {
            int number;

            lock (_lock) { number = _random.Next(maxValue); }

            return number;
        }
    }
}