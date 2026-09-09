using PoliceMP.Client.Services.Interfaces;
using PoliceMP.Core.Shared.Extensions;
using PoliceMP.Shared.Models;
using System;
using System.Collections.Concurrent;
using System.Linq;
using PoliceMP.Shared.Services.Interfaces;

namespace PoliceMP.Client.Services
{
    public class PedInfoCacheService : IPedInfoCacheService
    {
        public const int CacheForMinutes = 5;
        private readonly ConcurrentDictionary<int, CachedPedInfo> _pedInfoCache = new ConcurrentDictionary<int, CachedPedInfo>();
        private readonly IDateTimeService _dateTime;

        public PedInfoCacheService(IDateTimeService dateTime)
        {
            _dateTime = dateTime;
        }

        public PedInfo GetByNetworkId(int networkId)
        {
            if (_pedInfoCache.TryGetValue(networkId, out var cachedPedInfo))
            {
                if ((_dateTime.Now - cachedPedInfo.CachedAt).TotalMinutes < CacheForMinutes)
                {
                    return cachedPedInfo.PedInfo;
                }

                _pedInfoCache.TryRemove(networkId, out _);
            }

            return null;
        }

        public PedInfo GetByName(string name)
        {
            var cachedPedInfo = _pedInfoCache.Values.FirstOrDefault(_ => _.PedInfo.FullName.EqualsIgnoreCase(name));
            return cachedPedInfo?.PedInfo;
        }

        public bool Cache(PedInfo pedInfo)
        {
            var cachedPedInfo = new CachedPedInfo(pedInfo);

            if (_pedInfoCache.ContainsKey(pedInfo.NetworkId))
            {
                _pedInfoCache[pedInfo.NetworkId] = cachedPedInfo;
                return true;
            }

            return _pedInfoCache.TryAdd(pedInfo.NetworkId, cachedPedInfo);
        }
    }
}