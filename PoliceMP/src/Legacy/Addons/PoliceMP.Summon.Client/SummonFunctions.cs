using CitizenFX.Core;
using CitizenFX.Core.Native;
using System.Linq;
using System.Threading.Tasks;
using PoliceMP.Main.Core.Client;

namespace PoliceMP.Summon.Client
{
    class SummonFunctions
    {
        public static int drivingStyle = 525119;

        public async static Task<VehicleDestination> PedGetClosestSideOfRoad(Ped player)
        {
            await ClientFunctions.LoadModelAsync("prop_devin_box_dummy_01");
            await ClientFunctions.LoadModelAsync("a_c_pigeon");
            Vector3 vehicleNodePos = Vector3.Zero;
            float vehicleNodeHeading = 0F;
            API.GetClosestVehicleNodeWithHeading(player.Position.X, player.Position.Y, player.Position.Z, ref vehicleNodePos, ref vehicleNodeHeading, 1, 3.0F, 0);
            var v2 = API.CreateObject(API.GetHashKey("prop_devin_box_dummy_01"), vehicleNodePos.X, vehicleNodePos.Y, vehicleNodePos.Z, true, true, true);
            API.PlaceObjectOnGroundProperly(v2);
            Ped vectorPed = await World.CreatePed("a_c_pigeon", vehicleNodePos, vehicleNodeHeading);
            Vector3 sidePos = vehicleNodePos;
            // Create a random vehicle in the sea for this command
            Vehicle randomVehicle = await World.CreateVehicle("panto", new Vector3(2000, 4000, 200), vehicleNodeHeading);
            while (API.IsPointOnRoad(sidePos.X, sidePos.Y, sidePos.Z, randomVehicle.Handle))
            {
                sidePos = sidePos + vectorPed.RightVector + 0.25F;
                API.SetEntityCoords(v2, sidePos.X, sidePos.Y, sidePos.Z, true, true, true, false);
                API.PlaceObjectOnGroundProperly(v2);
                sidePos = API.GetEntityCoords(v2, false);
                vectorPed.Position = sidePos;
                Debug.WriteLine("Finding side of road...");
            }
            if (vehicleNodePos == sidePos)
            {
                Debug.WriteLine("Didn't seem to find a spot off-road? Forcing over a lane");
                sidePos = sidePos + vectorPed.RightVector + 4F;
                API.SetEntityCoords(v2, sidePos.X, sidePos.Y, sidePos.Z, true, true, true, false);
                API.PlaceObjectOnGroundProperly(v2);
                sidePos = API.GetEntityCoords(v2, false);
            }
            float groundLevel = 0F;
            API.GetGroundZFor_3dCoord(sidePos.X, sidePos.Y, sidePos.Z, ref groundLevel, true);
            if (groundLevel != 0F)
            {
                sidePos.Z = groundLevel;
            }
            randomVehicle.Delete();
            vectorPed.Delete();
            API.DeleteEntity(ref v2);
            return new VehicleDestination
            {
                Position = sidePos,
                Heading = vehicleNodeHeading
            };
        }

