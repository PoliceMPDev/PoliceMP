using Microsoft.CSharp.RuntimeBinder;
using PoliceMP.Main.Shared.Models;
using PoliceMP.Main.Shared.Readers;
using System.Collections.Generic;
using System.Dynamic;
using PoliceMP.Main.Core.Shared;

namespace PoliceMP.Main.Shared.Factories
{
    /// <summary>
    ///     Creates Item objects.
    /// </summary>
    public static class ItemFactory
    {
        private const int CHANCE_MODIFIER = 15;

        /// <summary>
        ///     Converts an Item to a dynamic object.
        /// </summary>
        /// <param name="item">The Item.</param>
        /// <returns>The dynamic object.</returns>
        public static dynamic ToDynamic(Item item)
        {
            dynamic dyn = new ExpandoObject();

            dyn.Id = item.Id;
            dyn.Name = item.Name;
            dyn.Desc = item.Desc;
            dyn.Legal = item.Legal;
            dyn.Chance = item.Chance;

            return dyn;
        }

        /// <summary>
        ///     Converts a dynamic object to an Item.
        /// </summary>
        /// <param name="dyn">The dynamic object.</param>
        /// <returns>The Item.</returns>
        public static Item FromDynamic(dynamic dyn)
        {
            try
            {
                var item = new Item
                {
                    Id = dyn.Id,
                    Name = dyn.Name,
                    Desc = dyn.Desc,
                    Legal = dyn.Legal,
                    Chance = dyn.Chance
                };

                return item;
            }
            catch (RuntimeBinderException)
            {
                return null;
            }
        }

        public static List<Item> FromStringArray(string[] itemNames)
        {
            var allItems = ItemReader.All();
            var items = new List<Item>();

            // Probably shit way to do it but
            foreach (var itemName in itemNames)
                foreach (var allItem in allItems)
                    if (allItem.Name.Equals(itemName))
                    {
                        items.Add(allItem);
                        break;
                    }

            return items;
        }

        /// <summary>
        ///     Creates and returns a random list of Items.
        /// </summary>
        /// <returns>The random list of Items.</returns>
        public static List<Item> RandomList()
        {
            var items = ItemReader.All();
            var randomItems = new List<Item>();

            var quantity = PoliceMpRandom.Next(0, 5);

            for (var i = 0; i < quantity; i++)
            {
                var index = PoliceMpRandom.Next(items.Count - 1);

                if (PoliceMpRandom.Next(100) <= (items[i].Chance + CHANCE_MODIFIER))
                    randomItems.Add(items[index]);
            }

            return randomItems;
        }
    }
}