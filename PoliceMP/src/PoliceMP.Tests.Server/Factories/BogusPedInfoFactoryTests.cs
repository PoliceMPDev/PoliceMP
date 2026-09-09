using Microsoft.VisualStudio.TestTools.UnitTesting;
using PoliceMP.Server.Factories;
using PoliceMP.Shared.Enums;
using PoliceMP.Tests.Server.TestDataFactories;
using System.Linq;

namespace PoliceMP.Tests.Server.Factories
{
    [TestClass]
    public class BogusPedInfoFactoryTests
    {
        [TestMethod]
        public void Random_MaxIllegalItemsChance_ShouldReturnPedInfoWithIllegalItems()
        {
            var bogusPedInfoFactory = new BogusPedInfoFactory(
                OptionsFactory.PedInfoOptions(hasIllegalItemsChance: 100),
                OptionsFactory.WorldOptions(),
                OptionsFactory.ItemOptions(numberOfIllegalItems: 5));

            var pedInfo = bogusPedInfoFactory.Random(1, Gender.Male);
            
            Assert.IsTrue(pedInfo.Items.Any(item => item.IsIllegal));
        }

        [TestMethod]
        public void Random_NoIllegalItemsChance_ShouldReturnPedInfoWithNoIllegalItems()
        {
            var bogusPedInfoFactory = new BogusPedInfoFactory(
                OptionsFactory.PedInfoOptions(hasIllegalItemsChance: 0),
                OptionsFactory.WorldOptions(),
                OptionsFactory.ItemOptions(numberOfIllegalItems: 5));

            var pedInfo = bogusPedInfoFactory.Random(1, Gender.Male);

            Assert.IsFalse(pedInfo.Items.Any(item => item.IsIllegal));
        }

        [TestMethod]
        public void Random_MaxLegalItemsChance_ShouldReturnPedInfoWithLegalItems()
        {
            var bogusPedInfoFactory = new BogusPedInfoFactory(
                OptionsFactory.PedInfoOptions(hasLegalItemsChance: 100),
                OptionsFactory.WorldOptions(),
                OptionsFactory.ItemOptions(numberOfLegalItems: 5));

            var pedInfo = bogusPedInfoFactory.Random(1, Gender.Male);

            Assert.IsTrue(pedInfo.Items.Any(item => !item.IsIllegal));
        }

        [TestMethod]
        public void Random_NoLegalItemsChance_ShouldReturnPedInfoWithNoLegalItems()
        {
            var bogusPedInfoFactory = new BogusPedInfoFactory(
                OptionsFactory.PedInfoOptions(hasLegalItemsChance: 0),
                OptionsFactory.WorldOptions(),
                OptionsFactory.ItemOptions(numberOfLegalItems: 5));

            var pedInfo = bogusPedInfoFactory.Random(1, Gender.Male);

            Assert.IsFalse(pedInfo.Items.Any(item => !item.IsIllegal));
        }
    }
}