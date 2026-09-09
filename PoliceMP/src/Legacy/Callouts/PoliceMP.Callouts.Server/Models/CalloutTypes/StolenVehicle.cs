using CitizenFX.Core;
using PoliceMP.Main.Core.Server;
using PoliceMP.Main.Core.Server.Enums;
using PoliceMP.Main.Core.Shared.Constants;

namespace PoliceMP.Callouts.Server.Models.CalloutTypes
{
    public class StolenVehicle : Callout
    {
        public StolenVehicle(int id) : base(id)
        {
            Title = "Stolen Vehicle";
            Grade = 2;
            LocationSetting = LocationSetting.GetNextPositionOnStreet;
        }

        public override void Setup()
        {
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
                Markers = new string[] { "Stolen" }
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
