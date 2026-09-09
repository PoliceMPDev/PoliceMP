using Microsoft.CSharp.RuntimeBinder;
using PoliceMP.Main.Core.Shared.Constants;
using PoliceMP.Main.Core.Shared.Extensions;
using PoliceMP.Main.Shared.Models;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using PoliceMP.Main.Core.Shared;
using static Bogus.DataSets.Name;

namespace PoliceMP.Main.Shared.Factories
{
    /// <summary>
    ///     Creates Person objects.
    /// </summary>
    public static class PersonFactory
    {
        /// <summary>
        ///     The chance for the Person to be drunk.
        /// </summary>
        private const int DRUNK_CHANCE = 15;

        /// <summary>
        ///     The chance for the Person to be on cocaine.
        /// </summary>
        private const int COCAINE_CHANCE = 9;

        /// <summary>
        ///     The chance for the Person to have smoked cannabis.
        /// </summary>
        private const int CANNABIS_CHANCE = 10;

        /// <summary>
        ///     The chance for the Person to have taken ecstasy.
        /// </summary>
        private const int ECSTASY_CHANCE = 7;

        /// <summary>
        ///     The chance for the Person to be on heroin.
        /// </summary>
        private const int HEROIN_CHANCE = 7;

        /// <summary>
        ///     The chance for the Person to have a weapon.
        /// </summary>
        private const int WEAPON_CHANCE = 0;

        /// <summary>
        ///     The chance for the Person to have a criminal marker.
        /// </summary>
        private const int MARKER_CHANCE = 10;

        /// <summary>
        ///     The chance for the Person to have a warrant.
        /// </summary>
        private const int WARRANT_CHANCE = 10;

        /// <summary>
        ///     The chance for the Person to have a charge.
        /// </summary>
        private const int CHARGE_CHANCE = 12;

        /// <summary>
        ///     The chance for the Person to have multiple charges.
        /// </summary>
        private const int MULTIPLE_CHARGES_CHANCE = 12;

        /// <summary>
        ///     The maximum amount of charges a Person can have.
        /// </summary>
        private const int MAX_CHARGES = 3;

        /// <summary>
        ///     The chance for the Person to have a driving license.
        /// </summary>
        private const int HAS_DRIVING_LICENSE_CHANCE = 88;

        /// <summary>
        ///     The chance for the Person to be banned from driving.
        /// </summary>
        private const int IS_BANNED_FROM_DRIVING_CHANCE = 7;

        /// <summary>
        ///     The chance for the Person to have any points on their license.
        /// </summary>
        private const int HAS_POINTS_CHANCE = 45;

        /// <summary>
        ///     The chance for the person to not be wearing a seatbelt.
        /// </summary>
        private const int NO_SEATBELT_CHANCE = 20;

        /// <summary>
        ///     Converts a Person object to dynamic.
        /// </summary>
        /// <param name="person">The Person.</param>
        /// <returns>Dynamic version of Person.</returns>
        public static dynamic ToDynamic(Person person)
        {
            dynamic dyn = new ExpandoObject();
            var items = new List<dynamic>();
            foreach (var item in person.Items)
                items.Add(ItemFactory.ToDynamic(item));

            var dateOfBirth = person.DateOfBirth.ToBinary();

            dyn.NetworkId = person.NetworkId;
            dyn.FirstName = person.FirstName;
            dyn.LastName = person.LastName;
            dyn.DateOfBirth = dateOfBirth;
            dyn.Street = person.Street;
            dyn.PhoneNumber = person.PhoneNumber;
            dyn.Company = person.Company;
            dyn.JobTitle = person.JobTitle;
            dyn.Attitude = person.Attitude;
            dyn.AlcoholLevel = person.AlcoholLevel;
            dyn.OnCocaine = person.OnCocaine;
            dyn.SmokedCannabis = person.SmokedCannabis;
            dyn.HasDrivingLicense = person.HasDrivingLicense;
            dyn.DrivingLicensePoints = person.DrivingLicensePoints;
            dyn.IsBannedFromDriving = person.IsBannedFromDriving;
            dyn.OnEcstasy = person.OnEcstasy;
            dyn.OnHeroin = person.OnHeroin;
            dyn.Items = items;
            dyn.Warrants = person.Warrants.Cast<dynamic>().ToList();
            dyn.Charges = person.Charges.Cast<dynamic>().ToList();
            dyn.Markers = person.Markers.Cast<dynamic>().ToList();
            dyn.Weapon = person.Weapon;
            dyn.WearsSeatbelt = person.WearsSeatbelt;

            return dyn;
        }

