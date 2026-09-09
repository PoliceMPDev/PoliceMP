using CitizenFX.Core;
using CitizenFX.Core.Native;
using CitizenFX.Core.UI;
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
using System.Linq;
using System.Threading.Tasks;
using PoliceMP.Client.Scripts.Admin;
using PoliceMP.Client.Scripts.ELS;
using PoliceMP.Core.Client.Extensions;
using PoliceMP.Shared.Constants.Decors;
using PoliceMP.Core.Client.Extensions;
using PoliceMP.Core.Client.Scripts;
using PoliceMP.Shared.Enums;
using PoliceMP.Client.Overlays.NewNotification;

namespace PoliceMP.Client.Scripts.Garage
{
    internal class Garage : Script, IGarageInterface
    {
        private readonly ILegacyClientCommunicationsManager _comms;
        private readonly ILogger<Garage> _logger;
		private readonly INewNotificationOverlay _newNotificationOverlay;
		private readonly ITickManager _ticks;
        private readonly IPermissionService _permissionService;
        private readonly IELS _els;
        private readonly ICommonFunctionsService _common;
        private readonly IAdmin _admin;

        private DateTime lastTime;

        private List<GarageEntity> Garages = new List<GarageEntity>();

        private Menu GarageMenu = new Menu("Garage");
        private int camera;

        private GarageEntity GarageEntered;
        private bool InMenu = false;
        private bool SpawningWithVehicle = false;
        public int PersonalVehicleNetId { get; private set; }

        private bool IsEaServer = false;

        public Garage(
            ILegacyClientCommunicationsManager server,
            ILogger<Garage> logger,
            INewNotificationOverlay newNotificationOverlay, ITickManager ticks,
            IPermissionService permissionService, IELS els, ICommonFunctionsService common, IAdmin admin, IFeatureService featureService)
        {
            _comms = server;
            _logger = logger;
			_newNotificationOverlay = newNotificationOverlay;
			_ticks = ticks;
            _permissionService = permissionService;
            _els = els;
            _common = common;
            _admin = admin;
            _comms.On<bool>(ClientEvents.PlayerSpawned, async (firstSpawn) =>
            {
                await GetGaragePoints();
                //_logger.Debug($"Received {Garages?.Count} garages!");
                await GetGarageData();
            });
            IsEaServer = featureService.IsFeatureEnabled(FeatureToggle.IsEarlyAccess);
        }

        protected override async Task OnStartAsync()
        {
            MenuController.MenuAlignment = MenuController.MenuAlignmentOption.Right;
            MenuController.AddMenu(GarageMenu);

            GarageMenu.InstructionalButtons.Clear();
            MenuController.EnableMenuToggleKeyOnController = false;
            MenuController.MenuToggleKey = (Control)(-1);

            Function.Call((Hash)3520272001, "car.defaultlight.night.emissive.on", 3500.00f);
            Function.Call((Hash)3520272001, "car.defaultlight.day.emissive.on", 3000.00f);

            GarageMenu.OnMenuOpen += (menu) =>
            {
                SpawningWithVehicle = false;
                InMenu = true;
            };

            GarageMenu.OnMenuClose += (menu) =>
            {
                if (SubMenuOpen) return;
                if (SpawningWithVehicle) return;
                InMenu = false;

                Ped player = Game.Player.Character;
                API.NetworkFadeOutEntity(player.Handle, true, false);
                API.DoScreenFadeOut(1000);

                ShowIrrelevants();
                player.IsPositionFrozen = false;
                API.EnableAllControlActions(0);

                API.SetCamActive(camera, false);
                API.RenderScriptCams(false, false, 0, false, false);
                API.DestroyCam(camera, false);
                API.DestroyAllCams(true);
                API.DisplayRadar(true);
                Screen.Hud.IsVisible = true;

                Vector3 ExitPoint = new Vector3(
                    (IsEaServer ? GarageEntered.xWalkExitEA ?? GarageEntered.xWalkExit : GarageEntered.xWalkExit),
                    (IsEaServer ? GarageEntered.yWalkExitEA ?? GarageEntered.yWalkExit : GarageEntered.yWalkExit),
                    (IsEaServer ? GarageEntered.zWalkExitEA ?? GarageEntered.zWalkExit : GarageEntered.zWalkExit)
                );
                Game.PlayerPed.Position = ExitPoint;

                API.NetworkFadeInEntity(player.Handle, true);
                API.DoScreenFadeIn(1000);
                SpawningWithVehicle = false;
                InMenu = false;
            };

            GarageMenu.OnItemSelect += (menu, item, index) => { SubMenuOpen = true; };
            //await GetGarageData();

            _ticks.On(GarageTick);
            _ticks.On(PersonalVehicleBlipTick);
        }

        private async Task PersonalVehicleBlipTick()
        {
            if (PersonalVehicleNetId == default)
            {
                await Script.Delay(5000);
                return;
            }

            if (!API.NetworkDoesEntityExistWithNetworkId(PersonalVehicleNetId)) return;

            Entity personalVehicle = Entity.FromNetworkId(PersonalVehicleNetId);

            if (personalVehicle == null || personalVehicle is not Vehicle)
            {
                await Script.Delay(5000);
                return;
            }

            if (!personalVehicle.Exists())
            {
                await Script.Delay(5000);
                return;
            }


            if (personalVehicle.AttachedBlips.Any())
            {
                await Script.Delay(5000);
                return;
            }

            var personalBlip = personalVehicle.AttachBlip();
            personalBlip.Name = "Personal Vehicle";
            personalBlip.Sprite = BlipSprite.PersonalVehicleCar;

        }


