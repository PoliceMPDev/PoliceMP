using System;
using CitizenFX.Core;
using CitizenFX.Core.Native;
using CitizenFX.Core.UI;
using MenuAPI;
using PoliceMP.Client.Services.Interfaces;
using PoliceMP.Core.Client;
using PoliceMP.Core.Client.Commands.Interfaces;
using PoliceMP.Core.Client.Communications.Interfaces;
using PoliceMP.Core.Client.Interface;
using PoliceMP.Core.Shared;
using PoliceMP.Shared.Constants;
using PoliceMP.Shared.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PoliceMP.Shared.Enums;
using Newtonsoft.Json;
using PoliceMP.Client.Services;
using PoliceMP.Core.Client.Scripts;
using PoliceMP.Core.Mediator;
using PoliceMP.Shared.Options;

namespace PoliceMP.Client.Scripts.ClothingLocker
{
    public class ClothingLocker : Script, IClothingLocker
    {
        private readonly ILegacyClientCommunicationsManager _comms;
        private readonly ILogger<ClothingLocker> _logger;
        private readonly ICommandManager _commands;
        private readonly INotificationService _notifications;
        private readonly ITickManager _ticks;
        private readonly IPermissionService _permission;

        private readonly List<PedOutfit> _usableOutfits = new();
        private List<Locker> _lockers = new();
        private List<PedOutfit> _allOutfits = new();
        private List<LockerItem> _allLockerItems = new();

        private int _camera;
        private bool _inMenu = false;
        private Vector3 _exitPosition;
        private float _exitHeading;

        private readonly Menu _lockerMenu = new Menu("Locker");

        private bool IsEaServer = false;

        public ClothingLocker(
            ILegacyClientCommunicationsManager server,
            ILogger<ClothingLocker> logger,
            ICommandManager commands,
            INotificationService notifications,
            ITickManager ticks,
            IPermissionService permissionService,
            IFeatureService featureService
        ) {
            _comms = server;
            _logger = logger;
            _commands = commands;
            _notifications = notifications;
            _ticks = ticks;
            _permission = permissionService;
            IsEaServer = featureService.IsFeatureEnabled(FeatureToggle.IsEarlyAccess);
        }

        public void LoudOutfitName(string outfitName)
        {
            var outfit = _usableOutfits.FirstOrDefault(s => s.Name == outfitName);
            ApplySelection(outfit);
        }

        private async void OnMenuOpen(Menu menu)
        {
            try
            {
                _lockerMenu.ClearMenuItems();

                var userAces = await _permission.GetUserAces();
                var outfitList = new List<PedOutfit>();

                bool hasOutfit = false;
                foreach (var outfit in _usableOutfits)
                {
                    if (CanUserAccessOutfit(userAces, outfit))
                    {
                        hasOutfit = true;
                        AddOutfitToMenu(outfit, outfitList);
                    }
                }

                if (!hasOutfit)
                {
                    _notifications.Error("No Outfits", "There are no outfits configured for your selected division or with your permissions. Please run as something else and contact Dev team for advice.");
                }

                _lockerMenu.OnItemSelect += (_, item, index) =>
                {
                    if (item.ItemData is PedOutfit outfitData)
                    {
                        ApplySelection(outfitData);
                        _lockerMenu.CloseMenu();
                        CloseMenu(false);
                    }
                    else
                    {
                        _logger.Error($"Unable to find the Outfit Data for {item.Label}");
                    }
                };

                _lockerMenu.OnIndexChange += (_, oldItem, newItem, oldIndex, newIndex) =>
                {
                    if (newItem.ItemData is PedOutfit outfitData)
                    {
                        ApplySelection(outfitData);
                    }
                    else
                    {
                        _logger.Error($"Unable to find the Outfit Data for {newItem.Label}");
                    }
                };

            }
            catch (Exception ex)
            {
                _logger.Error($"An error occurred while opening the menu: {ex}");
            }
        }

