using Newtonsoft.Json;
using PoliceMP.Main.Shared.Models;
using System.Collections.Generic;
using System.IO;

namespace PoliceMP.Main.Shared.Readers
{
    /// <summary>
    ///     Reads the items from the JSON file.
    /// </summary>
    public static class ItemReader
    {
        /// <summary>
        ///     The location of the JSON file.
        /// </summary>
        private const string FILE_LOCATION = "Items.json";

        /// <summary>
        ///     The list of items.
        /// </summary>
        private static List<Item> _items;

        /// <summary>
        ///     Get all the items.
        /// </summary>
        /// <returns>A list of all the items.</returns>
        public static List<Item> All()
        {
            if (_items != null && _items.Count != 0) return _items;

            using (var file = File.OpenText(FILE_LOCATION))
            {
                var serializer = new JsonSerializer();
                _items = (List<Item>)serializer.Deserialize(file, typeof(List<Item>));
            }

            return _items;
        }
    }
}