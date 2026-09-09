using Microsoft.Extensions.Options;
using PoliceMP.Core.Shared;
using PoliceMP.Core.Shared.Extensions;
using PoliceMP.Shared.Enums;
using PoliceMP.Shared.Models;
using PoliceMP.Shared.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using Bogus;
using PoliceMP.Core.Server.Interfaces.Factories;

namespace PoliceMP.Server.Factories
{
    public class BogusPedInfoFactory : IPedInfoFactory
    {
        private readonly PedInfoOptions _pedInfoOptions;
        private readonly WorldOptions _worldOptions;
        private readonly ItemOptions _itemOptions;

        public BogusPedInfoFactory(IOptions<PedInfoOptions> pedInfoOptions,
            IOptions<WorldOptions> worldOptions,
            IOptions<ItemOptions> itemOptions)
        {
            _itemOptions = itemOptions.Value;
            _pedInfoOptions = pedInfoOptions.Value;
            _worldOptions = worldOptions.Value;
        }

        public PedInfo Random(int networkId, Gender gender)
        {
            var fakerGender = (Bogus.DataSets.Name.Gender)gender;
            var faker = new Faker("en_GB");
            var pedInfoFaker = new Bogus.Faker<PedInfo>("en_GB");

            pedInfoFaker
                .RuleFor(p => p.FirstName, f => f.Name.FirstName(fakerGender))
                .RuleFor(p => p.LastName, f => f.Name.LastName(fakerGender))
                .RuleFor(p => p.DateOfBirth,
                    f => f.Date.Between(DateTime.Now.AddYears(-60), DateTime.Now.AddYears(-18)))
                .RuleFor(p => p.Street, f => $"{AppRandom.Next(1, 9999)} {f.Random.ListItem(_worldOptions.StreetNames)}")
                .RuleFor(p => p.Height, f => $"{AppRandom.Next(5, 6)}'{AppRandom.Next(0, 11)}\"")
                .RuleFor(p => p.AlcoholLevel,
                    f => CheckCondition(_pedInfoOptions.DrunkChance) ? 0f : f.Random.Float(0.01f, 0.12f).Truncate(2))
                .RuleFor(p => p.IsOnCocaine, f => CheckCondition(_pedInfoOptions.CocaineChance))
                .RuleFor(p => p.IsOnCannabis, f => CheckCondition(_pedInfoOptions.CannabisChance))
                .RuleFor(p => p.IsOnHeroin, f => CheckCondition(_pedInfoOptions.HeroinChance))
                .RuleFor(p => p.IsOnEcstasy, f => CheckCondition(_pedInfoOptions.EcstasyChance))
                .RuleFor(p => p.HasDrivingLicense, f => CheckCondition(_pedInfoOptions.HasDrivingLicenseChance))
                .RuleFor(p => p.IsBannedFromDriving, f => CheckCondition(_pedInfoOptions.IsBannedFromDrivingChance))
                .RuleFor(p => p.IsWearingSeatbelt, f => !CheckCondition(_pedInfoOptions.NoSeatbeltChance))
                .RuleFor(p => p.Attitude, f => AppRandom.Next(0, 100));
            
            var pedInfo = pedInfoFaker.Generate();
            pedInfo.CriminalMarkers = new List<string>();
            pedInfo.Warrants = new List<string>();
            pedInfo.Charges = new List<string>();
            pedInfo.Items = new List<Item>();

            if (CheckCondition(_pedInfoOptions.MarkerChance))
            {
                string marker = _pedInfoOptions.CriminalMarkers.GetRandom();
                pedInfo.CriminalMarkers.Add(marker);
            }

            if (CheckCondition(_pedInfoOptions.WarrantChance))
            {
                string warrant = _pedInfoOptions.Warrants.GetRandom();
                pedInfo.Warrants.Add(warrant);
            }

            if (CheckCondition(_pedInfoOptions.ChargeChance))
            {
                if (CheckCondition(_pedInfoOptions.MultipleChargesChance))
                {
                    int numberOfCharges = AppRandom.Next(2, _pedInfoOptions.MaxCharges);
                    var charges = _pedInfoOptions.Charges.GetRandomRange(numberOfCharges);
                    pedInfo.Charges.AddRange(charges);
                }
                else
                {
                    string charge = _pedInfoOptions.Charges.GetRandom();
                    pedInfo.Charges.Add(charge);
                }
            }

            if (CheckCondition(_pedInfoOptions.HasIllegalItemsChance))
            {
                int numberOfIllegalItems = AppRandom.Next(1, _pedInfoOptions.MaxIllegalItems);
                var illegalItems = _itemOptions.Items
                    .Where(x => x.IsIllegal)
                    .GetRandomRange(numberOfIllegalItems);
                pedInfo.Items.AddRange(illegalItems);
            }

            if (CheckCondition(_pedInfoOptions.HasLegalItemsChance))
            {
                int numberOfLegalItems = AppRandom.Next(1, _pedInfoOptions.MaxLegalItems);
                var legalItems = _itemOptions.Items
                    .Where(x => !x.IsIllegal)
                    .GetRandomRange(numberOfLegalItems);
                pedInfo.Items.AddRange(legalItems);
            }
            
            bool isUnemployed = CheckCondition(_pedInfoOptions.UnemployedChance);
            pedInfo.Company = isUnemployed ? "N/A" : faker.Company.CompanyName();
            pedInfo.JobTitle = isUnemployed ? "Unemployed" : faker.Name.JobTitle();

            pedInfo.ReceivedDrivingLicenseDate = faker.Date.Between(
                pedInfo.DateOfBirth.AddYears(17), DateTime.Now.AddDays(-1));
            pedInfo.DrivingBanExpiryDate = pedInfo.IsBannedFromDriving
                ? DateTime.MinValue
                : faker.Date.Between(DateTime.Now, DateTime.Now.AddYears(5));

            pedInfo.DrivingLicensePoints = pedInfo.IsBannedFromDriving ? 12 : AppRandom.Next(0, 11);

            pedInfo.Gender = gender;
            pedInfo.NetworkId = networkId;

            return pedInfo;
        }

        private bool CheckCondition(double chance) => chance >= AppRandom.Next(0, 100);
    }
}