        private bool CanUserAccessOutfit(UserAces userAces, PedOutfit outfit)
        {
            if (!userAces.IsWhiteListed && !outfit.AceGroupsRequired.Any())
                return true;

            if (userAces.IsModerator && outfit.AceGroupsRequired.Contains("Police.modAuth"))
                return true;
            
            if (userAces.IsBandOne && outfit.AceGroupsRequired.Contains("staff.bandOne"))
                return true;
            
            if (userAces.IsAdmin)
                return true;

            var role = _permission.CurrentUserRole;
            return role.Branch switch
            {
                UserBranch.Police => CheckPolicePermissions(userAces, role, outfit),
                UserBranch.Nhs => CheckNhsPermissions(userAces, role, outfit),
                UserBranch.Fire => CheckFirePermissions(userAces, role, outfit),
                UserBranch.Highways => outfit.AceGroupsRequired.Contains("heto.Trained"),
                UserBranch.Control => outfit.AceGroupsRequired.Contains("control.trained"),
                _ => false
            };
        }

        private bool CheckPolicePermissions(UserAces userAces, UserRole role, PedOutfit outfit)
        {
            if (outfit.AceGroupsRequired.Contains("Police.sergeant"))
                return true;

            return role.Division switch
            {
                UserDivision.Afo => outfit.AceGroupsRequired.Contains("Police.afoTrained"),
                UserDivision.Cid => outfit.AceGroupsRequired.Contains("Police.cidTrained"),
                UserDivision.Dsu => outfit.AceGroupsRequired.Contains("Police.dogTrained"),
                UserDivision.Ert => outfit.AceGroupsRequired.Contains("Police.whitelisted"),
                UserDivision.Npas => outfit.AceGroupsRequired.Contains("Police.npasTrained"),
                UserDivision.Rpu => outfit.AceGroupsRequired.Contains("Police.rpuTrained"),
                UserDivision.Tsg => outfit.AceGroupsRequired.Contains("Police.tsg"),
                _ => false
            };
        }

        private bool CheckNhsPermissions(UserAces userAces, UserRole role, PedOutfit outfit)
        {

            if (outfit.LockerType == "nhs" && role.Division == UserDivision.NhsMananagement)
                return true;
            
            return role.Division switch
            {
                UserDivision.Clinical => outfit.AceGroupsRequired.Contains("nhs.paramedic"),
                UserDivision.ClinicalAdv => outfit.AceGroupsRequired.Contains("nhs.clinicalADV"),
                UserDivision.ClinicalStudent => outfit.AceGroupsRequired.Contains("nhs.lasStudent"),
                UserDivision.hart => outfit.AceGroupsRequired.Contains("nhs.hart"),
                UserDivision.Hems => outfit.AceGroupsRequired.Contains("nhs.hems"),
                UserDivision.HemsDoctor => outfit.AceGroupsRequired.Contains("group.lasDoctor"),
                UserDivision.BeepDoctor => outfit.AceGroupsRequired.Contains("group.beepDoc"),
                UserDivision.blood => outfit.AceGroupsRequired.Contains("don.blood"),
                UserDivision.Survive => outfit.AceGroupsRequired.Contains("group.survive"),
                _ => false
            };
        }

        private bool CheckFirePermissions(UserAces userAces, UserRole role, PedOutfit outfit)
        {
            if (userAces.IsFireTrained && outfit.AceGroupsRequired.Contains("Fire.Trained"))
                return true;

            return role.Division switch
            {
                UserDivision.CoastGuard => outfit.AceGroupsRequired.Contains("coast.guard"),
                UserDivision.mountainRescue => outfit.AceGroupsRequired.Contains("group.mountain"),
                UserDivision.FRU => outfit.AceGroupsRequired.Contains("group.fru"),
                _ => false
            };
        }

        private void AddOutfitToMenu(PedOutfit outfit, List<PedOutfit> outfitList)
        {
            _lockerMenu.AddMenuItem(new MenuItem(outfit.Name) { ItemData = outfit });
            outfitList.Add(outfit);
        }

