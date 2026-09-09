using CitizenFX.Core;
using CitizenFX.Core.Native;
using MenuAPI;
using PoliceMP.Client.Services.Interfaces;
using PoliceMP.Core.Client;
using PoliceMP.Core.Client.Communications.Interfaces;
using PoliceMP.Core.Client.Interface;
using PoliceMP.Core.Shared;
using PoliceMP.Shared.Constants;
using PoliceMP.Shared.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using PoliceMP.Client.Scripts.Civ;
using PoliceMP.Core.Client.Scripts;
using PoliceMP.Shared.Enums;

namespace PoliceMP.Client.Scripts.Armoury
{
    public class Armoury : Script, IArmoury
    {
        private readonly ILegacyClientCommunicationsManager _comms;
        private readonly ILogger<Armoury> _logger;
        private readonly INotificationService _notifications;
        private readonly ITickManager _ticks;
        private readonly IPermissionService _permissionService;
        private bool _isInMenu = false;
        private Menu _armouryMenu = new("Armoury");
        private UserAces _userAces;
        private DateTime _lastMenuTime = DateTime.Now;

        private readonly List<Vector3> _armouryLocations = new()
        {
            // Mission Row Armory
            new Vector3(452.39f, -980.05f,30.68f),
            //Zancudo Armory
            new Vector3(-1776.579f, 3065.046f, 32.81f),
            // Vespucci PD
            new Vector3(-1098.78f, -826.07f, 14.28f),
            // Vinewood PD
            new Vector3(621.36f, -18.84f, 82.78f),
           // Sandy PD
             new Vector3(1853.185f, 3697.302f, 34.2693f),
            // Heathrow
            //new(-879.81481933594f,-2382.6013183594f,14.065853118896f),
            // Paleto Bay
            new(-443.7349f, 5987.8071f, 31.7162f),
            // Leman Street Firearms Base
            new(-315.9585f, -2688.625f, 7.567268f)
        };

        public Armoury(ILegacyClientCommunicationsManager comms,
            ILogger<Armoury> logger,
            INotificationService notifications,
            ITickManager ticks,
            IPermissionService permissionService)
        {
            _comms = comms;
            _logger = logger;
            _notifications = notifications;
            _ticks = ticks;
            _permissionService = permissionService;
        }

        protected override async Task OnStartAsync()
        {
            _userAces = await _permissionService.GetUserAces();

            if (_userAces.IsAdmin || _userAces.IsDeveloper || _userAces.IsAfoTrained)
            {
                _logger.Debug("Able to access Armoury");

                MenuController.MenuAlignment = MenuController.MenuAlignmentOption.Right;
                MenuController.AddMenu(_armouryMenu);

                _armouryMenu.InstructionalButtons.Clear();

                MenuController.EnableMenuToggleKeyOnController = false;
                MenuController.MenuToggleKey = (Control)(-1);

                _armouryMenu.OnMenuOpen += menu =>
                {
                    if (menu == _armouryMenu)
                    {
                        _isInMenu = true;
                    }
                };

                _armouryMenu.OnMenuClose += menu =>
                {
                    if (menu == _armouryMenu)
                    {
                        _isInMenu = false;
                    }
                };
            }

            _ticks.On(ArmouryTick);
            _ticks.On(KeepInHandChecker);
            _ticks.On(FireResistant);
        }

        private async Task KeepInHandChecker()
        {
            //Keep the medbag in hand
            if (API.HasPedGotWeapon(Game.PlayerPed.Handle, (uint)API.GetHashKey("WEAPON_ALS"), false))
            {
                API.SetCurrentPedWeapon(Game.PlayerPed.Handle, (uint)API.GetHashKey("WEAPON_ALS"), true);
            }
            //Keep the defib in hand
            else if (API.HasPedGotWeapon(Game.PlayerPed.Handle, (uint)API.GetHashKey("WEAPON_ECG"), false))
            {
                API.SetCurrentPedWeapon(Game.PlayerPed.Handle, (uint)API.GetHashKey("WEAPON_ECG"), true);
            }
        }

