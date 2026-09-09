using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using CitizenFX.Core;
using CitizenFX.Core.Native;
using CitizenFX.Core.UI;
using MenuAPI;
using Newtonsoft.Json;
using PoliceMP.Client.Extensions;
using PoliceMP.Client.Overlays.Interaction;
using PoliceMP.Client.Overlays.NewNotification;
using PoliceMP.Client.Scripts.Sound;
using PoliceMP.Client.Services;
using PoliceMP.Client.Services.Interfaces;
using PoliceMP.Core.Client;
using PoliceMP.Core.Client.Commands.Interfaces;
using PoliceMP.Core.Client.Communications.Interfaces;
using PoliceMP.Core.Client.Extensions;
using PoliceMP.Core.Client.Interface;
using PoliceMP.Core.Client.Scripts;
using PoliceMP.Core.Shared;
using PoliceMP.Core.Shared.Communications.Interfaces;
using PoliceMP.Shared.Constants;
using PoliceMP.Shared.Constants.Decors;
using PoliceMP.Shared.Enums;
using PoliceMP.Shared.Models;

namespace PoliceMP.Client.Scripts.Backup
{
    public class BackupMenu : Script
    {
        private readonly IFiveEventManager _fiveEvents;
        private readonly ILegacyClientCommunicationsManager _comms;
        private readonly ILogger<BackupMenu> _logger;
        private readonly ITickManager _ticks;
        private readonly ICommandManager _command;
        private readonly IPermissionService _permissionService;
        private readonly IPlayerService _playerService;
        private readonly ICommonFunctionsService _common;
        private readonly ISoundHandler _soundHandler;
        private readonly INewNotificationOverlay _newNotificationOverlay;
        private bool _hasModPerms = false;
        private DateTime lastTime;

        private static readonly bool _hasSpawned = false;

        private enum MainMenuItem
        {
            CancelBackup,
            TrackPlayer,
            RequestBackup,
            PanicButton,
            TrackPanicRequest,
            TrackBackupRequest
        }

        private interface IMdtBackupRequest
        {
            string Branch { get; }
            string Division { get; }
        }

        private bool _friendlyEnabled = false;

        private UserAces _userAces = null;

        private bool _cooldown = false;

        private readonly int _cooldownTimeout = 10000;

        private int _currentlyTrackingNetworkId;

        private BackupRequest _activeBackupResponse;

        private readonly List<BackupRequest> _panicRequests = new();

        private readonly List<BackupRequest> _backupRequests = new();

        private List<Func<Task>> _clearBackupRequests = new();

        private bool CanSkipCooldown => _userAces.IsDeveloper || _userAces.IsModerator || _userAces.IsAdmin;

        private static readonly Dictionary<BackupType, BackupItemAttribute> BackupTypes = Enum
            .GetValues(typeof(BackupType))
            .Cast<BackupType>().ToArray()
            .ToDictionary(
                t => t,
                t => t.GetCustomAttribute<BackupItemAttribute>()
            );

        private static readonly List<MenuItem> BackupMenuItems = BackupTypes
            .Where(t => t.Value != null)
            .Select(t => t.Value != null
                ? new MenuItem(t.Value.Text)
                {
                    Label = "→",
                    ItemData = t.Key,
                    Description = t.Value.Description
                }
                : null).ToList();

        #region Menus

        private readonly Menu _rootMenu = new("Response Menu", "Main Menu");
        private readonly Menu _backupMenu = new("Request Backup", "Request Backup from Other Units");
        private readonly Menu _trackMenu = new("Track Players", "Select a unit to track");
        private readonly Menu _panicMenu = new("Active Panics", "Active panic buttons");
        private readonly Menu _backupRequestMenu = new("Active Backups", "Active backup requests");

        #endregion Menus

        public BackupMenu(IFiveEventManager fiveEvents, ILegacyClientCommunicationsManager comms,
            ILogger<BackupMenu> logger, ITickManager ticks, ICommandManager command,
            IPermissionService permissionService, IPlayerService playerService, ICommonFunctionsService common,
            ISoundHandler soundHandler, INewNotificationOverlay newNotificationOverlay)
        {
            _fiveEvents = fiveEvents;
            _comms = comms;
            _logger = logger;
            _ticks = ticks;
            _command = command;
            _permissionService = permissionService;
            _playerService = playerService;
            _common = common;
            _soundHandler = soundHandler;
            _newNotificationOverlay = newNotificationOverlay;

            // listen to external MDTBackupRequest:
            // _fiveEvents.On("MDTBackupRequest", new Func<IMdtBackupRequest, Task>(MdtBackupRequest));
        }

        protected override async Task OnStartAsync()
        {
            await Delay(0);
            _ticks.On(AdminHandlerTick);
            _ticks.On(TrackPlayerTick);

            _comms.On<bool>(ClientEvents.PlayerSpawned, HandlePlayerSpawned);
            _comms.On<BackupRequest, int>(ClientEvents.ReceiveBackupRequest, OnReceiveBackupRequestAsync);
            _comms.On<string>(ServerEvents.SendLFBMDTSoundToPlayers, OnReceiveLfbMdtSoundRequest);

            MenuController.MenuAlignment = MenuController.MenuAlignmentOption.Right;
            MenuController.EnableMenuToggleKeyOnController = false;
            MenuController.MenuToggleKey = (Control)(-1);
            MenuController.AddMenu(_rootMenu);

            _panicMenu.OnItemSelect += HandlePanicClick;
            _backupRequestMenu.OnItemSelect += HandleBackupRequestClick;
            _rootMenu.OnItemSelect += OnRootMenuOnItemSelect;
            _trackMenu.OnItemSelect += OnTrackMenuSelect;
            _backupMenu.OnItemSelect += OnBackupMenuSelect;

            foreach (var backupItem in BackupTypes
                         .Where(backupItem => backupItem.Value?.Command != null))
                _command.Register(backupItem.Value.Command).WithHandler(() => { SendBackupRequest(backupItem.Key); });

            _command.Register("panic").WithHandler(() => { SendBackupRequest(BackupType.Panic); });

            _command.Register("cordonarea50").WithHandler(() =>
            {
                if (_permissionService.CurrentUserRole.Division == UserDivision.Afo)
                    SendBackupRequest(BackupType.CordonArea50);
                if (_permissionService.CurrentUserRole.Division == UserDivision.hart)
                    SendBackupRequest(BackupType.CordonArea50Hart);
                return;
            });

            _command.Register("cordonarea100").WithHandler(() =>
            {
                if (_permissionService.CurrentUserRole.Division == UserDivision.Afo)
                    SendBackupRequest(BackupType.CordonArea100);
                if (_permissionService.CurrentUserRole.Division == UserDivision.hart)
                    SendBackupRequest(BackupType.CordonArea100Hart);
                return;
            });

            _command.Register("cordonarea150").WithHandler(() =>
            {
                if (_permissionService.CurrentUserRole.Division == UserDivision.Afo)
                    SendBackupRequest(BackupType.CordonArea150);
                if (_permissionService.CurrentUserRole.Division == UserDivision.hart ||
                    _permissionService.CurrentUserRole.Division == UserDivision.FRU)
                    SendBackupRequest(BackupType.CordonArea150Hart);
                return;
            });

            _command.Register("t").WithHandler((Func<string, Task>)TrackPlayerByCallsign);
            _command.Register("t").WithHandler((Func<Task>)CancelTrackingIfActive);

            _command.Register("panicroute").WithHandler(RouteToLastPanic);
            _command.Register("togglefriendly").WithHandler(ToggleFriendly);
        }