        protected override async Task OnStartAsync()
        {
            await GetLockerPoints();
            await GetPresetOutfits();
            await GetLockerItems();

            MenuController.MenuAlignment = MenuController.MenuAlignmentOption.Right;
            MenuController.AddMenu(_lockerMenu);
            MenuController.EnableMenuToggleKeyOnController = false;
            MenuController.MenuToggleKey = (Control)(-1);

            _lockerMenu.InstructionalButtons.Clear();
            _lockerMenu.OnMenuOpen += OnMenuOpen;

            _lockerMenu.OnMenuClose += (_) =>
            {
                CloseMenu(true);
            };

            _comms.On(ClientEvents.SetSpawnClothes, SetSpawnClothesAsync);

            _commands.Register("outfitdetails").WithHandler(() =>
            {
                var ph = Game.PlayerPed.Handle;
                var output = "Your Outfit Chief: (SEND TO A DEV WITH DESCRIPTION AND WHITELISTING)";

                for (var i = 0; i <= 11; i++)
                {
                    output += "\n COMP [" + PrefixMe(false, i) + "]   \t   Drawable ID: " + API.GetPedDrawableVariation(ph, i) + "   \t   Texture ID: " + API.GetPedTextureVariation(ph, i);
                }

                for (var i = 0; i <= 3; i++)
                {
                    output += "\n PROP [" + PrefixMe(true, i) + "]   \t   Drawable ID: " + API.GetPedPropIndex(ph, i) + "   \t  Texture ID: " + API.GetPedPropTextureIndex(ph, i);
                }
                _logger.Debug(output);
                _notifications.Success("Outfit", output);

                string PrefixMe(bool prop, int i)
                {
                    switch (prop)
                    {
                        case true:
                            switch (i)
                            {
                                case 0:
                                    return "HEAD";

                                case 1:
                                    return "EYES";

                                case 2:
                                    return "EARS";

                                case 3:
                                    return "MOUTH";
                            }

                            break;
                    }

                    return i switch
                    {
                        0 => "HEAD",
                        1 => "BERD",
                        2 => "HAIR",
                        3 => "UPPR",
                        4 => "LOWR",
                        5 => "HAND",
                        6 => "FEET",
                        7 => "TEEF",
                        8 => "ACCS",
                        9 => "TASK",
                        10 => "DECL",
                        11 => "JBIB",
                        _ => "SOMETHING BROKEN HELP ME"
                    };
                }
            });

            _ticks.On(ClothingLockerTick);
        }

