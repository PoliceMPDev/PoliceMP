using CitizenFX.Core;
using PoliceMP.Main.Core.Server.Enums;
using PoliceMP.Main.Core.Shared.Constants;

namespace PoliceMP.Callouts.Server.Models.CalloutTypes
{
    class HouseRaid : Callout
    {

        private House house;
        private Interior houseInterior;


        public HouseRaid(int id) : base(id)
        {
            house = Util.GetRandomHouse();
            houseInterior = Util.GetInteriorFromHouse(house);
            Title = "House Raid" + " [" + house.HouseNameOrNumber + ", " + house.HouseStreet + "]";
            Grade = 3;
            Location = new Vector3(house.FrontDoorX, house.FrontDoorY, house.FrontDoorZ);
        }

        public override void Setup()
        {
            AddEntity(new CalloutPed()
            {
                Key = "Civ",
                Hash = (PedHash)(uint)PedModels.GetRandomHuman(),
                HasBlip = true,
                SpawnLocation = new Vector3(houseInterior.XFront, houseInterior.YFront, houseInterior.ZFront),
                Warrants = new string[] { "Arrest and Search" },
                Charges = new string[] { Util.GetRandomHouseRaidCrime() },
                Markers = new string[] { "Has a warrant" }
            });
        }

        protected override void OnEntitiesReady()
        {
            base.OnEntitiesReady();

            var civ = (CalloutPed)GetEntity("Civ");
            TriggerEvent("PoliceMPHousing:RegisterEntityInHouse", house.HouseID, civ.NetworkId);
        }

        protected override void OnFirstPlayerArrived(Player player)
        {
            base.OnFirstPlayerArrived(player);

            var civ = (CalloutPed)GetEntity("Civ");
            ClientEventAPI.SetPedFleeFromPlayer(player, civ.NetworkId);

            // Add blip

        }

        public override void OnPlayerArrived(Player player)
        {
            base.OnPlayerArrived(player);
            ClientEventAPI.AddBlipForCoord(player, (int)BlipSprite.CaptureHouse, this.Location, (int)BlipColor.Red, false, false, "Raid Target");
        }
    }
}
