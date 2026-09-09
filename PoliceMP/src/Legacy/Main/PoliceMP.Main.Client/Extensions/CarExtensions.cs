using CitizenFX.Core;
using CitizenFX.Core.Native;
using PoliceMP.Main.Shared.Models;

namespace PoliceMP.Main.Client.Extensions
{
    public static class CarExtensions
    {
        /// <summary>
        ///     Gets the Entity ID of the car.
        /// </summary>
        /// <param name="car">The Car.</param>
        /// <returns>The Entity ID.</returns>
        public static int EntityId(this Car car)
        {
            return API.NetworkGetEntityFromNetworkId(car.NetworkId);
        }

        /// <summary>
        ///     Gets the Vehicle object of the car.
        /// </summary>
        /// <param name="car">The Car.</param>
        /// <returns>The Vehicle object.</returns>
        public static Vehicle Vehicle(this Car car)
        {
            return (Vehicle)Entity.FromHandle(car.EntityId());
        }
    }
}
