using System.Collections.Generic;
using PoliceMP.Shared.Models;

namespace PoliceMP.Core.Server.Interfaces.Services
{
    public interface IVehicleInfoService
    {
        VehicleInfo GetByNetworkId(int networkId, string plate, int ownerPedNetworkId = -1);
        bool UpdateMarkers(int networkId, List<string> markers);
        bool UpdateExpiredMarkers(int networkId, List<bool> expiredMarkers);
    }
}