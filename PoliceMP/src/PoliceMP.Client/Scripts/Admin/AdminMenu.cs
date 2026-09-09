using CitizenFX.Core;
using CitizenFX.Core.Native;
using CitizenFX.Core.UI;
using MenuAPI;
using PoliceMP.Client.Extensions;
using PoliceMP.Client.Overlays.NewNotification;
using PoliceMP.Client.Scripts.Afk;
using PoliceMP.Client.Services;
using PoliceMP.Client.Services.Interfaces;
using PoliceMP.Core.Client;
using PoliceMP.Core.Client.Commands.Interfaces;
using PoliceMP.Core.Client.Communications.Interfaces;
using PoliceMP.Core.Client.Extensions;
using PoliceMP.Core.Client.Interface;
using PoliceMP.Core.Client.Scripts;
using PoliceMP.Core.Shared;
using PoliceMP.Core.Shared.Enums;
using PoliceMP.Shared.Constants;
using PoliceMP.Shared.Constants.Decors;
using PoliceMP.Shared.Constants.States;
using PoliceMP.Shared.Enums;
using PoliceMP.Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace PoliceMP.Client.Scripts.Admin
{
    public class AdminMenu : Script, IAdmin
    {
        #region Interfaces

        private readonly ICommandManager _commandManager;
        private readonly ILogger<AdminMenu> _logger;
        private readonly INewNotificationOverlay _newNotificationOverlay;
        private readonly IPermissionService _permissionService;
        private readonly ITickManager _tickManager;
        private readonly IPlayerService _playerService;
        private readonly ILegacyClientCommunicationsManager _comms;
        private readonly ICommonFunctionsService _common;
        private readonly IFeatureService _featureService;
        private bool _weatherFeatureEnabled = false;

        #endregion Interfaces

        #region Variables

        private UserAces _userAces;

        public static bool NoClipActive { get; set; } = false;

        public static string NoClipPublicSpeed;

        private int _prevVest;
        private int _prevVestText;

        private int MovingSpeed { get; set; } = 0;
        private int Scale { get; set; } = -1;
        private bool FollowCamMode { get; set; }
        private bool TempHideBlipState { get; set; }

        private readonly List<string> _noClipSpeeds = new()
        {
            "(1/8) Very Slow",
            "(2/8) Slow",
            "(3/8) Normal",
            "(4/8) Fast",
            "(5/8) Very Fast",
            "(6/8) Extremely Fast",
            "(7/8) Extremely Fast v2.0",
            "(8/8) Max Speed"
        };

        private readonly Menu _mainMenu = new("Mod Menu", "Moderation Menu");

        #endregion Variables

        public AdminMenu(ICommandManager commandManager, ILogger<AdminMenu> logger,
            INewNotificationOverlay newNotificationOverlay, IPermissionService permissionService, ITickManager tickManager,
            IPlayerService playerService, ILegacyClientCommunicationsManager comms, ICommonFunctionsService common, IFeatureService featureService)
        {
            _commandManager = commandManager;
            _logger = logger;
            _newNotificationOverlay = newNotificationOverlay;
            _permissionService = permissionService;
            _tickManager = tickManager;
            _playerService = playerService;
            _comms = comms;
            _common = common;
            _featureService = featureService;

            _comms.On<Vector3>(ClientEvents.AdminClearAreaAroundPosition, (position) =>
            {
                API.ClearAreaOfEverything(position.X, position.Y, position.Z, 100f, false, false, false, false);
                var vehicleList = World.GetAllVehicles();
                foreach (var vehicle in vehicleList)
                {
                    if (vehicle.Position.Distance(position) > 100) continue;
                    if (vehicle.HasDriver() && vehicle.Driver.IsPlayer) continue;
                    vehicle.Delete();
                }
            });
            API.RegisterKeyMapping("am", "Mod Menu", "keyboard", "F9");
            API.RegisterKeyMapping("noclip", "No Clip", "keyboard", "F10");
        }

        protected override async Task OnStartAsync()
        {
            _userAces = await _permissionService.GetUserAces();
            
            _commandManager.Register("am").WithHandler(async () => { await ShowAdminMenu(); });
            _commandManager.Register("noclip").WithHandler(async () =>
            {
                await ToggleNoClip();
            });

            _commandManager.Register("pos").WithHandler(() =>
            {
                _logger.Debug($"Player Position: X: {Game.PlayerPed.Position.X} Y: {Game.PlayerPed.Position.Y} Z: {Game.PlayerPed.Position.Z}");
                _logger.Debug($"Player Rotation: X: {Game.PlayerPed.Rotation.X} Y: {Game.PlayerPed.Rotation.Y} Z: {Game.PlayerPed.Rotation.Z}");
            });

            _commandManager.Register("modc").HasGreedyArgs().WithHandler((message) =>
            {
                try
                {
                    if (string.IsNullOrEmpty(message) || message == "" || message == string.Empty)
                    {
                        return;
                    }
                    
                    var modPlayer = Game.PlayerPed.Handle;
                    var modPlayerID = API.PlayerId();
                    var modPlayerName = API.GetPlayerName(modPlayerID);
                    
                    _comms.ToServer(ServerEvents.SendModMessageToServer, message, modPlayerName);
                }
                catch
                {
                    return;
                }
            });
            
            
            _commandManager.Register("mod").WithHandler(ModVestToggle);

            //_tickManager.On(AttachedTick);
            _tickManager.On(NoClipTickEvent);
            _tickManager.On(WeatherFeatureCheck);

            API.DecorRegister(VehicleDecors.AdminAttached, (int)DecorType.Int);

            _comms.On<int>(ClientEvents.AdminTeleportIntoVehicle, async vehicleNetworkId =>
            {
                var vehicle = (Vehicle)Entity.FromNetworkId(vehicleNetworkId);
                var vehicleTry = 0;
                while (vehicle == null && vehicleTry < 10)
                {
                    vehicleTry++;
                    vehicle = (Vehicle)Entity.FromNetworkId(vehicleNetworkId);
                    await Script.Delay(100);
                }

                if (vehicle == null)
                {
                    _newNotificationOverlay.SendNotification(new NewNotificationMessage("Teleport", "error", "Unable to teleport you into the vehicle. Try again!", new NewNotificationMessageContent[0]));
                    return;
                }

                Game.PlayerPed.Task.WarpIntoVehicle(vehicle, VehicleSeat.Any);
                _newNotificationOverlay.SendNotification(new NewNotificationMessage("Teleport", "success", "Teleported you into their vehicle", new NewNotificationMessageContent[0]));
            });
            _comms.On(ClientEvents.AdminSendKillEventToPlayer, () =>
            {
                API.AddExplosion(Game.PlayerPed.Position.X, Game.PlayerPed.Position.Y, Game.PlayerPed.Position.Z, 81, 0f,
                    true, false, 50f);
                API.SetEntityHealth(Game.PlayerPed.Handle, 0);
            });
            _comms.On(ClientEvents.AdminSendFreezeEventToPlayer, () =>
            {
                var localPlayer = Game.PlayerPed;

                localPlayer.IsPositionFrozen = !localPlayer.IsPositionFrozen;
            });
            _comms.On<int>(ClientEvents.AdminSendAttachEventToPlayer, (adminNetworkId) =>
            {
                var localPlayer = Game.PlayerPed;
                var adminPlayer = (Ped)Entity.FromNetworkId(adminNetworkId);

                if (localPlayer.IsInVehicle())
                {
                    var vehicle = localPlayer.CurrentVehicle;

                    if (vehicle.IsAttachedTo(adminPlayer))
                    {
                        vehicle.SetIntDecor(VehicleDecors.AdminAttached, 0);
                        vehicle.Detach();
                        return;
                    }

                    vehicle.SetIntDecor(VehicleDecors.AdminAttached, adminNetworkId);
                    vehicle.AttachTo(adminPlayer, new Vector3(2f, 2f, 1f));
                    return;
                }

                if (localPlayer.IsAttachedTo(adminPlayer))
                {
                    localPlayer.Detach();
                    return;
                }

                localPlayer.AttachTo(adminPlayer, new Vector3(1f, 1f, 0f));
            });
        }

        public bool IsNoClipActive()
        {
            return NoClipActive;
        }

        /*
        private async Task AttachedTick()
        {
            var localPlayer = Game.PlayerPed;
            var attachedEntity = localPlayer.GetEntityAttachedTo();
            if (attachedEntity == null) return;
            if (API.GetEntityType(attachedEntity.Handle) == 2)
            {
                //Vehicle
                var attachedVehicle = (Vehicle)attachedEntity;

                var hasAttachedDecor = attachedVehicle.HasDecor(VehicleDecors.AdminAttached);
                if (hasAttachedDecor) return;
                var attachedDecorNetId = attachedVehicle.GetIntDecor(VehicleDecors.AdminAttached);
                if (attachedDecorNetId == 0) return;
                if (attachedVehicle.HasDriver()) return;
                bool getControl = await attachedVehicle.TryRequestNetworkEntityControl();
                if (!getControl) return;
                attachedVehicle.Detach();
                return;
            }
        }
        */

        private void ShowPlayerListMenu(Menu parentMenu, RoutingBucket? bucket)
        {
            var playerListMenu = new Menu("Players", bucket.HasValue ? $"RoutingBucket {bucket}" : "All Players");

            playerListMenu.OnMenuOpen += async menu =>
            {
                //_newNotificationOverlay.SendNotification(new NewNotificationMessage("Players", "info", "Loading List of Players", new NewNotificationMessageContent[0]));

                playerListMenu.ClearMenuItems();
                var allPlayerInfo = await _playerService.FetchAllRecentPlayerInfo();

                if (bucket.HasValue)
                    allPlayerInfo = allPlayerInfo.Where(p => p.RoutingBucket == bucket).ToList();

                var ownNetworkId = Game.PlayerPed.NetworkId;

                allPlayerInfo = allPlayerInfo.OrderBy(o => o.Name).ToList();
                //allPlayerInfo.Sort((a, b) => string.Compare(a.Name, b.Name));

                foreach (var playerInfo in allPlayerInfo)
                {
                    if (playerInfo.NetworkId == ownNetworkId) continue;
                    var callSign = (string.IsNullOrEmpty(playerInfo.CallSign)) ? "NO CALLSIGN" : playerInfo.CallSign;
                    var bucket = playerInfo.RoutingBucket;
                    var menuItem = new MenuItem($"[{callSign}] {_common.TildeStripper(playerInfo.Name)}") { ItemData = playerInfo.NetworkId, Label = bucket.ToString() + "→→→" };
                    playerListMenu.AddMenuItem(menuItem);
                }

                //_newNotificationOverlay.SendNotification(new NewNotificationMessage("Players", "success", "Player List loaded!", new NewNotificationMessageContent[0]));

                playerListMenu.OnItemSelect += async (plListMenu, item, index) =>
                {
                    var networkId = (int)item.ItemData;
                    ShowPlayerMenu(playerListMenu, networkId);
                };
            };

            MenuController.AddSubmenu(parentMenu, playerListMenu);
            parentMenu.CloseMenu();
            playerListMenu.OpenMenu();
        }

        private async Task ShowPlayerMenu(Menu parentMenu, int networkId)
        {
            var playerData = _playerService.FetchCahcedPlayerInfoFromNetworkId(networkId);
            var bucket = await _comms.Request<RoutingBucket>(ServerEvents.AdminGetPlayerBucket, playerData.ServerHandle);

            if (playerData == null)
            {
                _logger.Error($"Player Data null for {networkId}");
                parentMenu.CloseMenu();
                return;
            }

            var playerMenu = new Menu(playerData.Name, $"Instance: {bucket}");
            MenuController.AddSubmenu(parentMenu, playerMenu);

            var teleportItem = new MenuItem("Teleport To", "Teleport to the player");
            var summonPlayer = new MenuItem("Summon Player", "Bring the player to you");
            var killPlayer = new MenuItem("Kill Player", "Send them the Hammer of Death");
            var freezePlayer = new MenuItem("Freeze Player", "Freeze or Unfreeze the Player");
            var joinBucket = new MenuItem("Join Instance", "BETA: Joins the player instance");
            var sendPopup = new MenuItem("Send Awareness Popup", "Tell the player a mod is speaking to them");
            var moveToBucket = new MenuItem("Move to instance", "BETA: Moves the player to another instance")
            {
                Label = "→→→"
            };
            var moveToMyBucket = new MenuItem("Move to current Instance", "BETA: Moves the player to your instance");
            var bindPlayer = new MenuItem("Attach Player", "BETA: Attach the player to you")
            {
                LeftIcon = MenuItem.Icon.WARNING
            };

            playerMenu.AddMenuItem(teleportItem);
            playerMenu.AddMenuItem(summonPlayer);
            playerMenu.AddMenuItem(killPlayer);
            playerMenu.AddMenuItem(freezePlayer);
            playerMenu.AddMenuItem(sendPopup);
            playerMenu.AddMenuItem(joinBucket);
            playerMenu.AddMenuItem(moveToBucket);
            playerMenu.AddMenuItem(moveToMyBucket);
            playerMenu.AddMenuItem(bindPlayer);

            //MenuController.AddSubmenu(parentMenu, playerMenu);

            parentMenu.CloseMenu();
            playerMenu.OpenMenu();

            //playerMenu.OnMenuClose += plMenu =>
            //{
            //    parentMenu.OpenMenu();
            //};

            playerMenu.OnItemSelect += async (plMenu, menuItem, itemIndex) =>
            {
                _logger.Debug($"Selected {menuItem.Text}");
                var targetPed = (Ped)Entity.FromNetworkId(playerData.NetworkId);

                #region Teleport Item

                if (menuItem == teleportItem)
                {
                    _comms.ToServer(ServerEvents.AdminTeleportPlayerToPlayer, playerData.NetworkId);
                    if (targetPed == null)
                    {
                        Game.PlayerPed.Position = playerData.Position.ToCitizenVector3();
                        _newNotificationOverlay.SendNotification(new NewNotificationMessage("Teleport", "info", "Teleporting you now. We are trying to get you closer!", new NewNotificationMessageContent[0]));

                        var tryCount = 0;
                        while (targetPed == null && tryCount < 10)
                        {
                            tryCount++;
                            targetPed = (Ped)Entity.FromNetworkId(playerData.NetworkId);
                            await Script.Delay(10);
                        }
                    }

                    if (targetPed == null)
                    {
                        _newNotificationOverlay.SendNotification(new NewNotificationMessage("Teleport", "error", "Unable to get you any closer! Try again!", new NewNotificationMessageContent[0]));
                        return;
                    }

                    var currentVehicle = targetPed.CurrentVehicle;
                    if (currentVehicle == null)
                    {
                        Game.PlayerPed.Position = targetPed.Position;
                        _newNotificationOverlay.SendNotification(new NewNotificationMessage("Teleport", "success", $"Teleported you to {playerData.Name}!", new NewNotificationMessageContent[0]));
                        return;
                    }

                    Game.PlayerPed.Task.WarpIntoVehicle(currentVehicle, VehicleSeat.Any);
                    _newNotificationOverlay.SendNotification(new NewNotificationMessage("Teleport", "success", $"Teleported you to {playerData.Name} and put you in their vehicle!", new NewNotificationMessageContent[0]));
                    return;
                }

                #endregion Teleport Item

                #region Summon Item

                if (menuItem == summonPlayer)
                {
                    _comms.ToServer(ServerEvents.AdminSummonPlayerToPlayer, playerData.NetworkId);
                    return;
                }

                #endregion Summon Item

                #region Kill Player

                if (menuItem == killPlayer)
                {
                    _comms.ToServer(ServerEvents.AdminKillPlayer, playerData.NetworkId);
                    return;
                }

                #endregion Kill Player

                #region Freeze Player

                if (menuItem == freezePlayer)
                {
                    _comms.ToServer(ServerEvents.AdminSendFreezeEventToServer, playerData.NetworkId);
                    return;
                }

                #endregion Freeze Player

                #region Send Popup

                if (menuItem == sendPopup)
                {
                    var targetPlayerName = playerData.Name;
                    API.ExecuteCommand($":smpu \"{targetPlayerName}\"");
                    _newNotificationOverlay.SendNotification(new NewNotificationMessage("Send Popup", "success", $"You have sent a Moderator Notice popup to {targetPlayerName}! They will now be notified that you are attempting to speak with them.", new NewNotificationMessageContent[0]));
                    return;
                }

                #endregion Send Popup

                #region Buckets

                if (menuItem == joinBucket)
                {
                    var bucket = await _comms.Request<RoutingBucket>(ServerEvents.AdminGetPlayerBucket, playerData.ServerHandle);
                    _logger.Debug($"Joining bucket {bucket}");
                    _comms.ToServer(ServerEvents.AdminJoinBucket, bucket);
                }

                if (menuItem == moveToBucket)
                {
                    var bucket = await ShowRoutingBucketMenu(playerMenu);
                    if (bucket.HasValue)
                    {
                        _logger.Debug($"Moving {playerData.Name} to bucket {bucket}");
                        _comms.ToServer(ServerEvents.AdminMovePlayerToBucket, playerData.ServerHandle, bucket);
                    }
                }

                if (menuItem == moveToMyBucket)
                {
                    var myInfo = _playerService.FetchCahcedPlayerInfoFromNetworkId(Game.PlayerPed.NetworkId);
                    _comms.ToServer(ServerEvents.AdminMovePlayerToBucket, playerData.ServerHandle, myInfo.RoutingBucket);
                }

                #endregion Buckets

                #region Attach Player

                if (menuItem == bindPlayer)
                {
                    _comms.ToServer(ServerEvents.AdminAttachPedToServer, playerData.NetworkId);
                    return;
                }

                #endregion Attach Player

                playerData = await _playerService.FetchPlayerInfoFromNetworkId(playerData.NetworkId);
            };
        }

        private async Task<RoutingBucket?> ShowRoutingBucketMenu(Menu parentMenu)
        {
            RoutingBucket? result = null;
            var bucketValues = (RoutingBucket[])Enum.GetValues(typeof(RoutingBucket));
            var bucketCount = bucketValues.Length;
            var bucketsMenu = new Menu("Instances", "Manage instances");

            bool movePlayersFrom = false;
            RoutingBucket movePlayersSource = (RoutingBucket)(-1);
            RoutingBucket movePlayersDest = (RoutingBucket)(-1);

            MenuItem[] bucketsMenuItems = new MenuItem[bucketCount];
            Menu[] bucketsSubMenus = new Menu[bucketCount];

            for (int i = 0; i < bucketCount; i++)
            {
                _logger.Debug($"RoutingBucket {i}");
                var bucket = bucketValues[i];
                var attribute = bucket.GetCustomAttribute<RoutingBucketAttribute>();
                var text = attribute?.Name ?? Enum.GetName(typeof(RoutingBucket), bucket);
                //text = int.TryParse(text, out var _) ? $"Instance {text}" : $"{text} Instance";

                bucketsMenuItems[i] = new MenuItem(text) { ItemData = (RoutingBucket)i };
                bucketsSubMenus[i] = new Menu(text);

                bucketsMenu.AddMenuItem(bucketsMenuItems[i]);

                //    // Seems odd but this is to capture the index, otherwise C# would change it in the for loop
                //    // We take a copy of i and keep it stored
                //    var iCapture = i;
                //    bucketsSubMenus[iCapture].OnMenuOpen += async _ =>
                //    {
                //
                //    };
            }

            bool isComplete = false;
            bucketsMenu.OnItemSelect += (menu, item, index) =>
            {
                result = item.ItemData;
                isComplete = true;
            };

            bucketsMenu.OnMenuClose += menu =>
            {
                isComplete = true;
            };

            MenuController.AddSubmenu(parentMenu, bucketsMenu);
            parentMenu.CloseMenu();
            bucketsMenu.OpenMenu();

            while (!isComplete)
            {
                await Delay(10);
            }

            bucketsMenu.CloseMenu();
            parentMenu.OpenMenu();

            return result;
        }

        private async Task ShowBucketOptionsMenu(Menu parentMenu, RoutingBucket bucket)
        {
            var bucketMenu = new Menu(bucket.ToString(), $"Managing instance {bucket}");

            var allPlayers = await _playerService.FetchAllRecentPlayerInfo();
            var count = allPlayers.Count(p => p.RoutingBucket == bucket);

            var playersBucketItem = new MenuItem($"Players ({count})")
            {
                Enabled = count > 0
            };

            var enterBucketItem = new MenuItem("Join Instance");
            var moveAllPlayersToItem = new MenuItem("Move all players to...");
            var moveAllPlayersFromItem = new MenuItem("Move all players from...");

            bucketMenu.AddMenuItem(playersBucketItem);
            bucketMenu.AddMenuItem(enterBucketItem);
            bucketMenu.AddMenuItem(moveAllPlayersToItem);
            bucketMenu.AddMenuItem(moveAllPlayersFromItem);

            bucketMenu.OnItemSelect += async (menu, item, index) =>
            {
                if (item == playersBucketItem)
                    ShowPlayerListMenu(bucketMenu, bucket);

                if (item == enterBucketItem)
                {
                    _comms.ToServer(ServerEvents.AdminJoinBucket, bucket);

                    bucketMenu.CloseMenu();
                }

                if (item == moveAllPlayersToItem)
                {
                    var sourceBucket = bucket;
                    var destBucket = await ShowRoutingBucketMenu(bucketMenu);
                    if (destBucket == null) return;

                    var movePlayers = (await _playerService.FetchAllRecentPlayerInfo())
                        .Where(p => p.RoutingBucket == sourceBucket)
                        .Select(p => p.ServerHandle);

                    _comms.ToServer(ServerEvents.AdminMoveAllPlayersToBucket, movePlayers, destBucket);
                    bucketMenu.CloseMenu();
                }

                if (item == moveAllPlayersFromItem)
                {
                    var sourceBucket = await ShowRoutingBucketMenu(bucketMenu);
                    if (sourceBucket == null) return;

                    var destBucket = bucket;
                    var movePlayers = (await _playerService.FetchAllRecentPlayerInfo())
                        .Where(p => p.RoutingBucket == sourceBucket)
                        .Select(p => p.ServerHandle);

                    _comms.ToServer(ServerEvents.AdminMoveAllPlayersToBucket, movePlayers, destBucket);
                    bucketMenu.CloseMenu();
                }
            };

            MenuController.AddSubmenu(parentMenu, bucketMenu);
            parentMenu.CloseMenu();
            bucketMenu.OpenMenu();
        }

        private async Task ShowAdminMenu()
        {
            var isProduction = _featureService.IsFeatureEnabled(FeatureToggle.IsProduction);
            if (isProduction) { if (!_userAces.IsAdmin && !_userAces.IsDeveloper && !_userAces.IsModerator && !_userAces.IsDigitalTeam) return; }
            else if (!_userAces.IsAdmin && !_userAces.IsDeveloper && !_userAces.IsModerator && !_userAces.IsDigitalTeam && !_userAces.IsTierTwo) return;

            #region Main Menu

            MenuController.EnableMenuToggleKeyOnController = false;
            MenuController.MenuToggleKey = (Control)(-1);

            _mainMenu.ClearMenuItems();

            var toggleModVest = new MenuItem("Toggle Mod Vest", "Toggles the Moderator vest");
            var sendModChat = new MenuItem("Send Mod Chat", "Sends chat to other mods");
            var playerListItem = new MenuItem("Players", "Manage Players");
            var noClipItem = new MenuItem("Toggle NoClip", "Become God");
            var clearAreaItem = new MenuItem("Clear Area", "Clear your local area");
            var giveTaserItem = new MenuItem("Give Taser", "Give yourself the Taser");
            var weatherItem = new MenuItem("Change Weather", "Set the Weather for 1 hour");
            var timeItem = new MenuItem("Change Time", "Adjust the game time");
            var bucketsItem = new MenuItem("Instances", "BETA: Manage instances");

            if (_userAces.IsAdmin || _userAces.IsDeveloper || _userAces.IsModerator || _userAces.IsTierTwo)
            {
                _mainMenu.AddMenuItem(toggleModVest);
                _mainMenu.AddMenuItem(sendModChat);
                _mainMenu.AddMenuItem(playerListItem);
                _mainMenu.AddMenuItem(noClipItem);
                _mainMenu.AddMenuItem(clearAreaItem);
                _mainMenu.AddMenuItem(giveTaserItem);
                _mainMenu.AddMenuItem(bucketsItem);
            }

            if (_userAces.IsAdmin || _userAces.IsDeveloper || _userAces.IsTierTwo || (_userAces.IsDigitalTeam && _weatherFeatureEnabled))
            {
                _mainMenu.AddMenuItem(weatherItem);
                _mainMenu.AddMenuItem(timeItem);
            }
            
            var isEaServer = _featureService.IsFeatureEnabled(FeatureToggle.IsEarlyAccess);
            var isDevServer = _featureService.IsFeatureEnabled(FeatureToggle.IsDevelopment);

            if (isDevServer && _userAces.IsDigitalTeam || isEaServer && _userAces.IsDigitalTeam)
            {
                _mainMenu.AddMenuItem(noClipItem);
                _mainMenu.AddMenuItem(clearAreaItem);
            }

            MenuController.AddMenu(_mainMenu);

            _mainMenu.OnItemSelect += async (menu, item, index) =>
            {
                if (item == toggleModVest)
                {
                    API.ExecuteCommand("mod");
                }
                
                if (item == sendModChat)
                {
                    OnNewModChat();
                }
                
                if (item == playerListItem)
                {
                    ShowPlayerListMenu(_mainMenu, null);
                }

                if (item == noClipItem)
                {
                    await ToggleNoClip();
                }

                if (item == clearAreaItem)
                {
                    _comms.ToServer(ServerEvents.ClearAreaOfPlayer);
                }

                if (item == giveTaserItem)
                {
                    Game.PlayerPed.Weapons.Give(WeaponHash.StunGun, 5, true, true);
                }

                if (item == bucketsItem)
                {
                    var selectedBucket = await ShowRoutingBucketMenu(_mainMenu);
                    if (selectedBucket.HasValue)
                    {
                        ShowBucketOptionsMenu(_mainMenu, selectedBucket.Value);
                    }
                }
            };

            #endregion Main Menu



            #region Weather Menu

            var weatherMenu = new Menu("Weather", "Adjust the weather for an hour");

            if (_userAces.IsAdmin || _userAces.IsDeveloper || _userAces.IsTierTwo || (_userAces.IsDigitalTeam && _weatherFeatureEnabled))
            {
                MenuController.BindMenuItem(_mainMenu, weatherMenu, weatherItem);

                foreach (var weather in Enum.GetNames(typeof(WeatherType)))
                {
                    var menuItem = new MenuItem(weather)
                    {
                        ItemData = weather
                    };
                    weatherMenu.AddMenuItem(menuItem);
                }

                weatherMenu.OnItemSelect += async (menu, item, index) =>
                {
                    var itemData = item.ItemData;
                    _logger.Debug($"Selected Weather: {itemData}");
                    _comms.ToServer(ServerEvents.ForceSetWeatherToServer, itemData);
                    return;
                };
            }

            #endregion Weather Menu

            #region Time Menu

            var timeMenu = new Menu("Set Time", "Adjust the game time");

            MenuController.BindMenuItem(_mainMenu, timeMenu, timeItem);

            if (_userAces.IsAdmin || _userAces.IsDeveloper || _userAces.IsTierTwo || (_userAces.IsDigitalTeam && _weatherFeatureEnabled))
            {
                var resetTimeItem = new MenuItem("Restart Time Cycle", "Restarts the in game time cycle")
                {
                    ItemData = "RESETTIME",
                    LeftIcon = MenuItem.Icon.WARNING
                };
                var freezeTimeItem = new MenuItem("Freeze the Time Cycle", "Freezes the Time changes")
                {
                    ItemData = "FREEZETIME"
                };

                var hourList = new List<string>();
                for (int i = 0; i <= 24; i++)
                {
                    hourList.Add(i.ToString());
                }
                var hourListItem = new MenuListItem("Hour", hourList, 0);

                var minuteList = new List<string>();
                for (int i = 0; i <= 59; i++)
                {
                    minuteList.Add(i.ToString());
                }
                var minuteListItem = new MenuListItem("Minute", minuteList, 0);

                var setTimeItem = new MenuItem("Set Time", "Sets the time from the above options")
                {
                    ItemData = "SETTIME"
                };

                timeMenu.AddMenuItem(resetTimeItem);
                timeMenu.AddMenuItem(freezeTimeItem);
                timeMenu.AddMenuItem(hourListItem);
                timeMenu.AddMenuItem(minuteListItem);
                timeMenu.AddMenuItem(setTimeItem);

                timeMenu.OnItemSelect += (menu, item, index) =>
                {
                    var selectedHour = hourListItem.GetCurrentSelection();
                    var selectedMinute = minuteListItem.GetCurrentSelection();
                    _logger.Debug($"Selected Time: {selectedHour}:{selectedMinute}");
                    _comms.ToServer(ServerEvents.AdminManageServerTime, item.ItemData, selectedHour, selectedMinute);
                };
            }

            #endregion Time Menu

            #region Buckets

            //MenuController.BindMenuItem(MainMenu, bucketsMenu, bucketsItem);

            //MenuItem pcsoQuietMenuEnableItem = null;

            //bucketsMenu.OnMenuOpen += async menu =>
            //{
            //    bucketsMenu.ClearMenuItems();

            //    for (int i = 0; i < 99; i++)
            //    {
            //        bucketsMenu.AddMenuItem(new MenuItem(text, $"{count} players in bucket"));
            //    }
            //}

            //var buckets = await _comms.Request<string[]>(ServerEvents.AdminListBucket);
            //foreach (var bucket in buckets)
            //{
            //    pcsoQuietMenu.AddMenuItem(new MenuItem($"Join {bucket} RoutingBucket")
            //    {
            //        ItemData = bucket
            //    });
            //}

            //pcsoQuietMenu.OnItemSelect += (menu, item, index) =>
            //{
            //    if (item == pcsoQuietMenuEnableItem)
            //    {
            //        _comms.ToServer(ServerEvents.AdminSetPcsoQuiet, item.ItemData);
            //        pcsoQuietEnabled = item.ItemData;
            //        item.Text = pcsoQuietEnabled ? "Disable" : "Enable";
            //        item.ItemData = !pcsoQuietEnabled;
            //    }
            //    else if (item.Text.StartsWith("Join "))
            //    {
            //        _comms.ToServer(ServerEvents.AdminJoinBucket, item.ItemData);
            //    }
            //};

            #endregion Buckets

            _mainMenu.OpenMenu();
        }

        private async Task WeatherFeatureCheck()
        {
            _weatherFeatureEnabled = _featureService.IsFeatureEnabled(FeatureToggle.AllowMediaWeatherChange);
            await Delay(30000);
        }

        private async void OnNewModChat()
        {
            var modPlayer = Game.PlayerPed.Handle;
            var modPlayerID = API.PlayerId();
            var modPlayerName = API.GetPlayerName(modPlayerID);
            
            API.AddTextEntry("FMMC_KEY_TIP1", "Enter short message to Mods");
            API.DisplayOnscreenKeyboard(0, "FMMC_KEY_TIP1", "", "", "", "", "", 60);

            API.UpdateOnscreenKeyboard();

            while (API.UpdateOnscreenKeyboard() == 0)
            {
                API.DisableControlAction(0, (int)Control.MpTextChatAll, true); // stop text chat input :)
                await Delay(10);
                API.UpdateOnscreenKeyboard();
            }
            
            API.EnableControlAction(0, (int)Control.MpTextChatAll, true);
            
            var newMessage = API.GetOnscreenKeyboardResult();
            _comms.ToServer(ServerEvents.SendModMessageToServer, newMessage, modPlayerName);
        }
        
        
        private async void ModVestToggle()
        {
            if (!_userAces.IsModerator)
            {
                _newNotificationOverlay.SendNotification(new NewNotificationMessage("Moderator Command", "error", $"Hmmmm... Something smells off! I have checked the database but I can't seem to find you as a moderator!", new NewNotificationMessageContent[0]));
                return;
            }

            var modVestIndex = 74;
            var modVestTexture = 2;
            var player = Game.PlayerPed.Handle;

            if (API.GetPedDrawableVariation(player, 9) != modVestIndex)
            {
                _prevVest = API.GetPedDrawableVariation(player, 9);
                _prevVestText = API.GetPedTextureVariation(player, 9);
                API.SetPedComponentVariation(player, 9, modVestIndex, modVestTexture, 0);
                _newNotificationOverlay.SendNotification(new NewNotificationMessage("Moderator Vest", "success", $"Abra-Ca-Dabra! You have magic'd your Moderator vest!", new NewNotificationMessageContent[0]));
                return;
            }

            API.SetPedComponentVariation(player, 9, _prevVest, _prevVestText, 0);
            _newNotificationOverlay.SendNotification(new NewNotificationMessage("Moderator Vest", "success", $"Bravo-6 going dark! You have hidden your Moderator vest! Good luck out there!", new NewNotificationMessageContent[0]));
        }


        private async Task ToggleNoClip()
        {
            var isProduction = _featureService.IsFeatureEnabled(FeatureToggle.IsProduction);
            if (isProduction) { if (!_userAces.IsAdmin && !_userAces.IsDeveloper && !_userAces.IsModerator) return; }
            else if (!_userAces.IsAdmin && !_userAces.IsDeveloper && !_userAces.IsModerator && !_userAces.IsDigitalTeam && !_userAces.IsTierTwo) return;

            if (!NoClipActive)
            {
                TempHideBlipState = Game.Player.State.Get<bool>(PlayerStates.HideBlipState);
            }

            NoClipActive = !NoClipActive;

            Game.Player.State.Set<bool>(PlayerStates.HideBlipState, NoClipActive || TempHideBlipState);
        }

        private async Task NoClipTickEvent()
        {
            if (NoClipActive)
            {
                Scale = API.RequestScaleformMovie("INSTRUCTIONAL_BUTTONS");
                while (!API.HasScaleformMovieLoaded(Scale))
                {
                    await Script.Delay(0);
                }
            }

            while (NoClipActive)
            {
                NoClipPublicSpeed = _noClipSpeeds[MovingSpeed];

                if (!API.IsHudHidden())
                {
                    API.BeginScaleformMovieMethod(Scale, "CLEAR_ALL");
                    API.EndScaleformMovieMethod();

                    API.BeginScaleformMovieMethod(Scale, "SET_DATA_SLOT");
                    API.ScaleformMovieMethodAddParamInt(0);
                    API.PushScaleformMovieMethodParameterString("~INPUT_SPRINT~");
                    API.PushScaleformMovieMethodParameterString($"Change Speed ({_noClipSpeeds[MovingSpeed]})");
                    API.EndScaleformMovieMethod();

                    API.BeginScaleformMovieMethod(Scale, "SET_DATA_SLOT");
                    API.ScaleformMovieMethodAddParamInt(1);
                    API.PushScaleformMovieMethodParameterString("~INPUT_MOVE_LR~");
                    API.PushScaleformMovieMethodParameterString($"Turn Left/Right");
                    API.EndScaleformMovieMethod();

                    API.BeginScaleformMovieMethod(Scale, "SET_DATA_SLOT");
                    API.ScaleformMovieMethodAddParamInt(2);
                    API.PushScaleformMovieMethodParameterString("~INPUT_MOVE_UD~");
                    API.PushScaleformMovieMethodParameterString($"Move");
                    API.EndScaleformMovieMethod();

                    API.BeginScaleformMovieMethod(Scale, "SET_DATA_SLOT");
                    API.ScaleformMovieMethodAddParamInt(3);
                    API.PushScaleformMovieMethodParameterString("~INPUT_MULTIPLAYER_INFO~");
                    API.PushScaleformMovieMethodParameterString($"Down");
                    API.EndScaleformMovieMethod();

                    API.BeginScaleformMovieMethod(Scale, "SET_DATA_SLOT");
                    API.ScaleformMovieMethodAddParamInt(4);
                    API.PushScaleformMovieMethodParameterString("~INPUT_COVER~");
                    API.PushScaleformMovieMethodParameterString($"Up");
                    API.EndScaleformMovieMethod();

                    API.BeginScaleformMovieMethod(Scale, "SET_DATA_SLOT");
                    API.ScaleformMovieMethodAddParamInt(5);
                    API.PushScaleformMovieMethodParameterString("~INPUT_VEH_HEADLIGHT~");
                    API.PushScaleformMovieMethodParameterString($"Cam Mode");
                    API.EndScaleformMovieMethod();

                    API.BeginScaleformMovieMethod(Scale, "SET_DATA_SLOT");
                    API.ScaleformMovieMethodAddParamInt(6);
                    API.PushScaleformMovieMethodParameterString(
                        API.GetControlInstructionalButton(0, (int)Control.DropAmmo, 1));
                    API.PushScaleformMovieMethodParameterString($"Toggle NoClip");
                    API.EndScaleformMovieMethod();

                    API.BeginScaleformMovieMethod(Scale, "DRAW_INSTRUCTIONAL_BUTTONS");
                    API.ScaleformMovieMethodAddParamInt(0);
                    API.EndScaleformMovieMethod();

                    API.DrawScaleformMovieFullscreen(Scale, 255, 255, 255, 255, 0);
                }

                var noclipEntity = Game.PlayerPed.IsInVehicle()
                    ? Game.PlayerPed.CurrentVehicle.Handle
                    : Game.PlayerPed.Handle;

                API.FreezeEntityPosition(noclipEntity, true);
                API.SetEntityInvincible(noclipEntity, true);

                Vector3 newPos;
                Game.DisableControlThisFrame(0, Control.MoveUpOnly);
                Game.DisableControlThisFrame(0, Control.MoveUp);
                Game.DisableControlThisFrame(0, Control.MoveUpDown);
                Game.DisableControlThisFrame(0, Control.MoveDown);
                Game.DisableControlThisFrame(0, Control.MoveDownOnly);
                Game.DisableControlThisFrame(0, Control.MoveLeft);
                Game.DisableControlThisFrame(0, Control.MoveLeftOnly);
                Game.DisableControlThisFrame(0, Control.MoveLeftRight);
                Game.DisableControlThisFrame(0, Control.MoveRight);
                Game.DisableControlThisFrame(0, Control.MoveRightOnly);
                Game.DisableControlThisFrame(0, Control.Cover);
                Game.DisableControlThisFrame(0, Control.MultiplayerInfo);
                Game.DisableControlThisFrame(0, Control.VehicleHeadlight);
                if (Game.PlayerPed.IsInVehicle())
                    Game.DisableControlThisFrame(0, Control.VehicleRadioWheel);

                var yoff = 0.0f;
                var zoff = 0.0f;

                if (Game.CurrentInputMode == InputMode.MouseAndKeyboard && API.UpdateOnscreenKeyboard() != 0 &&
                    !Game.IsPaused)
                {
                    if (Game.IsControlJustPressed(0, Control.Sprint))
                    {
                        MovingSpeed++;
                        if (MovingSpeed == _noClipSpeeds.Count)
                        {
                            MovingSpeed = 0;
                        }
                    }

                    if (Game.IsDisabledControlPressed(0, Control.MoveUpOnly))
                    {
                        yoff = 0.5f;
                    }

                    if (Game.IsDisabledControlPressed(0, Control.MoveDownOnly))
                    {
                        yoff = -0.5f;
                    }

                    if (!FollowCamMode && Game.IsDisabledControlPressed(0, Control.MoveLeftOnly))
                    {
                        API.SetEntityHeading(Game.PlayerPed.Handle, API.GetEntityHeading(Game.PlayerPed.Handle) + 3f);
                    }

                    if (!FollowCamMode && Game.IsDisabledControlPressed(0, Control.MoveRightOnly))
                    {
                        API.SetEntityHeading(Game.PlayerPed.Handle, API.GetEntityHeading(Game.PlayerPed.Handle) - 3f);
                    }

                    if (Game.IsDisabledControlPressed(0, Control.Cover))
                    {
                        zoff = 0.21f;
                    }

                    if (Game.IsDisabledControlPressed(0, Control.MultiplayerInfo))
                    {
                        zoff = -0.21f;
                    }

                    if (Game.IsDisabledControlJustPressed(0, Control.VehicleHeadlight))
                    {
                        FollowCamMode = !FollowCamMode;
                    }
                }

                float moveSpeed = (float)MovingSpeed;
                if (MovingSpeed > _noClipSpeeds.Count / 2)
                {
                    moveSpeed *= 1.8f;
                }

                moveSpeed = moveSpeed / (1f / API.GetFrameTime()) * 60;
                newPos = API.GetOffsetFromEntityInWorldCoords(noclipEntity, 0f, yoff * (moveSpeed + 0.3f),
                    zoff * (moveSpeed + 0.3f));

                var heading = API.GetEntityHeading(noclipEntity);
                API.SetEntityVelocity(noclipEntity, 0f, 0f, 0f);
                API.SetEntityRotation(noclipEntity, 0f, 0f, 0f, 0, false);
                API.SetEntityHeading(noclipEntity, FollowCamMode ? API.GetGameplayCamRelativeHeading() : heading);
                API.SetEntityCollision(noclipEntity, false, false);
                API.SetEntityCoordsNoOffset(noclipEntity, newPos.X, newPos.Y, newPos.Z, true, true, true);

                API.SetEntityVisible(noclipEntity, false, false);
                API.SetLocalPlayerVisibleLocally(true);
                API.SetEntityAlpha(noclipEntity, (int)(255 * 0.2), 0);

                API.SetEveryoneIgnorePlayer(Game.PlayerPed.Handle, true);
                API.SetPoliceIgnorePlayer(Game.PlayerPed.Handle, true);

                // After the next game tick, reset the entity properties.
                await Delay(0);
                API.FreezeEntityPosition(noclipEntity, false);
                API.SetEntityInvincible(noclipEntity, false);
                API.SetEntityCollision(noclipEntity, true, true);

                // If the player is not set as invisible by PlayerOptions or if the noclip entity is not the player ped, reset the visibility
                if (Game.PlayerPed.IsVisible || (!Game.PlayerPed.IsVisible && noclipEntity == Game.PlayerPed.Handle))
                {
                    API.SetEntityVisible(noclipEntity, true, false);
                    API.SetLocalPlayerVisibleLocally(true);
                }

                // Always reset the alpha.
                API.ResetEntityAlpha(noclipEntity);

                API.SetEveryoneIgnorePlayer(Game.PlayerPed.Handle, false);
                API.SetPoliceIgnorePlayer(Game.PlayerPed.Handle, false);
            }

            await Task.FromResult(0);
        }
    }
}