        /// <summary>
        ///     Converts a dynamic object to a Person object.
        /// </summary>
        /// <param name="input">The dynamic object.</param>
        /// <returns>The person object.</returns>
        public static Person FromDynamic(dynamic input)
        {
            try
            {
                var inputItems = (List<dynamic>)input.Items;
                var items = new List<Item>();

                foreach (var item in inputItems) items.Add(ItemFactory.FromDynamic(item));

                var dateOfBirth = DateTime.FromBinary(input.DateOfBirth);

                var warrants = input.Warrants as List<dynamic>;
                var charges = input.Charges as List<dynamic>;
                var markers = input.Markers as List<dynamic>;

                var person = new Person
                {
                    NetworkId = input.NetworkId,
                    FirstName = input.FirstName,
                    LastName = input.LastName,
                    DateOfBirth = dateOfBirth,
                    Street = input.Street,
                    PhoneNumber = input.PhoneNumber,
                    Company = input.Company,
                    JobTitle = input.JobTitle,
                    Attitude = input.Attitude,
                    AlcoholLevel = input.AlcoholLevel,
                    OnCocaine = input.OnCocaine,
                    SmokedCannabis = input.SmokedCannabis,
                    HasDrivingLicense = input.HasDrivingLicense,
                    DrivingLicensePoints = input.DrivingLicensePoints,
                    IsBannedFromDriving = input.IsBannedFromDriving,
                    OnEcstasy = input.OnEcstasy,
                    OnHeroin = input.OnHeroin,
                    Items = items,
                    Warrants = warrants.Cast<string>().ToList(),
                    Charges = charges.Cast<string>().ToList(),
                    Markers = markers.Cast<string>().ToList(),
                    Weapon = input.Weapon,
                    WearsSeatbelt = input.WearsSeatbelt
                };

                return person;
            }
            catch (RuntimeBinderException)
            {
                return null;
            }
        }

        /// <summary>
        ///     Creates a legal person that will not have any criminal things.
        /// </summary>
        /// <param name="networkId">The ped network ID.</param>
        /// <param name="gender">The ped gender.</param>
        /// <returns>The legal Person.</returns>
        public static Person Legal(int networkId, bool gender)
        {
            var person = Random(networkId, gender);

            // Set to legal
            person.NetworkId = networkId;
            person.Weapon = string.Empty;
            person.Items = new List<Item>();
            person.Charges = new List<string>();
            person.Warrants = new List<string>();
            person.Markers = new List<string>();

            person.Attitude = 0;
            person.AlcoholLevel = 0f;
            person.OnCocaine = false;
            person.SmokedCannabis = false;
            person.OnHeroin = false;
            person.OnEcstasy = false;

            person.HasDrivingLicense = true;
            person.IsBannedFromDriving = false;
            person.DrivingLicensePoints = 0;
            person.WearsSeatbelt = true;

            return person;
        }

        public static Person Custom(int networkId,
            string firstName,
            string lastName,
            DateTime birthDate,
            float alcoholLevel,
            bool onCocaine,
            bool onCannabis,
            bool onHeroin,
            bool onEcstasy,
            bool hasDrivingLicense,
            bool isBannedFromDriving,
            int drivingLicensePoints,
            bool wearsSeatbelt,
            string[] items,
            string[] warrants,
            string[] charges,
            string[] markers)
        {
            var faker = new Bogus.Faker<Person>();

            faker.RuleFor(p => p.FirstName, f => f.Name.FirstName(Gender.Male))
                .RuleFor(p => p.LastName, f => f.Name.LastName(Gender.Male))
                .RuleFor(p => p.DateOfBirth, f => f.Date.Between(new DateTime(1960, 1, 1), new DateTime(2000, 1, 1)))
                .RuleFor(p => p.Street, f => $"{f.Random.Number(1, 300)} {f.Random.ListItem(StreetNames.Streets)}")
                .RuleFor(p => p.PhoneNumber, f => f.Phone.PhoneNumber("##### ######"))
                .RuleFor(p => p.Company, f => f.Company.CompanyName())
                .RuleFor(p => p.JobTitle, f => f.Name.JobTitle());

            var person = faker.Generate();

            person.NetworkId = networkId;
            if (!string.IsNullOrEmpty(firstName))
                person.FirstName = firstName;

            if (!string.IsNullOrEmpty(lastName))
                person.LastName = lastName;

            if (birthDate != DateTime.Now && birthDate != DateTime.MinValue)
                person.DateOfBirth = birthDate;

            person.Attitude = 0;
            person.AlcoholLevel = alcoholLevel;
            person.OnCocaine = onCocaine;
            person.SmokedCannabis = onCannabis;
            person.OnHeroin = onHeroin;
            person.OnEcstasy = onEcstasy;
            person.HasDrivingLicense = hasDrivingLicense;
            person.IsBannedFromDriving = isBannedFromDriving;
            person.DrivingLicensePoints = drivingLicensePoints;
            person.WearsSeatbelt = wearsSeatbelt;
            person.Warrants = warrants;
            person.Charges = charges;
            person.Markers = markers;

            person.Items = ItemFactory.FromStringArray(items);

            return person;
        }

