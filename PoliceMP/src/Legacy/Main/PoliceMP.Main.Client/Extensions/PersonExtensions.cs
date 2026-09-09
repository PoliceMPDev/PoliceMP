using CitizenFX.Core;
using CitizenFX.Core.Native;
using PoliceMP.Main.Shared.Models;

namespace PoliceMP.Main.Client.Extensions
{
    public static class PersonExtensions
    {
        /// <summary>
        ///     Gets the Entity ID of the person.
        /// </summary>
        /// <param name="person">The Person.</param>
        /// <returns>The Entity ID.</returns>
        public static int EntityId(this Person person)
        {
            return API.NetworkGetEntityFromNetworkId(person.NetworkId);
        }

        /// <summary>
        ///     Gets the Ped object of the person.
        /// </summary>
        /// <param name="person">The Person.</param>
        /// <returns>The Ped object.</returns>
        public static Ped Ped(this Person person)
        {
            return (Ped)Entity.FromHandle(person.EntityId());
        }
    }
}
