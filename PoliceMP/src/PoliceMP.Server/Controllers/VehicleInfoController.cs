using System.Collections.Generic;
using CitizenFX.Core;
using PoliceMP.Core.Server.Communications.Interfaces;
using PoliceMP.Core.Server.Networking;
using PoliceMP.Core.Shared;
using PoliceMP.Shared.Constants;
using PoliceMP.Shared.Models;
using System.Threading.Tasks;
using PoliceMP.Core.Server.Interfaces.Services;

namespace PoliceMP.Server.Controllers
{
    public class VehicleInfoController : Controller
    {
        private readonly ILegacyServerCommunicationsManager _comms;
        private readonly IVehicleInfoService _vehicleInfoService;
        private readonly ILogger<VehicleInfoController> _logger;

        public VehicleInfoController(ILegacyServerCommunicationsManager comms,
            IVehicleInfoService vehicleInfoService,
            ILogger<VehicleInfoController> logger)
        {
            _comms = comms;
            _vehicleInfoService = vehicleInfoService;
            _logger = logger;

            _comms.OnRequest<int, string, int, VehicleInfo>(ServerEvents.GetVehicleInfoByNetworkId,
                OnGetVehicleInfoByNetworkId);
            _comms.OnRequest<int, List<string>, bool>(ServerEvents.UpdateVehicleInfoMarkers, OnUpdateVehicleMarkers);
            _comms.OnRequest<int, List<bool>, bool>(ServerEvents.UpdatedVehicleExpiredMarkers, OnUpdateVehicleExpiredMarkers);
        }

        private Task<VehicleInfo> OnGetVehicleInfoByNetworkId(Player player,
            int networkId,
            string plate,
            int ownerPedNetworkId)
        {
            _logger.Trace($"OnGetByNetworkId Player: {player.Name} | NetworkId: {networkId} | Plate: {plate}");
            var vehicleInfo = _vehicleInfoService.GetByNetworkId(networkId, plate, ownerPedNetworkId);
            return Task.FromResult(vehicleInfo);
        }

        private Task<bool> OnUpdateVehicleMarkers(Player player, int networkId, List<string> markers)
        {
            return Task.FromResult(_vehicleInfoService.UpdateMarkers(networkId, markers));
        }

        private Task<bool> OnUpdateVehicleExpiredMarkers(Player player, int networkId, List<bool> expiredMarkers)
        {
            return Task.FromResult(_vehicleInfoService.UpdateExpiredMarkers(networkId, expiredMarkers));
        }
    }
}