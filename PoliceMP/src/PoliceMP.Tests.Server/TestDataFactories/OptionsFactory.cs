using Microsoft.Extensions.Options;
using PoliceMP.Shared.Models;
using PoliceMP.Shared.Options;
using System.Collections.Generic;
using System.Linq;
using Options = Microsoft.Extensions.Options.Options;

namespace PoliceMP.Tests.Server.TestDataFactories
{
    public static class OptionsFactory
    {
        public static IOptions<VehicleInfoOptions> VehicleInfoOptions(
            int hasIllegalItemsChance = 50,
            int hasLegalItemsChance = 50,
            int maxIllegalItems = 5,
            int maxLegalItems = 5)
        {
            var vehicleInfoOptions = new VehicleInfoOptions
            {
                HasIllegalItemsChance = hasIllegalItemsChance,
                MaxIllegalItems = maxIllegalItems,
                HasLegalItemsChance = hasLegalItemsChance,
                MaxLegalItems = maxLegalItems
            };

            return Options.Create(vehicleInfoOptions);
        }

        public static IOptions<PedInfoOptions> PedInfoOptions(
            int hasIllegalItemsChance = 50,
            int hasLegalItemsChance = 50,
            int maxIllegalItems = 5,
            int maxLegalItems = 5)
        {
            var pedInfoOptions = new PedInfoOptions
            {
                HasIllegalItemsChance = hasIllegalItemsChance,
                MaxIllegalItems = maxIllegalItems,
                HasLegalItemsChance = hasLegalItemsChance,
                MaxLegalItems = maxLegalItems,
                CriminalMarkers = new List<string>(),
                Charges = new List<string>(),
                Warrants = new List<string>()
            };

            return Options.Create(pedInfoOptions);
        }

        public static IOptions<ItemOptions> ItemOptions(
            int numberOfLegalItems = 5,
            int numberOfIllegalItems = 5)
        {
            var itemOptions = new ItemOptions {Items = new List<Item>()};
            int id = 0;

            foreach (int index in Enumerable.Range(0, numberOfIllegalItems))
            {
                itemOptions.Items.Add(new Item
                {
                    Id = ++id,
                    IsIllegal = true,
                    Name = $"Illegal Item {index}"
                });
            }

            foreach (int index in Enumerable.Range(0, numberOfLegalItems))
            {
                itemOptions.Items.Add(new Item
                {
                    Id = ++id,
                    IsIllegal = false,
                    Name = $"Legal Item {index}"
                });
            }

            return Options.Create(itemOptions);
        }
            

        public static IOptions<WorldOptions> WorldOptions(int numberOfStreets = 5)
        {
            var worldOptions = new WorldOptions {StreetNames = new List<string>()};

            foreach (int index in Enumerable.Range(0, numberOfStreets))
            {
                worldOptions.StreetNames.Add($"Test Street {index}");
            }

            return Options.Create(worldOptions);
        }
    }
}