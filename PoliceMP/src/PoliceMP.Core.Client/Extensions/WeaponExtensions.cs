using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CitizenFX.Core;

namespace PoliceMP.Core.Client.Extensions
{
    public static class WeaponExtensions
    {
        public static bool IsLethal(this Weapon weapon)
        {
            return weapon.AmmoType == AmmoType.AssaultRifle
                   || weapon.AmmoType == AmmoType.Pistol
                   || weapon.AmmoType == AmmoType.StunGun
                   || weapon.AmmoType == AmmoType.MG
                   || weapon.AmmoType == AmmoType.SMG
                   || weapon.AmmoType == AmmoType.Minigun
                   || weapon.AmmoType == AmmoType.Shotgun;
        }
    }
}
