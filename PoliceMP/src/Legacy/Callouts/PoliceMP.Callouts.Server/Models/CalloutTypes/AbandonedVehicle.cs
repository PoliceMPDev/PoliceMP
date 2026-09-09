using CitizenFX.Core;
using PoliceMP.Main.Core.Server;
using PoliceMP.Main.Core.Shared;
using PoliceMP.Main.Core.Server.Enums;

namespace PoliceMP.Callouts.Server.Models.CalloutTypes
{
    public class AbandonedVehicle : Callout
    {
        public AbandonedVehicle(int id) : base(id)
        {
            Title = "Abandoned Vehicle";
            Grade = 3;
            Description = "Somebody has reported an abandoned vehicle.";
            LocationSetting = LocationSetting.GetNextPositionOnStreet;
        }

        public override void Setup()
        {
            AddEntity(new CalloutVehicle()
            {
                Key = "AbandonedVehicle",
                Hash = ServerFunctions.GetRandomVehicleHash(),
                SpawnLocation = Location,
                HasBlip = true,
                Colour = ServerFunctions.GetRandomVehicleColor(),
                PetrolTankHealth = PoliceMpRandom.Next(50, 1000),
                EngineHealth = PoliceMpRandom.Next(50, 1000),
                BodyHealth = PoliceMpRandom.Next(50, 1000),
                HasInsurance = PoliceMpRandom.Next(100) % 2 == 0,
                HasTax = false,
                HasMot = false,
                Markers = new string[] { "Wanker", "Dickhead" }
            });
        }

        public override void OnPlayerArrived(Player player)
        {
            base.OnPlayerArrived(player);
            ClientEventAPI.ShowSubtitle(player, "You have arrived. Investigate the ~y~abandoned vehicle~w~.", 5000);
        }
    }
}
