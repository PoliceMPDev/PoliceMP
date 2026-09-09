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

namespace PoliceMP.Client.Scripts.FireEquipment
{
    public class FireEquipment : Script
    {
        private readonly ICommandManager _commandManager;
        private readonly INewNotificationOverlay _newNotificationOverlay;
        private readonly ITickManager _ticks;
        private readonly IPermissionService _permissionService;
        private readonly ILogger<FireEquipment> _logger;

        public FireEquipment(ICommandManager commandManager, ILogger<FireEquipment> logger,
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


        protected override async Task OnStartAsync()
        {
            var userAces = await _permissionService.GetUserAces();

            //_commandManager.Register("dropmedicalkit").WithHandler(DropMedicalKit);
            API.RegisterKeyMapping("dropmedicalkit", "Place Medical Kit", "KEYBOARD", "e");

            _commandManager.Register("firebtm").WithHandler(async () =>
            {

                MenuController.CloseAllMenus();

                var fireEquipmentMenu = new Menu("Fire Equipment", "Collect your fire equipment");
                MenuController.EnableMenuToggleKeyOnController = false;
                MenuController.MenuToggleKey = (Control)(-1);
                MenuController.MenuAlignment = MenuController.MenuAlignmentOption.Right;
                MenuController.AddMenu(fireEquipmentMenu);

                await Delay(0);
                fireEquipmentMenu.OpenMenu();

                //if (currentUserRole.Division == UserDivision.Afo
                //if (currentUserRole.Branch == UserBranch.Nhs)
                var currentUserRole = _permissionService.CurrentUserRole;

                var clear = new MenuItem("Clear Equipment", "Return everything");
                fireEquipmentMenu.AddMenuItem(clear);

                var spreaders = new MenuItem("Spreaders", "Collect or return your Spreaders");
                var testMCX = new MenuItem("Test MCX", "");

                fireEquipmentMenu.AddMenuItem(spreaders);
                fireEquipmentMenu.AddMenuItem(testMCX);

                fireEquipmentMenu.OpenMenu();

                fireEquipmentMenu.OnItemSelect += (menu, item, index) =>
                {
                    if (menu != fireEquipmentMenu) return;

                    if (item == clear) CollectTool("null", "null");
                    if (item == spreaders) CollectTool("Spreaders", "w_ar_recip");
                    if (item == testMCX) CollectTool("TEST MCX", "WEAPON_SIGMCX");

                    fireEquipmentMenu.CloseMenu();
                };
            });
        }

        private async void CollectTool(string name, string stringHash)
        {
            var playerPed = Game.PlayerPed;
            var hash = (uint)API.GetHashKey(stringHash);

            if (playerPed.Weapons.HasWeapon((WeaponHash)hash))
            {
                playerPed.Weapons.Remove((WeaponHash)hash);
                playerPed.Weapons.Select((WeaponHash)API.GetHashKey("WEAPON_UNARMED"));
                _newNotificationOverlay.SendNotification(new NewNotificationMessage("Fire Equipment", "success", $"You have returned your {name}!", new NewNotificationMessageContent[0]));
                return;
            }

            playerPed.Weapons.Give((WeaponHash)hash, 100, true, true);
            playerPed.Weapons.Select((WeaponHash)hash);
            _newNotificationOverlay.SendNotification(new NewNotificationMessage("Fire Equipment", "success", $"You have collected your {name}!", new NewNotificationMessageContent[0]));
        }
    }
}