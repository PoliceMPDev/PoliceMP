using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PoliceMP.Server.Factories;
using PoliceMP.Tests.Server.TestDataFactories;

namespace PoliceMP.Tests.Server.Factories
{
    [TestClass]
    public class BogusVehicleInfoFactoryTests
    {
        [TestMethod]
        public void Random_MaxIllegalItemsChance_ShouldReturnVehicleInfoWithIllegalItems()
        {
            var bogusVehicleInfoFactory = new BogusVehicleInfoFactory(
                OptionsFactory.VehicleInfoOptions(hasIllegalItemsChance: 100),
                OptionsFactory.ItemOptions(numberOfIllegalItems: 5));

            var vehicleInfo = bogusVehicleInfoFactory.Random(1, "plate");

            Assert.IsTrue(vehicleInfo.Items.Any(item => item.IsIllegal));
        }

        [TestMethod]
        public void Random_NoIllegalItemsChance_ShouldReturnVehicleInfoNoIllegalItems()
        {
            var bogusVehicleInfoFactory = new BogusVehicleInfoFactory(
                OptionsFactory.VehicleInfoOptions(hasIllegalItemsChance: 0),
                OptionsFactory.ItemOptions(numberOfIllegalItems: 5));

            var vehicleInfo = bogusVehicleInfoFactory.Random(1, "plate");

            Assert.IsFalse(vehicleInfo.Items.Any(item => item.IsIllegal));
        }

        [TestMethod]
        public void Random_MaxLegalItemsChance_ShouldReturnVehicleInfoWithLegalItems()
        {
            var bogusVehicleInfoFactory = new BogusVehicleInfoFactory(
                OptionsFactory.VehicleInfoOptions(hasLegalItemsChance: 100),
                OptionsFactory.ItemOptions(numberOfLegalItems: 5));

            var vehicleInfo = bogusVehicleInfoFactory.Random(1, "plate");

            Assert.IsTrue(vehicleInfo.Items.Any(item => !item.IsIllegal));
        }

        [TestMethod]
        public void Random_NoLegalItemsChance_ShouldReturnVehicleInfoNoLegalItems()
        {
            var bogusVehicleInfoFactory = new BogusVehicleInfoFactory(
                OptionsFactory.VehicleInfoOptions(hasLegalItemsChance: 0),
                OptionsFactory.ItemOptions(numberOfLegalItems: 5));

            var vehicleInfo = bogusVehicleInfoFactory.Random(1, "plate");

            Assert.IsFalse(vehicleInfo.Items.Any(item => !item.IsIllegal));
        }
    }
}