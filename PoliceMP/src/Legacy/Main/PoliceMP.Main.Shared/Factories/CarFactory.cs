using Bogus;
using Microsoft.CSharp.RuntimeBinder;
using PoliceMP.Main.Core.Shared.Constants;
using PoliceMP.Main.Shared.Models;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using PoliceMP.Main.Core.Shared;

namespace PoliceMP.Main.Shared.Factories
{
    /// <summary>
    ///     Creates Car objects.
    /// </summary>
    public static class CarFactory
    {
        /// <summary>
        ///     The chance for the car to have MOT.
        /// </summary>
        private const int HAS_MOT_CHANCE = 94;

        /// <summary>
        ///     The chance for the car to have tax.
        /// </summary>
        private const int HAS_TAX_CHANCE = 94;

        /// <summary>
        ///     The chance for the car to have insurance.
        /// </summary>
        private const int HAS_INSURANCE_CHANCE = 94;

        /// <summary>
        ///     The chance for the car to have any criminal markers.
        /// </summary>
        private const int MARKER_CHANCE = 10;

        /// <summary>
        ///     The chance for the driver to not be the owner.
        /// </summary>
        private const int DRIVER_NOT_OWNER_CHANCE = 8;

        /// <summary>
        ///     Converts a Car object to dynamic.
        /// </summary>
        /// <param name="car">The Car object.</param>
        /// <returns>The dynamic object.</returns>
        public static dynamic ToDynamic(Car car)
        {
            dynamic dyn = new ExpandoObject();
            var items = new List<dynamic>();
            foreach (var item in car.Items) items.Add(ItemFactory.ToDynamic(item));

            var insuranceDue = car.InsuranceDue.ToBinary();
            var taxDue = car.TaxDue.ToBinary();
            var motDue = car.MotDue.ToBinary();

            dyn.NetworkId = car.NetworkId;
            dyn.Plate = car.Plate;
            dyn.Vin = car.Vin;
            dyn.Owner = car.Owner;

            dyn.InsuranceDue = insuranceDue;
            dyn.TaxDue = taxDue;
            dyn.MotDue = motDue;

            dyn.Markers = car.Markers.Cast<dynamic>().ToList();
            dyn.Items = items;

            return dyn;
        }

        /// <summary>
        ///     Converts a dynamic object to a Car object.
        /// </summary>
        /// <param name="input">The dynamic object.</param>
        /// <returns>The Car object.</returns>
        public static Car FromDynamic(dynamic input)
        {
            try
            {
                var inputItems = (List<dynamic>)input.Items;
                var items = new List<Item>();

                foreach (var item in inputItems) items.Add(ItemFactory.FromDynamic(item));

                var markers = input.Markers as List<dynamic>;

                var insuranceDue = DateTime.FromBinary(input.InsuranceDue);
                var taxDue = DateTime.FromBinary(input.TaxDue);
                var motDue = DateTime.FromBinary(input.MotDue);

                var car = new Car
                {
                    NetworkId = input.NetworkId,
                    Plate = input.Plate,
                    Vin = input.Vin,
                    Owner = input.Owner,

                    InsuranceDue = insuranceDue,
                    TaxDue = taxDue,
                    MotDue = motDue,

                    Items = items,
                    Markers = markers.Cast<string>().ToList()
                };

                return car;
            }
            catch (RuntimeBinderException)
            {
                return null;
            }
        }

        public static Car Custom(int networkId,
            string plate,
            string owner,
            bool hasInsurance,
            bool hasTax,
            bool hasMot,
            string[] markers,
            string[] items)
        {
            var faker = new Faker();

            var car = new Car
            {
                NetworkId = networkId,
                Plate = plate,
                Owner = string.IsNullOrEmpty(owner) ? faker.Name.FullName() : owner,
                Vin = faker.Vehicle.Vin(),
                Markers = markers.ToList(),
                Items = ItemFactory.FromStringArray(items)
            };

            if (hasTax)
                car.TaxDue = faker.Date.Between(DateTime.Now, DateTime.Now.AddDays(365));
            else
                car.TaxDue = faker.Date.Between(DateTime.Now.AddDays(-365), DateTime.Now);

            if (hasInsurance)
                car.InsuranceDue = faker.Date.Between(DateTime.Now, DateTime.Now.AddDays(365));
            else
                car.InsuranceDue = faker.Date.Between(DateTime.Now.AddDays(-365), DateTime.Now);

            if (hasMot)
                car.MotDue = faker.Date.Between(DateTime.Now, DateTime.Now.AddDays(365));
            else
                car.MotDue = faker.Date.Between(DateTime.Now.AddDays(-365), DateTime.Now);

            return car;
        }

        /// <summary>
        ///     Creates a random Car.
        /// </summary>
        /// <param name="networkId">The network ID of the vehicle.</param>
        /// <param name="plate">The vehicle number plate.</param>
        /// <param name="driver">The driver's name.</param>
        /// <returns>A random Car.</returns>
        public static Car Random(int networkId, string plate, string driver = "")
        {
            var faker = new Faker();

            if (driver == "" || PoliceMpRandom.Next(100) <= DRIVER_NOT_OWNER_CHANCE) driver = faker.Name.FullName();


            var car = new Car
            {
                NetworkId = networkId,
                Plate = plate,
                Owner = driver,
                Vin = faker.Vehicle.Vin(),
                Items = ItemFactory.RandomList(),
                Markers = new List<string>()
            };

            var hasMarker = PoliceMpRandom.Next(100) <= MARKER_CHANCE;
            if (hasMarker)
            {
                var markerIndex = PoliceMpRandom.Next(PersonGenerationDetails.VehicleMarkers.Length - 1);
                car.Markers.Add(PersonGenerationDetails.VehicleMarkers[markerIndex]);
            }

            var hasMot = PoliceMpRandom.Next(100) <= HAS_MOT_CHANCE;
            var hasTax = PoliceMpRandom.Next(100) <= HAS_TAX_CHANCE;
            var hasInsurance = PoliceMpRandom.Next(100) <= HAS_INSURANCE_CHANCE;

            // random date between now and this time last year
            if (hasTax)
                car.TaxDue = faker.Date.Between(DateTime.Now, DateTime.Now.AddDays(365));
            else
                car.TaxDue = faker.Date.Between(DateTime.Now.AddDays(-365), DateTime.Now);

            if (hasInsurance)
                car.InsuranceDue = faker.Date.Between(DateTime.Now, DateTime.Now.AddDays(365));
            else
                car.InsuranceDue = faker.Date.Between(DateTime.Now.AddDays(-365), DateTime.Now);

            if (hasMot)
                car.MotDue = faker.Date.Between(DateTime.Now, DateTime.Now.AddDays(365));
            else
                car.MotDue = faker.Date.Between(DateTime.Now.AddDays(-365), DateTime.Now);

            return car;
        }
    }
}