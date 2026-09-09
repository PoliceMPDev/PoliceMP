using System;
using System.Collections.Generic;
using System.Linq;
using PoliceMP.Client.Services;
using PoliceMP.Shared.Models;
using PoliceMP.Shared.Services;
using PoliceMP.Shared.Services.Interfaces;

namespace PoliceMP.Tests.Client.TestDataBuilders
{
    public class VehicleInfoCacheServiceBuilder
    {
        private List<VehicleInfo> _cache = new List<VehicleInfo>();
        private IDateTimeService _dateTimeService;

        public VehicleInfoCacheServiceBuilder WithThisManyVehicleInfos(int quantity)
        {
            foreach (int networkId in Enumerable.Range(0, quantity))
            {
                var vehicleInfo = new VehicleInfo {NetworkId = networkId};
                _cache.Add(vehicleInfo);
            }

            return this;
        }

        public VehicleInfoCacheServiceBuilder WithCachedNetworkId(int networkId)
        {
            var vehicleInfo = new VehicleInfo {NetworkId = networkId};
            _cache.Add(vehicleInfo);
            return this;
        }

        public VehicleInfoCacheServiceBuilder WithCachedVehicleInfo(VehicleInfo vehicleInfo)
        {
            _cache.Add(vehicleInfo);
            return this;
        }

        public VehicleInfoCacheServiceBuilder WithCurrentDateTime(DateTime dateTime)
        {
            _dateTimeService = new MockDateTimeService(dateTime);
            return this;
        }

        public VehicleInfoCacheService Build()
        {
            _dateTimeService = _dateTimeService ?? new DateTimeService();
            var vehicleInfoCacheService = new VehicleInfoCacheService(_dateTimeService);
            _cache.ForEach(vehicleInfo => vehicleInfoCacheService.Cache(vehicleInfo));
            return vehicleInfoCacheService;
        }
    }
}