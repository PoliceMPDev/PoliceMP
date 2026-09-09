using CitizenFX.Core;
using System.Linq;
using PoliceMP.Main.Core.Server;
using PoliceMP.Main.Core.Server.Enums;
using PoliceMP.Main.Core.Server.Extensions;
using PoliceMP.Main.Core.Shared;
using PoliceMP.Main.Core.Shared.Constants;

namespace PoliceMP.Callouts.Server.Models.CalloutTypes
{
    class DrinkDrive : Callout
    {
        public DrinkDrive(int id) : base(id)
        {
            Title = "Drink Driver";
            Grade = 1;
            Description = "Driving on alchohol.";
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
                Markers = new string[] { "Drink Driver" }
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

            var vehicle = GetEntity("Vehicle");

            foreach (var entity in _entities.Values.ToList())
            {
                if (entity is CalloutPed ped)
                {
                    ClientEventAPI.SetPedRandomIdleAnim(GetHost().Player, ped.NetworkId);
                    ped.AlcoholLevel = 50;

                    var r = PoliceMpRandom.Next(5);
                    if (r == 1) { ped.OnCannabis = true; }
                    else if (r == 2) { ped.OnCocaine = true; }
                    else if (r == 3) { ped.OnEcstasy = true; }
                    else if (r == 4) { ped.OnHeroin = true; }
                    else if (r == 5) { ped.OnCocaine = true; ped.OnEcstasy = true; ped.OnHeroin = true; ped.OnHeroin = true; }


                    var rw = PoliceMpRandom.Next(5);
                    if (rw <= 2)
                    {
                        //Bolt
                        ClientEventAPI.SetPedDriveToFast(player, ped.NetworkId, vehicle.NetworkId, Locations.GetRandomCallout().ToCitizenVector3());
                    }
                    else if (rw >= 3)
                    {
                        //Drive normal
                        ClientEventAPI.SetPedTaskDriveWander(player, ped.NetworkId, vehicle.NetworkId);
                    }
                }
            }
        }
    }
}
