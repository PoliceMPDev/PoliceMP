using System.Collections.Generic;
using PoliceMP.Main.Core.Server;
using PoliceMP.Main.Core.Server.Enums;
using PoliceMP.Main.Core.Shared.Constants;

namespace PoliceMP.Callouts.Server.Models.CalloutTypes
{
    public class IllegalImmigrants : Callout
    {
        public IllegalImmigrants(int id) : base(id)
        {
            Title = "Illegal Immigrants";
            Grade = 2;
            LocationSetting = LocationSetting.GetNextPositionOnStreet;
        }

        public override void Setup()
        {
            AddEntity(new CalloutVehicle()
            {
                Key = "Vehicle",
                Hash = VehicleHash.Boxville,
                SpawnLocation = Location,
                HasBlip = true,
                BlipColor = BlipColor.Red,
                Colour = ServerFunctions.GetRandomVehicleColor(),
                Driver = "Driver",
                Passengers = new List<string>() { "Immigrant1", "Immigrant2", "Immigrant3" },
                GenerateInfoManual = true,
                Markers = new string[] { "Illegal Immigrant Tip" }
            });

            AddEntity(new CalloutPed()
            {
                Key = "Driver",
                Hash = (PedHash)(uint)PedModels.GetRandomHuman(),
                BlipColor = BlipColor.Red,
                HasBlip = true,
                SpawnLocation = Location,
            });

            AddEntity(new CalloutPed()
            {
                Key = "Immigrant1",
                Hash = (PedHash)(uint)PedModels.GetRandomMexican(),
                BlipColor = BlipColor.Red,
                HasBlip = true,
                SpawnLocation = Location,
                GenerateInfoManual = true,
                HasDrivingLicense = false,
                Markers = new string[] { "Illegal Immigrant" }
            });

            AddEntity(new CalloutPed()
            {
                Key = "Immigrant2",
                Hash = (PedHash)(uint)PedModels.GetRandomMexican(),
                BlipColor = BlipColor.Red,
                HasBlip = true,
                SpawnLocation = Location,
                GenerateInfoManual = true,
                HasDrivingLicense = false,
                Markers = new string[] { "Illegal Immigrant" }
            });

            AddEntity(new CalloutPed()
            {
                Key = "Immigrant3",
                Hash = (PedHash)(uint)PedModels.GetRandomMexican(),
                BlipColor = BlipColor.Red,
                HasBlip = true,
                SpawnLocation = Location,
                GenerateInfoManual = true,
                HasDrivingLicense = false,
                Markers = new string[] { "Illegal Immigrant" }
            });
        }

        protected override void OnEntitiesReady()
        {
            base.OnEntitiesReady();
            var ped = GetEntity("Driver");
            var vehicle = GetEntity("Vehicle");
            ClientEventAPI.SetPedTaskDriveWander(GetHost().Player, ped.NetworkId, vehicle.NetworkId);
        }
    }
}