        private async Task ArmouryTick()
        {
            await HandleInteractionPoints();
        }

        public void ReloadArmouryMenu()
        {
            var ped = Game.PlayerPed.Handle;

            _armouryMenu.ClearMenuItems();

            var hasCombatPistol = Game.PlayerPed.Weapons.HasWeapon(WeaponHash.CombatPistol);
            string combatPistolLabel = hasCombatPistol ? "Return P226 Pistol" : "Take P226 Pistol";
            var combatPistol = new MenuItem(combatPistolLabel);

            var hasPistol = Game.PlayerPed.Weapons.HasWeapon(WeaponHash.Pistol);
            string pistolLabel = hasPistol ? "Return G17 Pistol" : "Take G17 Pistol";
            var pistol = new MenuItem(pistolLabel);

            var hasSNSPistol = Game.PlayerPed.Weapons.HasWeapon(WeaponHash.SNSPistol);
            string snsPistolLabel = hasSNSPistol ? "Return Chimano Pistol" : "Take Chimano Pistol";
            var snsPistol = new MenuItem(snsPistolLabel);

            if (!hasPistol && !hasSNSPistol)
            {
                _armouryMenu.AddMenuItem(combatPistol);
            }

            if (!hasCombatPistol && !hasSNSPistol)
            {
                _armouryMenu.AddMenuItem(pistol);
            }

            if (!hasPistol && !hasCombatPistol)
            {
                _armouryMenu.AddMenuItem(snsPistol);
            }

            bool hasShotgun = Game.PlayerPed.Weapons.HasWeapon(WeaponHash.SawnOffShotgun);
            string shotgunLabel = hasShotgun ? "Return Baton Gun" : "Take Baton Gun";
            var shotgun = new MenuItem(shotgunLabel);
            _armouryMenu.AddMenuItem(shotgun);

            var hasCarbine = Game.PlayerPed.Weapons.HasWeapon(WeaponHash.CarbineRifle);
            string carbineLabel = hasCarbine ? "Return SIG516 Rifle" : "Take SIG516 Rifle";
            var carbine = new MenuItem(carbineLabel);
            _armouryMenu.AddMenuItem(carbine);
            
            var hasSpecialCarbine = Game.PlayerPed.Weapons.HasWeapon(WeaponHash.SpecialCarbine);
            string specialCarbineLabel = hasSpecialCarbine ? "Return G36C Rifle" : "Take G36C Rifle";
            var specialCarbine = new MenuItem(specialCarbineLabel);
            _armouryMenu.AddMenuItem(specialCarbine);
            
            var hasSMG = Game.PlayerPed.Weapons.HasWeapon(WeaponHash.SMG);
            string smgLabel = hasSMG ? "Return MP5 Rifle" : "Take MP5 Rifle";
            var smg = new MenuItem(smgLabel);
            _armouryMenu.AddMenuItem(smg);

            var hasAssaultSMG = Game.PlayerPed.Weapons.HasWeapon(WeaponHash.AssaultSMG);
            string smgAssaultLabel = hasAssaultSMG ? "Return AUG SMG" : "Take AUG SMG";
            var assaultSmg = new MenuItem(smgAssaultLabel);
            _armouryMenu.AddMenuItem(assaultSmg);
            
            var hasSigMcx = Game.PlayerPed.Weapons.HasWeapon((WeaponHash)API.GetHashKey("WEAPON_SIGMCX"));
            string hasSigMcxLabel = hasSigMcx ? "Return SIG MCX Rifle" : "Take SIG MCX Rifle";
            var sigMcx = new MenuItem(hasSigMcxLabel);
            _armouryMenu.AddMenuItem(sigMcx);

            var hasSniperRifle = Game.PlayerPed.Weapons.HasWeapon(WeaponHash.SniperRifle);
            string sniperRifleLabel = hasSniperRifle ? "Return L115A Rifle" : "Take L115A Rifle";
            var sniperRifle = new MenuItem(sniperRifleLabel);
            _armouryMenu.AddMenuItem(sniperRifle);
            
            var hasSniperRifle2 = Game.PlayerPed.Weapons.HasWeapon((WeaponHash)API.GetHashKey("WEAPON_SIG716"));
            string sniperRifle2Label = hasSniperRifle2 ? "Return SIG716 Long Rifle" : "Take SIG716 Long Rifle";
            var sniperRifle2 = new MenuItem(sniperRifle2Label);
            _armouryMenu.AddMenuItem(sniperRifle2);
            
            var hasFlashBang = Game.PlayerPed.Weapons.HasWeapon((WeaponHash)API.GetHashKey("WEAPON_FLASHBANG"));
            string flashBangLabel = hasFlashBang ? "Return Flash Bangs" : "Take Flash Bangs (x5)";
            var flashBangItem = new MenuItem(flashBangLabel);
            _armouryMenu.AddMenuItem(flashBangItem);

            var hasMp5Training = Game.PlayerPed.Weapons.HasWeapon((WeaponHash)API.GetHashKey("WEAPON_TRAININGMP5SEMI"));
            string mp5TrainingLabel = hasMp5Training ? "Return Training Rifle" : "Take Training Rifle";
            var mp5TrainingItem = new MenuItem(mp5TrainingLabel);
            _armouryMenu.AddMenuItem(mp5TrainingItem);


            _armouryMenu.OnItemSelect += (menu, item, index) =>
            {
                Commands.OtherCommands.enableReachingAnim = false;
                _armouryMenu.CloseMenu();

                if (item == combatPistol)
                {
                    if (hasCombatPistol)
                    {
                        Game.PlayerPed.Weapons.Remove(WeaponHash.CombatPistol);
                    }
                    else
                    {
                        Game.PlayerPed.Weapons.Give(WeaponHash.CombatPistol, 100, true, true);
                        API.GiveWeaponComponentToPed(Game.PlayerPed.Handle, (uint)WeaponHash.CombatPistol, (uint)WeaponComponentHash.AtPiFlsh); // Add flashlight
                    }
                }

                if (item == pistol)
                {
                    if (hasPistol)
                    {
                        Game.PlayerPed.Weapons.Remove(WeaponHash.Pistol);
                    }
                    else
                    {
                        Game.PlayerPed.Weapons.Give(WeaponHash.Pistol, 100, true, true);
                        API.GiveWeaponComponentToPed(Game.PlayerPed.Handle, (uint)WeaponHash.Pistol, (uint)WeaponComponentHash.AtPiFlsh); // Add flashlight
                    }
                }

                if (item == snsPistol)
                {
                    if (hasSNSPistol)
                    {
                        Game.PlayerPed.Weapons.Remove(WeaponHash.SNSPistol);
                    }
                    else
                    {
                        Game.PlayerPed.Weapons.Give(WeaponHash.SNSPistol, 100, true, true);
                    }
                }

                if (item == shotgun)
                {
                    if (hasShotgun)
                    {
                        Game.PlayerPed.Weapons.Remove(WeaponHash.SawnOffShotgun);
                    }
                    else
                    {
                        Game.PlayerPed.Weapons.Give(WeaponHash.SawnOffShotgun, 100, true, true);
                    }
                }

                if (item == carbine)
                {
                    if (hasCarbine)
                    {
                        Game.PlayerPed.Weapons.Remove(WeaponHash.CarbineRifle);
                    }
                    else
                    {
                        Game.PlayerPed.Weapons.Give(WeaponHash.CarbineRifle, 100, true, true);
                        API.GiveWeaponComponentToPed(Game.PlayerPed.Handle, (uint)WeaponHash.CarbineRifle, (uint)WeaponComponentHash.AtArFlsh); // Add flashlight
                        //API.GiveWeaponComponentToPed(Game.PlayerPed.Handle, (uint)WeaponHash.CarbineRifle, (uint)WeaponComponentHash.AtScopeMedium); // Add scope
                    }
                }

                if (item == specialCarbine)
                {
                    if (hasSpecialCarbine)
                    {
                        Game.PlayerPed.Weapons.Remove(WeaponHash.SpecialCarbine);
                    }
                    else
                    {
                        Game.PlayerPed.Weapons.Give(WeaponHash.SpecialCarbine, 100, true, true);
                        API.GiveWeaponComponentToPed(Game.PlayerPed.Handle, (uint)WeaponHash.SpecialCarbine, (uint)WeaponComponentHash.AtArFlsh); // Add flashlight
                        API.GiveWeaponComponentToPed(Game.PlayerPed.Handle, (uint)WeaponHash.SpecialCarbine, (uint)WeaponComponentHash.AtScopeMedium); // Add scope
                    }
                }
                
                if (item == smg)
                {
                    if (hasSMG)
                    {
                        Game.PlayerPed.Weapons.Remove(WeaponHash.SMG);
                    }
                    else
                    {
                        Game.PlayerPed.Weapons.Give(WeaponHash.SMG, 100, true, true);
                        API.GiveWeaponComponentToPed(Game.PlayerPed.Handle, (uint)WeaponHash.SMG, (uint)WeaponComponentHash.AtArFlsh); // Add flashlight
                        API.GiveWeaponComponentToPed(Game.PlayerPed.Handle, (uint)WeaponHash.SMG, (uint)WeaponComponentHash.AtScopeMacro02); // Add scope
                        API.GiveWeaponComponentToPed(Game.PlayerPed.Handle, (uint)WeaponHash.SMG, (uint)WeaponComponentHash.SMGClip02); // Add extended clip
                    }
                }

                if (item == assaultSmg)
                {
                    if (hasAssaultSMG)
                    {
                        Game.PlayerPed.Weapons.Remove(WeaponHash.AssaultSMG);
                    }
                    else
                    {
                        Game.PlayerPed.Weapons.Give(WeaponHash.AssaultSMG, 100, true, true);
                        API.GiveWeaponComponentToPed(Game.PlayerPed.Handle, (uint)WeaponHash.AssaultSMG, (uint)WeaponComponentHash.AtArFlsh); // Add flashlight
                        API.GiveWeaponComponentToPed(Game.PlayerPed.Handle, (uint)WeaponHash.AssaultSMG, (uint)WeaponComponentHash.AtScopeMacro); // Add scope
                    }
                }

                if (item == sniperRifle)
                {
                    if (hasSniperRifle)
                    {
                        Game.PlayerPed.Weapons.Remove(WeaponHash.SniperRifle);
                    }
                    else
                    {
                        Game.PlayerPed.Weapons.Give(WeaponHash.SniperRifle, 100, true, true);
                        API.GiveWeaponComponentToPed(Game.PlayerPed.Handle, (uint)WeaponHash.SniperRifle, (uint)WeaponComponentHash.AtScopeMax); // Add advanced scope
                    }
                }
                
                if (item == sniperRifle2)
                {
                    if (hasSniperRifle2)
                    {
                        Game.PlayerPed.Weapons.Remove((WeaponHash)API.GetHashKey("WEAPON_SIG716"));
                    }
                    else
                    {
                        Game.PlayerPed.Weapons.Give((WeaponHash)API.GetHashKey("WEAPON_SIG716"), 100, true, true);
                    }
                }
                
                if (item == sigMcx)
                {
                    if (hasSigMcx)
                    {
                        Game.PlayerPed.Weapons.Remove((WeaponHash)API.GetHashKey("WEAPON_SIGMCX"));
                    }
                    else
                    {
                        Game.PlayerPed.Weapons.Give((WeaponHash)API.GetHashKey("WEAPON_SIGMCX"), 100, true, true);
                    }
                }
                
                if (item == flashBangItem)
                {
                    if (hasFlashBang)
                    {
                        Game.PlayerPed.Weapons.Remove((WeaponHash)API.GetHashKey("WEAPON_FLASHBANG"));
                    }
                    else
                    {
                        Game.PlayerPed.Weapons.Give((WeaponHash)API.GetHashKey("WEAPON_FLASHBANG"), 5, true, true);
                    }
                }

                if (item == mp5TrainingItem)
                {
                    var mp5TrainingHash = (WeaponHash)API.GetHashKey("WEAPON_TRAININGMP5SEMI");
                    if (hasMp5Training)
                    {
                        Game.PlayerPed.Weapons.Remove(mp5TrainingHash);
                    }
                    else
                    {
                        Game.PlayerPed.Weapons.Give(mp5TrainingHash, 100, true, true);
                        API.GiveWeaponComponentToPed(ped, (uint)mp5TrainingHash, (uint)API.GetHashKey("COMPONENT_MP5_TRAININGAUTO_CLIP_01"));
                        API.GiveWeaponComponentToPed(ped, (uint)mp5TrainingHash, (uint)API.GetHashKey("COMPONENT_AT_AR_FLSH"));
                        API.GiveWeaponComponentToPed(ped, (uint)mp5TrainingHash, (uint)API.GetHashKey("COMPONENT_AT_SIGHT_MP5"));
                    }
                }
            };

            _lastMenuTime = DateTime.Now;
            _armouryMenu.OpenMenu();

        }

