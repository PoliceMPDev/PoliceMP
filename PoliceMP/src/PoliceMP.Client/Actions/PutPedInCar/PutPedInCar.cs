using CitizenFX.Core;
using PoliceMP.Core.Client.Actions.Interfaces;

namespace PoliceMP.Client.Actions.PutPedInCar
{
    public class PutPedInCar : IAction
    {
        public Ped Subject { get; }
        public Ped Target { get; }
        public Vehicle Vehicle { get; }
        public VehicleDoorIndex VehicleDoorIndex { get; }


        public PutPedInCar(Ped subject, Ped target, Vehicle vehicle, VehicleDoorIndex vehicleDoorIndex = (VehicleDoorIndex)(-1))
        {
            Subject = subject;
            Target = target;
            Vehicle = vehicle;
            VehicleDoorIndex = vehicleDoorIndex;
        }
    }
}
