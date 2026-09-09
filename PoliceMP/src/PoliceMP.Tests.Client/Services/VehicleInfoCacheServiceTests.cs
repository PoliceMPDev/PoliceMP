using Microsoft.VisualStudio.TestTools.UnitTesting;
using PoliceMP.Client.Services;
using PoliceMP.Shared.Models;
using System;
using PoliceMP.Tests.Client.TestDataBuilders;

namespace PoliceMP.Tests.Client.Services
{
    [TestClass]
    public class VehicleInfoCacheServiceTests
    {
        [TestMethod]
        public void GetByNetworkId_ExistingNetworkId_ReturnsVehicleInfo()
        {
            var vehicleInfoCacheService = new VehicleInfoCacheServiceBuilder()
                .WithThisManyVehicleInfos(10)
                .WithCachedNetworkId(1337)
                .Build();

            var vehicleInfo = vehicleInfoCacheService.GetByNetworkId(1337);

            Assert.AreEqual(vehicleInfo.NetworkId, 1337);
        }

        [TestMethod]
        public void GetByNetworkId_NonExistingNetworkId_ReturnsNull()
        {
            var vehicleInfoCacheService = new VehicleInfoCacheServiceBuilder()
                .WithThisManyVehicleInfos(10)
                .Build();

            var vehicleInfo = vehicleInfoCacheService.GetByNetworkId(1337);

            Assert.IsNull(vehicleInfo);
        }

        [TestMethod]
        public void Cache_WithNewVehicleInfo_CanThenGetByNetworkId()
        {
            var vehicleInfoCacheService = new VehicleInfoCacheServiceBuilder()
                .WithThisManyVehicleInfos(10)
                .Build();
            var vehicleInfo = new VehicleInfo {NetworkId = 1337};

            bool cacheSuccess = vehicleInfoCacheService.Cache(vehicleInfo);
            var cachedVehicleInfo = vehicleInfoCacheService.GetByNetworkId(1337);

            Assert.IsTrue(cacheSuccess);
            Assert.AreEqual(cachedVehicleInfo.NetworkId, 1337);
        }

        [TestMethod]
        public void Cache_WithExistingVehicleInfo_CanThenGetUpdatedByNetworkId()
        {
            var vehicleInfoCacheService = new VehicleInfoCacheServiceBuilder()
                .WithThisManyVehicleInfos(10)
                .WithCachedVehicleInfo(new VehicleInfo
                {
                    NetworkId = 1337,
                    Plate = "OldPlate"
                })
                .Build();
            var vehicleInfo = new VehicleInfo {NetworkId = 1337, Plate = "NewPlate"};

            bool cacheSuccess = vehicleInfoCacheService.Cache(vehicleInfo);
            var cachedVehicleInfo = vehicleInfoCacheService.GetByNetworkId(1337);

            Assert.IsTrue(cacheSuccess);
            Assert.AreEqual(cachedVehicleInfo.NetworkId, 1337);
            Assert.AreEqual(cachedVehicleInfo.Plate, "NewPlate");
        }

        [TestMethod]
        public void GetByNetworkId_CacheTimeExpired_ReturnsNull()
        {
            var vehicleInfoCacheService = new VehicleInfoCacheServiceBuilder()
                .WithThisManyVehicleInfos(10)
                .WithCachedVehicleInfo(new VehicleInfo
                {
                    NetworkId = 1337,
                    Plate = "OldPlate"
                })
                .WithCurrentDateTime(DateTime.Now.AddMinutes(VehicleInfoCacheService.CacheForMinutes + 5))
                .Build();

            var vehicleInfo = vehicleInfoCacheService.GetByNetworkId(1337);

            Assert.IsNull(vehicleInfo);
        }
    }
}