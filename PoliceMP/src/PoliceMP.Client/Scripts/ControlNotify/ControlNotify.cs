using CitizenFX.Core;
using CitizenFX.Core.Native;
using CitizenFX.Core.UI;
using MenuAPI;
using Newtonsoft.Json;
using PoliceMP.Client.Overlays.NewNotification;
using PoliceMP.Client.Services.Interfaces;
using PoliceMP.Core.Client;
using PoliceMP.Core.Client.Commands.Interfaces;
using PoliceMP.Core.Client.Communications.Interfaces;
using PoliceMP.Core.Client.Extensions;
using PoliceMP.Core.Client.Interface;
using PoliceMP.Core.Client.Scripts;
using PoliceMP.Core.Shared;
using PoliceMP.Shared.Constants;
using PoliceMP.Shared.Enums;
using PoliceMP.Shared.Models;
using System;
using System.Threading.Tasks;

namespace PoliceMP.Client.Scripts.ControlNotify
{
    public class ControlNotify : Script
    {
        private readonly ILogger<ControlNotify> _logger;
        private readonly ITickManager _ticks;
        private readonly ILegacyClientCommunicationsManager _comms;
        private readonly ICommandManager _commands;
        private readonly IPermissionService _permission;
        private readonly INewNotificationOverlay _newNotificationOverlay;
        private readonly IFeatureService _featureService;
        private readonly IExportsAccessor _exportsAccessor;

        public ControlNotify(ILogger<ControlNotify> logger, ITickManager ticks, ILegacyClientCommunicationsManager comms,
            ICommandManager commands, IPermissionService permission, INewNotificationOverlay newNotificationOverlay, IFeatureService featureService, IExportsAccessor exportsAccessor)
        {
            _logger = logger;
            _ticks = ticks;
            _comms = comms;
            _commands = commands;
            _permission = permission;
            _newNotificationOverlay = newNotificationOverlay;
            _featureService = featureService;
            _exportsAccessor = exportsAccessor;
        }

