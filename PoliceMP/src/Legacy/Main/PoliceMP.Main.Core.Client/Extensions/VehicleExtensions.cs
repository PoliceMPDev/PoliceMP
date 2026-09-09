using CitizenFX.Core;
using CitizenFX.Core.Native;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace PoliceMP.Main.Core.Client.Extensions
{
    public static class VehicleExtensions
    {
        public static bool IsAnyTyreBurst(this Vehicle vehicle, bool completely) =>
            API.IsVehicleTyreBurst(vehicle.Handle, 0, completely) ||
            API.IsVehicleTyreBurst(vehicle.Handle, 1, completely) ||
            API.IsVehicleTyreBurst(vehicle.Handle, 4, completely) ||
            API.IsVehicleTyreBurst(vehicle.Handle, 5, completely);

        public static int GetMph(this Vehicle vehicle) => (int)Math.Round(vehicle.Speed * 2.2369);

        public async static Task HonkHornAsync(this Vehicle vehicle)
        {
            API.StartVehicleHorn(vehicle.Handle, 250, 0, false);
            await BaseScript.Delay(250);
            API.StartVehicleHorn(vehicle.Handle, 250, 0, false);
            await BaseScript.Delay(250);
            API.StartVehicleHorn(vehicle.Handle, 250, 0, false);
        }

        public static bool CanPlayerInteractWithDriver(this Vehicle vehicle)
        {
            if (vehicle.IsSeatFree(VehicleSeat.Driver)) return false;

            // Check if it's a certain vehicle that doesn't have a door or its door
            // is in an awkward position. If so, just make sure the player is close enough
            if (vehicle.ClassType == VehicleClass.Cycles ||
                vehicle.ClassType == VehicleClass.Motorcycles ||
                vehicle.ClassType == VehicleClass.Commercial)
                return Game.PlayerPed.IsCloseEnoughToEntity(vehicle, 3f);

            // Otherwise, make sure the player is at the driver door
            return Game.PlayerPed.IsAtVehicleDriverDoor(vehicle);
        }

        public static string GetNumberPlateText(this Vehicle vehicle)
        {
            return API.GetVehicleNumberPlateText(vehicle.Handle);
        }

        /// <summary>
        /// Get the last driver of the vehicle.
        /// </summary>
        /// <param name="vehicle">The vehicle.</param>
        /// <returns>The last driver ped or null if none found.</returns>
        public static Ped GetLastDriver(this Vehicle vehicle)
        {
            return World.GetAllPeds()
                .ToList()
                .FirstOrDefault(p => p.LastVehicle == vehicle);
        }
    }
}