        private async Task GarageTick()
        {
            if (InMenu)
            {
                HideIrrelevants();

                API.SetPedDensityMultiplierThisFrame(0f);
                API.SetScenarioPedDensityMultiplierThisFrame(0f, 0f);
                API.SetAmbientVehicleRangeMultiplierThisFrame(0f);
                API.SetParkedVehicleDensityMultiplierThisFrame(0f);
                API.SetRandomVehicleDensityMultiplierThisFrame(0f);
                API.SetVehicleDensityMultiplierThisFrame(0f);
            }
            else
            {
                await HandleEntryPoints();
            }

            if (API.NetworkDoesEntityExistWithNetworkId(PersonalVehicleNetId))
            {
                if (Entity.FromNetworkId(PersonalVehicleNetId) is Vehicle personalVehicle)
                {
                    if (!Entity.Exists(personalVehicle)) return;

                    if (!API.NetworkGetEntityIsNetworked(personalVehicle.Handle)) return;

                    if (personalVehicle?.Exists() == true)
                    {
                        if (!personalVehicle.IsPersistent)
                            personalVehicle.IsPersistent = true;

                        CheckLockKeys();
                    }
                }
            }
        }


        private async Task CheckLockKeys()

        {
            if (!API.IsControlJustReleased(1, 167)) return;
            var personalVehicle = (Vehicle)Entity.FromNetworkId(PersonalVehicleNetId);
            if (!Entity.Exists(personalVehicle)) return;
            //if (!await personalVehicle.TryRequestNetworkEntityControl()) return;

            var vehiclePos = personalVehicle.Position;
            var playerPos = Game.PlayerPed.Position;

                var timeNow = DateTime.Now;
                var timeSinceLastBackup = timeNow - lastTime;
                if (timeSinceLastBackup.TotalSeconds < 10)
                {
                    _newNotificationOverlay.SendNotification(new NewNotificationMessage("Personal Vehicle", "error", "Your keyfob has jammed, try waiting before pressing again!.", new NewNotificationMessageContent[0]));
                    return;
                }

                lastTime = DateTime.Now;

            if (API.GetDistanceBetweenCoords(vehiclePos.X, vehiclePos.Y, vehiclePos.Z, playerPos.X, playerPos.Y,
                playerPos.Z, true) >= 10f)
            {
                _newNotificationOverlay.SendNotification(new NewNotificationMessage("Personal Vehicle", "warning", "You must be closer to your vehicle to use the keys.", new NewNotificationMessageContent[0]));
                return;
            }

            var hasNetControl = API.NetworkHasControlOfNetworkId(personalVehicle.NetworkId);

            switch (personalVehicle.LockStatus)
            {
                case VehicleLockStatus.Unlocked:
                case VehicleLockStatus.None:
                    if (hasNetControl)
                    {
                        personalVehicle.LockStatus = VehicleLockStatus.Locked;
                    }
                    else
                    {
                        _comms.ToServer(ServerEvents.NetOwnerClientRequestingCarToggleLock, personalVehicle.NetworkId, true);
                    }


                    API.SetVehicleDoorsLockedForAllPlayers(personalVehicle.Handle, true);
                    API.PlaySoundFromEntity(-1, "Remote_Control_Open", Game.Player.Character.Handle, "PI_Menu_Sounds",
                        true, 0);
                    _newNotificationOverlay.SendNotification(new NewNotificationMessage("Personal Vehicle", "success", "Your personal vehicle is now locked.", new NewNotificationMessageContent[0]));
                    break;

                case VehicleLockStatus.Locked:
                    if (hasNetControl)
                    {
                        personalVehicle.LockStatus = VehicleLockStatus.Unlocked;
                    }
                    else
                    {
                        _comms.ToServer(ServerEvents.NetOwnerClientRequestingCarToggleLock, personalVehicle.NetworkId, false);
                    }

                    API.SetVehicleDoorsLockedForAllPlayers(personalVehicle.Handle, false);
                    API.PlaySoundFromEntity(-1, "Remote_Control_Open", Game.Player.Character.Handle, "PI_Menu_Sounds",
                        true, 0);
                    _newNotificationOverlay.SendNotification(new NewNotificationMessage("Personal Vehicle", "success", "Your personal vehicle is now unlocked.", new NewNotificationMessageContent[0]));
                    break;
            }
        }

        private async Task HandleEntryPoints()
        {
            if (_admin.IsNoClipActive()) return;

            if (InMenu) return;

            if (Game.PlayerPed.CurrentVehicle != null) return;

            var playerPos = Game.PlayerPed.Position;

            foreach (var g in Garages)
            {
                if (g.BlipType == 361) continue;
                var distance = API.GetDistanceBetweenCoords(
                    playerPos.X,
                    playerPos.Y,
                    playerPos.Z,
                    (IsEaServer ? g.xEntryEA ?? g.xEntry : g.xEntry),
                    (IsEaServer ? g.yEntryEA ?? g.yEntry : g.yEntry),
                    (IsEaServer ? g.zEntryEA ?? g.zEntry : g.zEntry),
                    true
                );
                var scale = 0.1F * API.GetGameplayCamFov();

                if (!(distance < 5.0f)) continue;
                API.DrawMarker(
                    1,
                    (IsEaServer ? g.xEntryEA ?? g.xEntry : g.xEntry),
                    (IsEaServer ? g.yEntryEA ?? g.yEntry : g.yEntry),
                    (IsEaServer ? g.zEntryEA ?? g.zEntry : g.zEntry) - 1,
                    0, 0, 0, 0, 0, 0, 1F, 1F, 2F, 20, 20, 200, 50,
                    false, true, 2, false, null, null, false
                );

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
                API.AddTextComponentString(g.GarageName);
                API.SetDrawOrigin(
                    (IsEaServer ? g.xEntryEA ?? g.xEntry : g.xEntry),
                    (IsEaServer ? g.yEntryEA ?? g.yEntry : g.yEntry),
                    (IsEaServer ? g.zEntryEA ?? g.zEntry : g.zEntry) + 1F,
                    0
                );
                API.DrawText(0, 0);
                API.ClearDrawOrigin();

                if (!(distance < 1.0f)) continue;
                GarageEntered = g;

                GarageMenu.ClearMenuItems();
                //_logger.Debug(GarageMenuCats.Count + " total menu cats");

                foreach (var mi in GarageMenuCats)
                {
                    //_logger.Debug("Checking label " + mi.Text);
                    if (!g.GarageCategories.Contains(mi.Text)) continue;
                    //_logger.Debug("MATCH FOUND");
                    GarageMenu.AddMenuItem(mi);
                }

                var interiorPosition = new Vector3(-1253.7454f, -3009.9180f, -48.4902f);
                var interior =
                    API.GetInteriorAtCoords(interiorPosition.X, interiorPosition.Y, interiorPosition.Z);
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
                        //_logger.Trace("Interior Loaded...");
                    }

                    await Delay(100);
                    attempt++;
                    if (attempt != 15 || loaded) continue;
                    _logger.Debug("Interior could not be loaded.");
                    return;
                }