        /// <summary>
        ///     Creates a random Person.
        /// </summary>
        /// <param name="networkId">The ped network ID.</param>
        /// <param name="gender">The ped gender.</param>
        /// <returns>The random Person.</returns>
        public static Person Random(int networkId, bool gender)
        {
            // Faker gen
            var fakerGender = gender ? Gender.Male : Gender.Female;

            var faker = new Bogus.Faker<Person>();

            faker.RuleFor(p => p.FirstName, f => f.Name.FirstName(fakerGender))
                .RuleFor(p => p.LastName, f => f.Name.LastName(fakerGender))
                .RuleFor(p => p.DateOfBirth, f => f.Date.Between(new DateTime(1960, 1, 1), new DateTime(2000, 1, 1)))
                .RuleFor(p => p.Street, f => $"{f.Random.Number(1, 300)} {f.Random.ListItem(StreetNames.Streets)}")
                .RuleFor(p => p.PhoneNumber, f => f.Phone.PhoneNumber("##### ######"))
                .RuleFor(p => p.Company, f => f.Company.CompanyName())
                .RuleFor(p => p.JobTitle, f => f.Name.JobTitle())
                .RuleFor(p => p.Attitude, f => f.Random.Number(0, 100))
                .RuleFor(p => p.AlcoholLevel, f =>
                    f.Random.Int(0, 100) <= DRUNK_CHANCE ? f.Random.Float(0.01f, 0.12f).Truncate(2) : 0f)
                .RuleFor(p => p.OnCocaine, f => f.Random.Int(0, 100) <= COCAINE_CHANCE)
                .RuleFor(p => p.SmokedCannabis, f => f.Random.Int(0, 100) <= CANNABIS_CHANCE)
                .RuleFor(p => p.OnEcstasy, f => f.Random.Int(0, 100) <= ECSTASY_CHANCE)
                .RuleFor(p => p.OnHeroin, f => f.Random.Int(0, 100) <= HEROIN_CHANCE)

                .RuleFor(p => p.HasDrivingLicense, f => f.Random.Int(0, 100) <= HAS_DRIVING_LICENSE_CHANCE)
                .RuleFor(p => p.WearsSeatbelt, f => !(f.Random.Int(0, 100) <= NO_SEATBELT_CHANCE));

            var person = faker.Generate();

            // Own gen
            person.NetworkId = networkId;
            person.Items = ItemFactory.RandomList();
            person.Warrants = new List<string>();
            person.Charges = new List<string>();
            person.Markers = new List<string>();

            if (person.HasDrivingLicense)
            {
                person.IsBannedFromDriving = false;
                person.DrivingLicensePoints = PoliceMpRandom.Next(100) <= HAS_POINTS_CHANCE ? PoliceMpRandom.Next(1, 11) : 0;
            }
            else
            {
                person.IsBannedFromDriving = PoliceMpRandom.Next(100) <= IS_BANNED_FROM_DRIVING_CHANCE;
                person.DrivingLicensePoints = 0;
            }

            var hasWeapon = PoliceMpRandom.Next(100) <= WEAPON_CHANCE;
            var hasWarrant = PoliceMpRandom.Next(100) <= WARRANT_CHANCE;
            var hasCharge = PoliceMpRandom.Next(100) <= CHARGE_CHANCE;
            var hasMultipleCharges = PoliceMpRandom.Next(100) <= MULTIPLE_CHARGES_CHANCE;
            var hasMarker = PoliceMpRandom.Next(100) <= MARKER_CHANCE;

            if (hasWeapon)
            {
                var weaponIndex = PoliceMpRandom.Next(PersonGenerationDetails.Weapons.Length - 1);
                person.Weapon = PersonGenerationDetails.Weapons[weaponIndex];
            }
            else
            {
                person.Weapon = string.Empty;
            }

            if (hasWarrant)
            {
                var warrantIndex = PoliceMpRandom.Next(PersonGenerationDetails.Warrants.Length - 1);
                person.Warrants.Add(PersonGenerationDetails.Warrants[warrantIndex]);
            }

            if (hasCharge)
            {
                if (hasMultipleCharges)
                {
                    var quantity = PoliceMpRandom.Next(2, MAX_CHARGES);
                    var charges = PersonGenerationDetails.Charges.ToList();
                    for (var i = 0; i < quantity; i++)
                    {
                        var chargeIndex = PoliceMpRandom.Next(charges.Count - 1);
                        person.Charges.Add(charges[chargeIndex]);
                        charges.Remove(charges[chargeIndex]);
                    }
                }
                else
                {
                    var chargeIndex = PoliceMpRandom.Next(PersonGenerationDetails.Charges.Length - 1);
                    person.Charges.Add(PersonGenerationDetails.Charges[chargeIndex]);
                }
            }

            if (hasMarker)
            {
                var markerIndex = PoliceMpRandom.Next(PersonGenerationDetails.CriminalMarkers.Length - 1);
                person.Markers.Add(PersonGenerationDetails.CriminalMarkers[markerIndex]);
            }

            return person;
        }
    }
}