using CitizenFX.Core;
using PoliceMP.Main.Core.Server;
using PoliceMP.Main.Core.Server.Enums;
using PoliceMP.Main.Core.Shared.Constants;

namespace PoliceMP.Callouts.Server.Models.CalloutTypes
{
    class FTS : Callout
    {
        public FTS(int id) : base(id)
        {
            Title = "FTS Intel";
            Grade = 1;
            Description = "A vehicle that has previously failed to stop for police has been spotted.";
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
                HasInsurance = false,
                HasTax = false,
                HasMot = false,
                Markers = new string[] { "Previously Failed To Stop" }
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
