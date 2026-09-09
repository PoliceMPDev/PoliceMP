using CitizenFX.Core;
using PoliceMP.Core.Server.Communications.Interfaces;
using PoliceMP.Core.Server.Networking;
using PoliceMP.Core.Shared;
using PoliceMP.Shared.Constants;
using PoliceMP.Shared.Models;
using System.Threading.Tasks;
using PoliceMP.Core.Server.Interfaces.Services;
using PoliceMP.Server.Extensions;
using PoliceMP.Shared.Constants.States;

namespace PoliceMP.Server.Controllers
{
    public class PedInfoController : Controller
    {
        private readonly ILegacyServerCommunicationsManager _comms;
        private readonly IPedInfoService _pedInfoService;
        private readonly ILogger<PedInfoController> _logger;

        public PedInfoController(ILegacyServerCommunicationsManager comms, IPedInfoService pedInfoService, ILogger<PedInfoController> logger)
        {
            _comms = comms;
            _pedInfoService = pedInfoService;
            _logger = logger;

            _comms.OnRequest<int, PedInfo>(ServerEvents.GetPedInfoByNetworkId, OnGetByNetworkId);
            _comms.OnRequest<string, PedInfo>(ServerEvents.GetPedInfoByName, OnGetByName);
        }

        private Task<PedInfo> OnGetByNetworkId(Player player, int networkId)
        {
            _logger.Trace($"OnGetByNetworkId Player: {player?.Name ?? "Unknown"} | NetworkId: {networkId}");

            var pedInfo = _pedInfoService.GetByNetworkId(networkId);
            if (pedInfo == null)
            {
                _logger.Warn($"No PedInfo found for network ID: {networkId}");
                return Task.FromResult<PedInfo>(null);
            }

            var ped = Entity.FromNetworkId(pedInfo.NetworkId);
            if (ped == null)
            {
                _logger.Warn($"Entity could not be found from network ID: {pedInfo.NetworkId}");
                return Task.FromResult(pedInfo);
            }

            ped.State.Set(PedStates.FullName, pedInfo.FullName, true);
            return Task.FromResult(pedInfo);
        }

        private Task<PedInfo> OnGetByName(Player player, string name)
        {
            _logger.Trace($"OnGetByName Player: {player?.Name ?? "Unknown"} | Name: {name}");

            var pedInfo = _pedInfoService.GetByName(name);
            if (pedInfo == null)
            {
                _logger.Warn($"No PedInfo found for name: {name}");
                return Task.FromResult<PedInfo>(null);
            }

            var ped = Entity.FromNetworkId(pedInfo.NetworkId);
            if (ped == null)
            {
                _logger.Warn($"Entity could not be found from network ID: {pedInfo.NetworkId}");
                return Task.FromResult(pedInfo);
            }

            ped.State.Set(PedStates.FullName, pedInfo.FullName, true);
            return Task.FromResult(pedInfo);
        }
    }
}