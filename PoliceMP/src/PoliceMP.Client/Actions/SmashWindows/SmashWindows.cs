using CitizenFX.Core;
using PoliceMP.Core.Client.Actions.Interfaces;

namespace PoliceMP.Client.Actions.SmashWindows
{
    public class SmashWindows : IAction
    {
        public Vehicle Target { get; }
        public VehicleDoorIndex Window { get; set; }

        public SmashWindows(Vehicle target, VehicleDoorIndex window)
        {
            Target = target;
            Window = window;
        }
    }
}