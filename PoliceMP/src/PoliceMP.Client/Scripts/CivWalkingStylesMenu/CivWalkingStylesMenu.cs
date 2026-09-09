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
using System.Runtime.CompilerServices;
using PoliceMP.Client.Properties;

namespace PoliceMP.Client.Scripts.CivWalkingStylesMenu
{
    public class CivWalkingStylesMenu : Script
    {
        private readonly ICommandManager _commandManager;
        private readonly INewNotificationOverlay _newNotificationOverlay;
        private readonly ITickManager _ticks;
        private readonly IPermissionService _permissionService;
        private readonly ILogger<CivWalkingStylesMenu> _logger;

        public CivWalkingStylesMenu(ICommandManager commandManager, ILogger<CivWalkingStylesMenu> logger,
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
            _commandManager.Register("cvlptc").WithHandler(OnCustomPlateCommand);

            _commandManager.Register("walkingstyles").WithHandler(async () =>
            {

                MenuController.CloseAllMenus();

                var walkingStylesMenu = new Menu("Walking Styles", "Change your walking style");
                MenuController.EnableMenuToggleKeyOnController = false;
                MenuController.MenuToggleKey = (Control)(-1);
                MenuController.MenuAlignment = MenuController.MenuAlignmentOption.Right;
                MenuController.AddMenu(walkingStylesMenu);

                await Delay(0);
                walkingStylesMenu.OpenMenu();

                var reset = new MenuItem("Normal", "Normal walking style");
                var drunk = new MenuItem("Drunk", "Drunk walking style");
                var drunk2 = new MenuItem("Drunk 2", "Drunk 2 walking style");
                var brave = new MenuItem("Brave", "Brave walking style");
                var lester = new MenuItem("Lester", "Lester walking style");
                var lester2 = new MenuItem("Lester 2", "Lester 2 walking style");
                var arrogant = new MenuItem("Arrogant", "Arrogant walking style");
                var injured = new MenuItem("Injured", "Injured walking style");

                walkingStylesMenu.AddMenuItem(reset);
                walkingStylesMenu.AddMenuItem(drunk);
                walkingStylesMenu.AddMenuItem(drunk2);
                walkingStylesMenu.AddMenuItem(brave);
                walkingStylesMenu.AddMenuItem(lester);
                walkingStylesMenu.AddMenuItem(lester2);
                walkingStylesMenu.AddMenuItem(arrogant);
                walkingStylesMenu.AddMenuItem(injured);

                walkingStylesMenu.OpenMenu();


                walkingStylesMenu.OnItemSelect += (menu, item, index) =>
                {
                    if (menu != walkingStylesMenu) return;
                    if (item == reset) OnWalkReset();
                    if (item == drunk) OnWalkDrunk();
                    if (item == drunk2) OnWalkDrunk2();
                    if (item == brave) OnWalkBrave();
                    if (item == lester) OnWalkLester();
                    if (item == lester2) OnWalkLester2();
                    if (item == arrogant) OnWalkArrogant();
                    if (item == injured) OnWalkInjured();

                    walkingStylesMenu.CloseMenu();
                };
            });
        }