        public static DeathChance GetDeathCauseFromHash(WeaponHash hash)
        {
            WeaponHash[] rifleArray = new WeaponHash[] {
                WeaponHash.AdvancedRifle,  WeaponHash.AssaultRifle, WeaponHash.AssaultRifleMk2, WeaponHash.BullpupRifle,
                WeaponHash.BullpupRifleMk2, WeaponHash.CarbineRifle, WeaponHash.CarbineRifleMk2, WeaponHash.CompactRifle,
                WeaponHash.MarksmanRifle, WeaponHash.MarksmanRifleMk2, WeaponHash.SniperRifle, WeaponHash.HeavySniper,
                WeaponHash.HeavySniperMk2, WeaponHash.SpecialCarbine, WeaponHash.SpecialCarbineMk2, WeaponHash.Minigun,
                WeaponHash.Musket
            };
            if (rifleArray.Contains(hash))
            {
                return new DeathChance("a rifle", 25);
            }
            WeaponHash[] meleeArray = new WeaponHash[]
            {
                WeaponHash.FireExtinguisher, WeaponHash.Snowball, WeaponHash.Ball, WeaponHash.PetrolCan,
                WeaponHash.GrenadeLauncherSmoke, WeaponHash.Hammer, WeaponHash.Nightstick, WeaponHash.Crowbar,
                WeaponHash.Flashlight, WeaponHash.Bat, WeaponHash.Unarmed, WeaponHash.Bottle,
                WeaponHash.Parachute, WeaponHash.PoolCue, WeaponHash.Wrench, WeaponHash.GolfClub,
                WeaponHash.KnuckleDuster, WeaponHash.BattleAxe, WeaponHash.Dagger
            };
            if (meleeArray.Contains(hash))
            {
                return new DeathChance("a blunt object", 5);
            }
            WeaponHash[] pistolArray = new WeaponHash[]
            {
                WeaponHash.VintagePistol, WeaponHash.Pistol, WeaponHash.APPistol, WeaponHash.CombatPistol,
                WeaponHash.Pistol50, WeaponHash.SNSPistol, WeaponHash.Revolver, WeaponHash.HeavyPistol,
                WeaponHash.MachinePistol, WeaponHash.MarksmanPistol, WeaponHash.PistolMk2, WeaponHash.SNSPistolMk2
            };
            if (pistolArray.Contains(hash))
            {
                return new DeathChance("a pistol", 10);
            }
            WeaponHash[] shotgunArray = new WeaponHash[]
            {
                WeaponHash.PumpShotgun, WeaponHash.HeavyShotgun, WeaponHash.DoubleBarrelShotgun, WeaponHash.SawnOffShotgun,
                WeaponHash.BullpupShotgun, WeaponHash.AssaultShotgun, WeaponHash.SweeperShotgun, WeaponHash.PumpShotgunMk2
            };
            if (shotgunArray.Contains(hash))
            {
                return new DeathChance("a shotgun", 20);
            }
            WeaponHash[] riddledArray = new WeaponHash[]
            {
                WeaponHash.Railgun, WeaponHash.CombatMG, WeaponHash.CombatMGMk2, WeaponHash.MG,
                WeaponHash.RayCarbine, WeaponHash.RayMinigun, WeaponHash.RayPistol
            };
            if (riddledArray.Contains(hash))
            {
                return new DeathChance("a military-grade weapon", 30);
            }
            WeaponHash[] fireArray = new WeaponHash[]
            {
                WeaponHash.Molotov, WeaponHash.Flare, WeaponHash.FlareGun, WeaponHash.Firework
            };
            if (fireArray.Contains(hash))
            {
                return new DeathChance("fire", 50);
            }
            WeaponHash[] smgArray = new WeaponHash[]
            {
                WeaponHash.MiniSMG, WeaponHash.SMG, WeaponHash.CombatPDW, WeaponHash.MicroSMG,
                WeaponHash.AssaultSMG, WeaponHash.Gusenberg, WeaponHash.SMGMk2
            };
            if (smgArray.Contains(hash))
            {
                return new DeathChance("small arms fire", 15);
            }
            WeaponHash[] bladeArray = new WeaponHash[]
            {
                WeaponHash.Knife, WeaponHash.Machete, WeaponHash.SwitchBlade, WeaponHash.Hatchet,
                WeaponHash.StoneHatchet
            };
            if (bladeArray.Contains(hash))
            {
                return new DeathChance("a bladed object", 10);
            }
            WeaponHash[] explosionArray = new WeaponHash[]
            {
                WeaponHash.ProximityMine, WeaponHash.RPG, WeaponHash.SmokeGrenade, WeaponHash.PipeBomb,
                WeaponHash.CompactGrenadeLauncher, WeaponHash.Grenade, WeaponHash.StickyBomb, WeaponHash.HomingLauncher,
                WeaponHash.GrenadeLauncher
            };
            if (explosionArray.Contains(hash))
            {
                return new DeathChance("an explosion", 50);
            }
            if (hash == WeaponHash.StunGun)
            {
                return new DeathChance("electrocution", 25);
            }
            if (hash == WeaponHash.BZGas)
            {
                return new DeathChance("suffocation", 20);
            }
            if ((uint)hash == (uint)2741846334)
            {
                return new DeathChance("a vehicle", 15);
            }
            return new DeathChance("an unknown reason", 30);
        }

        public class VehicleDestination
        {
            public Vector3 Position { get; set; }
            public float Heading { get; set; }
        }
        public class DeathChance
        {
            public string DeathCause { get; set; }
            public int DeathChanceAmount { get; set; }

            public DeathChance(string newCause, int newAmount)
            {
                DeathCause = newCause;
                DeathChanceAmount = newAmount;
            }
        }
    }
}
