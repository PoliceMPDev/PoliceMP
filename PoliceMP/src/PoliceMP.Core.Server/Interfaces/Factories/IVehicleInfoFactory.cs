using PoliceMP.Shared.Models;

namespace PoliceMP.Core.Server.Interfaces.Factories
{
    public interface IVehicleInfoFactory
    {
        VehicleInfo Random(int networkId, string plate, string ownerName = null);
    }
}