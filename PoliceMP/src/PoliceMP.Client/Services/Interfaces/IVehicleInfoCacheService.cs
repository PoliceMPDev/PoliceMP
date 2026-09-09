using PoliceMP.Shared.Models;

namespace PoliceMP.Client.Services.Interfaces
{
    public interface IVehicleInfoCacheService
    {
        VehicleInfo GetByNetworkId(int networkId);
        bool Cache(VehicleInfo vehicleInfo);
        bool RemoveCache(int vehicleNetworkId);
    }
}