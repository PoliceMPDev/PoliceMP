using System.Collections.Generic;
using PoliceMP.Main.Core.Server.Enums;

namespace PoliceMP.Server.Controllers.AiCallouts
{
    public class AiCalloutsWeapons
    {
        public static readonly List<uint> ArmedRobberyWeapons = new List<uint>()
        {
            (uint)WeaponHash.AssaultRifleMk2,
            (uint)WeaponHash.Pistol,
            (uint)WeaponHash.Machete,
            (uint)WeaponHash.Pistol50,
            (uint)WeaponHash.SMGMk2,
            (uint)WeaponHash.MiniSMG,
            (uint)WeaponHash.AssaultSMG,
            (uint)WeaponHash.BullpupRifle,
            (uint)WeaponHash.CarbineRifle
        };

        public static readonly List<uint> NonLifeThreateningWeapons = new List<uint>()
        {
            (uint)WeaponHash.Bat,
            (uint)WeaponHash.Crowbar,
            (uint)WeaponHash.Hammer,
            (uint)WeaponHash.Wrench,
            (uint)WeaponHash.GolfClub,
            (uint)WeaponHash.Nightstick,
            (uint)WeaponHash.Unarmed,
            (uint)WeaponHash.KnuckleDuster,
            (uint)WeaponHash.PoolCue,
            (uint)WeaponHash.Bottle,
            (uint)WeaponHash.Dagger
        };

        public static readonly List<uint> MediumLifeThreateningWeapons = new List<uint>()
        {
            (uint)WeaponHash.Bat,
            (uint)WeaponHash.Crowbar,
            (uint)WeaponHash.Hammer,
            (uint)WeaponHash.Knife,
            (uint)WeaponHash.SwitchBlade,
            (uint)WeaponHash.Bottle,
            (uint)WeaponHash.Machete,
            (uint)WeaponHash.Hatchet
        };

        public static readonly List<uint> LifeThreateningWeapons = new List<uint>()
        {
            (uint)WeaponHash.Bat,
            (uint)WeaponHash.Crowbar,
            (uint)WeaponHash.Hammer,
            (uint)WeaponHash.Knife,
            (uint)WeaponHash.SwitchBlade,
            (uint)WeaponHash.Bottle,
            (uint)WeaponHash.KnuckleDuster
        };
    }
}