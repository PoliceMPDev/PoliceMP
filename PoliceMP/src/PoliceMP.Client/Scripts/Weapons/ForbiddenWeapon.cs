using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CitizenFX.Core;
using CitizenFX.Core.Native;
using PoliceMP.Client.Services.Interfaces;
using PoliceMP.Core.Client;
using PoliceMP.Core.Client.Interface;
using PoliceMP.Core.Client.Scripts;
using PoliceMP.Shared.Models;

namespace PoliceMP.Client.Scripts.Weapons
{
    public class ForbiddenWeapon : Script
    {
        #region Services
        
        private readonly ITickManager _tickManager;
        private readonly IPermissionService _permissionService;

        #endregion

        #region Variables

        private UserAces _userAces;

        #endregion
        
        #region Lists

        private List<PickupType> _pickupTypes = new List<PickupType>();
        private List<WeaponHash> _forbiddenWeapons = new List<WeaponHash>();

        #endregion
        
        public ForbiddenWeapon(ITickManager tickManager, IPermissionService permissionService)
        {
            _tickManager = tickManager;
            _permissionService = permissionService;
        }
        
        protected override async Task OnStartAsync()
        {
            _userAces = await _permissionService.GetUserAces();
            
            _tickManager.On(ForbiddenWeaponTick);

            

            #region Fetch All Pickups

            foreach (PickupType pickupType in Enum.GetValues(typeof(PickupType)))
            {
                _pickupTypes.Add(pickupType);
            }
            
            #endregion

            #region Forbidden Weapons

            //_forbiddenWeapons.Add(WeaponHash.AdvancedRifle);
            //_forbiddenWeapons.Add(WeaponHash.APPistol);
            //_forbiddenWeapons.Add(WeaponHash.AssaultRifleMk2);
            _forbiddenWeapons.Add(WeaponHash.AssaultShotgun);
            //_forbiddenWeapons.Add(WeaponHash.AssaultSMG);
            //_forbiddenWeapons.Add(WeaponHash.BullpupRifle);
            //_forbiddenWeapons.Add(WeaponHash.BullpupRifleMk2);
            _forbiddenWeapons.Add(WeaponHash.BullpupShotgun);
            //_forbiddenWeapons.Add(WeaponHash.BZGas);
            //_forbiddenWeapons.Add(WeaponHash.CarbineRifleMk2);
            _forbiddenWeapons.Add(WeaponHash.CombatMG);
            _forbiddenWeapons.Add(WeaponHash.CombatMGMk2);
            //_forbiddenWeapons.Add(WeaponHash.CombatPDW);
            _forbiddenWeapons.Add(WeaponHash.CompactGrenadeLauncher);
            _forbiddenWeapons.Add(WeaponHash.DoubleBarrelShotgun);
            _forbiddenWeapons.Add(WeaponHash.Firework);
            _forbiddenWeapons.Add(WeaponHash.Grenade);
            _forbiddenWeapons.Add(WeaponHash.GrenadeLauncher);
            _forbiddenWeapons.Add(WeaponHash.GrenadeLauncherSmoke);
            _forbiddenWeapons.Add(WeaponHash.Hatchet);
            //_forbiddenWeapons.Add(WeaponHash.HeavyPistol);
            _forbiddenWeapons.Add(WeaponHash.HeavyShotgun);
            _forbiddenWeapons.Add(WeaponHash.HeavySniper);
            //_forbiddenWeapons.Add(WeaponHash.HeavySniperMk2); // Civ Hunting Rifle
            _forbiddenWeapons.Add(WeaponHash.HomingLauncher);
            _forbiddenWeapons.Add(WeaponHash.MarksmanPistol);
            _forbiddenWeapons.Add(WeaponHash.MarksmanRifleMk2);
            _forbiddenWeapons.Add(WeaponHash.MG);
            //_forbiddenWeapons.Add(WeaponHash.MicroSMG);
            _forbiddenWeapons.Add(WeaponHash.Minigun);
            _forbiddenWeapons.Add(WeaponHash.NightVision);
            _forbiddenWeapons.Add(WeaponHash.PipeBomb);
            //_forbiddenWeapons.Add(WeaponHash.PistolMk2);
            _forbiddenWeapons.Add(WeaponHash.ProximityMine);
            //_forbiddenWeapons.Add(WeaponHash.PumpShotgunMk2); // AFO Baton Gun
            _forbiddenWeapons.Add(WeaponHash.Railgun);
            _forbiddenWeapons.Add(WeaponHash.RayCarbine);
            _forbiddenWeapons.Add(WeaponHash.RayMinigun);
            _forbiddenWeapons.Add(WeaponHash.RayPistol);
            _forbiddenWeapons.Add(WeaponHash.Revolver);
            _forbiddenWeapons.Add(WeaponHash.RevolverMk2);
            _forbiddenWeapons.Add(WeaponHash.RPG);
            //_forbiddenWeapons.Add(WeaponHash.SMGMk2); // AFO Training Rifle
            _forbiddenWeapons.Add(WeaponHash.SmokeGrenade);
            //_forbiddenWeapons.Add(WeaponHash.Snowball);
            //_forbiddenWeapons.Add(WeaponHash.SNSPistolMk2);
            //_forbiddenWeapons.Add(WeaponHash.SpecialCarbineMk2);
            _forbiddenWeapons.Add(WeaponHash.StickyBomb);
            _forbiddenWeapons.Add(WeaponHash.StoneHatchet);
            _forbiddenWeapons.Add(WeaponHash.SweeperShotgun);

            #endregion
        }

