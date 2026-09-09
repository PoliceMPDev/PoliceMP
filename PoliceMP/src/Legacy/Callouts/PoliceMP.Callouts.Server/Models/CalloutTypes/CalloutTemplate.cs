using CitizenFX.Core;
using PoliceMP.Callouts.Server.Enums;
using System;
using System.Collections.Generic;
using PoliceMP.Main.Core.Server.Enums;
using PoliceMP.Main.Core.Shared.Constants;

namespace PoliceMP.Callouts.Server.Models.CalloutTypes
{
    public class CalloutTemplate : Callout
    {
        public CalloutTemplate(int id) : base(id)
        {
            // Example vehicle
            AddEntity(new CalloutVehicle()
            {
                Key = "VehicleName", // must be unique for each callout entity
                Hash = VehicleHash.Alpha,
                BlipColor = BlipColor.Yellow, // optional, will default to BlipSprite.Yellow
                BlipSprite = BlipSprite.Standard, // optional, will default to BlipSprite.Standard
                HasBlip = true, // optional, will default to true
                SpawnLocation = Vector3.Zero,

                BodyHealth = 534f, // optional, default 1000f
                EngineHealth = 333f, // optional, default 1000f
                PetrolTankHealth = 0f, // optional, default 1000f
                Colour = VehicleColor.Green,
                Driver = "PedName", // optional, the ped key of the driver, default none
                Passengers = new List<string>() { "AnotherPed" } // optional, ped keys of all passengers (max 3), default empty list
            });

            // Example ped
            AddEntity(new CalloutPed()
            {
                Key = "PedName",
                Hash = PedHash.DeadHooker,
                BlipColor = BlipColor.Green, // optional, as above
                BlipSprite = BlipSprite.Rockets, // optional, as above
                HasBlip = true, // optional, as above
                SpawnLocation = Vector3.Zero
            });

            // Example ped
            AddEntity(new CalloutPed()
            {
                Key = "AnotherPed",
                Hash = PedHash.DeadHooker,
                BlipColor = BlipColor.Green, // optional, as above
                BlipSprite = BlipSprite.Rockets, // optional, as above
                HasBlip = true, // optional, as above
                SpawnLocation = Vector3.Zero
            });

            AddEntity(new CalloutPed()
            {
                Key = "Ped",
                Hash = (PedHash)(uint)PedModels.GetRandomHuman(),
                SpawnLocation = Location,
                HasBlip = false,
                FirstName = "John",
                LastName = "Cena",
                BirthDate = DateTime.Now,
                AlcoholLevel = 0.05f,
                OnCannabis = true,
                OnCocaine = true,
                OnHeroin = true,
                OnEcstasy = true,
                HasDrivingLicense = true,
                IsBannedFromDriving = true,
                DrivingLicensePoints = 3,
                WearsSeatbelt = false,
                Items = new string[] { "Cannabis Grinder", "Cannabis Joint" },
                Warrants = new string[] { "Wanker" },
                Charges = new string[] { "Total wanker" },
                Markers = new string[] { "Suck a cock", "dickhead" }
            });
        }

        public override void Update()
        {
            base.Update();
        }

        public override void OnPlayerArrived(Player player)
        {
            base.OnPlayerArrived(player);
        }

        public override void OnPlayerWithinRange(Player player)
        {
            base.OnPlayerWithinRange(player);
        }

        protected override void OnEnded(CalloutResult result)
        {
            base.OnEnded(result);
        }

        protected override void OnStarted()
        {
            base.OnStarted();
        }
    }
}
