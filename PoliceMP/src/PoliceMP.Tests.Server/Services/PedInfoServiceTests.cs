using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using PoliceMP.Core.Server.Interfaces.Factories;
using PoliceMP.Core.Shared;
using PoliceMP.Server.Services;
using PoliceMP.Shared.Enums;
using PoliceMP.Shared.Models;

namespace PoliceMP.Tests.Server.Services
{
    [TestClass]
    public class PedInfoServiceTests
    {
        [TestMethod]
        public void GetByNetworkId_NewNetworkId_ReturnsNewPedInfo()
        {
            var mockPedInfoFactory = new Mock<IPedInfoFactory>();
            mockPedInfoFactory
                .Setup(x => x.Random(1, Gender.Male))
                .Returns(new PedInfo {NetworkId = 1, FirstName = "TestFirstName", LastName = "TestLastName"});
            var pedInfoService = new PedInfoService(mockPedInfoFactory.Object);

            var pedInfo = pedInfoService.GetByNetworkId(1);

            Assert.AreEqual(pedInfo.FullName, "TestFirstName TestLastName");
            Assert.AreEqual(pedInfo.NetworkId, 1);
        }

        [TestMethod]
        public void GetByNetworkId_ExistingNetworkId_ReturnsExistingPedInfo()
        {
            var mockPedInfoFactory = new Mock<IPedInfoFactory>();
            mockPedInfoFactory
                .Setup(x => x.Random(1, Gender.Male))
                .Returns(new PedInfo
                {
                    NetworkId = 1,
                    FirstName = "TestFirstName",
                    LastName = "TestLastName",
                    Attitude = AppRandom.Next(0, 9999)
                });
            var pedInfoService = new PedInfoService(mockPedInfoFactory.Object);

            var firstPedInfo = pedInfoService.GetByNetworkId(1);
            var secondPedInfo = pedInfoService.GetByNetworkId(1);

            Assert.AreEqual(secondPedInfo.NetworkId, 1);
            Assert.AreEqual(secondPedInfo.Attitude, firstPedInfo.Attitude);
        }

        [TestMethod]
        public void GetByName_ExistingName_ReturnsPedInfo()
        {
            var mockPedInfoFactory = new Mock<IPedInfoFactory>();
            mockPedInfoFactory
                .Setup(x => x.Random(1, Gender.Male))
                .Returns(new PedInfo { NetworkId = 1, FirstName = "TestFirstName", LastName = "TestLastName" });
            var pedInfoService = new PedInfoService(mockPedInfoFactory.Object);

            pedInfoService.GetByNetworkId(1);
            var pedInfo = pedInfoService.GetByName("TestFirstName TestLastName");

            Assert.AreEqual(pedInfo.FullName, "TestFirstName TestLastName");
        }

        [TestMethod]
        public void GetByName_NonExistingName_ReturnsNull()
        {
            var mockPedInfoFactory = new Mock<IPedInfoFactory>();
            var pedInfoService = new PedInfoService(mockPedInfoFactory.Object);

            var pedInfo = pedInfoService.GetByName("NonExisting");

            Assert.IsNull(pedInfo);
        }
    }
}