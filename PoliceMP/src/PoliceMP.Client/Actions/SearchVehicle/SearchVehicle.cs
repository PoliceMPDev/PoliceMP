using CitizenFX.Core;
using PoliceMP.Core.Client.Actions.Interfaces;

namespace PoliceMP.Client.Actions.SearchVehicle
{
    public class SearchVehicle : IAction
    {
        public Vehicle Target { get; set; }

        public SearchVehicle(Vehicle target)
        {
            Target = target;
        }
    }
}