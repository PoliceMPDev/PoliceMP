using System;
using System.Collections.Generic;
using System.Linq;

namespace PoliceMP.Core.Shared.Extensions
{
    public static class EnumerableExtensions
    {
        public static T GetRandom<T>(this IEnumerable<T> enumerable)
        {
            return enumerable.OrderBy(_ => Guid.NewGuid()).FirstOrDefault();
        }

        public static IEnumerable<T> GetRandomRange<T>(this IEnumerable<T> enumerable, int range)
        {
            return enumerable.OrderBy(_ => Guid.NewGuid()).Take(range);
        }
    }
}