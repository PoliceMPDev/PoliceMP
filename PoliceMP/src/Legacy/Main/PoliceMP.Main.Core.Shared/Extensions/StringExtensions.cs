using System;
using System.Linq;

namespace PoliceMP.Main.Core.Shared.Extensions
{
    public static class StringExtensions
    {
        public static int NthOccurence(this string s, char t, int n)
        {
            var count = 0;

            for (var i = 0; i < s.Length; i++)
            {
                if (s[i] != t) continue;

                count++;

                if (count == n)
                    return i;
            }

            return -1;
        }

        /// <summary>
        ///     Makes the first char of the string upper case.
        /// </summary>
        /// <param name="input">The input string.</param>
        /// <returns>The input string with upper case first char.</returns>
        public static string FirstCharToUpper(this string input)
        {
            switch (input)
            {
                case null: throw new ArgumentNullException(nameof(input));
                case "": throw new ArgumentException($"{nameof(input)} cannot be empty", nameof(input));
                default: return input.First().ToString().ToUpper() + input.Substring(1);
            }
        }
    }
}