        private async Task SetSpawnClothesAsync()
        {
            var attempts = 0;
            while (_usableOutfits.Count == 0 && attempts < 20)
            {
                attempts++;
                _logger.Trace("Awaiting default outfits.");
                await Delay(100);
            }

            if (_usableOutfits.Count == 0)
            {
                API.SetPedDefaultComponentVariation(Game.PlayerPed.Handle);
                _logger.Trace("Default spawn clothes could not be found.");
                return;
            }

            var aces = await _permission.GetUserAces();
            
            //_logger.Debug(Game.PlayerPed.Model.ToString()); 
            
            while (!Game.PlayerPed.Exists ()){
                await Delay(10);
                
                //_logger.Debug(Game.PlayerPed.Model.ToString()); 
            }

            while (API.GetPlayerSwitchState() != 5)
            {
                //_logger.Debug(API.GetPlayerSwitchState().ToString());
                await Delay(10);
            }

            while (API.IsPlayerSwitchInProgress())
            {
                //_logger.Debug(API.GetPlayerSwitchState().ToString());
                await Delay(10);
            }
            
            //_logger.Debug(Game.PlayerPed.Model.ToString()); 
            
            var playerPed = Game.PlayerPed.Model;
            bool isMale = (playerPed == (uint)PedHash.FreemodeMale01);
            
            PedOutfit outfit = null;
            
            _logger.Debug($"User Branch: {_permission.CurrentUserRole.Branch} - Division: {_permission.CurrentUserRole.Division} - Male: {isMale}");
            
            if (!isMale)
            {
                /*
                 * THIS SETS THE DEFAULT OUTFITS FOR FEMALES
                 * */
                
                switch (_permission.CurrentUserRole.Branch)
                {
                    case UserBranch.Police:
                        SetUsableOutfits("police");
                        outfit = aces.IsWhiteListed switch
                        {
                            false => _usableOutfits.FirstOrDefault(s => s.Name == "PCSO 1 F"),
                            true when _permission.CurrentUserRole.Division == UserDivision.Ert || _permission.CurrentUserRole.Division == UserDivision.None =>
                                _usableOutfits.FirstOrDefault(s => s.Name == "Response 1 F"),
                            true => _permission.CurrentUserRole.Division switch
                            {
                                UserDivision.Afo => _usableOutfits.FirstOrDefault(s => s.Name == "Firearms 1 (Female)"),
                                UserDivision.Cid => _usableOutfits.FirstOrDefault(s => s.Name == "Detective 3 F"),
                                UserDivision.Dsu => _usableOutfits.FirstOrDefault(s => s.Name == "Dog Handler 1 (Female)"),
                                UserDivision.Npas => _usableOutfits.FirstOrDefault(s => s.Name == "Pilot Flight Suit F"),
                                UserDivision.Rpu => _usableOutfits.FirstOrDefault(s => s.Name == "Roads Policing 1 (Female)"),
                                _ => _usableOutfits.FirstOrDefault(s => s.Name == "Response 1 F")
                            }
                        };
                        break;

                    case UserBranch.Fire:
                        SetUsableOutfits("fire");
                        outfit = _permission.CurrentUserRole.Division switch
                        {
                            UserDivision.LFB => _usableOutfits.FirstOrDefault(s => s.Name == "Probationary Firefighter F"),
                            UserDivision.CoastGuard => _usableOutfits.FirstOrDefault(s => s.Name == "MCA Ground Crew F"),
                            UserDivision.mountainRescue => _usableOutfits.FirstOrDefault(s => s.Name == "Mountain Rescue Team F"),
                            UserDivision.FRU => _usableOutfits.FirstOrDefault(s => s.Name == "Probationary Firefighter F"),
                            _ => _usableOutfits.FirstOrDefault(s => s.Name == "Probationary Firefighter F")
                        };
                        break;


                    case UserBranch.Nhs:
                        SetUsableOutfits("nhs");
                        outfit = _permission.CurrentUserRole.Division switch
                        {
                            UserDivision.StJohn => _usableOutfits.FirstOrDefault(s => s.Name == "St John - First Responder F"),
                            UserDivision.blood => _usableOutfits.FirstOrDefault(s => s.Name == "NHS Blood Team - Bike F"),
                            UserDivision.hart => _usableOutfits.FirstOrDefault(s => s.Name == "HART - Paramedic Turnout F"),
                            UserDivision.Clinical => _usableOutfits.FirstOrDefault(s => s.Name == "Clinical - Paramedic (Female)"),
                            UserDivision.ClinicalAdv => _usableOutfits.FirstOrDefault(s => s.Name == "Clinical - Advanced Paramedic (Female)"),
                            UserDivision.ClinicalStudent => _usableOutfits.FirstOrDefault(s => s.Name == "Clinical - Student Paramedic 1 F"),
                            UserDivision.Hems => _usableOutfits.FirstOrDefault(s => s.Name == "HEMS - Paramedic F"),
                            UserDivision.HemsDoctor => _usableOutfits.FirstOrDefault(s => s.Name == "HEMS - Doctor Ground Team F"),
                            UserDivision.BeepDoctor => _usableOutfits.FirstOrDefault(s => s.Name == "Beep Paramedic 1 F"),
                            _ => _usableOutfits.FirstOrDefault(s => s.Name == "Clinical - Student Paramedic 1 (Female)")
                        };
                        break;

                    case UserBranch.Civ:
                        SetUsableOutfits("civ");
                        break;

                    case UserBranch.Highways:
                        SetUsableOutfits("heto");
                        outfit = _usableOutfits.FirstOrDefault(s => s.Name == "Highways Traffic Officer F");
                        break;

                    case UserBranch.Control:
                        SetUsableOutfits("police");
                        outfit = _usableOutfits.FirstOrDefault(s => s.Name == "Control Room Operator F");
                        break;

                    default:
                        _logger.Debug("Branch not recognized. Defaulting to police outfits.");
                        SetUsableOutfits("police");
                        outfit = _usableOutfits.FirstOrDefault();
                        break;
                }
            }
            else
            {
                /*
                 * THIS SETS THE DEFAULT OUTFITS FOR MALES
                 * */

                switch (_permission.CurrentUserRole.Branch)
                {
                    case UserBranch.Police:
                        SetUsableOutfits("police");
                        outfit = aces.IsWhiteListed switch
                        {
                            false => _usableOutfits.FirstOrDefault(s => s.Name == "PCSO 1"),
                            true when _permission.CurrentUserRole.Division == UserDivision.Ert || _permission.CurrentUserRole.Division == UserDivision.None =>
                                _usableOutfits.FirstOrDefault(s => s.Name == "Response 1"),
                            true => _permission.CurrentUserRole.Division switch
                            {
                                UserDivision.Afo => _usableOutfits.FirstOrDefault(s => s.Name == "Firearms 1"),
                                UserDivision.Cid => _usableOutfits.FirstOrDefault(s => s.Name == "Detective 3"),
                                UserDivision.Dsu => _usableOutfits.FirstOrDefault(s => s.Name == "Dog Handler 1"),
                                UserDivision.Npas => _usableOutfits.FirstOrDefault(s => s.Name == "Pilot Flight Suit"),
                                UserDivision.Rpu => _usableOutfits.FirstOrDefault(s => s.Name == "Roads Policing 4"),
                                UserDivision.Tsg => _usableOutfits.FirstOrDefault(s => s.Name == "Territorial Support Group"),
                                _ => _usableOutfits.FirstOrDefault(s => s.Name == "Response 1")
                            }
                        };
                        break;

                    case UserBranch.Fire:
                        SetUsableOutfits("fire");
                        outfit = _permission.CurrentUserRole.Division switch
                        {
                            UserDivision.LFB => _usableOutfits.FirstOrDefault(s => s.Name == "Probationary Firefighter"),
                            UserDivision.CoastGuard => _usableOutfits.FirstOrDefault(s => s.Name == "MCA Ground Crew"),
                            UserDivision.mountainRescue => _usableOutfits.FirstOrDefault(s => s.Name == "Mountain Rescue Team"),
                            UserDivision.FRU => _usableOutfits.FirstOrDefault(s => s.Name == "Probationary Firefighter"),
                            _ => _usableOutfits.FirstOrDefault(s => s.Name == "Probationary Firefighter")
                        };
                        break;

                    case UserBranch.Nhs:
                        SetUsableOutfits("nhs");
                        outfit = _permission.CurrentUserRole.Division switch
                        {
                            UserDivision.StJohn => _usableOutfits.FirstOrDefault(s => s.Name == "St John - First Responder"),
                            UserDivision.blood => _usableOutfits.FirstOrDefault(s => s.Name == "NHS Blood Team - Bike"),
                            UserDivision.hart => _usableOutfits.FirstOrDefault(s => s.Name == "HART - Paramedic Turnout"),
                            UserDivision.Clinical => _usableOutfits.FirstOrDefault(s => s.Name == "Clinical - Paramedic 1"),
                            UserDivision.ClinicalAdv => _usableOutfits.FirstOrDefault(s => s.Name == "Clinical - Advanced Paramedic 2"),
                            UserDivision.ClinicalStudent => _usableOutfits.FirstOrDefault(s => s.Name == "Clinical - Student Paramedic 1"),
                            UserDivision.Hems => _usableOutfits.FirstOrDefault(s => s.Name == "HEMS - Paramedic"),
                            UserDivision.HemsDoctor => _usableOutfits.FirstOrDefault(s => s.Name == "HEMS - Doctor Ground Team"),
                            UserDivision.BeepDoctor => _usableOutfits.FirstOrDefault(s => s.Name == "Beep Paramedic 1"),
                            _ => _usableOutfits.FirstOrDefault(s => s.Name == "Clinical - Student Paramedic 4")
                        };
                        break;

                    case UserBranch.Civ:
                        SetUsableOutfits("civ");
                        break;

                    case UserBranch.Highways:
                        SetUsableOutfits("heto");
                        outfit = _usableOutfits.FirstOrDefault(s => s.Name == "Highways Traffic Officer");
                        break;

                    case UserBranch.Control:
                        SetUsableOutfits("police");
                        outfit = _usableOutfits.FirstOrDefault(s => s.Name == "Control Room Operator");
                        break;

                    default:
                        _logger.Debug("Branch not recognized. Defaulting to police outfits.");
                        SetUsableOutfits("police");
                        outfit = _usableOutfits.FirstOrDefault();
                        break;
                }
            }

            if (outfit == null)
            {
                _logger.Debug("Outfit not found. Using the first available outfit.");
                outfit = _usableOutfits.FirstOrDefault();
            }

            _logger.Debug($"Setting default outfit to {outfit.Name}");
            ApplySelection(outfit);
        }

