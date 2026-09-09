using CitizenFX.Core;
using PoliceMP.Main.Shared.Events;
using PoliceMP.Main.Shared.Factories;
using PoliceMP.Main.Shared.Models;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;

namespace PoliceMP.Main.Server.Trackers
{
    /// <summary>
    ///     Keeps track of all the created Cars and handles
    ///     sending them to the client upon request.
    /// </summary>
    public class CarTracker : BaseScript
    {
        /// <summary>
        ///     The list of all the Cars that have been created.
        /// </summary>
        private static readonly List<Car> _cars = new List<Car>();

        /// <summary>
        ///     Handles the event for the client to receive a Car by its ID.
        /// </summary>
        /// <param name="player">The source player.</param>
        /// <param name="networkId">The vehicle network ID.</param>
        /// <param name="plate">The vehicle number plate.</param>
        /// <param name="driverNetworkId">The vehicle driver network ID. -1 if there is no driver.</param>
        /// <param name="driverGender">The gender of the driver if applicable.</param>
        [EventHandler(ServerEvents.GET_CAR_BY_ID)]
        private void GetCarById([FromSource] Player player, int networkId, string plate, int driverNetworkId = -1,
            bool driverGender = false)
        {
            // If there is a car in the list that matches this network ID but the plate is different
            // then remove that car so a new one can be generated. This is to do with network IDs being
            // assigned to different entities.
            var car = _cars.FirstOrDefault(c => c.NetworkId == networkId);
            if (car != null && !car.Plate.Equals(plate, StringComparison.OrdinalIgnoreCase))
            {
                _cars.Remove(car);
                car = null;
            }

            // If no valid car was found, then create a new one.
            if (car == null)
            {
                // Check if the car has a driver. If so, then create a person
                // for the driver.
                if (driverNetworkId != -1)
                {
                    var person = PersonTracker.GetPerson(driverNetworkId);
                    if (person == null)
                    {
                        person = PersonFactory.Random(driverNetworkId, driverGender);
                        PersonTracker.AddPerson(person);
                    }

                    car = CarFactory.Random(networkId, plate, person.FullName);
                }
                else
                {
                    car = CarFactory.Random(networkId, plate);
                }

                _cars.Add(car);
            }

            // Finally, send the car to the client.
            SendCarToClient(player, car);
        }

        [EventHandler(ServerEvents.ADD_CAR_MANUAL)]
        private void AddCarManual(int networkId,
            string plate,
            string owner,
            bool hasInsurance,
            bool hasTax,
            bool hasMot,
            List<dynamic> markers,
            List<dynamic> items)
        {
            var car = CarFactory.Custom(networkId,
                plate,
                owner,
                hasInsurance,
                hasTax,
                hasMot,
                markers.Cast<string>().ToArray(),
                items.Cast<string>().ToArray());

            _cars.Add(car);
        }

        /// <summary>
        ///     Sends the Car to the client.
        /// </summary>
        /// <param name="player">The client player.</param>
        /// <param name="car">The Car.</param>
        private static void SendCarToClient(Player player, Car car)
        {
            var dynCar = CarFactory.ToDynamic(car);
            var dict = new ExpandoObject() as IDictionary<string, dynamic>;

            dict.Add("1", dynCar);
            player.TriggerEvent(ClientEvents.RECEIVE_CAR, dict);
        }
    }
}