        private async Task ForbiddenWeaponTick()
        {
            #region Remove Pickups

            foreach (var pickupType in _pickupTypes)
            {
                var pickup = (uint) pickupType;
                var playerPosition = Game.PlayerPed.Position;

                if (API.IsPickupWithinRadius(pickup, playerPosition.X, playerPosition.Y, playerPosition.Z, 20f))
                {
                    API.RemoveAllPickupsOfType(pickup);
                }

                await Delay(0);
            }

            #endregion


            if (_userAces.IsAdmin || _userAces.IsDeveloper || _userAces.IsDevFunNight) return;
            #region Forbidden Weapons

            for (int i = 0; i < _forbiddenWeapons.Count; i++)
            {
                var weapon = _forbiddenWeapons[i];
                Game.PlayerPed.Weapons.Remove(weapon);
                await Delay(0);
            }

            #endregion

            #region AFO Weapon Check

            if (!_userAces.IsAfoTrained && !_userAces.IsCivGunTrained)
            {
                Game.PlayerPed.Weapons.Remove(WeaponHash.CombatPistol);
                await Delay(0);

                Game.PlayerPed.Weapons.Remove(WeaponHash.PumpShotgun);
                await Delay(0);

                Game.PlayerPed.Weapons.Remove(WeaponHash.SpecialCarbine);
                await Delay(0);
                
                Game.PlayerPed.Weapons.Remove(WeaponHash.CarbineRifle);
                await Delay(0);

                Game.PlayerPed.Weapons.Remove(WeaponHash.SMG);
                await Delay(0);

                Game.PlayerPed.Weapons.Remove(WeaponHash.MicroSMG);
                await Delay(0);

                Game.PlayerPed.Weapons.Remove(WeaponHash.PumpShotgunMk2);
                await Delay(0);

                Game.PlayerPed.Weapons.Remove(WeaponHash.HeavySniperMk2);
                await Delay(0);

                Game.PlayerPed.Weapons.Remove(WeaponHash.SniperRifle);
                await Delay(0);

                Game.PlayerPed.Weapons.Remove(WeaponHash.SNSPistol);
                await Delay(0);

                Game.PlayerPed.Weapons.Remove(WeaponHash.PetrolCan);
                await Delay(0);

                Game.PlayerPed.Weapons.Remove(WeaponHash.Pistol);
                await Delay(0);

                Game.PlayerPed.Weapons.Remove(WeaponHash.SawnOffShotgun);
                await Delay(0);

                Game.PlayerPed.Weapons.Remove(WeaponHash.MarksmanRifle);
                await Delay(0);
            }

            #endregion
        }
    }
}