        private async Task GetLockerPoints()
        {
            var obj = await _comms.Request<List<Locker>>(ServerEvents.GetLockerPoints);
            _lockers = obj;
            _logger.Trace("Started " + _lockers.Count + " stations");

            foreach (var l in _lockers)
            {
                var b = World.CreateBlip(new Vector3(
                    (IsEaServer ? l.xPosEA ?? l.xPos : l.xPos),
                    (IsEaServer ? l.yPosEA ?? l.yPos : l.yPos),
                    (IsEaServer ? l.yPosEA ?? l.yPos : l.yPos)
                ));
                b.IsShortRange = false;

                switch (l.LockerType)
                {
                    case "police":
                        b.Sprite = BlipSprite.PoliceStation;
                        b.Name = "Police Station";
                        break;

                    case "heto":
                        b.Sprite = (BlipSprite)67;
                        b.Color = BlipColor.TrevorOrange;
                        b.Name = "Motorways England";
                        break;

                    case "nhs":
                        b.Sprite = BlipSprite.Hospital;
                        b.Name = "Hospital";
                        break;

                    case "fire":
                        b.Sprite = BlipSprite.CarWash;
                        b.Color = BlipColor.Red;
                        b.Name = "Fire Brigade";
                        break;
                }
            }
        }

        private async Task GetPresetOutfits()
        {
            var obj = await _comms.Request<List<PedOutfit>>(ServerEvents.GetLockerOutfits);
            _allOutfits = obj;
            SetUsableOutfits("police");
        }

