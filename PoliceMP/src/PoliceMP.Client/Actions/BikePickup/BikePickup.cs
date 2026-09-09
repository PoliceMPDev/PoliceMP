using CitizenFX.Core;
using PoliceMP.Core.Client.Actions.Interfaces;

namespace PoliceMP.Client.Actions.BikePickup
{
    public class BikePickup : IAction
    {
        public Ped Subject { get; set; }
        public Vehicle Target { get; set; }

        public BikePickup(Ped subject, Vehicle target)
        {
            Subject = subject;
            Target = target;
        }
    }
}