        protected override async Task OnStartAsync()
        {
            _commands.Register("controlnotify").WithHandler(async () =>
            {
                MenuController.CloseAllMenus();

                var aces = await _permission.GetUserAces();
                if (!aces.IsDeveloper && !aces.IsControl && !aces.IsFimTrained)
                {
                    _newNotificationOverlay.SendNotification(
                        new NewNotificationMessage("Control Chat Menu", "error", 
                        "Only Control Room Operators can notify units on Channel Status!", new NewNotificationMessageContent[0]));
                    return;
                }

                var controlNotifyMenu = new Menu("Control Chat Menu", "Notify units of Control's status!");
                MenuController.EnableMenuToggleKeyOnController = false;
                MenuController.MenuToggleKey = (Control)(-1);
                MenuController.MenuAlignment = MenuController.MenuAlignmentOption.Right;
                MenuController.AddMenu(controlNotifyMenu);

                // Online channel items
                var metDescSouthOnline = new MenuItem("Met Desc - South Online");
                controlNotifyMenu.AddMenuItem(metDescSouthOnline);

                var metDescNorthOnline = new MenuItem("Met Desc - North Online");
                controlNotifyMenu.AddMenuItem(metDescNorthOnline);

                var lasOpsOnline = new MenuItem("LAS Ops Online");
                controlNotifyMenu.AddMenuItem(lasOpsOnline);

                var hemsDescOnline = new MenuItem("HEMS/HART Desc Online");
                controlNotifyMenu.AddMenuItem(hemsDescOnline);

                var lfbOpsOnline = new MenuItem("LFB Ops Online");
                controlNotifyMenu.AddMenuItem(lfbOpsOnline);

                var highwayOpsOnline = new MenuItem("Highway Ops Online");
                controlNotifyMenu.AddMenuItem(highwayOpsOnline);

                var fimOnline = new MenuItem("FIM Online");
                controlNotifyMenu.AddMenuItem(fimOnline);

                var fimOffline = new MenuItem("FIM Offline");
                controlNotifyMenu.AddMenuItem(fimOffline);

                var metDescSouthOffline = new MenuItem("Met Desc - South Offline");
                controlNotifyMenu.AddMenuItem(metDescSouthOffline);

                var metDescNorthOffline = new MenuItem("Met Desc - North Offline");
                controlNotifyMenu.AddMenuItem(metDescNorthOffline);

                var lasOpsOffline = new MenuItem("LAS Ops Offline");
                controlNotifyMenu.AddMenuItem(lasOpsOffline);

                var hemsDescOffline = new MenuItem("HEMS/HART Desc Offline");
                controlNotifyMenu.AddMenuItem(hemsDescOffline);

                var lfbOpsOffline = new MenuItem("LFB Ops Offline");
                controlNotifyMenu.AddMenuItem(lfbOpsOffline);

                var highwayOpsOffline = new MenuItem("Highway Ops Offline");
                controlNotifyMenu.AddMenuItem(highwayOpsOffline);

                await Delay(0);
                controlNotifyMenu.OpenMenu();

                controlNotifyMenu.OnItemSelect += (menu, item, index) =>
                {
                    if (menu != controlNotifyMenu) return;

                    string message = "";
                    
                    // Online notifications
                    if (item == metDescSouthOnline)
                    {
                        message = "Control is active! 'Met Desc - South' channel is ONLINE. All units, please book on to CAD (cad.policemp.com) and set your /callsign.";
                        _comms.ToServer(ServerEvents.ControlStatus, true);
                        _comms.ToServer(ServerEvents.SendControlNotificationToServer, message);
                    }
                    else if (item == fimOnline)
                    {
                        message = "Control FIM (Force Incident Manager) is ONLINE!";
                        _comms.ToServer(ServerEvents.ControlStatus, true);
                        _comms.ToServer(ServerEvents.SendControlNotificationToServer, message);
                    }
                    else if (item == fimOffline)
                    {
                        message = "Control FIM (Force Incident Manager) is OFFLINE!";
                        _comms.ToServer(ServerEvents.ControlStatus, true);
                        _comms.ToServer(ServerEvents.SendControlNotificationToServer, message);
                    }
                    else if (item == metDescNorthOnline)
                    {
                        message = "Control is active! 'Met Desc - North' channel is ONLINE. All units, please book on to CAD (cad.policemp.com) and set your /callsign.";
                        _comms.ToServer(ServerEvents.ControlStatus, true);
                        _comms.ToServer(ServerEvents.SendControlNotificationToServer, message);
                    }
                    else if (item == lasOpsOnline)
                    {
                        message = "Control is active! 'LAS Ops' channel is ONLINE. All units, please book on to CAD (cad.policemp.com) and set your /callsign.";
                        _comms.ToServer(ServerEvents.ControlStatus, true);
                        _comms.ToServer(ServerEvents.SendControlNotificationToServer, message);
                    }
                    else if (item == hemsDescOnline)
                    {
                        message = "Control is active! 'HEMS/HART Desc' channel is ONLINE. All units, please book on to CAD (cad.policemp.com) and set your /callsign.";
                        _comms.ToServer(ServerEvents.ControlStatus, true);
                        _comms.ToServer(ServerEvents.SendControlNotificationToServer, message);
                    }
                    else if (item == lfbOpsOnline)
                    {
                        message = "Control is active! 'LFB Ops' channel is ONLINE. All units, please book on to CAD (cad.policemp.com) and set your /callsign.";
                        _comms.ToServer(ServerEvents.ControlStatus, true);
                        _comms.ToServer(ServerEvents.SendControlNotificationToServer, message);
                    }
                    else if (item == highwayOpsOnline)
                    {
                        message = "Control is active! 'Highway Ops' channel is ONLINE. All units, please book on to CAD (cad.policemp.com) and set your /callsign.";
                        _comms.ToServer(ServerEvents.ControlStatus, true);
                        _comms.ToServer(ServerEvents.SendControlNotificationToServer, message);
                    }
                    // Offline notifications
                    else if (item == metDescSouthOffline)
                    {
                        message = "'Met Desc - South' channel is now OFFLINE.";
                        _comms.ToServer(ServerEvents.ControlStatus, false);
                        _comms.ToServer(ServerEvents.SendControlNotificationToServer, message);
                    }
                    else if (item == metDescNorthOffline)
                    {
                        message = "'Met Desc - North' channel is now OFFLINE.";
                        _comms.ToServer(ServerEvents.ControlStatus, false);
                        _comms.ToServer(ServerEvents.SendControlNotificationToServer, message);
                    }
                    else if (item == lasOpsOffline)
                    {
                        message = "'LAS Ops' channel is now OFFLINE.";
                        _comms.ToServer(ServerEvents.ControlStatus, false);
                        _comms.ToServer(ServerEvents.SendControlNotificationToServer, message);
                    }
                    else if (item == hemsDescOffline)
                    {
                        message = "'HEMS/HART Desc' channel is now OFFLINE.";
                        _comms.ToServer(ServerEvents.ControlStatus, false);
                        _comms.ToServer(ServerEvents.SendControlNotificationToServer, message);
                    }
                    else if (item == lfbOpsOffline)
                    {
                        message = "'LFB Ops' channel is now OFFLINE.";
                        _comms.ToServer(ServerEvents.ControlStatus, false);
                        _comms.ToServer(ServerEvents.SendControlNotificationToServer, message);
                    }
                    else if (item == highwayOpsOffline)
                    {
                        message = "'Highway Ops' channel is now OFFLINE.";
                        _comms.ToServer(ServerEvents.ControlStatus, false);
                        _comms.ToServer(ServerEvents.SendControlNotificationToServer, message);
                    }
                };
            });
        }
    }
}