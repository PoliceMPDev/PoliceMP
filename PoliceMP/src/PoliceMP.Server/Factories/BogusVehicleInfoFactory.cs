using Microsoft.Extensions.Options;
using PoliceMP.Core.Shared;
using PoliceMP.Core.Shared.Extensions;
using PoliceMP.Shared.Models;
using PoliceMP.Shared.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using Bogus;
using PoliceMP.Core.Server.Interfaces.Factories;

namespace PoliceMP.Server.Factories
{
    public class BogusVehicleInfoFactory : IVehicleInfoFactory
    {
        private readonly VehicleInfoOptions _vehicleInfoOptions;
        private readonly ItemOptions _itemOptions;

        public BogusVehicleInfoFactory(IOptions<VehicleInfoOptions> vehicleInfoOptions,
            IOptions<ItemOptions> itemOptions)
        {
            _vehicleInfoOptions = vehicleInfoOptions.Value;
            _itemOptions = itemOptions.Value;
        }

        public VehicleInfo Random(int networkId, string plate, string ownerName = null)
        {
            var vehicleInfoFaker = new Bogus.Faker<VehicleInfo>("en_GB");
            var faker = new Faker("en_GB");

            if (CheckCondition(_vehicleInfoOptions.InsuranceExpiredChance))
                vehicleInfoFaker.RuleFor(v => v.InsuranceExpiryDate,
                    f => f.Date.Between(DateTime.Now.AddYears(-5), DateTime.Now.AddDays(-1)));
            else
                vehicleInfoFaker.RuleFor(v => v.InsuranceExpiryDate,
                    f => f.Date.Between(DateTime.Now.AddDays(1), DateTime.Now.AddYears(1)));

            if (CheckCondition(_vehicleInfoOptions.MotExpiredChance))
                vehicleInfoFaker.RuleFor(v => v.MotExpiryDate,
                    f => f.Date.Between(DateTime.Now.AddYears(-5), DateTime.Now.AddDays(-1)));
            else
                vehicleInfoFaker.RuleFor(v => v.MotExpiryDate,
                    f => f.Date.Between(DateTime.Now.AddDays(1), DateTime.Now.AddYears(1)));

            if (CheckCondition(_vehicleInfoOptions.TaxExpiredChance))
                vehicleInfoFaker.RuleFor(v => v.TaxExpiryDate,
                        f => f.Date.Between(DateTime.Now.AddYears(-5), DateTime.Now.AddDays(-1)));
            else
                vehicleInfoFaker.RuleFor(v => v.TaxExpiryDate,
                        f => f.Date.Between(DateTime.Now.AddDays(1), DateTime.Now.AddYears(1)));

            vehicleInfoFaker.RuleFor(v => v.VIN, f => f.Vehicle.Vin());

            var vehicleInfo = vehicleInfoFaker.Generate();
            vehicleInfo.NetworkId = networkId;
            vehicleInfo.Plate = plate.ToUpper();
            vehicleInfo.Markers = new List<string>();
            vehicleInfo.OwnerName = !string.IsNullOrEmpty(ownerName) ? ownerName : faker.Name.FullName();
            vehicleInfo.Items = new List<Item>();

            if (CheckCondition(_vehicleInfoOptions.MarkerChance))
            {
                if (CheckCondition(_vehicleInfoOptions.MultipleMarkersChance))
                {
                    var markers = _vehicleInfoOptions.Markers.GetRandomRange(_vehicleInfoOptions.MaxMarkers);
                    vehicleInfo.Markers.AddRange(markers);
                }
                else
                {
                    string marker = _vehicleInfoOptions.Markers.GetRandom();
                    vehicleInfo.Markers.Add(marker);
                }
            }

            if (CheckCondition(_vehicleInfoOptions.HasIllegalItemsChance))
            {
                int numberOfIllegalItems = AppRandom.Next(1, _vehicleInfoOptions.MaxIllegalItems);
                var illegalItems = _itemOptions.Items
                    .Where(x => x.IsIllegal)
                    .GetRandomRange(numberOfIllegalItems);
                vehicleInfo.Items.AddRange(illegalItems);
            }

            if (CheckCondition(_vehicleInfoOptions.HasLegalItemsChance))
            {
                int numberOfLegalItems = AppRandom.Next(1, _vehicleInfoOptions.MaxLegalItems);
                var legalItems = _itemOptions.Items
                    .Where(x => !x.IsIllegal)
                    .GetRandomRange(numberOfLegalItems);
                vehicleInfo.Items.AddRange(legalItems);
            }

            return vehicleInfo;
        }

        private bool CheckCondition(double chance) => chance >= AppRandom.Next(0, 100);
    }
}