        private void SetUsableOutfits(string lockerType)
        {
            lock (_usableOutfits)
            {
                _usableOutfits.Clear();

                var playerPed = Game.PlayerPed.Model;
                bool isMale = (playerPed == (uint)PedHash.FreemodeMale01);

                foreach (var p in _allOutfits.Where(p => p.LockerType == lockerType))
                {
                    if (isMale && p.MaleOutfit) _usableOutfits.Add(p);
                    if (!isMale && !p.MaleOutfit) _usableOutfits.Add(p);
                }
            }
        }

        private async Task GetLockerItems()
        {
            var obj = await _comms.Request<List<LockerItem>>(ServerEvents.GetLockerClothes);
            _allLockerItems = obj;
            _logger.Trace("Found " + _allLockerItems.Count + " total items of clothing");
        }

        private Task ClothingLockerTick()
        {
            if ((_lockers == null || _lockers.Count == 0)) { } else { if (!_inMenu) { LockerEntries(); } }
            if (_inMenu) { HidePlayers(); }

            return Task.FromResult(0);
        }

        private static void HidePlayers()
        {
            for (var i = 0; i <= 256; i++)
            {
                if (API.PlayerId() == i) { continue; }
                API.NetworkConcealPlayer(i, true, true);
            }
        }

        private async Task LockerEntries()
        {
            var playerPos = Game.PlayerPed.Position;

            foreach (var l in _lockers)
            {
                var distance = API.GetDistanceBetweenCoords(playerPos.X, playerPos.Y, playerPos.Z, (IsEaServer ? l.xPosEA ?? l.xPos : l.xPos), (IsEaServer ? l.yPosEA ?? l.yPos : l.yPos), (IsEaServer ? l.zPosEA ?? l.zPos : l.zPos), true);
                var scale = 0.1F * API.GetGameplayCamFov();

                if (!(distance < 5.0f)) continue;

                API.DrawMarker(1, (IsEaServer ? l.xPosEA ?? l.xPos : l.xPos), (IsEaServer ? l.yPosEA ?? l.yPos : l.yPos), (IsEaServer ? l.zPosEA ?? l.zPos : l.zPos) - 1, 0, 0, 0, 0, 0, 0, 1F, 1F, 2F, 20, 20, 200, 50, false, true, 2, false, null, null, false);

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
                API.AddTextComponentString(l.Name);
                API.SetDrawOrigin(
                    (IsEaServer ? l.xPosEA ?? l.xPos : l.xPos), 
                    (IsEaServer ? l.yPosEA ?? l.yPos : l.yPos), 
                    (IsEaServer ? l.zPosEA ?? l.zPos : l.zPos) + 1F,
                    0
                );
                API.DrawText(0, 0);
                API.ClearDrawOrigin();

                if (!(distance < 1.0f)) continue;

                SetUsableOutfits(l.LockerType);
                _exitPosition = new Vector3(
                    (IsEaServer ? l.xPosExitEA ?? l.xPosExit : l.xPosExit),
                    (IsEaServer ? l.yPosExitEA ?? l.yPosExit : l.yPosExit),
                    (IsEaServer ? l.zPosExitEA ?? l.zPosExit : l.zPosExit)
                );
                _exitHeading = l.exitHeading;

                var interiorPosition = new Vector3(462.8663f, 4833.794f, -59.99385f);
                var interior = API.GetInteriorAtCoords(interiorPosition.X, interiorPosition.Y, interiorPosition.Z);
                API.LoadInterior(interior);
                var attempt = 0;
                var loaded = false;
                while (attempt <= 15 && !loaded)
                {
                    API.LoadInterior(interior);
                    //_logger.Debug("Waiting for interior to load. (" + attempt + ")");
                    if (API.IsInteriorReady(interior))
                    {
                        loaded = true;
                        _logger.Trace("Interior Loaded...");
                    }
                    await Delay(100);
                    attempt++;
                    if (attempt == 15 && !loaded)
                    {
                        _logger.Debug("Interior could not be loaded.");
                        return;
                    }
                }

                _inMenu = true;
                _lockerMenu.OpenMenu();

                API.DisableAllControlActions(0);
                Game.Player.CanControlCharacter = false;

                var player = Game.Player.Character;

                API.NetworkFadeOutEntity(player.Handle, true, false);
                API.DoScreenFadeOut(1000);

                player.Position = interiorPosition;
                player.Heading = -94.45885f;
                API.PlaceObjectOnGroundProperly(player.Handle);
                API.DisplayRadar(false);
                Screen.Hud.IsVisible = false;
                var camPos = API.GetOffsetFromEntityInWorldCoords(player.Handle, 0f, 3f, 0.5f);
                var camHeading = MathUtil.Mod((player.Heading + 180), 360);
                _camera = API.CreateCameraWithParams(26379945, camPos.X, camPos.Y, camPos.Z, -8f, 0f, camHeading, 50.0F, true, 2);
                API.SetCamActive(_camera, true);
                API.RenderScriptCams(true, false, 0, false, false);
                API.NetworkFadeInEntity(player.Handle, true);
                API.DoScreenFadeIn(1000);

                Game.Player.CanControlCharacter = true;
                player.IsPositionFrozen = true;
            }
        }

