using System;
using System.Collections.Generic;
using System.Linq;
using PoliceMP.Client.Services;
using PoliceMP.Shared.Models;
using PoliceMP.Shared.Services;
using PoliceMP.Shared.Services.Interfaces;

namespace PoliceMP.Tests.Client.TestDataBuilders
{
    public class PedInfoCacheServiceBuilder
    {
        private List<PedInfo> _cache = new List<PedInfo>();
        private IDateTimeService _dateTimeService;

        public PedInfoCacheServiceBuilder WithThisManyCachedPedInfos(int quantity)
        {
            foreach (int networkId in Enumerable.Range(0, quantity))
            {
                var pedInfo = new PedInfo {NetworkId = networkId};
                _cache.Add(pedInfo);
            }

            return this;
        }

        public PedInfoCacheServiceBuilder WithCachedNetworkId(int networkId)
        {
            var pedInfo = new PedInfo { NetworkId = networkId };
            _cache.Add(pedInfo);
            return this;
        }

        public PedInfoCacheServiceBuilder WithCachedName(string firstName, string lastName)
        {
            var pedInfo = new PedInfo
            {
                NetworkId = -1,
                FirstName = firstName,
                LastName = lastName
            };
            _cache.Add(pedInfo);
            return this;
        }

        public PedInfoCacheServiceBuilder WithCachedPedInfo(PedInfo pedInfo)
        {
            _cache.Add(pedInfo);
            return this;
        }

        public PedInfoCacheServiceBuilder WithCurrentDateTime(DateTime dateTime)
        {
            _dateTimeService = new MockDateTimeService(dateTime);
            return this;
        }

        public PedInfoCacheService Build()
        {
            _dateTimeService = _dateTimeService ?? new DateTimeService();
            var pedInfoCacheService = new PedInfoCacheService(_dateTimeService);
            _cache.ForEach(pedInfo => pedInfoCacheService.Cache(pedInfo));
            return pedInfoCacheService;
        }
    }
}