        // private BackupType DivisionToBackupType(string division)
        // {
        //     switch (division)
        //     {
        //         case "Rpu":
        //             return BackupType.Rpu;
        //         case "Dsu":
        //             return BackupType.Dsu;
        //         case "Npas":
        //             return BackupType.Npas;
        //         case "Afo":
        //             return BackupType.Afo;
        //         case "Cid":
        //             return BackupType.Cid;
        //         case "Ert":
        //             return BackupType.Response;
        //         default:
        //             return BackupType.Response;
        //     }
        // }

        // private async Task MdtBackupRequest(IMdtBackupRequest request)
        // {
        //     var netId = Game.Player.Handle;
        //     var pedNetId = Game.PlayerPed.Handle;

        //     BackupRequest backupRequest = new()
        //     {
        //         Type = DivisionToBackupType(request.Division),
        //         Player = netId,
        //         Ped = pedNetId,
        //         NetworkId = API.NetworkGetNetworkIdFromEntity(pedNetId),
        //         CoordsX = API.GetEntityCoords(pedNetId, Game.PlayerPed.IsAlive)[0],
        //         CoordsY = API.GetEntityCoords(pedNetId, Game.PlayerPed.IsAlive)[1],
        //         CoordsZ = API.GetEntityCoords(pedNetId, Game.PlayerPed.IsAlive)[2],
        //         Location = Game.PlayerPed.Position.ToString(), // What is a location as a string?
        //         Timestamp = new DateTime(),
        //         Name = "What do I put here?"
        //     };
        //     await OnReceiveBackupRequestAsync(backupRequest, Game.Player.Handle);
        // }

        private async Task OnReceiveLfbMdtSoundRequest(string emptyString)
        {
            await Delay(0);
            _logger.Debug($"LFRS MDT Request");
            foreach (var vehicle in World.GetAllVehicles())
            {
                var hasSirenCode = vehicle.HasDecor(ELSDecors.SIREN_TYPE_CODE);
                if (!hasSirenCode) continue;
                var sirenCode = vehicle.GetIntDecor(ELSDecors.SIREN_TYPE_CODE);
                _logger.Debug($"Siren Code: {sirenCode}");
                if (sirenCode != 3) continue;
                _logger.Debug($"Playing Sound to {vehicle.NetworkId}");
                _soundHandler.ToRadius(vehicle.NetworkId, 20f, "firemdtsound.wav", 0.05f);
            }
        }

        private async Task TrackPlayerTick()
        {
            if (_currentlyTrackingNetworkId == 0) return;

            var targetPed = (Ped)Entity.FromNetworkId(_currentlyTrackingNetworkId);
            if (targetPed == null)
            {
                var targetData = _playerService.FetchCahcedPlayerInfoFromNetworkId(_currentlyTrackingNetworkId);
                if (targetData == null)
                {
                    StopTracking();
                    return;
                }

                API.SetNewWaypoint(targetData.Position.X, targetData.Position.Y);
                await Delay(1000);
                return;
            }

            if (API.IsWaypointActive())
            {
                API.ClearGpsPlayerWaypoint();
                API.SetWaypointOff();
            }

            if (targetPed.AttachedBlip == null)
            {
                await Delay(1000);
                return;
            }

            targetPed.AttachedBlip.ShowRoute = true;
            await Delay(1000);
        }

        private Task AdminHandlerTick()
        {
            if (_rootMenu.Visible || !Game.IsControlJustPressed(0, (Control)289) ||
                Game.IsControlPressed(0, (Control)21) || !API.IsInputDisabled(0))
                return Task.FromResult(0);

            ReloadRootMenu();
            _rootMenu.OpenMenu();

            return Task.FromResult(0);
        }

        #region Backup / Track Menu

        private void ReloadBackupMenu()
        {
            _backupMenu.ClearMenuItems();

            foreach (var menuItem in BackupMenuItems)
                _backupMenu.AddItem(menuItem);
        }

