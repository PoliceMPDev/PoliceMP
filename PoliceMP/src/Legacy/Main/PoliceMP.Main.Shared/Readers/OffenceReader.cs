using Newtonsoft.Json;
using PoliceMP.Main.Shared.Models;
using System.Collections.Generic;
using System.IO;

namespace PoliceMP.Main.Shared.Readers
{
    /// <summary>
    ///     Loads all the offences from the JSON file.
    /// </summary>
    public static class OffenceReader
    {
        /// <summary>
        ///     The location of the offences JSON file.
        /// </summary>
        private const string FILE_LOCATION = "Offences.json";

        /// <summary>
        ///     A list of all the offences.
        /// </summary>
        private static List<Offence> _offences;

        /// <summary>
        ///     Get all the offences.
        /// </summary>
        /// <returns>A list of all the offences.</returns>
        public static List<Offence> All()
        {
            if (_offences != null && _offences.Count != 0) return _offences;

            using (var file = File.OpenText(FILE_LOCATION))
            {
                var serializer = new JsonSerializer();
                _offences = (List<Offence>)serializer.Deserialize(file, typeof(List<Offence>));
            }

            return _offences;
        }
    }
}