using System.Drawing;
using System.Threading.Tasks;
using CitizenFX.Core;
using CitizenFX.Core.Native;
using MenuAPI;
using PoliceMP.Client.Extensions;
using PoliceMP.Client.Services.Interfaces;
using PoliceMP.Core.Client;
using PoliceMP.Core.Client.Scripts;
using PoliceMP.Core.Shared;

namespace PoliceMP.Client.Scripts.Civ
{
    public interface ICivArmoury
    {
        Task ShowCivArmouryMenu();
    }

    public class CivArmoury : Script, ICivArmoury
    {
        #region Services

        private readonly ILogger<CivArmoury> _logger;
        private readonly IPermissionService _permissionService;
        
        #endregion

        #region Variables

        private Menu _menu;

        #endregion
        
        public CivArmoury(ILogger<CivArmoury> logger, IPermissionService permissionService)
        {
            _logger = logger;
            _permissionService = permissionService;
        }

        public async Task ShowCivArmouryMenu()
        {
            _logger.Debug("Showing Civ Armoury Menu");

            _menu?.ClearMenuItems();

            var userAces = await _permissionService.GetUserAces();

            _menu = new Menu("Civ Armoury", "Get Guns Bro");
            
            MenuController.AddMenu(_menu);

            
            #region Meele

            var removeItem = new MenuItem("Remove All Weapons");
            _menu.AddItem(removeItem);
            
            _menu.AddMenuItem(new MenuItem("Parachute") { ItemData = WeaponHash.Parachute });
            _menu.AddMenuItem(new MenuItem("Dagger") { ItemData = WeaponHash.Dagger });
            _menu.AddMenuItem(new MenuItem("Ball") { ItemData = WeaponHash.Ball });
            _menu.AddMenuItem(new MenuItem("Baseball Bat") { ItemData = WeaponHash.Bat });
            _menu.AddMenuItem(new MenuItem("Crowbar"){ ItemData = WeaponHash.Crowbar });
            _menu.AddMenuItem(new MenuItem("Hammer"){ ItemData = WeaponHash.Hammer });
            _menu.AddMenuItem(new MenuItem("Knife") { ItemData = WeaponHash.Knife });
            _menu.AddMenuItem(new MenuItem("Knuckleduster") { ItemData = WeaponHash.KnuckleDuster });
            _menu.AddMenuItem(new MenuItem("Machete") { ItemData = WeaponHash.Machete });
            _menu.AddMenuItem(new MenuItem("Switch Blade") { ItemData = WeaponHash.SwitchBlade});
            _menu.AddMenuItem(new MenuItem("Wrench"){ ItemData = WeaponHash.Wrench });
            _menu.AddMenuItem(new MenuItem("Shiv"){ ItemData = WeaponHash.Bottle });
            _menu.AddMenuItem(new MenuItem("Sledgehammer"){ ItemData = WeaponHash.PoolCue });
            //_menu.AddMenuItem(new MenuItem("Golf Club"){ ItemData = WeaponHash.golfclub }); //being used as big red key

            


            #endregion

            #region Throwables

            _menu.AddMenuItem(new MenuItem("Flare") { ItemData = WeaponHash.Flare });
            //_menu.AddMenuItem(new MenuItem("Snowball"){ ItemData = WeaponHash.snowball }); //For Christmas Time

            #endregion

            #region Weapons

            if (userAces.IsCivGunTrained || userAces.IsAdmin || userAces.IsDeveloper)
            {
                _menu.AddMenuItem(new MenuItem("AK47 Rifle") { ItemData = WeaponHash.CompactRifle });
                _menu.AddMenuItem(new MenuItem("Flare Gun"){ItemData = WeaponHash.FlareGun});
                _menu.AddMenuItem(new MenuItem("Musket") {ItemData = WeaponHash.Musket});
                _menu.AddMenuItem(new MenuItem("Molotov") {ItemData = WeaponHash.Molotov});
                _menu.AddMenuItem(new MenuItem("Petrol Can") { ItemData = WeaponHash.PetrolCan });
                _menu.AddMenuItem(new MenuItem("Pistol") { ItemData = WeaponHash.Pistol });
                _menu.AddMenuItem(new MenuItem("Old Revolver") { ItemData = (WeaponHash)(uint)API.GetHashKey("WEAPON_NAVYREVOLVER")});
                _menu.AddMenuItem(new MenuItem("Tommy Gun") {ItemData = WeaponHash.Gusenberg});
                _menu.AddMenuItem(new MenuItem("Hunting Rifle") {ItemData = WeaponHash.HeavySniperMk2});
                _menu.AddMenuItem(new MenuItem("Shotgun") {ItemData = WeaponHash.PumpShotgunMk2});
                _menu.AddMenuItem(new MenuItem("Uzi") {ItemData = WeaponHash.MicroSMG});
               
            }

            if (userAces.IsAdmin || userAces.IsDeveloper)
            {
                _menu.AddMenuItem(new MenuItem("SMT: Fireworks") { ItemData = 2138347493 });
                _menu.AddMenuItem(new MenuItem("SMT: CS gas grenade") { ItemData = 2694266206 });
                _menu.AddMenuItem(new MenuItem("SMT: StickyBomb") { ItemData = 741814745 });
                _menu.AddMenuItem(new MenuItem("SMT: Smoke Grenade") { ItemData = WeaponHash.SmokeGrenade });
                _menu.AddMenuItem(new MenuItem("SMT: Smoke Grenade Launcher") { ItemData = WeaponHash.GrenadeLauncherSmoke });

            }
            
            if (userAces.IsCivCommand)
            {
                var customWeapon = new MenuItem("Red/Gold Assault Rifle") { ItemData = (WeaponHash)(uint)API.GetHashKey("WEAPON_ASSAULTRIFLE_MK2") };
                _menu.AddMenuItem(customWeapon);
                
                var customWeapon2 = new MenuItem("Green Military Rifle") { ItemData = (WeaponHash)(uint)API.GetHashKey("WEAPON_MILITARYRIFLE") };
                _menu.AddMenuItem(customWeapon2);
            }


            #endregion

            _menu.OpenMenu();

            _menu.OnItemSelect += (menu, item, index) =>
            {
                if (menu != _menu) return;

                if (item == removeItem)
                {
                    Game.PlayerPed.Weapons.RemoveAll();
                    return;
                }
                
                var weaponData = (WeaponHash) item.ItemData;

                if (weaponData == (WeaponHash)(uint)API.GetHashKey("WEAPON_ASSAULTRIFLE_MK2"))
                {
                    Game.PlayerPed.Weapons.Give(weaponData, 100, true, true);
                    API.GiveWeaponComponentToPed(Game.PlayerPed.Handle, (uint)weaponData, (uint)API.GetHashKey(("COMPONENT_AT_AR_FLSH")));
                    API.GiveWeaponComponentToPed(Game.PlayerPed.Handle, (uint)weaponData, (uint)API.GetHashKey(("COMPONENT_AT_SCOPE_MEDIUM_MK2")));
                    API.GiveWeaponComponentToPed(Game.PlayerPed.Handle, (uint)weaponData, (uint)API.GetHashKey(("COMPONENT_ASSAULTRIFLE_MK2_CLIP_02")));
                    API.SetPedWeaponTintIndex(Game.PlayerPed.Handle, (uint)weaponData, 32);

                }
                
                if (weaponData == (WeaponHash)(uint)API.GetHashKey("WEAPON_MILITARYRIFLE"))
                {
                    Game.PlayerPed.Weapons.Give(weaponData, 100, true, true);
                    API.GiveWeaponComponentToPed(Game.PlayerPed.Handle, (uint)weaponData, (uint)API.GetHashKey(("COMPONENT_AT_SCOPE_SMALL")));
                    API.GiveWeaponComponentToPed(Game.PlayerPed.Handle, (uint)weaponData, (uint)API.GetHashKey(("COMPONENT_AT_AR_FLSH")));
                    API.GiveWeaponComponentToPed(Game.PlayerPed.Handle, (uint)weaponData, (uint)API.GetHashKey(("COMPONENT_MILITARYRIFLE_CLIP_02")));
                    API.SetPedWeaponTintIndex(Game.PlayerPed.Handle, (uint)weaponData, 1);

                }

                Game.PlayerPed.Weapons.Give(weaponData, 1000, true, true);

                menu.CloseMenu();
            };
        }
    }
}