        private void OnWalkReset()
        {
            MovementHandler.currentStyle = "Normal";
            MovementHandler.WalkingStyle = "DEFAULT_ACTION";
            _newNotificationOverlay.SendNotification(new NewNotificationMessage("Walking Style", "sucess", $"You have set your current walking style to {MovementHandler.currentStyle}!", new NewNotificationMessageContent[0]));
        }
        private void OnWalkDrunk()
        {
            MovementHandler.currentStyle = "Drunk";
            MovementHandler.WalkingStyle = "move_m@drunk@a";
            _newNotificationOverlay.SendNotification(new NewNotificationMessage("Walking Style", "sucess", $"You have set your current walking style to {MovementHandler.currentStyle}!", new NewNotificationMessageContent[0]));
        }
        private void OnWalkDrunk2()
        {
            MovementHandler.currentStyle = "Drunk2";
            MovementHandler.WalkingStyle = "move_m@buzzed";
            _newNotificationOverlay.SendNotification(new NewNotificationMessage("Walking Style", "sucess", $"You have set your current walking style to {MovementHandler.currentStyle}!", new NewNotificationMessageContent[0]));
        }
        private void OnWalkBrave()
        {
            MovementHandler.currentStyle = "Brave";
            MovementHandler.WalkingStyle = "move_m@brave";
            _newNotificationOverlay.SendNotification(new NewNotificationMessage("Walking Style", "sucess", $"You have set your current walking style to {MovementHandler.currentStyle}!", new NewNotificationMessageContent[0]));
        }
        private void OnWalkLester()
        {
            MovementHandler.currentStyle = "Lester";
            MovementHandler.WalkingStyle = "move_heist_lester";
            _newNotificationOverlay.SendNotification(new NewNotificationMessage("Walking Style", "sucess", $"You have set your current walking style to {MovementHandler.currentStyle}!", new NewNotificationMessageContent[0]));
        }
        private void OnWalkLester2()
        {
            MovementHandler.currentStyle = "Lester2";
            MovementHandler.WalkingStyle = "move_lester_caneup";
            _newNotificationOverlay.SendNotification(new NewNotificationMessage("Walking Style", "sucess", $"You have set your current walking style to {MovementHandler.currentStyle}!", new NewNotificationMessageContent[0]));
        }
        private void OnWalkArrogant()
        {
            MovementHandler.currentStyle = "Arrogant";
            MovementHandler.WalkingStyle = "move_f@arrogant@a";
            _newNotificationOverlay.SendNotification(new NewNotificationMessage("Walking Style", "sucess", $"You have set your current walking style to {MovementHandler.currentStyle}!", new NewNotificationMessageContent[0]));
        }
        private void OnWalkInjured()
        {
            MovementHandler.currentStyle = "Injured";
            MovementHandler.WalkingStyle = "move_injured_generic";
            _newNotificationOverlay.SendNotification(new NewNotificationMessage("Walking Style", "sucess", $"You have set your current walking style to {MovementHandler.currentStyle}!", new NewNotificationMessageContent[0]));
        }



        private async void OnCustomPlateCommand()
        {
            var player = Game.PlayerPed.Handle;
            var currentUserRole = _permissionService.CurrentUserRole;

            if (currentUserRole.Branch != UserBranch.Civ)
            {
                _newNotificationOverlay.SendNotification(new NewNotificationMessage("Vehicle VRN", "error", "This is a Civillian Feature!", new NewNotificationMessageContent[0]));
                return;
            }

            if (!API.IsPedInAnyVehicle(player, false))
            {
                _newNotificationOverlay.SendNotification(new NewNotificationMessage("Vehicle VRN", "error", "You must be in a vehicle to use this!", new NewNotificationMessageContent[0]));
                return;
            }

            if (API.IsPedInAnyHeli(player)) return;
            if (API.IsPedInAnyBoat(player)) return;

            var vehicleIsIn = API.GetVehiclePedIsIn(player, false);
            API.AddTextEntry("FMMC_KEY_TIP1", "INSERT VEHICE REG PLATE");
            API.DisplayOnscreenKeyboard(0, "FMMC_KEY_TIP1", "", "AB12 CDE", "", "", "", 8);

            API.UpdateOnscreenKeyboard();

            while (API.UpdateOnscreenKeyboard() == 0)
            {
                await Delay(10);
                API.UpdateOnscreenKeyboard();
            }
            if (API.UpdateOnscreenKeyboard() == 1)
            {
                string newPlate = API.GetOnscreenKeyboardResult();
                newPlate = newPlate.ToUpper();
                API.SetVehicleNumberPlateText(vehicleIsIn, newPlate);
                _newNotificationOverlay.SendNotification(new NewNotificationMessage("Vehicle VRN", "success", $"You have set your VRN to: {newPlate}", new NewNotificationMessageContent[0]));
                API.ExecuteCommand("[ADMIN WARNING] This player has changed their VRN to: '" + newPlate + "'! [ADMIN WARNING]");
            }

        }
    }
}