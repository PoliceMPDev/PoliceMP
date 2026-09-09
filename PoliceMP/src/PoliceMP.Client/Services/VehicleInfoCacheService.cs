using PoliceMP.Client.Services.Interfaces;
using PoliceMP.Shared.Models;
using PoliceMP.Shared.Services.Interfaces;
using System.Collections.Concurrent;

namespace PoliceMP.Client.Services
{
    public class VehicleInfoCacheService : IVehicleInfoCacheService
    {
        public const int CacheForMinutes = 5;
        private readonly ConcurrentDictionary<int, CachedVehicleInfo> _vehicleInfoCache =
            new ConcurrentDictionary<int, CachedVehicleInfo>();

        private readonly IDateTimeService _dateTime;

        public VehicleInfoCacheService(IDateTimeService dateTime)
        {
            _dateTime = dateTime;
        }

        public VehicleInfo GetByNetworkId(int networkId)
        {
            if (_vehicleInfoCache.TryGetValue(networkId, out var cachedVehicleInfo))
            {
                if ((_dateTime.Now - cachedVehicleInfo.CachedAt).TotalMinutes < CacheForMinutes)
                {
                    return cachedVehicleInfo.VehicleInfo;
                }

                _vehicleInfoCache.TryRemove(networkId, out _);
            }

            return null;
        }

        public bool Cache(VehicleInfo vehicleInfo)
        {
            var cachedVehicleInfo = new CachedVehicleInfo(vehicleInfo);

            if (_vehicleInfoCache.ContainsKey(vehicleInfo.NetworkId))
            {
                _vehicleInfoCache[vehicleInfo.NetworkId] = cachedVehicleInfo;
                return true;
            }

            return _vehicleInfoCache.TryAdd(vehicleInfo.NetworkId, cachedVehicleInfo);
        }

        public bool RemoveCache(int vehicleNetworkId)
        {
            var tryGet = _vehicleInfoCache.TryGetValue(vehicleNetworkId, out CachedVehicleInfo cachedVehicleInfo);
            if (!tryGet) return true;
            return _vehicleInfoCache.TryRemove(vehicleNetworkId, out cachedVehicleInfo);
        }
    }
}