using CitizenFX.Core;
using CitizenFX.Core.Native;
using MenuAPI;
using PoliceMP.Client.Scripts.Dsu;
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
using PoliceMP.Core.Client.Scripts;
using PoliceMP.Shared.Enums;
using PoliceMP.Client.Scripts.ClothingLocker;
using PoliceMP.Client.Extensions;
using System.Drawing;
using CitizenFX.Core.UI;
using PoliceMP.Client.Scripts.MedicalEquipment;
using PoliceMP.Client.Overlays.NewNotification;

namespace PoliceMP.Client.Scripts.Boot
{
    public class BootSystem : Script, IBootSystem
    {
        private readonly ILegacyClientCommunicationsManager _comms;
        private readonly ILogger<BootSystem> _logger;
        private readonly INotificationService _notifications;
        private readonly INewNotificationOverlay _newNotificationOverlay;
        private readonly ITickManager _ticks;
        private readonly IPermissionService _permissionService;
        private readonly IClothingLocker _locker;
        private UserAces _userAces;
        private Menu _bootMenu = new Menu("Vehicle Boot");
        private Menu _ppeLas = new("LHS PPE Menu");
        private Menu _ppeLasCommand = new("LHS Command Menu");
        private Menu _ppeLfb = new("LFRS PPE Menu");
        private Menu _ppeLfbkit = new("LFRS Equipment Menu");
        private Menu _ppeCID = new("CID Boot");
        private Menu _ppeHEMS = new("HEMS Boot");
        private Menu _ppeIC = new("Police IC Vests");
        private bool _inputDisabled = false;
        private bool _afoStatus = false;
        private bool _tsgStatus = false;
        private bool _dogStatus = false;
        private bool _shieldEquipped = false;
        private bool _tsgShieldEquipped = false;
        private int _helmetId = -1;
        private int _helmetTexture = 0;
        private int _afoHelmet = 227;
        private string tsgOptionName = "Switch to PSU";
        private int prevMask;

        #region Weapon Vehicle Network Ids

        private int _batonVehicle = -1;
        private int _g36CVehicle = -1;
        private int _sig516Vehicle = -1;
        private int _l115A3Vehicle = -1;
        private int _sniper2Vehicle = -1;
        private int _sigMcxVehicle = -1;
        private int _mp5Vehicle = -1;
        private int _flashBangVehicle = -1;
        private int _combatPistolVehicle = -1;
        private int _pistolVehicle = -1;
        private int _augVehicle = -1;
        private int _marskmanVehicle = -1;

        #endregion Weapon Vehicle Network Ids

        public static event EventHandler<string> DeployDog;
        public static event EventHandler<string> DeployDog2;

        public static event EventHandler ReturnDog;

        /// <summary>
        /// Fetches if user is DSU
        /// </summary>
        /// <returns>True = Is DSU / Dog Section</returns>
        public bool FetchDsuStatus()
        {
            return _dogStatus;
        }

        public BootSystem(ILegacyClientCommunicationsManager comms, ILogger<BootSystem> logger,
            INotificationService notifications, ITickManager ticks, IPermissionService permissionService,
            IClothingLocker locker, INewNotificationOverlay newNotificationOverlay)
        {
            _comms = comms;
            _logger = logger;
            _notifications = notifications;
            _ticks = ticks;
            _permissionService = permissionService;
            _newNotificationOverlay = newNotificationOverlay;
            _locker = locker;

            MenuController.EnableMenuToggleKeyOnController = false;
            MenuController.MenuToggleKey = (Control)(-1);
            MenuController.MenuAlignment = MenuController.MenuAlignmentOption.Right;

            MenuController.AddMenu(_bootMenu);
            MenuController.AddMenu(_ppeLas);
            MenuController.AddMenu(_ppeLasCommand);
            MenuController.AddMenu(_ppeLfb);
            MenuController.AddMenu(_ppeLfbkit);
            MenuController.AddMenu(_ppeCID);
            MenuController.AddMenu(_ppeIC);
            MenuController.AddMenu(_ppeHEMS);
        }

        protected override async Task OnStartAsync()
        {
            updateAces();
            _userAces = await _permissionService.GetUserAces();
            _ticks.On(BootSystemTick);
        }


        private async Task updateAces()
        {
            _userAces = await _permissionService.GetUserAces();

            _afoStatus = _userAces.IsAfoTrained || _userAces.IsAdmin || _userAces.IsDeveloper;
            _tsgStatus = _userAces.IsTsgTrained || _userAces.IsAdmin || _userAces.IsDeveloper;
            _dogStatus = _userAces.IsDsuTrained || _userAces.IsAdmin || _userAces.IsDeveloper;
        }

        private Task BootSystemTick()
        {
            if (MenuController.DisableBackButton
                && !Game.IsDisabledControlPressed(32, Control.Aim))
            {
                MenuController.DisableBackButton = false;
            }

            if (_inputDisabled)
            {
                API.DisableAllControlActions(0);
                API.DisableAllControlActions(1);
            }

            return Task.FromResult(0);
        }

        private bool _bootMenuLoading = false;

