using CitizenFX.Core;
using System.Linq;
using PoliceMP.Main.Core.Server;
using PoliceMP.Main.Core.Server.Enums;
using PoliceMP.Main.Core.Shared;
using PoliceMP.Main.Core.Shared.Constants;

namespace PoliceMP.Callouts.Server.Models.CalloutTypes
{
    public class ANPRHit : Callout
    {
        public ANPRHit(int id) : base(id)
        {
            Title = "ANPR Hit";
            Grade = 2;
            LocationSetting = LocationSetting.GetNextPositionOnStreet;
        }

        public override void Setup()
        {
            var markers = PersonGenerationDetails.VehicleMarkers
                .OrderBy(x => PoliceMpRandom.Next(5))
                .Take(PoliceMpRandom.Next(0, 2));

            AddEntity(new CalloutVehicle()
            {
                Key = "Vehicle",
                Hash = ServerFunctions.GetRandomVehicleHash(),
                SpawnLocation = Location,
                HasBlip = true,
                BlipColor = BlipColor.Red,
                Colour = ServerFunctions.GetRandomVehicleColor(),
                Driver = "Driver",

                GenerateInfoManual = true,
                HasInsurance = PoliceMpRandom.Next(100) % 2 == 0,
                HasTax = PoliceMpRandom.Next(100) % 2 == 0,
                HasMot = PoliceMpRandom.Next(100) % 2 == 0,
                Markers = markers.ToArray()
            });

            AddEntity(new CalloutPed()
            {
                Key = "Driver",
                Hash = (PedHash)(uint)PedModels.GetRandomHuman(),
                BlipColor = BlipColor.Red,
                HasBlip = true,
                SpawnLocation = Location,
            });
        }

        protected override void OnFirstPlayerArrived(Player player)
        {
            base.OnFirstPlayerArrived(player);
            var ped = GetEntity("Driver");
            var vehicle = GetEntity("Vehicle");
            ClientEventAPI.SetPedTaskDriveWander(player, ped.NetworkId, vehicle.NetworkId);
        }
    }
}
