using PoliceMP.Shared.Enums;
using PoliceMP.Shared.Models;
using System;
using System.Collections.Concurrent;
using System.Linq;
using PoliceMP.Core.Server.Interfaces.Factories;
using PoliceMP.Core.Server.Interfaces.Services;

namespace PoliceMP.Server.Services
{
    public class PedInfoService : IPedInfoService
    {
        private readonly IPedInfoFactory _pedInfoFactory;
        private readonly ConcurrentDictionary<int, PedInfo> _peds;

        public PedInfoService(IPedInfoFactory pedInfoFactory)
        {
            _pedInfoFactory = pedInfoFactory;
            _peds = new ConcurrentDictionary<int, PedInfo>();
        }

        public PedInfo GetByNetworkId(int networkId)
        {
            if (_peds.TryGetValue(networkId, out var pedInfo))
            {
                return pedInfo;
            }

            pedInfo = _pedInfoFactory.Random(networkId, Gender.Male);
            return _peds.TryAdd(networkId, pedInfo) ? pedInfo : new PedInfo { NetworkId = -1 };
        }

        public PedInfo GetByName(string name)
        {
            var pedInfo = _peds.FirstOrDefault(x =>
                x.Value.FullName.Equals(name, StringComparison.InvariantCultureIgnoreCase))
                .Value;
            return pedInfo;
        }

        public bool AddOrUpdate(PedInfo pedInfo)
        {
            return _peds.TryAdd(pedInfo.NetworkId, pedInfo);
        }
    }
}