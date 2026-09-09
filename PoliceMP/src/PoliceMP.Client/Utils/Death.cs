using System.Collections.Generic;
using CitizenFX.Core;

namespace PoliceMP.Client.Utils
{
    public class Death
    {
        public string Cause { get; }
        public int Amount { get; } = 0;

        public Death(string cause, int amount = 0)
        {
            Cause = cause;
            Amount = amount;
        }

        public static Death GetDeathCauseByWeapon(WeaponHash weapon)
        {
            List<WeaponHash> rifles = new List<WeaponHash>{
                WeaponHash.AdvancedRifle,  WeaponHash.AssaultRifle, WeaponHash.AssaultRifleMk2, WeaponHash.BullpupRifle,
                WeaponHash.BullpupRifleMk2, WeaponHash.CarbineRifle, WeaponHash.CarbineRifleMk2, WeaponHash.CompactRifle,
                WeaponHash.MarksmanRifle, WeaponHash.MarksmanRifleMk2, WeaponHash.SniperRifle, WeaponHash.HeavySniper,
                WeaponHash.HeavySniperMk2, WeaponHash.SpecialCarbine, WeaponHash.SpecialCarbineMk2, WeaponHash.Minigun,
                WeaponHash.Musket
            };
            if (rifles.Contains(weapon))
            {
                return new Death("a rifle", 25);
            }
            List<WeaponHash> meeles = new List<WeaponHash>
            {
                WeaponHash.FireExtinguisher, WeaponHash.Snowball, WeaponHash.Ball, WeaponHash.PetrolCan,
                WeaponHash.GrenadeLauncherSmoke, WeaponHash.Hammer, WeaponHash.Nightstick, WeaponHash.Crowbar,
                WeaponHash.Flashlight, WeaponHash.Bat, WeaponHash.Unarmed, WeaponHash.Bottle,
                WeaponHash.Parachute, WeaponHash.PoolCue, WeaponHash.Wrench, WeaponHash.GolfClub,
                WeaponHash.KnuckleDuster, WeaponHash.BattleAxe, WeaponHash.Dagger
            };
            if (meeles.Contains(weapon))
            {
                return new Death("a blunt object", 5);
            }
            List<WeaponHash> pistolArray = new List<WeaponHash>
            {
                WeaponHash.VintagePistol, WeaponHash.Pistol, WeaponHash.APPistol, WeaponHash.CombatPistol,
                WeaponHash.Pistol50, WeaponHash.SNSPistol, WeaponHash.Revolver, WeaponHash.HeavyPistol,
                WeaponHash.MachinePistol, WeaponHash.MarksmanPistol, WeaponHash.PistolMk2, WeaponHash.SNSPistolMk2
            };
            if (pistolArray.Contains(weapon))
            {
                return new Death("a pistol", 10);
            }
            List<WeaponHash> shotgunArray = new List<WeaponHash>
            {
                WeaponHash.PumpShotgun, WeaponHash.HeavyShotgun, WeaponHash.DoubleBarrelShotgun, WeaponHash.SawnOffShotgun,
                WeaponHash.BullpupShotgun, WeaponHash.AssaultShotgun, WeaponHash.SweeperShotgun, WeaponHash.PumpShotgunMk2
            };
            if (shotgunArray.Contains(weapon))
            {
                return new Death("a shotgun", 20);
            }
            List<WeaponHash> riddledArray = new List<WeaponHash>
            {
                WeaponHash.Railgun, WeaponHash.CombatMG, WeaponHash.CombatMGMk2, WeaponHash.MG,
                WeaponHash.RayCarbine, WeaponHash.RayMinigun, WeaponHash.RayPistol
            };
            if (riddledArray.Contains(weapon))
            {
                return new Death("a military-grade weapon", 30);
            }
            List<WeaponHash> fireArray = new List<WeaponHash>
            {
                WeaponHash.Molotov, WeaponHash.Flare, WeaponHash.FlareGun, WeaponHash.Firework
            };
            if (fireArray.Contains(weapon))
            {
                return new Death("fire", 50);
            }
            List<WeaponHash> smgArray = new List<WeaponHash>
            {
                WeaponHash.MiniSMG, WeaponHash.SMG, WeaponHash.CombatPDW, WeaponHash.MicroSMG,
                WeaponHash.AssaultSMG, WeaponHash.Gusenberg, WeaponHash.SMGMk2
            };
            if (smgArray.Contains(weapon))
            {
                return new Death("small arms fire", 15);
            }
            List<WeaponHash> bladeArray = new List<WeaponHash>
            {
                WeaponHash.Knife, WeaponHash.Machete, WeaponHash.SwitchBlade, WeaponHash.Hatchet,
                WeaponHash.StoneHatchet
            };
            if (bladeArray.Contains(weapon))
            {
                return new Death("a bladed object", 10);
            }
            List<WeaponHash> explosionArray = new List<WeaponHash>
            {
                WeaponHash.ProximityMine, WeaponHash.RPG, WeaponHash.SmokeGrenade, WeaponHash.PipeBomb,
                WeaponHash.CompactGrenadeLauncher, WeaponHash.Grenade, WeaponHash.StickyBomb, WeaponHash.HomingLauncher,
                WeaponHash.GrenadeLauncher
            };
            if (explosionArray.Contains(weapon))
            {
                return new Death("an explosion", 50);
            }
            if (weapon == WeaponHash.StunGun)
            {
                return new Death("electrocution", 25);
            }
            if (weapon == WeaponHash.BZGas)
            {
                return new Death("suffocation", 20);
            }
            if ((uint)weapon == (uint)2741846334)
            {
                return new Death("a vehicle", 15);
            }
            return new Death("an unknown reason", 30);
        }
    }
}