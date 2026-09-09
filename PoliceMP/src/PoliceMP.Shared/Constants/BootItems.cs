using System.Collections;
using System.Collections.Generic;
using PoliceMP.Shared.Enums;
using PoliceMP.Shared.Models;

namespace PoliceMP.Shared.Constants
{
    // public enum BootItem
    // {
    //     [BootItem(WeaponHash = Weapon.Enforcer)]
    //     Enforcer,
    //     [BootItem(WeaponHash = Weapon.BatonGun)]
    //     BatonGun,
    //     [BootItem(WeaponHash = Weapon.SIGSauerP226)]
    //     SIGSauerP226,
    //     [BootItem(WeaponHash = Weapon.GlockG17)]
    //     GlockG17,
    //     [BootItem(WeaponHash = Weapon.G36C)]
    //     G36C,
    //     [BootItem(WeaponHash = Weapon.L115A3)]
    //     L115A3,
    //     [BootItem(WeaponHash = Weapon.ScarDMR)]
    //     ScarDMR,
    //     [BootItem(WeaponHash = Weapon.MP5)]
    //     MP5
    //     
    // }

    public class BootItems : List<BootItem>
    {
        protected BootItems()
        {
            AddRange(new [] {
                new BootItem {
                    DisplayName = "Enforcer",
                    Weapon = Weapon.Enforcer
                },
                new BootItem {
                    DisplayName = "Baton Gun",
                    Weapon = Weapon.BatonGun
                }
            });
        }
    }
}


