using CitizenFX.Core;
using PoliceMP.Core.Client.Actions.Interfaces;

namespace PoliceMP.Client.Actions.UseVehicleDoor
{
    public class UseVehicleDoor : IAction
    {
        public Vehicle Target { get; }
        public VehicleDoorIndex Door { get; set; }

        public UseVehicleDoor(Vehicle target, VehicleDoorIndex door)
        {
            Target = target;
            Door = door;
        }
    }
}