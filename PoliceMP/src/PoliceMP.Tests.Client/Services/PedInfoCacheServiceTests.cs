using Microsoft.VisualStudio.TestTools.UnitTesting;
using PoliceMP.Client.Services;
using PoliceMP.Shared.Models;
using System;
using PoliceMP.Tests.Client.TestDataBuilders;

namespace PoliceMP.Tests.Client.Services
{
    [TestClass]
    public class PedInfoCacheServiceTests
    {
        [TestMethod]
        public void GetByNetworkId_ExistingNetworkId_ReturnsPedInfo()
        {
            var pedInfoCacheService = new PedInfoCacheServiceBuilder()
                .WithThisManyCachedPedInfos(10)
                .WithCachedNetworkId(1337)
                .Build();

            var pedInfo = pedInfoCacheService.GetByNetworkId(1337);

            Assert.AreEqual(pedInfo.NetworkId, 1337);
        }

        [TestMethod]
        public void GetByNetworkId_NonExistingNetworkId_ReturnsNull()
        {
            var pedInfoCacheService = new PedInfoCacheServiceBuilder()
                .WithThisManyCachedPedInfos(10)
                .Build();

            var pedInfo = pedInfoCacheService.GetByNetworkId(1337);

            Assert.IsNull(pedInfo);
        }

        [TestMethod]
        public void GetByName_ExistingName_ReturnsNull()
        {
            var pedInfoCacheService = new PedInfoCacheServiceBuilder()
                .WithThisManyCachedPedInfos(10)
                .WithCachedName("I dae", "exist")
                .Build();

            var pedInfo = pedInfoCacheService.GetByName("I dae exist");

            Assert.AreEqual(pedInfo.FullName, "I dae exist");
        }

        [TestMethod]
        public void GetByName_NonExistingName_ReturnsNull()
        {
            var pedInfoCacheService = new PedInfoCacheServiceBuilder()
                .WithThisManyCachedPedInfos(10)
                .Build();

            var pedInfo = pedInfoCacheService.GetByName("I dinnae exist");

            Assert.IsNull(pedInfo);
        }

        [TestMethod]
        public void Cache_WithNewPedInfo_CanThenGetByNetworkId()
        {
            var pedInfoCacheService = new PedInfoCacheServiceBuilder()
                .WithThisManyCachedPedInfos(10)
                .Build();
            var pedInfo = new PedInfo {NetworkId = 1337};

            bool cacheSuccess = pedInfoCacheService.Cache(pedInfo);
            var cachedPedInfo = pedInfoCacheService.GetByNetworkId(1337);

            Assert.IsTrue(cacheSuccess);
            Assert.AreEqual(cachedPedInfo.NetworkId, 1337);
        }

        [TestMethod]
        public void Cache_WithExistingPedInfo_CanThenGetUpdatedByNetworkId()
        {
            var pedInfoCacheService = new PedInfoCacheServiceBuilder()
                .WithThisManyCachedPedInfos(10)
                .WithCachedPedInfo(new PedInfo
                {
                    NetworkId = 1337,
                    FirstName = "OldFirstName",
                    LastName = "OldLastName"
                })
                .Build();
            var pedInfo = new PedInfo { NetworkId = 1337, FirstName = "NewFirstName", LastName = "NewLastName"};

            bool cacheSuccess = pedInfoCacheService.Cache(pedInfo);
            var cachedPedInfo = pedInfoCacheService.GetByNetworkId(1337);

            Assert.IsTrue(cacheSuccess);
            Assert.AreEqual(cachedPedInfo.NetworkId, 1337);
            Assert.AreEqual(cachedPedInfo.FirstName, "NewFirstName");
            Assert.AreEqual(cachedPedInfo.LastName, "NewLastName");
        }

        [TestMethod]
        public void GetByNetworkId_CacheTimeExpired_ReturnsNull()
        {
            var pedInfoCacheService = new PedInfoCacheServiceBuilder()
                .WithThisManyCachedPedInfos(10)
                .WithCachedPedInfo(new PedInfo
                {
                    NetworkId = 1337,
                    FirstName = "OldFirstName",
                    LastName = "OldLastName"
                })
                .WithCurrentDateTime(DateTime.Now.AddMinutes(PedInfoCacheService.CacheForMinutes + 5))
                .Build();

            var pedInfo = pedInfoCacheService.GetByNetworkId(1337);

            Assert.IsNull(pedInfo);
        }
    }
}