        private async Task HandleBootMenu(int vehicleId)
        {
            var currentUserRole = _permissionService.CurrentUserRole;

            if (_bootMenuLoading) return;

            _bootMenuLoading = true;

            updateAces();

            Vehicle vehicle = (Vehicle)Entity.FromHandle(vehicleId);
            Ped playerPed = Game.PlayerPed;

            _bootMenu.ClearMenuItems();

            MenuItem medicalEquipment = new MenuItem("Medical Equipment");
            _bootMenu.AddMenuItem(medicalEquipment);

            MenuItem reloadTaser = new MenuItem("Reload Taser");
            _bootMenu.AddMenuItem(reloadTaser);


            #region PoliceBoot

            if (_permissionService.CurrentUserRole.Branch == UserBranch.Police)
            {
                var player = Game.PlayerPed.Handle;
                var tsgHelmet = 226;
                var tsgCap = 243;
                if (API.GetPedPropIndex(player, 0) == tsgHelmet || API.GetPedPropIndex(player, 0) == tsgCap)
                {
                    if (_permissionService.CurrentUserRole.Division == UserDivision.Tsg)
                    {
                        MenuItem tsgBeltChange = new MenuItem(tsgOptionName);
                        _bootMenu.AddMenuItem(tsgBeltChange);
                    }
                }

                #region AFO Shield
                
                if (_permissionService.CurrentUserRole.Division == UserDivision.Afo)
                {
                    MenuItem equipShield = new MenuItem("Equip/Return AFO Shield");
                    _bootMenu.AddMenuItem(equipShield);

                    _bootMenu.OnItemSelect += async (sender, item, index) =>
                    {
                        if (item == equipShield)
                        {
                            var playerPed = Game.PlayerPed;

                            if (!_shieldEquipped)
                            {
                                var playerPos = playerPed.Position;

                                // Find nearest vehicle
                                Vehicle nearestVehicle = null;
                                float nearestDistance = float.MaxValue;

                                foreach (var vehicle in World.GetAllVehicles())
                                {
                                    float distance = Vector3.Distance(playerPos, vehicle.Position);
                                    if (distance < nearestDistance)
                                    {
                                        nearestVehicle = vehicle;
                                        nearestDistance = distance;
                                    }
                                }

                                if (nearestVehicle != null)
                                {
                                    // Calculate position 2.0f away
                                    Vector3 direction = Vector3.Normalize(playerPos - nearestVehicle.Position);
                                    Vector3 newPos = nearestVehicle.Position + direction * 4.0f;

                                    // Teleport player
                                    Function.Call(Hash.SET_ENTITY_COORDS, playerPed.Handle, newPos.X, newPos.Y,
                                        newPos.Z, false, false, false, true);
                                    await BaseScript.Delay(100);
                                }

                                API.ExecuteCommand("shield firearms");
                                _shieldEquipped = true;
                            }
                            else
                            {
                                API.ExecuteCommand("shield firearms");
                                _shieldEquipped = false;
                            }
                        }
                    };
                }
                #endregion AFO Shield

                #region TSG Shield

                if (_permissionService.CurrentUserRole.Division == UserDivision.Tsg)
                {
                    MenuItem equipTsgShield = new MenuItem("Equip/Return TSG Shield");
                    _bootMenu.AddMenuItem(equipTsgShield);

                    _bootMenu.OnItemSelect += async (sender, item, index) =>
                    {
                        if (item == equipTsgShield)
                        {
                            var playerPed = Game.PlayerPed;

                            if (!_tsgShieldEquipped)
                            {
                                var playerPos = playerPed.Position;

                                // Find nearest vehicle
                                Vehicle nearestVehicle = null;
                                float nearestDistance = float.MaxValue;

                                foreach (var vehicle in World.GetAllVehicles())
                                {
                                    float distance = Vector3.Distance(playerPos, vehicle.Position);
                                    if (distance < nearestDistance)
                                    {
                                        nearestVehicle = vehicle;
                                        nearestDistance = distance;
                                    }
                                }

                                if (nearestVehicle != null)
                                {
                                    // Calculate position 4.0f away
                                    Vector3 direction = Vector3.Normalize(playerPos - nearestVehicle.Position);
                                    Vector3 newPos = nearestVehicle.Position + direction * 5.0f;

                                    // Teleport player
                                    Function.Call(Hash.SET_ENTITY_COORDS, playerPed.Handle, newPos.X, newPos.Y,
                                        newPos.Z, false, false, false, true);
                                    await BaseScript.Delay(100);
                                }

                                API.ExecuteCommand("shield long2");
                                _tsgShieldEquipped = true;
                            }
                            else
                            {
                                API.ExecuteCommand("shield long2");
                                _tsgShieldEquipped = false;
                            }
                        }
                    };
                }

                #endregion TSG Shield
                
                if (_userAces.IsProDonator && (_permissionService.CurrentUserRole.Division == UserDivision.Afo ||
                                               _permissionService.CurrentUserRole.Division == UserDivision.Dsu ||
                                               _permissionService.CurrentUserRole.Division == UserDivision.Cid))
                {
                    MenuItem equipSnood = new MenuItem("Equip/Return Snood");
                    _bootMenu.AddMenuItem(equipSnood);
                }

                MenuItem emgHammer = new MenuItem("Take EMG Hammer");
                _bootMenu.AddMenuItem(emgHammer);

                MenuItem equipGloves = new MenuItem("Equip/Return Gloves");
                _bootMenu.AddMenuItem(equipGloves);

                #region Speed Gun

                bool hasSpeedGun = playerPed.Weapons.HasWeapon(WeaponHash.VintagePistol);
                string speedGunLabel = hasSpeedGun ? "Return Speed Gun" : "Take Speed Gun";
                MenuItem speedGun = new MenuItem(speedGunLabel);
                _bootMenu.AddMenuItem(speedGun);

                #endregion Speed Gun

                #region Enforcer

                if (playerPed.Weapons.HasWeapon(WeaponHash.GolfClub) && (_afoStatus || _tsgStatus))
                {
                    // Has Enforcer
                    _bootMenu.AddMenuItem(new MenuItem("Rack Enforcer"));
                }
                else if (!playerPed.Weapons.HasWeapon(WeaponHash.GolfClub) && (_afoStatus || _tsgStatus))
                {
                    _bootMenu.AddMenuItem(new MenuItem("Un Rack Enforcer"));
                }

                #endregion Enforcer

                #region Explosive Detection Device

                WeaponHash digiscannerHash = (WeaponHash)Game.GenerateHash("weapon_digiscanner");

                if (playerPed.Weapons.HasWeapon(digiscannerHash) &&
                    _permissionService.CurrentUserRole.Division == UserDivision.Afo)
                {
                    // Has Device
                    _bootMenu.AddMenuItem(new MenuItem("Store Explosive Detection Device"));
                }
                else if (!playerPed.Weapons.HasWeapon(digiscannerHash) &&
                         _permissionService.CurrentUserRole.Division == UserDivision.Afo)
                {
                    _bootMenu.AddMenuItem(new MenuItem("Take Explosive Detection Device"));
                }
                //Debug.WriteLine($"Hash for weapon_digiscanner: {digiscannerHash}");

                #endregion Explosive Detection Device

                #region Baton Gun

                if (playerPed.Weapons.HasWeapon(WeaponHash.SawnOffShotgun))
                {
                    // Has Baton Gun
                    MenuItem rackBatonGun = new MenuItem("Rack Baton Gun");
                    _bootMenu.AddMenuItem(rackBatonGun);
                }
                else if (!playerPed.Weapons.HasWeapon(WeaponHash.SawnOffShotgun) && _afoStatus)
                {
                    if (_batonVehicle == vehicle.NetworkId)
                    {
                        MenuItem unRackBatonGun = new MenuItem("Un Rack Baton Gun");
                        _bootMenu.AddMenuItem(unRackBatonGun);
                    }
                }

                #endregion Baton Gun

                #region Combat Pistol

                if (playerPed.Weapons.HasWeapon(WeaponHash.CombatPistol))
                {
                    MenuItem rackCombatPistol = new MenuItem("Rack P226");
                    _bootMenu.AddMenuItem(rackCombatPistol);
                }
                else if (!playerPed.Weapons.HasWeapon(WeaponHash.CombatPistol) && _afoStatus)
                {
                    if (_combatPistolVehicle == vehicle.NetworkId)
                    {
                        MenuItem unRackCombatPistol = new MenuItem("Un Rack P226");
                        _bootMenu.AddMenuItem(unRackCombatPistol);
                    }
                }

                #endregion Combat Pistol

                #region Pistol

                if (playerPed.Weapons.HasWeapon(WeaponHash.Pistol))
                {
                    MenuItem rackPistol = new MenuItem("Rack Glock G17");
                    _bootMenu.AddMenuItem(rackPistol);
                }
                else if (!playerPed.Weapons.HasWeapon(WeaponHash.Pistol) && _afoStatus)
                {
                    if (_pistolVehicle == vehicle.NetworkId)
                    {
                        MenuItem unRackPistol = new MenuItem("Un Rack Glock G17");
                        _bootMenu.AddMenuItem(unRackPistol);
                    }
                }

                #endregion Pistol

                #region POBaton

                var poBaton = (WeaponHash)API.GetHashKey("WEAPON_POBATON");
                if (playerPed.Weapons.HasWeapon(poBaton))
                {
                    MenuItem rackBaton = new MenuItem("Rack POBaton");
                    _bootMenu.AddMenuItem(rackBaton);
                }
                else if (!playerPed.Weapons.HasWeapon(poBaton) && _tsgStatus)
                {
                    MenuItem unRackBaton = new MenuItem("Un Rack POBaton");
                    _bootMenu.AddMenuItem(unRackBaton);
                }

                #endregion

                #region G36C

                if (playerPed.Weapons.HasWeapon(WeaponHash.SpecialCarbine))
                {
                    // Has G36C
                    MenuItem rackG36C = new MenuItem("Rack G36C");
                    _bootMenu.AddMenuItem(rackG36C);
                }
                else if (!playerPed.Weapons.HasWeapon(WeaponHash.SpecialCarbine) && _afoStatus)
                {
                    if (_g36CVehicle == vehicle.NetworkId)
                    {
                        MenuItem unRackG36C = new MenuItem("Un Rack G36C");
                        _bootMenu.AddMenuItem(unRackG36C);
                    }
                }

                #endregion G36C

                #region SIG516

                if (playerPed.Weapons.HasWeapon(WeaponHash.CarbineRifle))
                {
                    // Has SIG516
                    MenuItem rackSIG516 = new MenuItem("Rack SIG516");
                    _bootMenu.AddMenuItem(rackSIG516);
                }

                else if (!playerPed.Weapons.HasWeapon(WeaponHash.CarbineRifle) && _afoStatus)
                {
                    if (_sig516Vehicle == vehicle.NetworkId)
                    {
                        MenuItem UnrackSIG516 = new MenuItem("Un Rack SIG516");
                        _bootMenu.AddMenuItem(UnrackSIG516);
                    }
                }

                #endregion SIG516

                #region L115A3

                if (playerPed.Weapons.HasWeapon(WeaponHash.SniperRifle))
                {
                    _bootMenu.AddMenuItem(new MenuItem("Rack L115A"));
                }
                else if (!playerPed.Weapons.HasWeapon(WeaponHash.SniperRifle) && _afoStatus)
                {
                    if (_l115A3Vehicle == vehicle.NetworkId)
                    {
                        _bootMenu.AddMenuItem(new MenuItem("Un Rack L115A"));
                    }
                }

                #endregion L115A3
                
                #region SIG716
                
                WeaponHash sniper2Hash = (WeaponHash)API.GetHashKey("WEAPON_SIG716");

                if (playerPed.Weapons.HasWeapon(sniper2Hash))
                {
                    _bootMenu.AddMenuItem(new MenuItem("Store SIG716 Long Rifle"));
                }
                else if (!playerPed.Weapons.HasWeapon(sniper2Hash) && _afoStatus)
                {
                    if (_sniper2Vehicle == vehicle.NetworkId)
                    {
                        _bootMenu.AddMenuItem(new MenuItem("Get SIG716 Long Rifle"));
                    }
                }

                #endregion SIG716
                
                #region SIGMCX
                
                WeaponHash sigMcxHash = (WeaponHash)API.GetHashKey("WEAPON_SIGMCX");

                if (playerPed.Weapons.HasWeapon(sigMcxHash))
                {
                    _bootMenu.AddMenuItem(new MenuItem("Store SIG MCX Rifle"));
                }
                else if (!playerPed.Weapons.HasWeapon(sigMcxHash) && _afoStatus)
                {
                    if (_sigMcxVehicle == vehicle.NetworkId)
                    {
                        _bootMenu.AddMenuItem(new MenuItem("Get SIG MCX Rifle"));
                    }
                }

                #endregion SIGMCX

                #region MP5

                if (playerPed.Weapons.HasWeapon(WeaponHash.SMG))
                {
                    // Has MP5
                    MenuItem rackMP5 = new MenuItem("Rack H&K MP5");
                    _bootMenu.AddMenuItem(rackMP5);
                }

                else if (!playerPed.Weapons.HasWeapon(WeaponHash.SMG) && _afoStatus)
                {
                    if (_mp5Vehicle == vehicle.NetworkId)
                    {
                        MenuItem UnrackMP5 = new MenuItem("Un Rack H&K MP5");
                        _bootMenu.AddMenuItem(UnrackMP5);
                    }
                }

                #endregion MP5

                #region AUG SMG

                if (playerPed.Weapons.HasWeapon(WeaponHash.AssaultSMG))
                {
                    // Has MP5
                    MenuItem rackAUG = new MenuItem("Rack AUG SMG");
                    _bootMenu.AddMenuItem(rackAUG);
                }

                else if (!playerPed.Weapons.HasWeapon(WeaponHash.AssaultSMG) && _afoStatus)
                {
                    if (_augVehicle == vehicle.NetworkId)
                    {
                        MenuItem UnrackAUG = new MenuItem("Un Rack AUG SMG");
                        _bootMenu.AddMenuItem(UnrackAUG);
                    }
                }

                #endregion

                #region Flashbangs

                WeaponHash flashBangHash = (WeaponHash)API.GetHashKey("WEAPON_FLASHBANG");

                if (playerPed.Weapons.HasWeapon(flashBangHash))
                {
                    _bootMenu.AddMenuItem(new MenuItem("Store Flash Bangs"));
                }
                else if (!playerPed.Weapons.HasWeapon(flashBangHash) && _afoStatus)
                {
                    if (_flashBangVehicle == vehicle.NetworkId)
                    {
                        _bootMenu.AddMenuItem(new MenuItem("Get Flash Bangs"));
                    }
                }

                #endregion Flashbangs

                #region DSU

                // TODO: Add Dog v2
                //if (_dog.HasTakenFromKennel())
                //{
                //    _bootMenu.AddMenuItem(!DogScript.DogSpawned ? new MenuItem("Deploy Tactical Dog") : new MenuItem("Cage Tactical Dog"));
                //}

                //if (_dog.HasTakenFromKennel())
                //{
                //    _bootMenu.AddMenuItem(!DogScript.DogSpawned ? new MenuItem("Deploy Search Dog") : new MenuItem("Cage Search Dog"));
                //}

                #endregion DSU

                #region Helmet

                var playerId = playerPed.Handle;

                bool hasHelmet = API.GetPedPropIndex(playerId, 0) == _afoHelmet;

                string helmetLabel = hasHelmet ? "Store Helmet" : "Take Helmet";

                MenuItem helmetItem = new MenuItem(helmetLabel);

                if (_afoStatus)
                {
                    _bootMenu.AddMenuItem(helmetItem);
                }

                #endregion Helmet

                _bootMenu.OnItemSelect += async (menu, item, index) =>
                {
                    if (item.Text == tsgOptionName)
                    {
                        var tsgCap = 243;
                        var tsgHelmet = 226;
                        var tsgPOMHelmet = 226;
                        var tsgCapBelt = 275;
                        var tsgHelmetBelt = 276;

                        var beltTexture = API.GetPedTextureVariation(player, 8);
                        var hatTexture = API.GetPedPropTextureIndex(player, 0);
                        var hatWorn = API.GetPedPropIndex(player, 0);

                        switch (API.GetPedPropIndex(player, 0))
                        {
                            case 226: // tsgHelmet
                                API.SetPedComponentVariation(player, 8, tsgHelmetBelt, beltTexture, 0);
                                API.SetPedPropIndex(player, 0, tsgCap, hatTexture, false);
                                tsgOptionName = ("Switch to Public Order");
                                break;
                            case 243: // tsgCap
                                if (hatWorn == tsgPOMHelmet)
                                {
                                    API.SetPedComponentVariation(player, 8, tsgCapBelt, beltTexture, 0);
                                    API.SetPedPropIndex(player, 0, tsgHelmet, 4, false);
                                    tsgOptionName = ("Switch to PSU");
                                    return;
                                }

                                API.SetPedComponentVariation(player, 8, tsgCapBelt, beltTexture, 0);
                                API.SetPedPropIndex(player, 0, tsgHelmet, hatTexture, false);
                                tsgOptionName = ("Switch to PSU");
                                break;
                        }
                    }

                    if (item == equipGloves)
                    {
                        API.ExecuteCommand("gloves");
                    }

                    if (item.Text == "Equip/Return Snood")
                    {
                        item.Text = "";
                        var player = Game.PlayerPed.Handle;
                        if (currentUserRole.Branch != UserBranch.Police || !_userAces.IsProDonator) return;

                        var snoodMask = 234;
                        var wearingMask = false;
                        if (API.GetPedDrawableVariation(player, 1) == snoodMask) wearingMask = true;

                        if (!wearingMask)
                        {
                            prevMask = API.GetPedDrawableVariation(player, 1);
                            API.SetPedComponentVariation(player, 1, snoodMask, 0, 0);
                            _newNotificationOverlay.SendNotification(new NewNotificationMessage("Snood", "success",
                                "You have put on your Snood!", new NewNotificationMessageContent[0]));
                        }
                        else if (wearingMask)
                        {
                            API.SetPedComponentVariation(player, 1, prevMask, 0, 0);
                            _newNotificationOverlay.SendNotification(new NewNotificationMessage("Snood", "success",
                                "You have removed your Snood!", new NewNotificationMessageContent[0]));
                        }
                    }


                    if (item == emgHammer)
                    {
                        var emgHash = (uint)API.GetHashKey("WEAPON_EmgHammer");
                        Game.PlayerPed.Weapons.Give((WeaponHash)emgHash, 100, true, true);
                        playerPed.Weapons.Select((WeaponHash)emgHash);
                    }

                    if (item == reloadTaser)
                    {
                        await BaseScript.Delay(100);
                        API.ExecuteCommand("resettaser");
                    }

                    if (item == helmetItem)
                    {
                        if (hasHelmet)
                        {
                            API.SetPedPropIndex(playerId, 0, _helmetId, _helmetTexture, true);
                        }
                        else
                        {
                            _helmetId = API.GetPedPropIndex(playerId, 0);
                            _helmetTexture = API.GetPedPropTextureIndex(playerId, 0);
                            API.SetPedPropIndex(playerId, 0, _afoHelmet, 0, true);
                        }
                    }

                    if (item == speedGun)
                    {
                        if (hasSpeedGun)
                        {
                            playerPed.Weapons.Select((WeaponHash)API.GetHashKey("WEAPON_UNARMED"));
                            playerPed.Weapons.Remove(WeaponHash.VintagePistol);
                        }
                        else
                        {
                            playerPed.Weapons.Give(WeaponHash.VintagePistol, 0, true, false);
                        }
                    }

                    #region Enforcer

                    if (item.Text == "Rack Enforcer")
                    {
                        playerPed.Weapons.Remove(WeaponHash.GolfClub);
                        playerPed.Weapons.Select((WeaponHash)API.GetHashKey("WEAPON_UNARMED"));
                    }

                    if (item.Text == "Un Rack Enforcer")
                    {
                        if (!_afoStatus && !_tsgStatus) return;
                        Game.PlayerPed.Weapons.Give(WeaponHash.GolfClub, 1, true, true);
                    }

                    #endregion Enforcer

                    #region Explosive Detection Device

                    if (item.Text == "Store Explosive Detection Device")
                    {
                        WeaponHash digiscannerHash = (WeaponHash)Game.GenerateHash("weapon_digiscanner");

                        // Remove the Explosive Detection Device
                        playerPed.Weapons.Remove(digiscannerHash);

                        // Select unarmed after removing the device
                        playerPed.Weapons.Select((WeaponHash)Game.GenerateHash("weapon_unarmed"));
                    }

                    if (item.Text == "Take Explosive Detection Device")
                    {
                        if (!_afoStatus) return;

                        WeaponHash digiscannerHash = (WeaponHash)Game.GenerateHash("weapon_digiscanner");

                        // Give the Explosive Detection Device
                        Game.PlayerPed.Weapons.Give(digiscannerHash, 1, true, true);
                    }

                    #endregion Explosive Detection Device

                    #region Baton Gun

                    if (item.Text == "Rack Baton Gun")
                    {
                        playerPed.Weapons.Remove(WeaponHash.SawnOffShotgun);

                        _batonVehicle = vehicle.NetworkId;
                        playerPed.Weapons.Select((WeaponHash)API.GetHashKey("WEAPON_UNARMED"));
                    }

                    if (item.Text == "Un Rack Baton Gun")
                    {
                        if (!_afoStatus) return;

                        if (_batonVehicle != vehicle.NetworkId)
                        {
                            _notifications.Error("Error", "Unable to get a Baton Gun!");
                            return;
                        }

                        _batonVehicle = -1;

                        Game.PlayerPed.Weapons.Give(WeaponHash.SawnOffShotgun, 100, true, true);
                    }

                    #endregion Baton Gun

                    #region G36C

                    if (item.Text == "Rack G36C")
                    {
                        playerPed.Weapons.Remove(WeaponHash.SpecialCarbine);

                        _g36CVehicle = vehicle.NetworkId;

                        playerPed.Weapons.Select((WeaponHash)API.GetHashKey("WEAPON_UNARMED"));
                    }

                    if (item.Text == "Un Rack G36C")
                    {
                        if (!_afoStatus) return;

                        if (_g36CVehicle != vehicle.NetworkId) return;

                        _g36CVehicle = -1;

                        Game.PlayerPed.Weapons.Give(WeaponHash.SpecialCarbine, 100, true, true);
                        API.GiveWeaponComponentToPed(playerPed.Handle, (uint)WeaponHash.SpecialCarbine,
                            (uint)API.GetHashKey("COMPONENT_AT_AR_FLSH"));
                        API.GiveWeaponComponentToPed(playerPed.Handle, (uint)WeaponHash.SpecialCarbine,
                            (uint)API.GetHashKey("COMPONENT_AT_SCOPE_MEDIUM"));
                    }

                    #endregion G36C

                    #region SIG516

                    if (item.Text == "Rack SIG516")
                    {
                        playerPed.Weapons.Remove(WeaponHash.CarbineRifle);

                        _sig516Vehicle = vehicle.NetworkId;

                        playerPed.Weapons.Select((WeaponHash)API.GetHashKey("WEAPON_UNARMED"));
                    }

                    if (item.Text == "Un Rack SIG516")
                    {
                        if (!_afoStatus) return;

                        if (_sig516Vehicle != vehicle.NetworkId) return;

                        _sig516Vehicle = -1;

                        Game.PlayerPed.Weapons.Give(WeaponHash.CarbineRifle, 100, true, true);
                        API.GiveWeaponComponentToPed(Game.PlayerPed.Handle, (uint)WeaponHash.CarbineRifle,
                            (uint)WeaponComponentHash.AtArFlsh);
                        API.GiveWeaponComponentToPed(Game.PlayerPed.Handle, (uint)WeaponHash.CarbineRifle,
                            (uint)WeaponComponentHash.AtScopeMedium);
                    }

                    #endregion SIG516

                    #region L115A3

                    if (item.Text == "Rack L115A")
                    {
                        playerPed.Weapons.Remove(WeaponHash.SniperRifle);

                        playerPed.Weapons.Select((WeaponHash)API.GetHashKey("WEAPON_UNARMED"));

                        _l115A3Vehicle = vehicle.NetworkId;
                    }

                    if (item.Text == "Un Rack L115A")
                    {
                        if (!_afoStatus) return;

                        if (_l115A3Vehicle != vehicle.NetworkId) return;

                        _l115A3Vehicle = -1;
                        Game.PlayerPed.Weapons.Give(WeaponHash.SniperRifle, 100, true, true);
                        API.GiveWeaponComponentToPed(Game.PlayerPed.Handle, (uint)WeaponHash.SniperRifle,
                            (uint)WeaponComponentHash.AtScopeMax);
                        API.GiveWeaponComponentToPed(Game.PlayerPed.Handle, (uint)WeaponHash.SniperRifle,
                            (uint)WeaponComponentHash.AtArSupp02);
                    }

                    #endregion L115A3

                    #region MP5

                    if (item.Text == "Rack H&K MP5")
                    {
                        playerPed.Weapons.Select((WeaponHash)API.GetHashKey("WEAPON_UNARMED"));
                        playerPed.Weapons.Remove(WeaponHash.SMG);

                        _mp5Vehicle = vehicle.NetworkId;
                    }

                    if (item.Text == "Un Rack H&K MP5")
                    {
                        Game.PlayerPed.Weapons.Give(WeaponHash.SMG, 100, true, true);
                        API.GiveWeaponComponentToPed(Game.PlayerPed.Handle, (uint)WeaponHash.SMG,
                            (uint)WeaponComponentHash.AtArFlsh);
                        API.GiveWeaponComponentToPed(Game.PlayerPed.Handle, (uint)WeaponHash.SMG,
                            (uint)WeaponComponentHash.AtScopeMacro02);
                        API.GiveWeaponComponentToPed(Game.PlayerPed.Handle, (uint)WeaponHash.SMG,
                            (uint)WeaponComponentHash.SMGClip02);
                    }

                    #endregion MP5

                    #region AUG SMG

                    if (item.Text == "Rack AUG SMG")
                    {
                        playerPed.Weapons.Select((WeaponHash)API.GetHashKey("WEAPON_UNARMED"));
                        playerPed.Weapons.Remove(WeaponHash.SMG);

                        _mp5Vehicle = vehicle.NetworkId;
                    }

                    if (item.Text == "Un Rack AUG SMG")
                    {
                        Game.PlayerPed.Weapons.Give(WeaponHash.AssaultSMG, 100, true, true);
                        API.GiveWeaponComponentToPed(Game.PlayerPed.Handle, (uint)WeaponHash.AssaultSMG,
                            (uint)WeaponComponentHash.AtArFlsh);
                        API.GiveWeaponComponentToPed(Game.PlayerPed.Handle, (uint)WeaponHash.AssaultSMG,
                            (uint)WeaponComponentHash.AtScopeMacro);
                    }

                    #endregion AUG SMG
                    
                    #region SIG716

                    if (item.Text == "Store SIG716 Long Rifle")
                    {
                        playerPed.Weapons.Remove(sniper2Hash);

                        playerPed.Weapons.Select((WeaponHash)API.GetHashKey("WEAPON_UNARMED"));

                        _sniper2Vehicle = vehicle.NetworkId;
                    }

                    if (item.Text == "Get SIG716 Long Rifle")
                    {
                        Game.PlayerPed.Weapons.Give(sniper2Hash, 5, true, true);
                    }

                    #endregion SIG716
                    
                    #region SIGMCX

                    if (item.Text == "Store SIG MCX Rifle")
                    {
                        playerPed.Weapons.Remove(sigMcxHash);

                        playerPed.Weapons.Select((WeaponHash)API.GetHashKey("WEAPON_UNARMED"));

                        _sigMcxVehicle = vehicle.NetworkId;
                    }

                    if (item.Text == "Get SIG MCX Rifle")
                    {
                        Game.PlayerPed.Weapons.Give(sigMcxHash, 5, true, true);
                    }

                    #endregion SIGMCX

                    #region FlashBangs

                    if (item.Text == "Store Flash Bangs")
                    {
                        playerPed.Weapons.Remove(flashBangHash);

                        playerPed.Weapons.Select((WeaponHash)API.GetHashKey("WEAPON_UNARMED"));

                        _flashBangVehicle = vehicle.NetworkId;
                    }

                    if (item.Text == "Get Flash Bangs")
                    {
                        Game.PlayerPed.Weapons.Give(flashBangHash, 5, true, true);
                    }

                    #endregion FlashBangs

                    #region DSU

                    string dogName = DsuNaming.FetchDogName();

                    //if (item.Text == "Deploy Tactical Dog")
                    //{ 
                    //   _dog.Deploy();
                    //}

                    //if (item.Text == "Deploy Search Dog")
                    //{
                    //    _dog.Deploy2();
                    //}

                    //if (item.Text == "Cage Dog")
                    //{
                    //    _dog.ReturnDog();
                    //}

                    //if (item.Text == "Cage Search Dog")
                    //{
                    //    _dog.ReturnDog();
                    //}

                    //if (item.Text == "Cage Tactical Dog")
                    //{
                    //    _dog.ReturnDog();
                    //}

                    #endregion DSU

                    #region Combat Pistol

                    if (item.Text == "Rack P226")
                    {
                        playerPed.Weapons.Remove(WeaponHash.CombatPistol);
                        playerPed.Weapons.Select((WeaponHash)API.GetHashKey("WEAPON_UNARMED"));
                        _combatPistolVehicle = vehicle.NetworkId;
                    }

                    if (item.Text == "Un Rack P226")
                    {
                        _combatPistolVehicle = -1;
                        Game.PlayerPed.Weapons.Give(WeaponHash.CombatPistol, 100, true, true);
                    }

                    #endregion Combat Pistol

                    #region Pistol

                    if (item.Text == "Rack Glock G17")
                    {
                        playerPed.Weapons.Remove(WeaponHash.Pistol);
                        playerPed.Weapons.Select((WeaponHash)API.GetHashKey("WEAPON_UNARMED"));
                        _pistolVehicle = vehicle.NetworkId;
                    }

                    if (item.Text == "Un Rack Glock G17")
                    {
                        _pistolVehicle = -1;
                        Game.PlayerPed.Weapons.Give(WeaponHash.Pistol, 100, true, true);
                        API.GiveWeaponComponentToPed(Game.PlayerPed.Handle, (uint)WeaponHash.Pistol,
                            (uint)WeaponComponentHash.AtPiFlsh);
                    }

                    #endregion Pistol

                    #region POBaton

                    if (item.Text == "Rack POBaton")
                    {
                        var poBaton = (WeaponHash)API.GetHashKey("WEAPON_POBATON");
                        playerPed.Weapons.Remove(poBaton);
                        playerPed.Weapons.Select((WeaponHash)API.GetHashKey("WEAPON_UNARMED"));
                    }

                    if (item.Text == "Un Rack POBaton")
                    {
                        Game.PlayerPed.Weapons.Give(poBaton, 100, true, true);
                    }

                    #endregion
                };
            }

            #endregion PoliceBoot

            #region MedicalBoot

            if (_permissionService.CurrentUserRole.Branch == UserBranch.Nhs)
            {
                MenuItem equipGloves = new MenuItem("Equip/Return Gloves");
                _bootMenu.AddMenuItem(equipGloves);

                #region Defib

                if (API.HasPedGotWeapon(Game.PlayerPed.Handle, (uint)API.GetHashKey("WEAPON_ECG"), false))
                {
                    MenuItem rackDefib = new MenuItem("Rack AI Defib Kit");
                    _bootMenu.AddMenuItem(rackDefib);
                }
                else if (!API.HasPedGotWeapon(Game.PlayerPed.Handle, (uint)API.GetHashKey("WEAPON_ECG"), false))
                {
                    MenuItem unrackDefib = new MenuItem("Un Rack AI Defib Kit");
                    _bootMenu.AddMenuItem(unrackDefib);
                }

                #endregion Defib

                #region LasEupBoot

                if (_userAces.IsHartTrained)
                {
                    _ppeLas.ClearMenuItems();

                    MenuItem LvlC = new MenuItem("PPE - Level C");
                    _ppeLas.AddMenuItem(LvlC);

                    MenuItem LvlBA = new MenuItem("Extended BA Kit");
                    _ppeLas.AddMenuItem(LvlBA);

                    MenuItem SORT1 = new MenuItem("SORT - Water Rescue");
                    _ppeLas.AddMenuItem(SORT1);

                    MenuItem SORT2 = new MenuItem("SORT - Level 3 PPE");
                    _ppeLas.AddMenuItem(SORT2);

                    MenuItem SORT3 = new MenuItem("SORT - Ballistic Support");
                    _ppeLas.AddMenuItem(SORT3);

                    MenuItem TSG = new MenuItem("TSG Medical");
                    _ppeLas.AddMenuItem(TSG);

                    MenuItem HART = new MenuItem("Normal Turnouts");
                    _ppeLas.AddMenuItem(HART);

                    MenuItem PPE = new MenuItem("LAS PPE Menu");
                    _bootMenu.AddMenuItem(PPE);

                    _bootMenu.OnItemSelect += (menu, item, index) =>
                    {
                        if (item == PPE)
                        {
                            _ppeLas.OpenMenu();
                            return;
                        }
                    };

                    _ppeLas.OnItemSelect += (menu, item, index) =>
                    {
                        API.RequestAnimDict("clothingtie_try_tie_negitive_a");
                        API.TaskPlayAnim(Game.PlayerPed.Handle, "clothingtie", "try_tie_negative_a", 8f, 8f, 1100, 51,
                            1f, false, false, false);

                        var playerPed = Game.PlayerPed.Model;
                        bool isMale = (playerPed == (uint)PedHash.FreemodeMale01);

                        if (item == LvlC)
                        {
                            if (isMale)
                            {
                                _locker.LoudOutfitName("HART - Paramedic Helmet");
                            }
                            else
                            {
                                _newNotificationOverlay.SendNotification(new NewNotificationMessage("Error!", "error",
                                    "This has not yet been implimented for females!",
                                    new NewNotificationMessageContent[0]));
                            }

                            _ppeLas.CloseMenu();
                            return;
                        }

                        if (item == LvlBA)
                        {
                            if (isMale)
                            {
                                _locker.LoudOutfitName("HART - Paramedic BA");
                            }
                            else
                            {
                                _newNotificationOverlay.SendNotification(new NewNotificationMessage("Error!", "error",
                                    "This has not yet been implimented for females!",
                                    new NewNotificationMessageContent[0]));
                            }

                            _ppeLas.CloseMenu();
                            return;
                        }

                        if (item == SORT1)
                        {
                            if (isMale)
                            {
                                _locker.LoudOutfitName("HART - Inland Water Rescue");
                            }
                            else
                            {
                                _newNotificationOverlay.SendNotification(new NewNotificationMessage("Error!", "error",
                                    "This has not yet been implimented for females!",
                                    new NewNotificationMessageContent[0]));
                            }

                            _ppeLas.CloseMenu();
                            return;
                        }

                        if (item == SORT2)
                        {
                            if (isMale)
                            {
                                _locker.LoudOutfitName("SORT - Level 3 PPE");
                            }
                            else
                            {
                                _newNotificationOverlay.SendNotification(new NewNotificationMessage("Error!", "error",
                                    "This has not yet been implimented for females!",
                                    new NewNotificationMessageContent[0]));
                            }

                            _ppeLas.CloseMenu();
                            return;
                        }

                        if (item == SORT3)
                        {
                            if (isMale)
                            {
                                _locker.LoudOutfitName("SORT - Ballistic Support Team");
                            }
                            else
                            {
                                _newNotificationOverlay.SendNotification(new NewNotificationMessage("Error!", "error",
                                    "This has not yet been implimented for females!",
                                    new NewNotificationMessageContent[0]));
                            }

                            _ppeLas.CloseMenu();
                            return;
                        }

                        if (item == HART)
                        {
                            if (isMale)
                            {
                                _locker.LoudOutfitName("HART - Paramedic Turnout");
                            }
                            else
                            {
                                _newNotificationOverlay.SendNotification(new NewNotificationMessage("Error!", "error",
                                    "This has not yet been implimented for females!",
                                    new NewNotificationMessageContent[0]));
                            }

                            _ppeLas.CloseMenu();
                            return;
                        }

                        if (item == TSG)
                        {
                            if (isMale)
                            {
                                _locker.LoudOutfitName("HART - TSG Support");
                            }
                            else
                            {
                                _newNotificationOverlay.SendNotification(new NewNotificationMessage("Error!", "error",
                                    "This has not yet been implimented for females!",
                                    new NewNotificationMessageContent[0]));
                            }

                            _ppeLas.CloseMenu();
                            return;
                        }
                    };
                }

                if (_userAces.IsNhsSectionLeader || _userAces.IsNhsClinicalTl)
                {
                    _ppeLasCommand.ClearMenuItems();

                    MenuItem CommandTabard = new MenuItem("Command Tabard");
                    _ppeLasCommand.AddMenuItem(CommandTabard);

                    MenuItem UniformLas = new MenuItem("Uniform");
                    _ppeLasCommand.AddMenuItem(UniformLas);

                    MenuItem PPECommand = new MenuItem("LAS Command Menu");
                    _bootMenu.AddMenuItem(PPECommand);

                    _bootMenu.OnItemSelect += (menu, item, index) =>
                    {
                        if (item == PPECommand)
                        {
                            _ppeLasCommand.OpenMenu();
                            return;
                        }
                    };

                    _ppeLasCommand.OnItemSelect += (menu, item, index) =>
                    {
                        API.RequestAnimDict("clothingtie_try_tie_negitive_a");
                        API.TaskPlayAnim(Game.PlayerPed.Handle, "clothingtie", "try_tie_negative_a", 8f, 8f, 1100, 51,
                            1f, false, false, false);

                        var playerPed = Game.PlayerPed.Model;
                        bool isMale = (playerPed == (uint)PedHash.FreemodeMale01);

                        if (item == CommandTabard)
                        {
                            if (isMale)
                            {
                                _locker.LoudOutfitName("Clinical - Incident Command");
                            }
                            else
                            {
                                _newNotificationOverlay.SendNotification(new NewNotificationMessage("Error!", "error",
                                    "This has not yet been implimented for females!",
                                    new NewNotificationMessageContent[0]));
                            }


                            _ppeLasCommand.CloseMenu();
                            return;
                        }

                        if (item == UniformLas)
                        {
                            if (isMale)
                            {
                                _locker.LoudOutfitName("Clinical - Team Leader 2");
                            }
                            else
                            {
                                _newNotificationOverlay.SendNotification(new NewNotificationMessage("Error!", "error",
                                    "This has not yet been implimented for females!",
                                    new NewNotificationMessageContent[0]));
                            }


                            _ppeLasCommand.CloseMenu();
                            return;
                        }
                    };
                }

                if (_permissionService.CurrentUserRole.Division == UserDivision.Hems ||
                    _permissionService.CurrentUserRole.Division == UserDivision.HemsDoctor ||
                    _permissionService.CurrentUserRole.Division == UserDivision.NhsMananagement)
                {
                    MenuItem HEMSPPE = new MenuItem("HEMS Flight Suits");
                    _bootMenu.AddMenuItem(HEMSPPE);

                    _bootMenu.OnItemSelect += (menu, item, index) =>
                    {
                        if (item == HEMSPPE)
                        {
                            _ppeHEMS.OpenMenu();
                            return;
                        }
                    };

                    _ppeHEMS.ClearMenuItems();

                    if (_userAces.IsNhsHems || _userAces.IsAdmin || _userAces.IsDeveloper || _userAces.IsTierTwo)
                    {
                        MenuItem ParaFlightSuit = new MenuItem("HEMS Paramedic Flight Suit");
                        _ppeHEMS.AddMenuItem(ParaFlightSuit);

                        MenuItem ParaSuit = new MenuItem("HEMS Paramedic Ground Team");
                        _ppeHEMS.AddMenuItem(ParaSuit);

                        _ppeHEMS.OnItemSelect += (menu, item, index) =>
                        {
                            var playerPed = Game.PlayerPed.Model;
                            bool isMale = (playerPed == (uint)PedHash.FreemodeMale01);

                            API.RequestAnimDict("clothingtie_try_tie_negitive_a");
                            API.TaskPlayAnim(Game.PlayerPed.Handle, "clothingtie", "try_tie_negative_a", 8f, 8f, 1100,
                                51, 1f, false, false, false);

                            if (item == ParaFlightSuit)
                            {
                                if (isMale)
                                {
                                    _locker.LoudOutfitName("HEMS - Paramedic (Helmet)");
                                }
                                else
                                {
                                    _newNotificationOverlay.SendNotification(new NewNotificationMessage("Error!",
                                        "error", "This has not yet been implimented for females!",
                                        new NewNotificationMessageContent[0]));
                                }


                                _ppeHEMS.CloseMenu();
                                return;
                            }

                            if (item == ParaSuit)
                            {
                                if (isMale)
                                {
                                    _locker.LoudOutfitName("HEMS - Paramedic");
                                }
                                else
                                {
                                    _newNotificationOverlay.SendNotification(new NewNotificationMessage("Error!",
                                        "error", "This has not yet been implimented for females!",
                                        new NewNotificationMessageContent[0]));
                                }

                                _ppeHEMS.CloseMenu();
                                return;
                            }
                        };
                    }

                    if (_userAces.IsNhsDoctor || _userAces.IsAdmin || _userAces.IsDeveloper || _userAces.IsTierTwo)
                    {
                        MenuItem DoctorFlightSuit = new MenuItem("HEMS Doctor Flight Suit");
                        _ppeHEMS.AddMenuItem(DoctorFlightSuit);

                        MenuItem DoctorSuit = new MenuItem("HEMS Doctor Ground Team");
                        _ppeHEMS.AddMenuItem(DoctorSuit);

                        _ppeHEMS.OnItemSelect += (menu, item, index) =>
                        {
                            var playerPed = Game.PlayerPed.Model;
                            bool isMale = (playerPed == (uint)PedHash.FreemodeMale01);

                            API.RequestAnimDict("clothingtie_try_tie_negitive_a");
                            API.TaskPlayAnim(Game.PlayerPed.Handle, "clothingtie", "try_tie_negative_a", 8f, 8f, 1100,
                                51, 1f, false, false, false);

                            if (item == DoctorFlightSuit)
                            {
                                if (isMale)
                                {
                                    _locker.LoudOutfitName("HEMS - Doctor (Helmet)");
                                }
                                else
                                {
                                    _newNotificationOverlay.SendNotification(new NewNotificationMessage("Error!",
                                        "error", "This has not yet been implimented for females!",
                                        new NewNotificationMessageContent[0]));
                                }

                                _ppeHEMS.CloseMenu();
                                return;
                            }

                            if (item == DoctorSuit)
                            {
                                if (isMale)
                                {
                                    _locker.LoudOutfitName("HEMS - Doctor");
                                }
                                else
                                {
                                    _newNotificationOverlay.SendNotification(new NewNotificationMessage("Error!",
                                        "error", "This has not yet been implimented for females!",
                                        new NewNotificationMessageContent[0]));
                                }

                                _ppeHEMS.CloseMenu();
                                return;
                            }
                        };
                    }
                }

                #endregion LasEupBoot


                _bootMenu.OnItemSelect += (menu, item, index) =>
                {
                    if (item.Text == "Equip/Return Gloves")
                    {
                        API.ExecuteCommand("gloves");
                    }

                    if (item.Text == "Rack AI Defib Kit")
                    {
                        API.RemoveWeaponFromPed(Game.PlayerPed.Handle, (uint)API.GetHashKey("WEAPON_ECG"));
                        playerPed.Weapons.Select((WeaponHash)API.GetHashKey("WEAPON_UNARMED"));
                    }

                    if (item.Text == "Un Rack AI Defib Kit")
                    {
                        API.GiveWeaponToPed(Game.PlayerPed.Handle, (uint)API.GetHashKey("WEAPON_ECG"), 1000, false,
                            true);
                        API.SetCurrentPedWeapon(Game.PlayerPed.Handle, (uint)API.GetHashKey("WEAPON_ECG"), true);
                    }

                    if (item.Text == "Rack AI Medical Kit")
                    {
                        API.RemoveWeaponFromPed(Game.PlayerPed.Handle, (uint)API.GetHashKey("WEAPON_ALS"));
                        playerPed.Weapons.Select((WeaponHash)API.GetHashKey("WEAPON_UNARMED"));
                    }

                    if (item.Text == "Un Rack AI Medical Kit")
                    {
                        API.GiveWeaponToPed(Game.PlayerPed.Handle, (uint)API.GetHashKey("WEAPON_ALS"), 1000, false,
                            true);
                        API.SetCurrentPedWeapon(Game.PlayerPed.Handle, (uint)API.GetHashKey("WEAPON_ALS"), true);
                    }
                };
            }

            #endregion MedicalBoot

            #region LfbBootEup

            if (_permissionService.CurrentUserRole.Branch == UserBranch.Fire)
            {
                if (_userAces.IsFireBoroughCommander || _userAces.IsFireStationCommander || _userAces.IsFireOfficer)
                {
                    _ppeLfb.ClearMenuItems();

                    MenuItem Turnout = new MenuItem("LFRS Incident Command");
                    _ppeLfb.AddMenuItem(Turnout);

                    MenuItem OfficerUniform = new MenuItem("LFRS Section Command");
                    _ppeLfb.AddMenuItem(OfficerUniform);

                    MenuItem Officer = new MenuItem("LFRS Saftey Officer");
                    _ppeLfb.AddMenuItem(Officer);

                    MenuItem Officer1 = new MenuItem("Firefighter Casual");
                    _ppeLfb.AddMenuItem(Officer1);

                    MenuItem Officer2 = new MenuItem("Station Officer Turnout");
                    _ppeLfb.AddMenuItem(Officer2);

                    MenuItem PpeLfb = new MenuItem("LFRS PPE Menu");
                    _bootMenu.AddMenuItem(PpeLfb);

                    _bootMenu.OnItemSelect += (menu, item, index) =>
                    {
                        if (item == PpeLfb)
                        {
                            _ppeLfb.OpenMenu();
                            return;
                        }
                    };

                    _ppeLfb.OnItemSelect += (menu, item, index) =>
                    {
                        API.RequestAnimDict("clothingtie_try_tie_negitive_a");
                        API.TaskPlayAnim(Game.PlayerPed.Handle, "clothingtie", "try_tie_negative_a", 8f, 8f, 1100, 51,
                            1f, false, false, false);

                        var playerPed = Game.PlayerPed.Model;
                        bool isMale = (playerPed == (uint)PedHash.FreemodeMale01);

                        if (item == Turnout)
                        {
                            if (isMale)
                            {
                                _locker.LoudOutfitName("LFRS Incident Command");
                            }
                            else
                            {
                                _newNotificationOverlay.SendNotification(new NewNotificationMessage("Error!", "error",
                                    "This has not yet been implimented for females!",
                                    new NewNotificationMessageContent[0]));
                            }

                            _ppeLfb.CloseMenu();
                            return;
                        }

                        if (item == OfficerUniform)
                        {
                            if (isMale)
                            {
                                _locker.LoudOutfitName("LFRS Section Command");
                            }
                            else
                            {
                                _newNotificationOverlay.SendNotification(new NewNotificationMessage("Error!", "error",
                                    "This has not yet been implimented for females!",
                                    new NewNotificationMessageContent[0]));
                            }

                            _ppeLfb.CloseMenu();
                            return;
                        }

                        if (item == Officer)
                        {
                            if (isMale)
                            {
                                _locker.LoudOutfitName("LFRS Saftey Officer");
                            }
                            else
                            {
                                _newNotificationOverlay.SendNotification(new NewNotificationMessage("Error!", "error",
                                    "This has not yet been implimented for females!",
                                    new NewNotificationMessageContent[0]));
                            }

                            _ppeLfb.CloseMenu();
                            return;
                        }

                        if (item == Officer)
                        {
                            if (isMale)
                            {
                                _locker.LoudOutfitName("Firefighter Casual");
                            }
                            else
                            {
                                _newNotificationOverlay.SendNotification(new NewNotificationMessage("Error!", "error",
                                    "This has not yet been implimented for females!",
                                    new NewNotificationMessageContent[0]));
                            }

                            _ppeLfb.CloseMenu();

                            return;
                        }

                        if (item == Officer2)
                        {
                            if (isMale)
                            {
                                _locker.LoudOutfitName("Station Officer Turnout");
                            }
                            else
                            {
                                _newNotificationOverlay.SendNotification(new NewNotificationMessage("Error!", "error",
                                    "This has not yet been implimented for females!",
                                    new NewNotificationMessageContent[0]));
                            }

                            _ppeLfb.CloseMenu();

                            return;
                        }
                    };
                }

                if (_userAces.IsFruTrained || _userAces.IsDeveloper)
                {
                    _ppeLfbkit.ClearMenuItems();

                    MenuItem Spreaders = new MenuItem("Take/Return - Spreaders");
                    _notifications.Success("PoliceMP Fire Tools", "Spreaders Equiped");
                    _ppeLfbkit.AddMenuItem(Spreaders);

                    MenuItem Cutters = new MenuItem("Take/Return - Cutters");
                    _notifications.Success("PoliceMP Fire Tools", "Cutters Equiped");
                    _ppeLfbkit.AddMenuItem(Cutters);

                    MenuItem Glass = new MenuItem("Take/Return - Glass Saw");
                    _notifications.Success("PoliceMP Fire Tools", "Glass Saw Equiped");
                    _ppeLfbkit.AddMenuItem(Glass);

                    MenuItem LfbEquip = new MenuItem("LFRS Equipment Menu");
                    _bootMenu.AddMenuItem(LfbEquip);

                    _bootMenu.OnItemSelect += (menu, item, index) =>
                    {
                        if (item == LfbEquip)
                        {
                            _ppeLfbkit.OpenMenu();
                            return;
                        }
                    };

                    _ppeLfbkit.OnItemSelect += (menu, item, index) =>
                    {
                        API.RequestAnimDict("clothingtie_try_tie_negitive_a");
                        API.TaskPlayAnim(Game.PlayerPed.Handle, "clothingtie", "try_tie_negative_a", 8f, 8f, 1100, 51,
                            1f, false, false, false);

                        if (item == Spreaders)
                        {
                            API.ExecuteCommand("spreader");
                            _ppeLfbkit.CloseMenu();
                            return;
                        }

                        if (item == Cutters)
                        {
                            API.ExecuteCommand("cutter");
                            _ppeLfbkit.CloseMenu();
                            return;
                        }

                        if (item == Glass)
                        {
                            API.ExecuteCommand("glassmaster");
                            _ppeLfbkit.CloseMenu();
                            return;
                        }
                    };
                }
            }

            #endregion LfbEupBoot


            if (_userAces.IsBandTwo || _userAces.IsAdmin || _userAces.IsDeveloper)
            {
                _ppeIC.ClearMenuItems();

                MenuItem Met1 = new MenuItem("Police IC Vest");
                _ppeIC.AddMenuItem(Met1);

                MenuItem Met2 = new MenuItem("Firearms IC Vest");
                _ppeIC.AddMenuItem(Met2);

                MenuItem Met3 = new MenuItem("ERT SGT 1");
                _ppeIC.AddMenuItem(Met3);

                MenuItem Met4 = new MenuItem("Firearms Sergeant 1");
                _ppeIC.AddMenuItem(Met4);

                MenuItem PPECommand1 = new MenuItem("Police IC Vests");
                _bootMenu.AddMenuItem(PPECommand1);

                _bootMenu.OnItemSelect += (menu, item, index) =>
                {
                    if (item == PPECommand1)
                    {
                        _ppeIC.OpenMenu();
                        return;
                    }
                };

                _ppeIC.OnItemSelect += (menu, item, index) =>
                {
                    API.RequestAnimDict("clothingtie_try_tie_negitive_a");
                    API.TaskPlayAnim(Game.PlayerPed.Handle, "clothingtie", "try_tie_negative_a", 8f, 8f, 1100, 51, 1f,
                        false, false, false);

                    var playerPed = Game.PlayerPed.Model;
                    bool isMale = (playerPed == (uint)PedHash.FreemodeMale01);

                    if (item == Met1)
                    {
                        if (isMale)
                        {
                            _locker.LoudOutfitName("Police IC Vest");
                        }
                        else
                        {
                            _newNotificationOverlay.SendNotification(new NewNotificationMessage("Error!", "error",
                                "This has not yet been implimented for females!",
                                new NewNotificationMessageContent[0]));
                        }

                        _ppeIC.CloseMenu();
                        return;
                    }

                    if (item == Met2)
                    {
                        if (isMale)
                        {
                            _locker.LoudOutfitName("Firearms IC Vest");
                        }
                        else
                        {
                            _newNotificationOverlay.SendNotification(new NewNotificationMessage("Error!", "error",
                                "This has not yet been implimented for females!",
                                new NewNotificationMessageContent[0]));
                        }

                        _ppeIC.CloseMenu();
                        return;
                    }

                    if (item == Met3)
                    {
                        if (isMale)
                        {
                            _locker.LoudOutfitName("ERT SGT 1");
                        }
                        else
                        {
                            _newNotificationOverlay.SendNotification(new NewNotificationMessage("Error!", "error",
                                "This has not yet been implimented for females!",
                                new NewNotificationMessageContent[0]));
                        }

                        _ppeIC.CloseMenu();
                        return;
                    }

                    if (item == Met4)
                    {
                        if (isMale)
                        {
                            _locker.LoudOutfitName("Firearms Sergeant 1");
                        }
                        else
                        {
                            _newNotificationOverlay.SendNotification(new NewNotificationMessage("Error!", "error",
                                "This has not yet been implimented for females!",
                                new NewNotificationMessageContent[0]));
                        }

                        _ppeIC.CloseMenu();
                        return;
                    }
                };
            }

            #region CidBootEup

            if (_permissionService.CurrentUserRole.Division == UserDivision.Cid)
            {
                if (_userAces.IsCidTrained)
                {
                    _ppeCID.ClearMenuItems();

                    MenuItem ForensicOutfit = new MenuItem("Forensic Outfit");
                    _ppeCID.AddMenuItem(ForensicOutfit);

                    MenuItem Detective1 = new MenuItem("Detective 1");
                    _ppeCID.AddMenuItem(Detective1);

                    MenuItem Detective2 = new MenuItem("Detective 2");
                    _ppeCID.AddMenuItem(Detective2);

                    MenuItem FlyingSquad1 = new MenuItem("Flying Squad 1");
                    _ppeCID.AddMenuItem(FlyingSquad1);

                    MenuItem FlyingSquad2 = new MenuItem("Flying Squad 2");
                    _ppeCID.AddMenuItem(FlyingSquad2);

                    MenuItem PpeCID = new MenuItem("CID EUP Menu");
                    _bootMenu.AddMenuItem(PpeCID);

                    _bootMenu.OnItemSelect += (menu, item, index) =>
                    {
                        if (item == PpeCID)
                        {
                            _ppeCID.OpenMenu();
                            return;
                        }
                    };

                    _ppeCID.OnItemSelect += (menu, item, index) =>
                    {
                        API.RequestAnimDict("clothingtie_try_tie_negitive_a");
                        API.TaskPlayAnim(Game.PlayerPed.Handle, "clothingtie", "try_tie_negative_a", 8f, 8f, 1100, 51,
                            1f, false, false, false);

                        var playerPed = Game.PlayerPed.Model;
                        bool isMale = (playerPed == (uint)PedHash.FreemodeMale01);

                        if (item == ForensicOutfit)
                        {
                            if (isMale)
                            {
                                _locker.LoudOutfitName("Forensic Outfit");
                            }
                            else
                            {
                                _newNotificationOverlay.SendNotification(new NewNotificationMessage("Error!", "error",
                                    "This has not yet been implimented for females!",
                                    new NewNotificationMessageContent[0]));
                            }

                            _ppeCID.CloseMenu();
                            return;
                        }

                        if (item == Detective1)
                        {
                            if (isMale)
                            {
                                _locker.LoudOutfitName("Detective 1");
                            }
                            else
                            {
                                _newNotificationOverlay.SendNotification(new NewNotificationMessage("Error!", "error",
                                    "This has not yet been implimented for females!",
                                    new NewNotificationMessageContent[0]));
                            }


                            _ppeCID.CloseMenu();
                            return;
                        }

                        if (item == Detective2)
                        {
                            if (isMale)
                            {
                                _locker.LoudOutfitName("Detective 2");
                            }
                            else
                            {
                                _newNotificationOverlay.SendNotification(new NewNotificationMessage("Error!", "error",
                                    "This has not yet been implimented for females!",
                                    new NewNotificationMessageContent[0]));
                            }


                            _ppeCID.CloseMenu();
                            return;
                        }

                        if (item == FlyingSquad1)
                        {
                            if (isMale)
                            {
                                _locker.LoudOutfitName("Flying Squad 1");
                            }
                            else
                            {
                                _newNotificationOverlay.SendNotification(new NewNotificationMessage("Error!", "error",
                                    "This has not yet been implimented for females!",
                                    new NewNotificationMessageContent[0]));
                            }


                            _ppeCID.CloseMenu();
                            return;
                        }

                        if (item == FlyingSquad2)
                        {
                            if (isMale)
                            {
                                _locker.LoudOutfitName("Flying Squad 2");
                            }
                            else
                            {
                                _newNotificationOverlay.SendNotification(new NewNotificationMessage("Error!", "error",
                                    "This has not yet been implimented for females!",
                                    new NewNotificationMessageContent[0]));
                            }


                            _ppeCID.CloseMenu();
                            return;
                        }
                    };
                }
            }

            #endregion CidBootEup

            _bootMenu.OnItemSelect += (menu, item, index) =>
            {
                if (item == medicalEquipment) API.ExecuteCommand("medqmb");
                _bootMenu.CloseMenu();
            };
            _bootMenuLoading = false;
            _bootMenu.OnMenuClose += menu =>
            {
                _inputDisabled = false;
                Game.PlayerPed.Task.StandStill(1);
            };
        }


        public void OpenMenu(Vehicle vehicle)
        {
            _inputDisabled = true;
            Game.PlayerPed.Task.PlayAnimation("missexile3", "ex03_dingy_search_case_base_michael");

            _bootMenu.OpenMenu();
            HandleBootMenu(vehicle.Handle);
        }

        public bool IsMenuActive()
        {
            return _bootMenu?.Visible == true;
        }
    }
}