        private void ReloadRootMenu()
        {
            _rootMenu.ClearMenuItems();

            if (_currentlyTrackingNetworkId != 0 || _activeBackupResponse != null)
            {
                _rootMenu.AddMenuItem(new MenuItem("Cancel Route", "Press this to cancel your GPS route")
                {
                    ItemData = MainMenuItem.CancelBackup
                });
            }

            if (_permissionService.CurrentUserRole.Branch != UserBranch.Civ)
            {
                ReloadPanicMenu();
                ReloadBackupRequestMenu();
            }

            _logger.Debug($"TrackBackupRequest ReloadRootMenu");

            var trackMenuItem = _rootMenu.AddItem(
                new MenuItem("Track Player", "Track players and respond to backup requests.")
                {
                    Label = "→→→",
                    ItemData = MainMenuItem.TrackPlayer
                });

            var backupMenuItem = _rootMenu.AddItem(new MenuItem("Request Backup", "Request backup from other officers")
            {
                Label = "→→→",
                ItemData = MainMenuItem.RequestBackup
            });

            if (_permissionService.CurrentUserRole.Branch != UserBranch.Civ)
            {

                // ADDED FOR BOYLES EVENT, WONT EFFECT PLAY AFTER EVENT BUT CAN BE REMOVED 
                /* ORIGINAL CODE:
                    _rootMenu.AddMenuItem(new MenuItem("~r~PANIC BUTTON", "~r~Use only in an emergency")
                    {
                        RightIcon = MenuItem.Icon.WARNING,
                        ItemData = MainMenuItem.PanicButton
                    });
                */

                DateTime now = DateTime.Now;
                DateTime eventDate = new DateTime(2025, 6, 20);

                DateTime start = eventDate.Date.AddHours(20).AddMinutes(30);
                DateTime end = eventDate.Date.AddHours(22);

                if (!(now >= start && now <= end))
                {
                    _rootMenu.AddMenuItem(new MenuItem("~r~PANIC BUTTON", "~r~Use only in an emergency")
                    {
                        RightIcon = MenuItem.Icon.WARNING,
                        ItemData = MainMenuItem.PanicButton
                    });
                }
            }

            MenuController.BindMenuItem(_rootMenu, _backupMenu, backupMenuItem);
            MenuController.BindMenuItem(_rootMenu, _trackMenu, trackMenuItem);
        }

        private void OnBackupMenuSelect(Menu menu, MenuItem item, int index)
        {
            if (!item.Selected)
                return;

            SendBackupRequest((BackupType)item.ItemData);
        }

        private async void OnTrackMenuSelect(Menu menu, MenuItem item, int index)
        {
            try
            {
                if (!item.Selected)
                    return;

                if (_currentlyTrackingNetworkId != 0)
                    StopTracking();

                await StartTrackingPed(item.ItemData);
            }
            catch (Exception ex)
            {
                _logger.Error("Backup menu pooped: " + ex);
            }
        }

        private async void OnRootMenuOnItemSelect(Menu menu, MenuItem item, int index)
        {
            try
            {
                if (!item.Selected)
                    return;

                switch ((MainMenuItem)item.ItemData)
                {
                    case MainMenuItem.TrackPanicRequest:
                        ReloadRootMenu();
                        break;

                    case MainMenuItem.TrackBackupRequest:
                        ReloadRootMenu();
                        break;

                    case MainMenuItem.RequestBackup:
                        ReloadBackupMenu();
                        break;

                    case MainMenuItem.CancelBackup:
                        StopTracking();
                        CancelActiveBackupResponse();
                        break;

                    case MainMenuItem.TrackPlayer:
                        await ShowPlayerMenu();
                        break;

                    case MainMenuItem.PanicButton:
                        SendBackupRequest(BackupType.Panic);
                        break;

                    default:
                        _newNotificationOverlay.SendNotification(new NewNotificationMessage("Menu", "error",
                            "Unexpected item, please try again later.", new NewNotificationMessageContent[0]));
                        break;
                }
            }
            catch (Exception ex)
            {
                _logger.Error("Backup menu pooped: " + ex);
            }
        }

        private void ReloadPanicMenu()
        {
            _panicMenu.ClearMenuItems();

            _panicRequests.RemoveAll((backupRequest) => (DateTime.UtcNow - backupRequest.Timestamp).TotalSeconds > 30);

            if (_panicRequests.Count <= 0)
                return;

            var activePanicMenuItem = _rootMenu.AddItem(
                new MenuItem("~r~Active Panic Buttons", "Respond to active panic buttons")
                {
                    Label = $"({_panicRequests.Count}) →→→",
                    ItemData = MainMenuItem.TrackPanicRequest
                });

            MenuController.BindMenuItem(_trackMenu, _panicMenu, activePanicMenuItem);

            _panicRequests.ForEach(backupRequest =>
            {
                if (backupRequest.Type != BackupType.Panic)
                    return;

                _panicMenu.AddItem(new MenuItem(backupRequest.Name)
                {
                    Enabled = backupRequest.AreaBlip > 0,
                    Label = "GPS →→→",
                    ItemData = backupRequest
                });
            });
        }

        private void ReloadBackupRequestMenu()
        {
            _backupRequestMenu.ClearMenuItems();

            _backupRequests.RemoveAll((backupRequest) => (DateTime.UtcNow - backupRequest.Timestamp).TotalSeconds > 60);

            if (_backupRequests.Count <= 0)
                return;

            var activeBackupMenuItems = _rootMenu.AddItem(
                new MenuItem("Active Backup Requests", "Respond to backup requests")
                {
                    Label = $"({_backupRequests.Count}) →→→",
                    ItemData = MainMenuItem.TrackBackupRequest
                });

            MenuController.BindMenuItem(_trackMenu, _backupRequestMenu, activeBackupMenuItems);

            _backupRequests.ForEach(backupRequest =>
            {
                if (backupRequest.Type == BackupType.Panic)
                    return;
                if (backupRequest.Type == BackupType.Moderator)
                {
                    _backupRequestMenu.AddItem(
                        new MenuItem($"[{BackupTypes[backupRequest.Type].Text}] - {backupRequest.Name}")
                        {
                            Label = "GPS ⚒️⚒️⚒️",
                            ItemData = backupRequest
                        });
                    return;
                }

                _backupRequestMenu.AddItem(
                    new MenuItem($"[{BackupTypes[backupRequest.Type].Text}] - {backupRequest.Name}")
                    {
                        Label = "GPS →→→",
                        ItemData = backupRequest
                    });
            });
        }