        private void CloseMenu(bool revert)
        {
            var player = Game.Player.Character;
            API.NetworkFadeOutEntity(player.Handle, true, false);
            API.DoScreenFadeOut(1000);

            player.IsPositionFrozen = false;
            API.EnableAllControlActions(0);

            API.SetCamActive(_camera, false);
            API.RenderScriptCams(false, false, 0, false, false);
            API.DestroyCam(_camera, false);
            API.DestroyAllCams(true);
            API.DisplayRadar(true);
            Screen.Hud.IsVisible = true;

            if (revert)
            {
            }

            player.Position = _exitPosition;
            player.Rotation = new Vector3(0, _exitHeading, 0);
            API.NetworkFadeInEntity(player.Handle, true);

            for (var i = 0; i <= 256; i++)
            {
                API.NetworkConcealPlayer(i, false, false);
            }

            API.DoScreenFadeIn(1000);
            _inMenu = false;
        }

        private bool ApplySelection(PedOutfit chosenOutfit)
        {
            if (chosenOutfit == null)
            {
                Debug.WriteLine("No outfit specified");
                return false;
            }

            var handle = Game.PlayerPed.Handle;

            //Set component variations
            API.SetPedComponentVariation(handle, 1, chosenOutfit.BERD.DrawableID, chosenOutfit.BERD.TextureID, chosenOutfit.BERD.PaletteID);
            API.SetPedComponentVariation(handle, 3, chosenOutfit.UPPR.DrawableID, chosenOutfit.UPPR.TextureID, chosenOutfit.UPPR.PaletteID);
            API.SetPedComponentVariation(handle, 4, chosenOutfit.LOWR.DrawableID, chosenOutfit.LOWR.TextureID, chosenOutfit.LOWR.PaletteID);
            API.SetPedComponentVariation(handle, 5, chosenOutfit.HAND.DrawableID, chosenOutfit.HAND.TextureID, chosenOutfit.HAND.PaletteID);
            API.SetPedComponentVariation(handle, 6, chosenOutfit.FEET.DrawableID, chosenOutfit.FEET.TextureID, chosenOutfit.FEET.PaletteID);
            API.SetPedComponentVariation(handle, 7, chosenOutfit.TEEF.DrawableID, chosenOutfit.TEEF.TextureID, chosenOutfit.TEEF.PaletteID);
            API.SetPedComponentVariation(handle, 8, chosenOutfit.ACCS.DrawableID, chosenOutfit.ACCS.TextureID, chosenOutfit.ACCS.PaletteID);
            API.SetPedComponentVariation(handle, 9, chosenOutfit.TASK.DrawableID, chosenOutfit.TASK.TextureID, chosenOutfit.TASK.PaletteID);
            API.SetPedComponentVariation(handle, 10, chosenOutfit.DECL.DrawableID, chosenOutfit.DECL.TextureID, chosenOutfit.DECL.PaletteID);
            API.SetPedComponentVariation(handle, 11, chosenOutfit.JBIB.DrawableID, chosenOutfit.JBIB.TextureID, chosenOutfit.JBIB.PaletteID);

            //Set prop variaitons
            API.SetPedPropIndex(handle, 0, chosenOutfit.headProp.DrawableID, chosenOutfit.headProp.TextureID, true);
            API.SetPedPropIndex(handle, 1, chosenOutfit.EYES.DrawableID, chosenOutfit.EYES.TextureID, true);
            API.SetPedPropIndex(handle, 2, chosenOutfit.EARS.DrawableID, chosenOutfit.EARS.TextureID, true);
            API.SetPedPropIndex(handle, 3, chosenOutfit.MOUTH.DrawableID, chosenOutfit.MOUTH.TextureID, true);

            return true;
        }
    }
}