        private async Task FireResistant()
        {
            if (!_userAces.IsTsgTrained && !_userAces.IsFireTrained)
            {
                return;
            }

            var player = Game.PlayerPed.Handle;
            var playerHat = API.GetPedPropIndex(Game.PlayerPed.Handle, 0);

            if (playerHat is 36 or 39 or 71)
            {
                await Delay(0);
                API.SetPedConfigFlag(player, 430, true);
                API.SetPedCanLosePropsOnDamage(player, false, 0);
                API.StopEntityFire(player);
                return;
            }

            API.SetPedConfigFlag(player, 430, false);
            API.SetPedCanLosePropsOnDamage(player, true, 0);
        }

        private async Task HandleInteractionPoints()
        {
            // if (!_userAces.IsAdmin || !_userAces.IsDeveloper)
            // {
            //     if (userRole.Division != UserDivision.Afo) return;
            // }

            if (MenuController.IsAnyMenuOpen()) return;

            Vector3 playerPos = Game.PlayerPed.Position;

            foreach (Vector3 armouryLocation in _armouryLocations)
            {
                float distance = API.GetDistanceBetweenCoords(playerPos.X, playerPos.Y, playerPos.Z, armouryLocation.X, armouryLocation.Y, armouryLocation.Z, true);
                float scale = 0.1F * API.GetGameplayCamFov();

                if (distance < 5.0f)
                {
                    API.DrawMarker(1, armouryLocation.X, armouryLocation.Y, armouryLocation.Z - 1, 0, 0, 0, 0, 0, 0, 1F, 1F, 2F, 232, 232, 0, 50,
                        false, true, 2, false, null, null, false);
                    API.SetTextScale(0.1F * scale, 0.1F * scale);
                    API.SetTextFont(4);
                    API.SetTextProportional(true);
                    API.SetTextColour(250, 250, 250, 255);
                    API.SetTextDropshadow(1, 1, 1, 1, 255);
                    API.SetTextEdge(2, 0, 0, 0, 255);
                    API.SetTextDropShadow();
                    API.SetTextOutline();
                    API.SetTextEntry("STRING");
                    API.SetTextCentre(true);
                    API.AddTextComponentString($"Police Armoury");
                    API.SetDrawOrigin(armouryLocation.X, armouryLocation.Y, armouryLocation.Z + 1F, 0);
                    API.DrawText(0, 0);
                    API.ClearDrawOrigin();

                    if (!(distance < 1.0f)) continue;

                    ReloadArmouryMenu();
                }
            }

            return;
        }
    }
}