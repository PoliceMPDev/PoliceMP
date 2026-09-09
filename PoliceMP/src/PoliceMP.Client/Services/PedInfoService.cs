using PoliceMP.Client.Services.Interfaces;
using PoliceMP.Core.Client.Communications.Interfaces;
using PoliceMP.Core.Shared;
using PoliceMP.Shared.Constants;
using PoliceMP.Shared.Models;
using System.Threading.Tasks;

namespace PoliceMP.Client.Services
{
    public class PedInfoService : IPedInfoService
    {
        private readonly ILegacyClientCommunicationsManager _comms;
        private readonly ILogger<PedInfoService> _logger;
        private readonly IPedInfoCacheService _pedInfoCacheService;

        public PedInfoService(ILegacyClientCommunicationsManager comms,
            ILogger<PedInfoService> logger,
            IPedInfoCacheService pedInfoCacheService)
        {
            _comms = comms;
            _logger = logger;
            _pedInfoCacheService = pedInfoCacheService;
        }
        

        public async Task<PedInfo> GetByNetworkId(int networkId)
        {
            var cachedPedInfo = _pedInfoCacheService.GetByNetworkId(networkId);
            if (cachedPedInfo != null) return cachedPedInfo;

            _logger.Trace($"Retrieving PedInfo for NetworkId {networkId}");
            var pedInfo = await _comms.Request<PedInfo>(ServerEvents.GetPedInfoByNetworkId, networkId);

            if (pedInfo == null)
            {
                _logger.Trace($"Failed to get PedInfo for NetworkId {networkId}");
                return null;
            }

            _logger.Trace($"Successfully got PedInfo for NetworkId {networkId}");
            _pedInfoCacheService.Cache(pedInfo);

            return pedInfo;
        }

        public async Task<PedInfo> GetByName(string name)
        {
            var cachedPedInfo = _pedInfoCacheService.GetByName(name);
            if (cachedPedInfo != null) return cachedPedInfo;

            _logger.Trace($"Retrieving PedInfo for name {name}");
            var pedInfo = await _comms.Request<PedInfo>(ServerEvents.GetPedInfoByName, name);

            if (pedInfo == null)
            {
                _logger.Trace($"Failed to get PedInfo for Name {name}");
                return null;
            }

            _logger.Trace($"Successfully got PedInfo for Name {name}");
            _pedInfoCacheService.Cache(pedInfo);

            return pedInfo;
        }
    }
}