                InMenu = true;
                GarageMenu.OpenMenu();

                API.DisableAllControlActions(0);
                Game.Player.CanControlCharacter = false;

                var player = Game.Player.Character;

                API.NetworkFadeOutEntity(player.Handle, true, false);
                API.DoScreenFadeOut(1000);

                player.Position = interiorPosition;

                API.PlaceObjectOnGroundProperly(player.Handle);
                API.DisplayRadar(false);
                Screen.Hud.IsVisible = false;
                var camPos = new Vector3(-1258.4115f, -3011.1509f, -46.4902f);
                const float heading = 100.6429f;
                camera = API.CreateCameraWithParams(26379945, camPos.X, camPos.Y, camPos.Z, -8f, 0f, heading,
                    50.0F, true, 2);
                API.SetCamActive(camera, true);
                API.RenderScriptCams(true, false, 0, false, false);
                API.NetworkFadeInEntity(player.Handle, true);

                while (!API.HasCollisionLoadedAroundEntity(Game.PlayerPed.Handle))
                    await Delay(0);

                // Wait for the level to load around player
                while (!API.HaveAllStreamingRequestsCompleted(Game.PlayerPed.Handle))
                    await Delay(0);

                API.DoScreenFadeIn(1000);

                Game.Player.CanControlCharacter = true;
                player.IsPositionFrozen = true;
            }
        }

        private void HideIrrelevants()
        {
            Game.PlayerPed.IsInvincible = true;

            var vehicles = World.GetAllVehicles();
            foreach (var vehicle in vehicles)
            {
                if (!Entity.Exists(vehicle)) continue;
                if (!API.NetworkGetEntityIsNetworked(vehicle.Handle)) continue;

                if (DummyVehicles.Count > 0)
                {
                    foreach (var dv in DummyVehicles.Where(Entity.Exists).Where(dv => API.NetworkGetEntityIsNetworked(dv.Handle)))
                    {
                        if (vehicle.NetworkId == dv.NetworkId)
                        {
                            vehicle.IsVisible = true;
                            vehicle.IsCollisionEnabled = false;
                            continue;
                        }
                        else
                        {
                            vehicle.IsVisible = false;
                            vehicle.IsCollisionEnabled = false;
                            continue;
                        }
                    }
                }
                else
                {
                    vehicle.IsVisible = false;
                    vehicle.IsCollisionEnabled = false;
                }
            }
        }

        private void ShowIrrelevants()
        {
            Game.PlayerPed.IsInvincible = false;

            foreach (var v in World.GetAllVehicles())
            {
                v.IsVisible = true;
                v.IsCollisionEnabled = true;
            }
        }

        private async Task GetGaragePoints()
        {
            var obj = await _comms.Request<List<GarageEntity>>(ServerEvents.GetGaragePoints);
            Garages = obj;

            foreach (var g in Garages)
            {
                if (g.BlipType == 0) continue;

                var b = World.CreateBlip(new Vector3(
                    (IsEaServer ? g.xEntryEA ?? g.xEntry : g.xEntry),
                    (IsEaServer ? g.yEntryEA ?? g.yEntry : g.yEntry),
                    (IsEaServer ? g.zEntryEA ?? g.zEntry : g.zEntry)
                ));
                API.SetBlipSprite(b.Handle, g.BlipType);
                API.SetBlipColour(b.Handle, g.BlipColour);
                if (g.BlipType == 361)
                {
                    b.Name = "Petrol Station";
                }
                else
                {
                    b.Name = "Garage";
                }
                b.IsShortRange = true;
            }
        }

        private List<Vehicle> DummyVehicles = new List<Vehicle>();
        private List<MenuItem> GarageMenuCats = new List<MenuItem>();
        private Vector3 DummyVehiclePosition = new Vector3(-1267.2274f, -3013.0884f, -48.8990f);
        private float DummyVehicleHeading = 330.2375f;
        private bool SubMenuOpen = false;

        private async Task GetGarageData()
        {
            var obj = await _comms.Request<List<GarageCategory>>(ServerEvents.GetGarageData);

            GarageMenu.ClearMenuItems();
            GarageMenuCats.Clear();

            var userAces = await _permissionService.GetUserAces();

            foreach (var gc in obj)
            {
                //_logger.Debug($"GC: {gc.CategoryName}");
                AddGarageCategory(gc, userAces, _permissionService.CurrentUserRole);
            }
        }

        private bool spawning = false;

        private async Task AddGarageCategory(GarageCategory gc, UserAces userAces, UserRole userRoles)
        {
            var m = new Menu(gc.CategoryName);
            MenuController.AddSubmenu(GarageMenu, m);

            var mi = new MenuItem(gc.CategoryName)
            {
                Label = "→→→"
            };

            GarageMenu.AddMenuItem(mi);
            MenuController.BindMenuItem(GarageMenu, m, mi);

            foreach (var v in gc.Vehicles)
            {
                m.AddMenuItem(new MenuItem(v.VehicleName, v.VehicleClass)
                {
                    ItemData = v
                });
            }

            m.OnItemSelect += async (_menu, _item, _index) =>
            {
                //Take current selection ootside
                if (spawning) return;
                spawning = true;

                var model = new Model(_item.Description);
                if (!await model.Request(10000))
                {
                    spawning = false;
                    return;
                }

                if (!API.HasModelLoaded((uint)API.GetHashKey(_item.Description)))
                {
                    _newNotificationOverlay.SendNotification(new NewNotificationMessage("Garage", "error", "Please wait until this model has loaded to spawn it. (Model will automatically download)", new NewNotificationMessageContent[0]));
                    return;
                }

                SpawningWithVehicle = true;
                API.DoScreenFadeOut(1000);
                var ExitPoint = new Vector3(GarageEntered.xWalkExit, GarageEntered.yWalkExit,
                    GarageEntered.zWalkExit);
                Game.PlayerPed.Position = ExitPoint;

                var attempt = 0;
                var pointfound = false;
                var exitpoint = new ExitPoint();
                while (attempt <= 5 && !pointfound)
                {
                    foreach (var ep in (IsEaServer ? GarageEntered.ExitPointsEA ?? GarageEntered.ExitPoints : GarageEntered.ExitPoints))
                    {
                        if (API.IsAnyObjectNearPoint(ep.xExit, ep.yExit, ep.zExit, 5f, false))
                        {
                            continue;
                        }

                        if (API.IsAnyVehicleNearPoint(ep.xExit, ep.yExit, ep.zExit, 5f))
                        {
                            continue;
                        }

                        if (API.IsAnyPedNearPoint(ep.xExit, ep.yExit, ep.zExit, 5f))
                        {
                            continue;
                        }

                        exitpoint = ep;
                        pointfound = true;
                    }

                    await Delay(100);
                    attempt++;
                }


                try
                {
                    if (API.NetworkDoesEntityExistWithNetworkId(PersonalVehicleNetId))
                    {
                        var personalVehicle = (Vehicle)Entity.FromNetworkId(PersonalVehicleNetId);
                        if (personalVehicle != null)
                        {
                            if (API.DoesEntityExist(personalVehicle.Handle))
                            {
                                personalVehicle.AttachedBlip.Delete();
                                personalVehicle.Delete();
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    spawning = false;
                    _logger.Error($"Errorzzzz: ");
                }


                if (!pointfound)
                {
                    var r = new Random();
                    var outpoint = new Vector3();
                    API.GetPointOnRoadSide(
                        (IsEaServer ? GarageEntered.xEntryEA ?? GarageEntered.xEntry : GarageEntered.xEntry) + r.Next(20, 50),
                        (IsEaServer ? GarageEntered.yEntryEA ?? GarageEntered.yEntry : GarageEntered.yEntry) + r.Next(20, 50),
                        (IsEaServer ? GarageEntered.zEntryEA ?? GarageEntered.zEntry : GarageEntered.zEntry),
                        0,
                        ref outpoint
                    );
                    await CreateVehicleAsync(new Vector3(outpoint.X, outpoint.Y, outpoint.Z), 0, _item.Description,
                        (GarageVehicles)_item.ItemData);
                }
                else
                {
                    await CreateVehicleAsync(new Vector3(exitpoint.xExit, exitpoint.yExit, exitpoint.zExit),
                        exitpoint.hExit, _item.Description, (GarageVehicles)_item.ItemData);
                }

                ShowIrrelevants();

                InMenu = false;
                m.CloseMenu();
                GarageMenu.CloseMenu();

                var player = Game.Player.Character;
                API.NetworkFadeOutEntity(player.Handle, true, false);

                player.IsPositionFrozen = false;
                API.EnableAllControlActions(0);
                API.SetCamActive(camera, false);
                API.RenderScriptCams(false, false, 0, false, false);
                API.DestroyCam(camera, false);
                API.DestroyAllCams(true);
                API.DisplayRadar(true);
                Screen.Hud.IsVisible = true;

                API.NetworkFadeInEntity(player.Handle, true);
                API.DoScreenFadeIn(1000);
                spawning = false;
            };

            var indexChanging = false;
            string old = null;

            m.OnIndexChange += async (_menu, _oldItem, _newItem, _oldIndex, _newIndex) =>
            {
                if (indexChanging)
                {
                    return;
                }

                indexChanging = true;
                ClearDummyVehicle();
                indexChanging = false;

                if (old != null)
                {
                    API.SetModelAsNoLongerNeeded((uint)API.GetHashKey(old));
                }

                old = _oldItem.Description;

                var model = new Model(_newItem.Description);
                if (await model.Request(30000))
                {
                    // Do we still have the vehicle selected?
                    if (_menu.GetCurrentMenuItem() == _newItem)
                    {
                        CreateDummyVehicle(model, _newItem.ItemData);
                    }
                    else
                    {
                        //_logger.Debug($"Vehicle selection changed. Skipping spawn of {_newItem.Description}...");
                    }
                }
            };

            m.OnMenuClose += (_menu) =>
            {
                SubMenuOpen = false;
                ClearDummyVehicle();

                if (GarageMenu.Visible)
                {
                    InMenu = true;
                }
            };

            m.OnMenuOpen += async (_menu) =>
            {
                SubMenuOpen = true;
                ClearDummyVehicle();
                var current = _menu.GetCurrentMenuItem();

                if (old != null)
                {
                    API.SetModelAsNoLongerNeeded((uint)API.GetHashKey(old));
                }
                old = current.Description;

                var model = new Model(current.Description);
                if (await model.Request(10000))
                {
                    CreateDummyVehicle(model, current.ItemData);
                }
            };


            if (userAces.IsLoa) // IS LOA
            {
                if (userAces.IsAdmin || userAces.IsDeveloper || userAces.IsBasicDonator)
                {
                    if (gc.AceGroupsRequired.Contains("Don.basic"))
                    {
                        GarageMenuCats.Add(mi);
                        return;
                    }
                }
                if (userAces.IsAdmin || userAces.IsDeveloper || userAces.HasNhsBloodDlc)
                {
                    if (gc.AceGroupsRequired.Contains("don.blood"))
                    {
                        GarageMenuCats.Add(mi);
                        return;
                    }
                }
                if (userAces.IsAdmin || userAces.IsDeveloper || userAces.IsProDonator)
                {
                    if (gc.AceGroupsRequired.Contains("Don.pro"))
                    {
                        GarageMenuCats.Add(mi);
                        return;
                    }
                }
                if (userAces.IsAdmin || userAces.IsDeveloper || userAces.HasCityPoliceDlc || userAces.IsDigitalTeam)
                {
                    if (gc.AceGroupsRequired.Contains("Don.colp"))
                    {
                        GarageMenuCats.Add(mi);
                        return;
                    }
                }
                if (userAces.IsAdmin || userAces.IsDeveloper || userAces.IsSurviveDonator)
                {
                    if (gc.AceGroupsRequired.Contains("group.survive"))
                    {
                        GarageMenuCats.Add(mi);
                        return;
                    }
                }
                if (userAces.IsAdmin || userAces.IsDeveloper || userAces.IsBtpDonator)
                {
                    if (gc.AceGroupsRequired.Contains("group.donBtp"))
                    {
                        GarageMenuCats.Add(mi);
                        return;
                    }
                }
                if (userAces.IsAdmin || userAces.IsDeveloper || userAces.IsElecDlc)
                {
                    if (gc.AceGroupsRequired.Contains("group.elecDlc"))
                    {
                        GarageMenuCats.Add(mi);
                        return;
                    }
                }
                if (userAces.IsAdmin || userAces.IsDeveloper || userAces.IsBeepDoctor)
                {
                    if (gc.AceGroupsRequired.Contains("group.beepDoc"))
                    {
                        GarageMenuCats.Add(mi);
                        return;
                    }
                }
                if (userAces.IsJruTrained)
                {
                    if (gc.AceGroupsRequired.Contains("trained.jru"))
                    {
                        GarageMenuCats.Add(mi);
                        return;
                    }
                }
                if (userAces.IsAdmin || userAces.IsDeveloper || userAces.IsJamPackDlc)
                {
                    if (gc.AceGroupsRequired.Contains("group.jam"))
                    {
                        GarageMenuCats.Add(mi);
                        return;
                    }
                }
                if (userAces.IsAdmin || userAces.IsDeveloper || userAces.IsTacOpsDlc)
                {
                    if (gc.AceGroupsRequired.Contains("group.tacOpsDlc"))
                    {
                        GarageMenuCats.Add(mi);
                        return;
                    }
                }

                switch (userRoles.Division)
                {
                    case UserDivision.Ert when (gc.AceGroupsRequired.Contains("Police.whitelisted") || gc.AceGroupsRequired.Contains("Police.tsg")):
                        //_logger.Debug("Added Whitelisted");
                        GarageMenuCats.Add(mi);
                        break;
                }

                switch (userRoles.Division)
                {
                    case UserDivision.StJohn when !gc.AceGroupsRequired.Any():
                        //_logger.Debug("Added StJohns");
                        GarageMenuCats.Add(mi);
                        break;
                }
            }


            if (!userAces.IsLoa) // NOT LOA
            {
                if (userAces.IsAdmin || userAces.IsDeveloper || !userAces.IsWhiteListed)
                {
                    GarageMenuCats.Add(mi);
                    return;
                }

                if (userAces.IsAdmin || userAces.IsDeveloper || userAces.IsBasicDonator)
                {
                    if (gc.AceGroupsRequired.Contains("Don.basic"))
                    {
                        GarageMenuCats.Add(mi);
                        return;
                    }
                }

                if (userAces.IsAdmin || userAces.IsDeveloper || userAces.IsModerator)
                {
                    if (gc.AceGroupsRequired.Contains("Police.modAuth"))
                    {
                        GarageMenuCats.Add(mi);
                        return;
                    }
                }
                
                if (userAces.IsAdmin || userAces.IsDeveloper || userAces.IsJamPackDlc)
                {
                    if (gc.AceGroupsRequired.Contains("group.jam"))
                    {
                        GarageMenuCats.Add(mi);
                        return;
                    }
                }
                
                if (userAces.IsAdmin || userAces.IsDeveloper || userAces.IsTacOpsDlc)
                {
                    if (gc.AceGroupsRequired.Contains("group.tacOpsDlc"))
                    {
                        GarageMenuCats.Add(mi);
                        return;
                    }
                }

                if (userAces.IsAdmin || userAces.IsDeveloper || userAces.HasNhsBloodDlc)
                {
                    if (gc.AceGroupsRequired.Contains("don.blood"))
                    {
                        GarageMenuCats.Add(mi);
                        return;
                    }
                }
                if (userAces.IsAdmin || userAces.IsDeveloper || userAces.IsMatrix)
                {
                    if (gc.AceGroupsRequired.Contains("group.matrix"))
                    {
                        GarageMenuCats.Add(mi);
                        return;
                    }
                }

                if (userAces.IsAdmin || userAces.IsDeveloper || userAces.IsBeepDoctor)
                {
                    if (gc.AceGroupsRequired.Contains("group.beepDoc"))
                    {
                        GarageMenuCats.Add(mi);
                        return;
                    }
                }

                if (userAces.IsAdmin || userAces.IsDeveloper || userAces.IsProDonator)
                {
                    if (gc.AceGroupsRequired.Contains("Don.pro"))
                    {
                        GarageMenuCats.Add(mi);
                        return;
                    }
                }

                if (userAces.IsAdmin || userAces.IsDeveloper || userAces.IsTierTwo)
                {
                    if (gc.AceGroupsRequired.Contains("group.TierTwo"))
                    {
                        GarageMenuCats.Add(mi);
                        return;
                    }
                }
                
                if (userAces.IsJruTrained)
                {
                    if (gc.AceGroupsRequired.Contains("trained.jru"))
                    {
                        GarageMenuCats.Add(mi);
                        return;
                    }
                }

                if (userAces.IsAdmin || userAces.IsDeveloper || userAces.HasCityPoliceDlc || userAces.IsDigitalTeam)
                {
                    if (gc.AceGroupsRequired.Contains("Don.colp"))
                    {
                        GarageMenuCats.Add(mi);
                        return;
                    }
                }

                if (userAces.IsAdmin || userAces.IsDeveloper || userAces.IsSurviveDonator)
                {
                    if (gc.AceGroupsRequired.Contains("group.survive"))
                    {
                        GarageMenuCats.Add(mi);
                        return;
                    }
				}
                
                if (userAces.IsAdmin || userAces.IsDeveloper || userAces.IsBtpDonator)
                {
                    if (gc.AceGroupsRequired.Contains("group.donBtp"))
                    {
                        GarageMenuCats.Add(mi);
                        return;
                    }
                }
                
                if (userAces.IsAdmin || userAces.IsDeveloper || userAces.IsElecDlc)
                {
                    if (gc.AceGroupsRequired.Contains("group.elecDlc"))
                    {
                        GarageMenuCats.Add(mi);
                        return;
                    }
                }

                if (userAces.IsAdmin || userAces.IsDeveloper || userAces.IsMrescueTrained)
                {
                    if (gc.AceGroupsRequired.Contains("group.mountain"))
                    {
                        GarageMenuCats.Add(mi);
                        return;
                    }
                }
                
				if (userAces.IsAdmin || userAces.IsDeveloper || userAces.IsContentCreator)
				{
					if (gc.AceGroupsRequired.Contains("Content.Creator"))
					{
						GarageMenuCats.Add(mi);
						return;
					}
				}

				if (userAces.IsAdmin || userAces.IsDeveloper || userAces.IsDigitalTeam)
				{
					if (gc.AceGroupsRequired.Contains("Digital.Team"))
					{
						GarageMenuCats.Add(mi);
						return;
					}
				}

				if (userAces.IsAdmin || userAces.IsDeveloper || userAces.IsRetired)
                {
                    if (gc.AceGroupsRequired.Contains("Retired"))
                    {
                        GarageMenuCats.Add(mi);
                        return;
                    }
                }

                if (userAces.IsAdmin || userAces.IsDeveloper || userAces.IsCollegeStaff)
                {
                    if (gc.AceGroupsRequired.Contains("Police.collegeStaff"))
                    {
                        GarageMenuCats.Add(mi);
                        return;
                    }
                }

                _logger.Debug($"User Branch: {userRoles.Branch} - Role: {userRoles.Division}");

                if (userRoles.Branch == UserBranch.Nhs)
                {
                    if (userRoles.Division == UserDivision.BeepDoctor)
                    {
                        if (gc.AceGroupsRequired.Contains("group.beepDoc") && userAces.IsBeepDoctor)
                        {
                            GarageMenuCats.Add(mi);
                            return;
                        }
                    }
                    if (userRoles.Division == UserDivision.NhsMananagement)
                    {
                        if (gc.AceGroupsRequired.Contains("nhs.clinicalTL") && userAces.IsNhsClinicalTl)
                        {
                            GarageMenuCats.Add(mi);
                            return;
                        }
                        if (gc.AceGroupsRequired.Contains("nhs.sectionleader") && userAces.IsNhsSectionLeader)
                        {
                            if (userAces.IsLoa) return;
                            GarageMenuCats.Add(mi);
                            return;
                        }
                        if (gc.AceGroupsRequired.Contains("nhs.hemsTL") && userAces.IsNhsHemsTl)
                        {
                            GarageMenuCats.Add(mi);
                            return;
                        }
                    }

                    if (userRoles.Division == UserDivision.HemsDoctor)
                    {
                        if (gc.AceGroupsRequired.Contains("group.lasDoctor") && userAces.IsNhsDoctor)
                        {
                            GarageMenuCats.Add(mi);
                            return;
                        }
                    }

                    if (userRoles.Division == UserDivision.Hems)
                    {
                        if (gc.AceGroupsRequired.Contains("nhs.hems") && userAces.IsNhsHems)
                        {
                            GarageMenuCats.Add(mi);
							return;
						}
                    }

                    if (userRoles.Division == UserDivision.hart)
                    {
                        if (gc.AceGroupsRequired.Contains("nhs.hart") && userAces.IsHartTrained)
                        {
                            GarageMenuCats.Add(mi);
							return;
						}
                    }
                    
                    if (userRoles.Division == UserDivision.ClinicalAdv)
                    {
                        if (gc.AceGroupsRequired.Contains("nhs.clinicalADV") && userAces.IsNhsClinicalAdv)
                        {
                            GarageMenuCats.Add(mi);
                            return;
                        }
                    }
                    
                    if (userRoles.Division == UserDivision.Clinical)
                    {
                        if (gc.AceGroupsRequired.Contains("nhs.paramedic") && userAces.IsNhsParamedic)
                        {
                            GarageMenuCats.Add(mi);
							return;
						}
                        if (gc.AceGroupsRequired.Contains("nhs.lasStudent") && userAces.IsStudentPara)
                        {
                            GarageMenuCats.Add(mi);
							return;
						}
                        if (gc.AceGroupsRequired.Contains("trained.jru") && userAces.IsJruTrained)
                        {
                            GarageMenuCats.Add(mi);
                            return;
                        }

                    }
                }


                _logger.Debug($"Branch: {userRoles.Branch} Division: {userRoles.Division} ");
                switch (userRoles.Branch)
                {
                    case UserBranch.Police:
                        switch (userRoles.Division)
                        {
                            case UserDivision.Afo when gc.AceGroupsRequired.Contains("Police.afoTrained"):
                                //_logger.Debug("Added AFO");
                                GarageMenuCats.Add(mi);
                                break;

                            case UserDivision.Cid when gc.AceGroupsRequired.Contains("Police.cidTrained"):
                                //_logger.Debug("Added CID");
                                GarageMenuCats.Add(mi);
                                break;

                            case UserDivision.Dsu when gc.AceGroupsRequired.Contains("Police.dogTrained"):
                                //_logger.Debug("Added DSU");
                                GarageMenuCats.Add(mi);
                                break;

                            case UserDivision.Ert when (gc.AceGroupsRequired.Contains("Police.whitelisted") || gc.AceGroupsRequired.Contains("Police.tsg")):
                                //_logger.Debug("Added Whitelisted");
                                GarageMenuCats.Add(mi);
                                break;

                            case UserDivision.Ert when (gc.AceGroupsRequired.Contains("Police.aro")):
                                //_logger.Debug("Added Area Response Unit");
                                GarageMenuCats.Add(mi);
                                break;

                            case UserDivision.Ert when (gc.AceGroupsRequired.Contains("Police.rural")):
                                //_logger.Debug("Rural");
                                GarageMenuCats.Add(mi);
                                break;

                            case UserDivision.Ert when !gc.AceGroupsRequired.Any():
                                //_logger.Debug("Added Response");
                                GarageMenuCats.Add(mi);
                                break;
                            
                            case UserDivision.Tsg when gc.AceGroupsRequired.Contains("Police.tsg"):
                                //_logger.Debug("Added TSG");
                                GarageMenuCats.Add(mi);
                                break;

                            case UserDivision.Npas when gc.AceGroupsRequired.Contains("Police.npasTrained"):
                                //_logger.Debug("Added NPAS");
                                GarageMenuCats.Add(mi);
                                break;

                            case UserDivision.Rpu when gc.AceGroupsRequired.Contains("Police.rpuTrained"):
                                //_logger.Debug("Added RPU");
                                GarageMenuCats.Add(mi);
                                break;

                            case UserDivision.mod when gc.AceGroupsRequired.Contains("Police.modAuth"):
                                //_logger.Debug("Added Mod Garage");
                                GarageMenuCats.Add(mi);
                                break;
                            
                            case UserDivision.Jam when gc.AceGroupsRequired.Contains("group.jam"):
                                //_logger.Debug("Added Jam Pack");
                                GarageMenuCats.Add(mi);
                                break;
                            
                            case UserDivision.TacOpsDlc when gc.AceGroupsRequired.Contains("group.tacOpsDlc"):
                                //_logger.Debug("Added Tac Ops DLC");
                                GarageMenuCats.Add(mi);
                                break;
                        }

                        break;

                    case UserBranch.Nhs:
                        switch (userRoles.Division)
                        {
                            case UserDivision.StJohn when !gc.AceGroupsRequired.Any():
                                GarageMenuCats.Add(mi);
                                break;

                            case UserDivision.Clinical when gc.AceGroupsRequired.Contains("nhs.paramedic"):
                                //_logger.Debug("Added Paramedic");
                                GarageMenuCats.Add(mi);
                                break;
                            
                            case UserDivision.Clinical when gc.AceGroupsRequired.Contains("trained.jru"):
                                //_logger.Debug("Added JRU");
                                GarageMenuCats.Add(mi);
                                break;

                            case UserDivision.ClinicalStudent when gc.AceGroupsRequired.Contains("nhs.lasStudent"):
                                //_logger.Debug("Added Student Paramedic");
                                GarageMenuCats.Add(mi);
                                break;

                            case UserDivision.Hems when gc.AceGroupsRequired.Contains("nhs.hems"):
                                //_logger.Debug("Added Hems");
                                GarageMenuCats.Add(mi);
                                break;
                            
                            case UserDivision.HemsDoctor when gc.AceGroupsRequired.Contains("group.lasDoctor"):
                                //_logger.Debug("Added Las Doctor");
                                GarageMenuCats.Add(mi);
                                break;
                            
                            case UserDivision.ClinicalAdv when gc.AceGroupsRequired.Contains("nhs.clinicalADV"):
                                //_logger.Debug("Added Adv Para");
                                GarageMenuCats.Add(mi);
                                break;
                            
                            case UserDivision.NhsMananagement when gc.AceGroupsRequired.Contains("nhs.clinicalTL"):
                                //_logger.Debug("Added Clinical TL");
                                GarageMenuCats.Add(mi);
                                break;

                            case UserDivision.Survive when gc.AceGroupsRequired.Contains("group.survive"):
                                //_logger.Debug("Added Survive DLC");
                                GarageMenuCats.Add(mi);
                                break;

                            case UserDivision.blood when gc.AceGroupsRequired.Contains("don.blood"):
                                //_logger.Debug("Added Blood DLC");
                                GarageMenuCats.Add(mi);
                                break;
                            
                            case UserDivision.BeepDoctor when gc.AceGroupsRequired.Contains("don.beepDoc"):
                                //_logger.Debug("Added Beep Doctor");
                                GarageMenuCats.Add(mi);
                                break;
                        }
                        break;

                    case UserBranch.Fire:
                        switch (userRoles.Division)
                        {
                            case UserDivision.LFB when gc.AceGroupsRequired.Contains("Fire.Trained"):
                                //_logger.Debug("Added LFB");
                                GarageMenuCats.Add(mi);
                                break;

                            case UserDivision.CoastGuard when gc.AceGroupsRequired.Contains("coast.guard"):
                                //_logger.Debug("Added Coast Guard");
                                GarageMenuCats.Add(mi);
                                break;
                            case UserDivision.mountainRescue when gc.AceGroupsRequired.Contains("rescue.mountain"):
                                //_logger.Debug("Added Mountain Rescue");
                                GarageMenuCats.Add(mi);
                                break;
                            case UserDivision.FRU when gc.AceGroupsRequired.Contains("group.fru"):
                                //_logger.Debug("Added FRU");
                                GarageMenuCats.Add(mi);
                                break;
                        }
                        break;

                    case UserBranch.Control when gc.AceGroupsRequired.Any(s => s == "control.fim"):
                        GarageMenuCats.Add(mi);
                        break;

                    case UserBranch.Civ when gc.AceGroupsRequired.Any(s => s == "Civ.Trained"):
                        GarageMenuCats.Add(mi);
                        break;

                    case UserBranch.Highways when gc.AceGroupsRequired.Any(s => s == "HETO.Trained"):
                        GarageMenuCats.Add(mi);
                        break;
                }
            }
        }


        private void CreateDummyVehicle(Model model, GarageVehicles itemData)
        {
            var vehid = API.CreateVehicle((uint)model.Hash, DummyVehiclePosition.X, DummyVehiclePosition.Y,
                DummyVehiclePosition.Z, 0, false, false);
            var dummyVehicle = (Vehicle)Entity.FromHandle(vehid);
            dummyVehicle.CanBeVisiblyDamaged = false;
            dummyVehicle.Heading = DummyVehicleHeading;
            dummyVehicle.IsCollisionEnabled = false;
            dummyVehicle.LockStatus = VehicleLockStatus.CannotBeTriedToEnter;
            dummyVehicle.IsPositionFrozen = true;
            dummyVehicle.IsInvincible = true;
            dummyVehicle.Repair();
            dummyVehicle.Wash();

            if (itemData.LiveryNumber != -1)
            {
                API.SetVehicleLivery(dummyVehicle.Handle, itemData.LiveryNumber);
            }

            DummyVehicles.Add(dummyVehicle);
        }

        private async Task CreateVehicleAsync(Vector3 position, float heading, string model, GarageVehicles itemData)
        {
            try
            {
                var v = await World.CreateVehicle(model, new Vector3(position.X, position.Y, position.Z));
                v.Heading = heading;

                /*
                var entitySpawn = new EntitySpawnVehicle
                {
                    Name = model,
                    Position = new Core.Shared.Models.Vector3(position.X, position.Y, position.Z),
                    Heading = heading,
                    MissionEntity = true
                };

                var veh = await _comms.Request<int>(ServerEvents.RequestSpawnEntityVehicle, entitySpawn);

                if (veh == -1)
                {
                    _notifications.Error("Garage", "Problem with spawning vehicle.");
                    return;
                }

                var breaker = 500;
                while (!API.NetworkDoesEntityExistWithNetworkId(veh))
                {
                    if(breaker == 0) break;
                    breaker--;
                    await Delay(10);
                }
                if (!API.NetworkDoesEntityExistWithNetworkId(veh))
                {
                    _notifications.Error("Garage", "Problem with spawning vehicle. (timeout)");
                    return;
                }

                var v = (Vehicle)Entity.FromNetworkId(veh);

                */

                v.CanBeVisiblyDamaged = false;
                v.Repair();
                v.FuelLevel = 100f;
                v.Wash();

                var obj = await _comms.Request<int>(ServerEvents.GetSirenType, itemData.VehicleClass);

                if (obj != -1)
                {
                    if (v.ClassType != VehicleClass.Helicopters && v.ClassType != VehicleClass.Planes)
                    {
                        //_logger.Debug($"$Waiting for siren type for {itemData.VehicleClass}");
                        await _els.InitElsVehicle(v.Handle, obj);
                    }
                }

                if (itemData.LiveryNumber != -1)
                {
                    API.SetVehicleLivery(v.Handle, itemData.LiveryNumber);
                }

                API.TaskWarpPedIntoVehicle(Game.PlayerPed.Handle, v.Handle, (int)VehicleSeat.Driver);
                var b = v.AttachBlip();
                b.Sprite = BlipSprite.PersonalVehicleCar;
                b.Name = "Personal Vehicle";
                API.SetVehicleHasBeenOwnedByPlayer(v.Handle, true);
                v.CanBeVisiblyDamaged = true;
                v.IsPersistent = true;
                PersonalVehicleNetId = v.NetworkId;
                //_comms.ToServer(ServerEvents.OnGarageVehicleCreated, v.NetworkId);
            }
            catch (Exception ex)
            {
                _logger.Error($"Error: {ex}");
            }
        }

        private void ClearDummyVehicle()
        {
            if (DummyVehicles == null) return;
            if (!DummyVehicles.Any()) return;
            foreach (var dv in DummyVehicles)
            {
                if (!API.IsAnEntity(dv.Handle)) { continue; }
                dv.Delete();
            }
        }
    }
}