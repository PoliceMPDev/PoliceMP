using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using PoliceMP.Core.Server.Interfaces.Factories;
using PoliceMP.Core.Server.Interfaces.Services;
using PoliceMP.Core.Shared;
using PoliceMP.Server.Services;
using PoliceMP.Shared.Models;

namespace PoliceMP.Tests.Server.Services
{
    [TestClass]
    public class VehicleInfoServiceTests
    {
        [TestMethod]
        public void GetByNetworkId_NewNetworkId_ReturnsNewVehicleInfo()
        {
            var mockVehicleInfoFactory = new Mock<IVehicleInfoFactory>();
            mockVehicleInfoFactory
                .Setup(x => x.Random(1, "NewPlate", null))
                .Returns(new VehicleInfo {NetworkId = 1, Plate = "NewPlate", VIN = "TestVin"});
            var mockPedInfoService = new Mock<IPedInfoService>();
            var vehicleInfoService = new VehicleInfoService(
                mockVehicleInfoFactory.Object,
                mockPedInfoService.Object);

            var vehicleInfo = vehicleInfoService.GetByNetworkId(1, "NewPlate");

            Assert.AreEqual(vehicleInfo.NetworkId, 1);
            Assert.AreEqual(vehicleInfo.Plate, "NewPlate");
            Assert.AreEqual(vehicleInfo.VIN, "TestVin");
        }

        [TestMethod]
        public void GetByNetworkId_ExistingNetworkId_ReturnsExistingVehicleInfo()
        {
            var mockVehicleInfoFactory = new Mock<IVehicleInfoFactory>();
            mockVehicleInfoFactory
                .Setup(x => x.Random(1, "NewPlate", null))
                .Returns(new VehicleInfo { NetworkId = 1, Plate = "NewPlate", VIN = AppRandom.Next(0, 9999).ToString() });
            var mockPedInfoService = new Mock<IPedInfoService>();
            var vehicleInfoService = new VehicleInfoService(
                mockVehicleInfoFactory.Object,
                mockPedInfoService.Object);

            var firstVehicleInfo = vehicleInfoService.GetByNetworkId(1, "NewPlate");
            var secondVehicleInfo = vehicleInfoService.GetByNetworkId(1, "NewPlate");

            Assert.AreEqual(secondVehicleInfo.NetworkId, 1);
            Assert.AreEqual(secondVehicleInfo.Plate, "NewPlate");
            Assert.AreEqual(secondVehicleInfo.VIN, firstVehicleInfo.VIN);
        }
    }
}