        private async Task ShowPlayerMenu()
        {
            _trackMenu.ClearMenuItems();

            var myInfo = _playerService.FetchCahcedPlayerInfoFromNetworkId(Game.PlayerPed.NetworkId);
            var recentPlayerInfo = (await _playerService.FetchAllRecentPlayerInfo())
                .Where(p => p.RoutingBucket == myInfo.RoutingBucket);

            //recentPlayerInfo.OrderBy(o => o.CallSign != null ? o.Name : String.Empty);

            foreach (var targetPlayerInfo in recentPlayerInfo)
            {
                if (targetPlayerInfo.NetworkId == Game.PlayerPed.NetworkId ||
                    targetPlayerInfo.ActiveBranch == UserBranch.Civ)
                    continue;

                var targetPlayer = (Ped)Entity.FromNetworkId(targetPlayerInfo.NetworkId);

                var callSign = (string.IsNullOrEmpty(targetPlayerInfo.CallSign))
                    ? "[NO CALLSIGN]"
                    : $"[{targetPlayerInfo.CallSign}]";

                var menuItem =
                    new MenuItem($"{callSign} {_common.TildeStripper(targetPlayerInfo.Name)}")
                    {
                        Label = $"[TG:{targetPlayerInfo.RadioChannel}]",
                        ItemData = targetPlayerInfo.NetworkId
                    };

                if (targetPlayer != null)
                {
                    if (targetPlayer.IsInPoliceVehicle)
                    {
                        if (targetPlayer.IsInBoat)
                            menuItem.RightIcon = MenuItem.Icon.INV_BOAT;
                        else if (targetPlayer.IsInHeli)
                            menuItem.RightIcon = MenuItem.Icon.INV_HELI;
                        else if (targetPlayer.IsOnBike)
                            menuItem.RightIcon = MenuItem.Icon.BIKE;
                        else
                            menuItem.RightIcon = MenuItem.Icon.INV_CAR;
                    }
                }

                _trackMenu.AddMenuItem(menuItem);
            }

            _trackMenu.SortMenuItems((x, y) => x.Text.CompareTo(y.Text));
        }

        private void CancelActiveBackupResponse()
        {
            if (_activeBackupResponse == null)
                return;

            if (_activeBackupResponse.AreaBlip != null)
            {
                var areaBlip = (int)_activeBackupResponse.AreaBlip;

                API.SetBlipRoute(areaBlip, false);
                API.RemoveBlip(ref areaBlip);
            }

            _activeBackupResponse = null;
        }

        private void RouteToBackupRequest(BackupRequest backupRequest)
        {
            try
            {
                if (_activeBackupResponse != null)
                    CancelActiveBackupResponse();

                if (backupRequest.AreaBlip == null)
                    return;

                _activeBackupResponse = backupRequest;

                var areaBlip = (int)backupRequest.AreaBlip;

                API.SetBlipRoute(areaBlip, true);

                if (backupRequest.Type == BackupType.Panic)
                    API.SetBlipRouteColour(areaBlip, (int)BlipColor.Red);
                else
                    API.SetBlipRouteColour(areaBlip, (int)BlipColor.Blue);
            }
            catch (Exception ex)
            {
                _newNotificationOverlay.SendNotification(new NewNotificationMessage("Backup Request", "error",
                    "Failed to route to backup request, please try again later.",
                    new NewNotificationMessageContent[0]));
                Debug.WriteLine("@@ Error route to panic: " + ex);
            }
        }

        private async Task StartTrackingPed(int targetNetworkId)
        {
            try
            {
                var userRole = _permissionService.GetUserRole(targetNetworkId);

                if (userRole.Branch == UserBranch.Civ)
                {
                    _newNotificationOverlay.SendNotification(new NewNotificationMessage("Error", "error",
                        "Unable to track this!", new NewNotificationMessageContent[0]));
                    return;
                }

                var targetPed = (Ped)Entity.FromNetworkId(targetNetworkId);

                if (targetPed == null)
                {
                    var targetPedData = _playerService.FetchCahcedPlayerInfoFromNetworkId(targetNetworkId);
                    if (targetPedData == null)
                    {
                        return;
                    }

                    API.SetNewWaypoint(targetPedData.Position.X, targetPedData.Position.Y);

                    _currentlyTrackingNetworkId = targetNetworkId;
                    return;
                }

                if (targetPed.AttachedBlip == null)
                {
                    _newNotificationOverlay.SendNotification(new NewNotificationMessage("Panic Response", "error",
                        "Unable to track player, please try again later.", new NewNotificationMessageContent[0]));
                    return;
                }

                targetPed.AttachedBlip.ShowRoute = true;
                API.SetBlipRouteColour(targetPed.AttachedBlip.Handle, (int)BlipColor.Yellow);

                _currentlyTrackingNetworkId = targetNetworkId;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("@@ Error track: " + ex);
            }
        }

        private void StopTracking()
        {
            try
            {
                if (_currentlyTrackingNetworkId == 0)
                {
                    return;
                }

                var ped = (Ped)Entity.FromNetworkId(_currentlyTrackingNetworkId);
                API.ClearGpsPlayerWaypoint();
                API.SetWaypointOff();
                if (ped == null)
                {
                    _currentlyTrackingNetworkId = 0;
                    return;
                }

                if (ped.AttachedBlip != null)
                {
                    ped.AttachedBlip.ShowRoute = false;
                }

                _currentlyTrackingNetworkId = 0;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("@@ Error stopping track: " + ex);
            }
        }

