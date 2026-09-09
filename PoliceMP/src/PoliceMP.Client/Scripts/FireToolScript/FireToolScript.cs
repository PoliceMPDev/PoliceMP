using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.Drawing;
using System.Drawing.Imaging;
using System.Net;
using System.Threading.Tasks;
using System.Windows;
using CitizenFX.Core;
using CitizenFX.Core.Native;
using CitizenFX.Core.NaturalMotion;
using CitizenFX.Core.UI;
using PoliceMP.Client.Actions.HandsUp;
using PoliceMP.Client.Scripts.Admin;
using PoliceMP.Client.Scripts.HideBlips;
using PoliceMP.Client.Services.Interfaces;
using PoliceMP.Core.Client;
using PoliceMP.Core.Client.Commands.Interfaces;
using PoliceMP.Core.Client.Communications.Interfaces;
using PoliceMP.Core.Client.Interface;
using PoliceMP.Core.Client.Scripts;
using PoliceMP.Core.Shared;
using PoliceMP.Shared.Enums;
using PoliceMP.Shared.Models;
using PoliceMP.Shared.Constants.States;
using PoliceMP.Client.Overlays.NewNotification;
using MenuAPI;
using System.Xml.Linq;
using PoliceMP.Core.Mediator;
using System.Security.Policy;
using System.Linq;

namespace PoliceMP.Client.Scripts.FireToolScript
{
    public class FireToolScript : Script
    {
        private readonly ICommandManager _commandManager;
        private readonly INewNotificationOverlay _newNotificationOverlay;
        private readonly ITickManager _ticks;
        private readonly IPermissionService _permissionService;
        private readonly ILogger<FireToolScript> _logger;

        public FireToolScript(ICommandManager commandManager, ILogger<FireToolScript> logger,
            INewNotificationOverlay newNotificationOverlay, IPermissionService permissionService, ITickManager ticks,
            IPlayerService playerService, ILegacyClientCommunicationsManager comms, ICommonFunctionsService common, IFeatureService featureService)
        {
            _commandManager = commandManager;
            _newNotificationOverlay = newNotificationOverlay;
            _permissionService = permissionService;
            _ticks = ticks;
            _commandManager = commandManager;
            _logger = logger;
            _permissionService = permissionService;
        }

        private int fireProp;
        private int firePropPlaced;

        protected override async Task OnStartAsync()
        {
            //_commandManager.Register("droppickupfiretool").WithHandler(DropPickupFireTool);
            //API.RegisterKeyMapping("droppickupfiretool", "Place Fire Tool", "KEYBOARD", "e");

            _commandManager.Register("btmfiret").WithHandler(async () =>
            {
                Debug.WriteLine("command used");

                MenuController.CloseAllMenus();

                var fireToolsMenu = new Menu("Fire Tools", "Collect your Fire Tools");
                MenuController.EnableMenuToggleKeyOnController = false;
                MenuController.MenuToggleKey = (Control)(-1);
                MenuController.MenuAlignment = MenuController.MenuAlignmentOption.Right;
                MenuController.AddMenu(fireToolsMenu);

                await Delay(0);
                fireToolsMenu.OpenMenu();

                //var currentUserRole = _permissionService.GetUserAces();
                //if (currentUserRole.Branch == UserBranch.Fire)

                var spreaders = new MenuItem("Spreaders", "Collect or return your Spreaders");
                fireToolsMenu.AddMenuItem(spreaders);
                var cutters = new MenuItem("Cutters", "Collect or return your Cutters");
                fireToolsMenu.AddMenuItem(spreaders);
                var saw = new MenuItem("Saw", "Collect or return your Saw");
                fireToolsMenu.AddMenuItem(saw);

                fireToolsMenu.OpenMenu();


                fireToolsMenu.OnItemSelect += (menu, item, index) =>
                {
                    if (menu != fireToolsMenu) return;

                    if (item == spreaders) FireTools("Spreaders", "w_mg_lfb_spreaders");
                    if (item == spreaders) FireTools("Cutters", "w_mg_lfb_cutters");
                    if (item == spreaders) FireTools("Saw", "w_mg_huskyvara");

                };

                fireToolsMenu.CloseMenu();
            });
        }

        private async void FireTools(string name, string strHash)
        {
            var player = Game.PlayerPed.Handle;
            var playerPos = API.GetEntityCoords(player, true);
            var playerPed = Game.PlayerPed;

            var toolHash = (WeaponHash)API.GetHashKey(strHash);
            if (playerPed.Weapons.HasWeapon(toolHash))
            {
                playerPed.Weapons.Give(toolHash, 999, true, true);
                playerPed.Weapons.Select(toolHash);
                _newNotificationOverlay.SendNotification(new NewNotificationMessage("Fire Tools", "success", $"You have taken your {name}!", new NewNotificationMessageContent[0]));
            }
            else if (!playerPed.Weapons.HasWeapon(toolHash))
            {
                playerPed.Weapons.Remove(toolHash);
                playerPed.Weapons.Select((WeaponHash)API.GetHashKey("WEAPON_UNARMED"));
                _newNotificationOverlay.SendNotification(new NewNotificationMessage("Fire Tools", "success", $"You have returned your {name}!", new NewNotificationMessageContent[0]));
            }
        }
    }
}