using CitizenFX.Core;
using CitizenFX.Core.Native;
using PoliceMP.Main.Core.Client;
using PoliceMP.Main.Core.Client.Extensions;
using PoliceMP.Main.Shared.Events;
using PoliceMP.Main.Shared.Factories;
using PoliceMP.Main.Shared.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PoliceMP.Main.Client.Retrievers
{
    /// <summary>
    ///     Retrieves cars from the server.
    /// </summary>
    public class CarRetriever : BaseScript
    {
        /// <summary>
        ///     The latest retrieved car.
        /// </summary>
        private static Car _retrievedCar;

        /// <summary>
        ///     Gets a car by its entity ID.
        /// </summary>
        /// <param name="entityId">The vehicle entity ID.</param>
        /// <returns>The car.</returns>
        public static async Task<Car> GetCar(int entityId)
        {
            if (!API.DoesEntityExist(entityId)) return null;

            _retrievedCar = null;

            var vehicle = (Vehicle)Entity.FromHandle(entityId);
            if (vehicle == null) return null;

            if (vehicle.ClassType == VehicleClass.Emergency ||
                vehicle.ClassDisplayName.Equals("trailer", StringComparison.OrdinalIgnoreCase))
            {
                Debug.WriteLine($"[CarRetriever] Skipped vehicle: Class={vehicle.ClassType}, DisplayName={vehicle.ClassDisplayName}");
                return null;
            }

            var networkId = API.NetworkGetNetworkIdFromEntity(entityId);
            var plate = API.GetVehicleNumberPlateText(entityId);

            var driverId = API.GetPedInVehicleSeat(entityId, -1);

            if (driverId == 0)
                driverId = vehicle.GetLastDriver() == null ? -1 : vehicle.GetLastDriver().Handle;

            int driverNetworkId = -1;
            bool gender = false;

            if (API.DoesEntityExist(driverId) && driverId != -1)
            {
                gender = API.IsPedMale(driverId);
                driverNetworkId = API.NetworkGetNetworkIdFromEntity(driverId);
            }

            TriggerServerEvent(ServerEvents.GET_CAR_BY_ID, networkId, plate, driverNetworkId, gender);

            var count = 0;
            while (_retrievedCar == null || _retrievedCar.NetworkId != networkId)
            {
                await Delay(100);
                if (count > 10000) return null;
                count++;
            }

            return _retrievedCar;
        }

        /// <summary>
        ///     Gets a car by its number plate.
        /// </summary>
        /// <param name="plate">The number plate.</param>
        /// <returns>The car.</returns>
        public static async Task<Car> GetCar(string plate)
        {
            var vehicle = ClientFunctions.GetVehicleFromPlateText(plate);
            if (vehicle == null) return null;

            return await GetCar(vehicle.Handle);
        }

        /// <summary>
        ///     Handles the event to receive a car from the server.
        /// </summary>
        /// <param name="input">The dynamic car object.</param>
        [EventHandler(ClientEvents.RECEIVE_CAR)]
        private void ReceiveCar(dynamic input)
        {
            foreach (KeyValuePair<string, dynamic> item in input)
            {
                _retrievedCar = CarFactory.FromDynamic(item.Value);
                if (_retrievedCar == null)
                    this.SendChatMessage("Error receiving the person. Please contact the admin.");
                return;
            }
        }
    }
}