        private async Task OnReceiveBackupRequestAsync(BackupRequest backupRequest, int playerNetworkId)
        {
            //await Task.Delay(0); // You can directly await Task.Delay(0) instead of Delay(0)
            try
            {
                _logger.Debug($"OnReceiveBackupRequest {JsonConvert.SerializeObject(backupRequest)}");

                var currentUserRole = _permissionService.CurrentUserRole;
                var receiveAll = _userAces.IsDeveloper || _userAces.IsModerator || _userAces.IsAdmin;

                var messageContentList = new List<NewNotificationMessageContent>();
                var playerName = backupRequest.Name;

                if (playerName == Game.Player.Name)
                {
                    _newNotificationOverlay.SendNotification(new NewNotificationMessage("Backup request", "success",
                        "You've sent a Backup Request", new NewNotificationMessageContent[0]));

                    if (!_userAces.IsDeveloper)
                        return;
                }

                if (backupRequest.Type == BackupType.Panic && currentUserRole.Branch != UserBranch.Police)
                    return;

                if (backupRequest.Type != BackupType.CordonArea100 &&
                    backupRequest.Type != BackupType.CordonArea150 &&
                    backupRequest.Type != BackupType.CordonArea50 &&
                    backupRequest.Type != BackupType.CordonArea100Hart &&
                    backupRequest.Type != BackupType.CordonArea150Hart &&
                    backupRequest.Type != BackupType.CordonArea50Hart &&
                    backupRequest.Type != BackupType.Panic)
                {
                    messageContentList.Add(new NewNotificationMessageContent("Unit", playerName));
                    messageContentList.Add(new NewNotificationMessageContent("Backup request",
                        BackupTypes[backupRequest.Type].Text));
                    messageContentList.Add(new NewNotificationMessageContent("Location", backupRequest.Location));
                }

                if (backupRequest.Type == BackupType.Panic)
                {
                    _panicRequests.Add(backupRequest);
                    await HandlePanicButton(backupRequest);
                    return;
                }

                if (backupRequest.Type == BackupType.CordonArea50)
                    await HandleCordonArea(backupRequest, 55);
                else if (backupRequest.Type == BackupType.CordonArea100)
                    await HandleCordonArea(backupRequest, 105);
                else if (backupRequest.Type == BackupType.CordonArea150)
                    await HandleCordonArea(backupRequest, 155);
                else if (backupRequest.Type == BackupType.CordonArea50Hart)
                    await HandleHartCordonArea(backupRequest, 55);
                else if (backupRequest.Type == BackupType.CordonArea100Hart)
                    await HandleHartCordonArea(backupRequest, 105);
                else if (backupRequest.Type == BackupType.CordonArea150Hart)
                    await HandleHartCordonArea(backupRequest, 155);
                else
                {
                    if ((backupRequest.Type == BackupType.Nhs && currentUserRole.Branch == UserBranch.Nhs) ||
                        (backupRequest.Type == BackupType.NationalHighways &&
                         currentUserRole.Branch == UserBranch.Highways) ||
                        (backupRequest.Type == BackupType.AARecovery &&
                         currentUserRole.Branch == UserBranch.Highways) ||
                        (backupRequest.Type == BackupType.MetSupervisor &&
                         currentUserRole.Branch == UserBranch.Police) ||
                        (backupRequest.Type == BackupType.Moderator && (_userAces.IsModerator || receiveAll)))
                    {
                        _newNotificationOverlay.SendNotification(new NewNotificationMessage("Backup request", "info",
                            string.Empty, messageContentList.ToArray()));
                        _backupRequests.Add(backupRequest);
                        AddBackupBlip(backupRequest);
                    }
                    else if ((backupRequest.Type == BackupType.Response &&
                              (currentUserRole.Division == UserDivision.Ert || receiveAll)) ||
                             (backupRequest.Type == BackupType.Rpu &&
                              (currentUserRole.Division == UserDivision.Rpu || receiveAll)) ||
                             (backupRequest.Type == BackupType.Afo &&
                              (currentUserRole.Division == UserDivision.Afo || receiveAll)) ||
                             (backupRequest.Type == BackupType.Cid &&
                              (currentUserRole.Division == UserDivision.Cid || receiveAll)) ||
                             (backupRequest.Type == BackupType.PrisonerTransport &&
                              (currentUserRole.Division == UserDivision.Ert || receiveAll)) ||
                             (backupRequest.Type == BackupType.Dsu &&
                              (currentUserRole.Division == UserDivision.Dsu || receiveAll)) ||
                             (backupRequest.Type == BackupType.Npas &&
                              (currentUserRole.Division == UserDivision.Npas || receiveAll)) ||
                             (backupRequest.Type == BackupType.Drone &&
                              (currentUserRole.Division == UserDivision.Npas || receiveAll)) ||
                             (backupRequest.Type == BackupType.Ciu &&
                              (currentUserRole.Division == UserDivision.Rpu || receiveAll)) ||
                             (backupRequest.Type == BackupType.Custody &&
                              (currentUserRole.Division == UserDivision.Cid || receiveAll)) ||
                             (backupRequest.Type == BackupType.Lfb &&
                              (currentUserRole.Division == UserDivision.LFB || receiveAll)) ||
                             (backupRequest.Type == BackupType.FRU &&
                              (currentUserRole.Division == UserDivision.LFB || receiveAll)) ||
                             (backupRequest.Type == BackupType.LfbStationOfficer &&
                              (currentUserRole.Division == UserDivision.LFB || receiveAll)) ||
                             (backupRequest.Type == BackupType.HART &&
                              (currentUserRole.Division == UserDivision.hart || receiveAll)) ||
                             (backupRequest.Type == BackupType.HEMS &&
                              (currentUserRole.Division == UserDivision.Hems || receiveAll)) ||
                             (backupRequest.Type == BackupType.HEMSDOCTOR &&
                              (currentUserRole.Division == UserDivision.HemsDoctor || receiveAll)) ||
                             (backupRequest.Type == BackupType.NhsClinicalStudent &&
                              (currentUserRole.Division == UserDivision.ClinicalStudent || receiveAll)))
                    {
                        _newNotificationOverlay.SendNotification(new NewNotificationMessage("Backup request", "info",
                            string.Empty, messageContentList.ToArray()));
                        _backupRequests.Add(backupRequest);
                        AddBackupBlip(backupRequest);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.Debug(ex.ToString());
            }
        }


        private int AddPanicAreaBlip(float x, float y, float z)
        {
            _logger.Debug($"AddPanicAreaBlip {x},{y},{z}");

            var blip = API.AddBlipForRadius(x, y, z, 100);

            API.SetBlipColour(blip, 1);
            API.SetBlipFlashes(blip, true);
            API.SetBlipAlpha(blip, 80);
            API.SetBlipFlashInterval(blip, 500);

            API.BeginTextCommandSetBlipName("STRING");
            API.AddTextComponentString("Panic Button");
            API.EndTextCommandSetBlipName(blip);

            return blip;
        }


        private int CordonGunIcon(float x, float y, float z)
        {
            _logger.Debug($"AddCordonGunIconBlip {x},{y},{z}");

            var gunIcon = API.AddBlipForCoord(x, y, z);

            API.SetBlipSprite(gunIcon, 110);
            API.SetBlipScale(gunIcon, 1.5f);
            API.SetBlipColour(gunIcon, 12);
            API.SetBlipFlashes(gunIcon, false);
            API.SetBlipAlpha(gunIcon, 255);

            API.BeginTextCommandSetBlipName("STRING");
            API.AddTextComponentSubstringPlayerName("Incident Cordon");
            API.EndTextCommandSetBlipName(gunIcon);

            return gunIcon;
        }

        private int CordonHazmatIcon(float x, float y, float z)
        {
            _logger.Debug($"AddCordonHazmatIconBlip {x},{y},{z}");

            var hazmatIcon = API.AddBlipForCoord(x, y, z);

            API.SetBlipSprite(hazmatIcon, 303);
            API.SetBlipScale(hazmatIcon, 1.5f);
            API.SetBlipColour(hazmatIcon, 12);
            API.SetBlipFlashes(hazmatIcon, false);
            API.SetBlipAlpha(hazmatIcon, 255);

            API.BeginTextCommandSetBlipName("STRING");
            API.AddTextComponentSubstringPlayerName("Incident Cordon");
            API.EndTextCommandSetBlipName(hazmatIcon);

            return hazmatIcon;
        }

        private int AddCordonAreaBlip(float x, float y, float z, int areaSize)
        {
            _logger.Debug($"AddCordonAreaBlip {x},{y},{z}");

            var circleBlip = API.AddBlipForRadius(x, y, z, areaSize);

            API.SetBlipColour(circleBlip, 1);
            API.SetBlipFlashes(circleBlip, false);
            API.SetBlipAlpha(circleBlip, 175);

            API.BeginTextCommandSetBlipName("STRING");
            API.AddTextComponentString("Area Cordon");
            API.EndTextCommandSetBlipName(circleBlip);

            return circleBlip;
        }

        private async Task HandlePanicButton(BackupRequest backupRequest)
        {
            var messageContentList = new List<NewNotificationMessageContent>
            {
                new("Backup request: ", "CODE 0"),
                new("Location: ", backupRequest.Location)
            };

            _newNotificationOverlay.SendNotification(new NewNotificationMessage("Panic Activated", "panic",
                string.Empty, messageContentList.ToArray()));

            await SoundPanicButton();

            _newNotificationOverlay.SendNotification(new NewNotificationMessage("ALERT - HOLD AIRWAVE", "warn",
                "Panic Button has been activated, please keep airwave clear", new NewNotificationMessageContent[0]));

            var targetHandle = API.GetPlayerPed(backupRequest.Player);
            var targetPlayer = (Ped)Entity.FromHandle(targetHandle);

            var areaBlip = AddPanicAreaBlip(backupRequest.CoordsX, backupRequest.CoordsY, backupRequest.CoordsZ);
            backupRequest.AreaBlip = areaBlip;

            if (targetPlayer.AttachedBlip != null)
            {
                targetPlayer.AttachedBlip.Color = BlipColor.Red;
                targetPlayer.AttachedBlip.IsFlashing = true;
            }

            _ = DelayAndHandlePanicCleanup(backupRequest, areaBlip, targetPlayer);
        }

        private async Task DelayAndHandlePanicCleanup(BackupRequest backupRequest, int areaBlip, Ped targetPlayer)
        {
            await Delay(30000); // ✅ FiveM-safe, game thread safe

            if (_activeBackupResponse?.Id != backupRequest.Id)
            {
                API.RemoveBlip(ref areaBlip);
                backupRequest.AreaBlip = null;

                if (targetPlayer.AttachedBlip != null && _activeBackupResponse?.NetworkId != targetPlayer.NetworkId)
                {
                    targetPlayer.AttachedBlip.Color = BlipColor.White;
                    targetPlayer.AttachedBlip.IsFlashing = false;
                }

                _panicRequests.Remove(backupRequest);
            }
        }
        
        private async Task SoundPanicButton()
        {
            for (var i = 0; i < 5; i++)
            {
                Game.PlaySound("CONFIRM_BEEP", "HUD_MINI_GAME_SOUNDSET");
                await Delay(250);
            }
        }

        private readonly DateTime _lastBackupRequest = DateTime.Now;

        private void SendBackupRequest(BackupType backupType)
        {
            Task.Factory.StartNew(async () =>
            {
                var timeNow = DateTime.Now;
                var timeSinceLastBackup = timeNow - lastTime;
                if (timeSinceLastBackup.TotalSeconds < 30)
                {
                    _newNotificationOverlay.SendNotification(new NewNotificationMessage("Backup request", "error",
                        "You have requested backup within the last 30 seconds, please wait",
                        new NewNotificationMessageContent[0]));
                    return;
                }

                lastTime = DateTime.Now;

                try
                {
                    if (_cooldown)
                    {
                        _newNotificationOverlay.SendNotification(new NewNotificationMessage("Backup request", "error",
                            "You've recently requested backup. Please wait!", new NewNotificationMessageContent[0]));
                        return;
                    }

                    _newNotificationOverlay.SendNotification(new NewNotificationMessage("Backup request", "info",
                        "Sending a backup request...", new NewNotificationMessageContent[0]));

                    await Cooldown();

                    var pedId = API.PlayerPedId();
                    if (!API.DoesEntityExist(pedId) || pedId == 0)
                    {
                        _logger.Debug("[BackupMenu] Invalid Ped. Skipping backup request.");
                        return;
                    }

                    var backupRequest = new BackupRequest
                    {
                        Type = backupType,
                        Player = API.PlayerId(),
                        Ped = pedId,
                        Timestamp = DateTime.UtcNow,
                        NetworkId = API.NetworkGetNetworkIdFromEntity(pedId),
                        Name = API.GetPlayerName(API.PlayerId())
                    };

                    // _logger.Debug($"NETWORK_GET_NETWORK_ID_FROM_ENTITY {MethodBase.GetCurrentMethod()?.Name}");
                    //backupRequest.NetworkId = API.NetworkGetNetworkIdFromEntity(backupRequest.Ped);
                    //backupRequest.Name = API.GetPlayerName(backupRequest.Player);

                    var coords = API.GetEntityCoords(backupRequest.Ped, true);

                    backupRequest.CoordsX = coords.X;
                    backupRequest.CoordsY = coords.Y;
                    backupRequest.CoordsZ = coords.Z;

                    uint street = 0;
                    uint crossStreet = 0;

                    API.GetStreetNameAtCoord(backupRequest.CoordsX, backupRequest.CoordsY, backupRequest.CoordsZ,
                        ref street, ref crossStreet);

                    var streetName = API.GetStreetNameFromHashKey(street);

                    if (crossStreet > 0)
                    {
                        var crossStreetName = API.GetStreetNameFromHashKey(crossStreet);
                        backupRequest.Location = $"{streetName} X ${crossStreetName}";
                    }
                    else
                    {
                        backupRequest.Location = streetName;
                    }

                    _logger.Debug($"Sending Backup Event to Server. {backupRequest.Type.ToString()}");

                    _comms.ToServer(ServerEvents.SendBackupRequestToServer, backupRequest);

                    //_comms.ToServerRaw("PoliceMP::BackupRequest", JsonConvert.SerializeObject(backupRequest));
                    if (backupRequest.Type != BackupType.Panic)
                    {
                        //_comms.ToServerRaw(ServerEvents.Legacy.CalloutBackupRequest,
                        // JsonConvert.SerializeObject(backupRequest));
                    }

                    if (backupType == BackupType.Panic)
                    {
                        await SoundPanicButton();
                    }

                    if (backupType == BackupType.CordonArea50)
                    {
                        _logger.Debug($"Cordon Area request has been sent.");
                    }

                    if (backupType == BackupType.CordonArea100)
                    {
                        _logger.Debug($"Cordon Area request has been sent.");
                    }

                    if (backupType == BackupType.CordonArea150)
                    {
                        _logger.Debug($"Cordon Area request has been sent.");
                    }

                    if (backupType == BackupType.CordonArea50Hart)
                    {
                        _logger.Debug($"Cordon Area request has been sent.");
                    }

                    if (backupType == BackupType.CordonArea100Hart)
                    {
                        _logger.Debug($"Cordon Area request has been sent.");
                    }

                    if (backupType == BackupType.CordonArea150Hart)
                    {
                        _logger.Debug($"Cordon Area request has been sent.");
                    }
                }
                catch (Exception ex)
                {
                    _logger.Debug($"{ex}");
                    //_notification.Error("Backup Request", "Please try again later.");
                }
            }, TaskCreationOptions.LongRunning);
        }

        private async Task Cooldown()
        {
            if (_cooldown || CanSkipCooldown)
            {
                return;
            }

            _cooldown = true;
            Debug.WriteLine("[BackupMenu] Cooldown active");
            await Delay(_cooldownTimeout);
            _cooldown = false;
            Debug.WriteLine("[BackupMenu] Cooldown expired");
        }

        #endregion Backup / Track Menu

        #region /Track Command

        private async Task TrackPlayerByCallsign(string callsign)
        {
            var currentUserRole = _permissionService.CurrentUserRole;

            if (currentUserRole.Branch != UserBranch.Nhs &&
                currentUserRole.Branch != UserBranch.Police &&
                currentUserRole.Branch != UserBranch.Highways &&
                currentUserRole.Branch != UserBranch.Fire)
            {
                _newNotificationOverlay.SendNotification(new NewNotificationMessage(
                    "Track Player",
                    "error",
                    "You do not have permission to use this command.",
                    new NewNotificationMessageContent[0]
                ));
                return;
            }

            var myInfo = _playerService.FetchCahcedPlayerInfoFromNetworkId(Game.PlayerPed.NetworkId);
            var players = await _playerService.FetchAllRecentPlayerInfo();
            _logger.Debug($"[TRACK] Found {players.Count()} players in recent player info");

            foreach (var p in players)
            {
                _logger.Debug(
                    $"[TRACK] Player: Name={p.Name}, CallSign={p.CallSign}, RoutingBucket={p.RoutingBucket}, NetworkId={p.NetworkId}");
            }

            var match = players.FirstOrDefault(p =>
                !string.IsNullOrEmpty(p.CallSign) &&
                p.CallSign.Replace(" ", "").Equals(callsign.Replace(" ", ""), StringComparison.OrdinalIgnoreCase) &&
                p.RoutingBucket == myInfo.RoutingBucket);

            if (match == null)
            {
                _newNotificationOverlay.SendNotification(new NewNotificationMessage(
                    "Track Player",
                    "error",
                    $"No player found with callsign '{callsign}'",
                    new NewNotificationMessageContent[0]
                ));
                return;
            }

            if (_currentlyTrackingNetworkId == match.NetworkId)
            {
                StopTracking();
                _newNotificationOverlay.SendNotification(new NewNotificationMessage(
                    "Tracking",
                    "success",
                    $"Stopped tracking {match.CallSign.ToUpper()} {match.Name.ToUpper()}",
                    new NewNotificationMessageContent[0]
                ));
                return;
            }

            if (_currentlyTrackingNetworkId != 0)
                StopTracking();

            await StartTrackingPed(match.NetworkId);

            _newNotificationOverlay.SendNotification(new NewNotificationMessage(
                "Tracking",
                "info",
                $"You are now tracking {match.CallSign.ToUpper()} {match.Name.ToUpper()}",
                new NewNotificationMessageContent[0]
            ));
        }
        private async Task CancelTrackingIfActive()
        {
            if (_currentlyTrackingNetworkId == 0)
            {
                _newNotificationOverlay.SendNotification(new NewNotificationMessage(
                    "Tracking",
                    "error",
                    "You are not currently tracking anyone.",
                    new NewNotificationMessageContent[0]
                ));
                await Delay(0);
                return;
            }

            StopTracking();

            _newNotificationOverlay.SendNotification(new NewNotificationMessage(
                "Tracking",
                "success",
                "Stopped tracking.",
                new NewNotificationMessageContent[0]
            ));

            await Delay(0);
        }
        #endregion /Track Command

        private async Task HandlePlayerSpawned(bool firstSpawn)
        {
            _userAces = await _permissionService.GetUserAces();
            _hasModPerms = _userAces.IsModerator || _userAces.IsAdmin || _userAces.IsDeveloper;
        }


        private async Task HandleCordonArea(BackupRequest backupRequest, float areaSize)
        {
            var player = Game.PlayerPed.Handle;
            int cordonAreaSize = 0;
            var playerPosition = API.GetEntityCoords(player, true);
            var distanceFromCordon = API.GetDistanceBetweenCoords(playerPosition.X, playerPosition.Y, playerPosition.Z,
                backupRequest.CoordsX, backupRequest.CoordsY, backupRequest.CoordsZ, false);

            if (distanceFromCordon < areaSize)
            {
                _newNotificationOverlay.SendNotification(new NewNotificationMessage("Firearms Incident!", "warn",
                    $"{backupRequest.Name} has cordoned {(int)areaSize - 5}m around their location! Ensure to remain out of this zone that is marked on the map!",
                    new NewNotificationMessageContent[0]));
            }

            switch (backupRequest.Type)
            {
                case BackupType.CordonArea50:
                    cordonAreaSize = 100;
                    break;

                case BackupType.CordonArea100:
                    cordonAreaSize = 200;
                    break;

                case BackupType.CordonArea150:
                    cordonAreaSize = 250;
                    break;
                default:
                    cordonAreaSize = 100;
                    break;
            }

            var targetHandle = API.GetPlayerPed(backupRequest.Player);
            var targetPlayer = (Ped)Entity.FromHandle(targetHandle);
            var cordonBlip = AddCordonAreaBlip(backupRequest.CoordsX, backupRequest.CoordsY, backupRequest.CoordsZ,
                cordonAreaSize);
            var gunIconBlip = CordonGunIcon(backupRequest.CoordsX, backupRequest.CoordsY, backupRequest.CoordsZ);

            backupRequest.AreaBlip = cordonBlip;

            if (targetPlayer.AttachedBlip != null)
            {
                targetPlayer.AttachedBlip.Color = BlipColor.Red;
            }

            await Script.Delay(120000);

            API.RemoveBlip(ref cordonBlip);
            API.RemoveBlip(ref gunIconBlip);
            backupRequest.AreaBlip = null;
        }


        private async Task HandleHartCordonArea(BackupRequest backupRequest, float areaSize)
        {
            var player = Game.PlayerPed.Handle;
            int cordonAreaSize = 0;
            var playerPosition = API.GetEntityCoords(player, true);
            var distanceFromCordon = API.GetDistanceBetweenCoords(playerPosition.X, playerPosition.Y, playerPosition.Z,
                backupRequest.CoordsX, backupRequest.CoordsY, backupRequest.CoordsZ, false);

            if (distanceFromCordon < areaSize)
            {
                _newNotificationOverlay.SendNotification(new NewNotificationMessage("Hazmat Incident!", "warn",
                    $"{backupRequest.Name} has cordoned {(int)areaSize - 5}m around their location! Ensure to remain out of this zone that is marked on the map!",
                    new NewNotificationMessageContent[0]));
            }

            switch (backupRequest.Type)
            {
                case BackupType.CordonArea50Hart:
                    cordonAreaSize = 100;
                    break;

                case BackupType.CordonArea100Hart:
                    cordonAreaSize = 200;
                    break;

                case BackupType.CordonArea150Hart:
                    cordonAreaSize = 250;
                    break;
                default:
                    cordonAreaSize = 100;
                    break;
            }

            var targetHandle = API.GetPlayerPed(backupRequest.Player);
            var targetPlayer = (Ped)Entity.FromHandle(targetHandle);
            var cordonBlip = AddCordonAreaBlip(backupRequest.CoordsX, backupRequest.CoordsY, backupRequest.CoordsZ,
                cordonAreaSize);
            var hazmatIconBlip = CordonHazmatIcon(backupRequest.CoordsX, backupRequest.CoordsY, backupRequest.CoordsZ);


            backupRequest.AreaBlip = cordonBlip;

            if (targetPlayer.AttachedBlip != null)
            {
                targetPlayer.AttachedBlip.Color = BlipColor.Red;
            }

            await Script.Delay(120000);

            API.RemoveBlip(ref cordonBlip);
            API.RemoveBlip(ref hazmatIconBlip);
            backupRequest.AreaBlip = null;
        }

        private async Task RouteToLastPanic()
        {
            try
            {
                var lastPanicRequest = _panicRequests.Last();

                if (lastPanicRequest == null)
                {
                    _newNotificationOverlay.SendNotification(new NewNotificationMessage("Panic Response", "error",
                        "There are no active panic buttons.", new NewNotificationMessageContent[0]));
                    return;
                }

                if (_currentlyTrackingNetworkId != 0)
                {
                    StopTracking();
                }

                await StartTrackingPed((int)lastPanicRequest.NetworkId);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("RouteToLastPanic Error", ex);
            }
        }

        private void ToggleFriendly()
        {
            if (!_hasModPerms) return;

            API.NetworkSetFriendlyFireOption(!_friendlyEnabled);
            API.SetCanAttackFriendly(API.PlayerPedId(), !_friendlyEnabled, false);
            _friendlyEnabled = !_friendlyEnabled;
            _newNotificationOverlay.SendNotification(new NewNotificationMessage("Friendly Fire", "success",
                $"Friendly fire has now been {(_friendlyEnabled ? "enabled" : "disabled")}",
                new NewNotificationMessageContent[0]));
        }

        private void HandlePanicClick(Menu menu, MenuItem item, int index)
        {
            var backupRequest = (BackupRequest)item.ItemData;

            RouteToBackupRequest(backupRequest);
        }

        private async void HandleBackupRequestClick(Menu menu, MenuItem item, int index)
        {
            try
            {
                if (!item.Selected)
                    return;

                if (_currentlyTrackingNetworkId != 0)
                    StopTracking();

                var backupRequest = (BackupRequest)item.ItemData;

                if (backupRequest == null)
                    return;

                await StartTrackingPed(backupRequest.NetworkId);
            }
            catch (Exception ex)
            {
                _logger.Error("Backup menu pooped: " + ex);
            }
        }

        private void AddBackupBlip(BackupRequest request)
        {
            var blip = World.CreateBlip(
                new CitizenFX.Core.Vector3(request.CoordsX, request.CoordsY, request.CoordsZ));
            blip.Sprite = BlipSprite.PointOfInterest;
            switch (request.Type)
            {
                case BackupType.Nhs:
                case BackupType.NhsTeamLeader:
                    blip.Color = BlipColor.Green;
                    break;

                case BackupType.Moderator:
                    blip.Color = BlipColor.White;
                    break;

                case BackupType.NationalHighways:
                case BackupType.AARecovery:
                    blip.Color = BlipColor.TrevorOrange;
                    break;

                case BackupType.Lfb:
                case BackupType.LfbStationOfficer:
                    blip.Color = BlipColor.Red;
                    break;

                default:
                    blip.Color = BlipColor.Blue;
                    break;
            }

            blip.Name = $"{BackupTypes[request.Type].Text} Backup";

            RemoveBlip(blip);
        }

        private async Task RemoveBlip(Blip blip)
        {
            await Delay(60000);
            blip.Delete();
        }
    }
}