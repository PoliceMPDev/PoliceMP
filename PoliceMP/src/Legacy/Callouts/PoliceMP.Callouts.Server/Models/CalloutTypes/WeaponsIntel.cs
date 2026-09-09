using CitizenFX.Core;
using PoliceMP.Main.Core.Server;
using PoliceMP.Main.Core.Server.Enums;
using PoliceMP.Main.Core.Server.Extensions;
using PoliceMP.Main.Core.Shared;
using PoliceMP.Main.Core.Shared.Constants;

namespace PoliceMP.Callouts.Server.Models.CalloutTypes
{
    class WeaponsIntel : Callout
    {
        public WeaponsIntel(int id) : base(id)
        {
            Title = "Weapons Intel";
            Grade = 1;
            Description = "A vehicle has weapons intel. This will require assistance of an ARV.";
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
                HasInsurance = PoliceMpRandom.Next(100) % 2 == 0,
                HasTax = PoliceMpRandom.Next(100) % 2 == 0,
                HasMot = PoliceMpRandom.Next(100) % 2 == 0,
                Markers = new string[] { "Weapons Intel" }
            });

            var perp = new CalloutPed()
            {
                Key = $"Driver",
                Hash = (PedHash)(uint)PedModels.GetRandomHuman(),
                BlipColor = BlipColor.Red,
                HasBlip = true,
                SpawnLocation = Location
            };

            var r = PoliceMpRandom.Next(10);
            if (r <= 1)
            {
                //Nothing
            }
            else if (r >= 2 && r <= 5)
            {
                //Mele
                var rw = PoliceMpRandom.Next(5);
                if (rw == 1)
                {
                    perp.WeaponHash = WeaponHash.Ball;
                }
                else if (rw == 2)
                {
                    perp.WeaponHash = WeaponHash.Crowbar;
                }
                else if (rw == 3)
                {
                    perp.WeaponHash = WeaponHash.BattleAxe;
                }
                else if (rw == 4)
                {
                    perp.WeaponHash = WeaponHash.Dagger;
                }
                else
                {
                    perp.WeaponHash = WeaponHash.Knife;
                }
            }
            else if (r >= 6 && r <= 9)
            {
                //Pistol
                perp.WeaponHash = WeaponHash.Pistol;
            }
            else
            {
                //Big gat
                perp.WeaponHash = WeaponHash.AssaultRifle;
            }
            AddEntity(perp);
        }

        protected override void OnFirstPlayerArrived(Player player)
        {
            base.OnFirstPlayerArrived(player);
            var ped = GetEntity("Driver");
            var vehicle = GetEntity("Vehicle");

            var rw = PoliceMpRandom.Next(5);
            if (rw <= 2)
            {
                //Bolt
                ClientEventAPI.SetPedDriveToFast(player, ped.NetworkId, vehicle.NetworkId, Locations.GetRandomCallout().ToCitizenVector3());
            }
            else if (rw >= 3)
            {
                //Fight
                ClientEventAPI.SetPedAttackPlayer(player, ped.NetworkId);
            }
        }
    }
}
