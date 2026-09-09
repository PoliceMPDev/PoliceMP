using PoliceMP.Main.Shared.Models;

namespace PoliceMP.Main.Shared.Factories
{
    /// <summary>
    ///     Used to create Offence objects.
    /// </summary>
    public static class OffenceFactory
    {
        /// <summary>
        ///     Convert a dynamic object to an Offence object.
        /// </summary>
        /// <param name="dyn">The dynamic object.</param>
        /// <returns>The Offence object.</returns>
        public static Offence FromDynamic(dynamic dyn)
        {
            return new Offence
            {
                Category = dyn.Category,
                Name = dyn.Name,
                Points = dyn.Points,
                Fine = dyn.Fine,
                SeizeVehicle = dyn.SeizeVehicle
            };
        }
    }
}