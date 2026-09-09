using System;
using System.Diagnostics.Eventing.Reader;
using System.Drawing;
using System.Drawing.Imaging;
using System.Net;
using System.Threading.Tasks;
using CitizenFX.Core;
using CitizenFX.Core.Native;
using CitizenFX.Core.UI;
using PoliceMP.Client.Actions.Follow;
using PoliceMP.Client.Actions.HandsUp;
using PoliceMP.Client.Overlays.NewNotification;
using PoliceMP.Client.Scripts.Admin;
using PoliceMP.Client.Services.Interfaces;
using PoliceMP.Core.Client;
using PoliceMP.Core.Client.Commands.Interfaces;
using PoliceMP.Core.Client.Interface;
using PoliceMP.Core.Client.Scripts;
using PoliceMP.Shared.Enums;
using PoliceMP.Shared.Models;

namespace PoliceMP.Client.Scripts.LorryWeighScript
{
    public class LorryWeighScript : Script
    {
        private readonly ICommandManager _commandManager;
        private readonly ITickManager _ticks;
        private readonly IPermissionService _permissionService;
        private readonly INewNotificationOverlay _newNotificationOverlay;

        public LorryWeighScript(ICommandManager commandManager, ITickManager ticks, IPermissionService permissionService, INewNotificationOverlay newNotificationOverlay)
        {
            _commandManager = commandManager;
            _newNotificationOverlay = newNotificationOverlay;
            _permissionService = permissionService;
            _ticks = ticks;
        }

        protected override async Task OnStartAsync()
        {
            _ticks.On(CheckWeighPrompt);
            _ticks.Off(ShowWeighPrompt);

            API.DecorRegister("VehicleWeight", 3);

            _commandManager.Register("setvehicleweight").WithHandler(OnSetVehicleWeight);
        }

        private async void OnSetVehicleWeight()
        {
            var player = Game.PlayerPed.Handle;
            var currentUserRole = _permissionService.CurrentUserRole;
            if (currentUserRole.Branch != UserBranch.Civ) return;
            var vehicle = API.GetVehiclePedIsIn(player, false);
            var vehicleClass = API.GetVehicleClass(vehicle);

            API.AddTextEntry("FMMC_KEY_TIP1", "Set HGV Vehicle Weight (in Kg)");
            API.DisplayOnscreenKeyboard(0, "FMMC_KEY_TIP1", "", "", "", "", "", 8);

            API.UpdateOnscreenKeyboard();

            while (API.UpdateOnscreenKeyboard() == 0)
            {
                await Delay(10);
                API.UpdateOnscreenKeyboard();
            }
            var stringWeight = API.GetOnscreenKeyboardResult();

            if (vehicleClass != 11 && vehicleClass != 20)
            {
                _newNotificationOverlay.SendNotification(new NewNotificationMessage("Vehicle Weight", "error", "You must be in a commercial vehicle to set its weight!", new NewNotificationMessageContent[0]));
                return;
            }

            if (!Int32.TryParse(stringWeight, out int intWeight))
            {
                _newNotificationOverlay.SendNotification(new NewNotificationMessage("Vehicle Weight", "error", "Error converting your input into a weight! Try again!", new NewNotificationMessageContent[0]));
                return;
            }

            var isLegal = intWeight <= 38000;
            var legalResult = "This makes your vehicle overweight/illegal!";
            if (isLegal) legalResult = "This makes your vehicle underweight/legal!";
            API.DecorSetInt(vehicle, "VehicleWeight", intWeight);
            _newNotificationOverlay.SendNotification(new NewNotificationMessage("Vehicle Weight", "success", $"You have set your vehicle weight to {intWeight}! {legalResult}", new NewNotificationMessageContent[0]));
        }


        private Vector3 weightPoint = new Vector3(1534.7178955078f, 847.38360595703f, 78.387496948242f);
        private Vector3 testPoint = new Vector3(1548.3946533203f, 848.69561767578f, 77.700820922852f);
        private bool usingWeigh = false;

        #region Weighing HGV Script
        private async Task CheckWeighPrompt()
        {
            await Delay(250);
            var currentUserRole = _permissionService.CurrentUserRole;
            if (currentUserRole == null) return;
            if (currentUserRole.Branch != UserBranch.Highways && currentUserRole.Division != UserDivision.Rpu)
            {
                _ticks.Off(ShowWeighPrompt);
                return;
            }
            var player = Game.PlayerPed.Handle;
            var playerPos = API.GetEntityCoords(player, true);
            var distanceFromWeightPoint = API.GetDistanceBetweenCoords(playerPos.X, playerPos.Y, playerPos.Z, testPoint.X, testPoint.Y, testPoint.Z, false);
            if (distanceFromWeightPoint > 3f)
            {
                _ticks.Off(ShowWeighPrompt);
                return;
            }
            _ticks.On(ShowWeighPrompt);
        }

        private async Task ShowWeighPrompt()
        {
            await Delay(7);
            Screen.DisplayHelpTextThisFrame("Press ~INPUT_PICKUP~ to use VOSA Weigh Station!");
            if (API.IsControlJustReleased(0, 46)) OnWeighVehicle();
        }

        private async void OnWeighVehicle()
        {
            if (usingWeigh) return;

            var player = Game.PlayerPed.Handle;
            var playerPos = API.GetEntityCoords(player, true);

            if (API.IsPedInAnyVehicle(player, true))
            {
                _newNotificationOverlay.SendNotification(new NewNotificationMessage("Weigh Vehicle", "error", "You must not be in a vehicle to read the weight!", new NewNotificationMessageContent[0]));
                return;
            }

            var vehicles = World.GetAllVehicles();
            var minDistance = float.MaxValue;
            var closestVehicle = 0;

            foreach (var vehicle in vehicles)
            {
                var vehiclePos = API.GetEntityCoords(vehicle.Handle, true);
                float distance = API.GetDistanceBetweenCoords(weightPoint.X, weightPoint.Y, weightPoint.Z, vehiclePos.X, vehiclePos.Y, vehiclePos.Z, false);

                if (distance < minDistance)
                {
                    minDistance = distance;
                    closestVehicle = vehicle.Handle;
                }
            }

            var vehicleClass = API.GetVehicleClass(closestVehicle);
            if (vehicleClass != 11 && vehicleClass != 20)
            {
                _newNotificationOverlay.SendNotification(new NewNotificationMessage("Weigh Vehicle", "error", "The weigh point cannot detect any commercial vehicle!", new NewNotificationMessageContent[0]));
                return;
            }

            API.PlaySoundFrontend(-1, "Found_Target", "POLICE_CHOPPER_CAM_SOUNDS", false);
            API.PlaySoundFrontend(-1, "Found_Target", "POLICE_CHOPPER_CAM_SOUNDS", false);
            API.PlaySoundFrontend(-1, "Found_Target", "POLICE_CHOPPER_CAM_SOUNDS", false);

            usingWeigh = true;

            if (API.DecorExistOn(closestVehicle, "VehicleWeight"))
            {
                var weight = API.DecorGetInt(closestVehicle, "VehicleWeight");

                API.ExecuteCommand("e clipboard");
                await Delay(500);
                Screen.ShowSubtitle("~y~Checking weight of vehicle...", 15000);
                await Delay(15000);

                API.PlaySoundFrontend(-1, "Found_Target", "POLICE_CHOPPER_CAM_SOUNDS", false);
                API.PlaySoundFrontend(-1, "Found_Target", "POLICE_CHOPPER_CAM_SOUNDS", false);
                API.PlaySoundFrontend(-1, "Found_Target", "POLICE_CHOPPER_CAM_SOUNDS", false);

                var isLegalWeight = weight <= 38000;

                if (isLegalWeight)
                {
                    Screen.ShowSubtitle($"~y~Vehicle weight is: ~g~{weight}kg! ~y~This is a ~g~legal ~y~weight!", 15000);
                    await Delay(500);
                    API.ExecuteCommand("e c");
                    usingWeigh = false;
                    return;
                }

                Screen.ShowSubtitle($"~y~Vehicle weight is: ~r~{weight}kg! ~y~This is an ~r~illegal ~y~weight!", 15000);
                await Delay(500);
                API.ExecuteCommand("e c");
                usingWeigh = false;
                return;
            }

            var random = new Random();
            var weightKg = random.Next(1000, 50001);
            var isLegal = weightKg <= 38000;

            API.ExecuteCommand("e clipboard");
            await Delay(500);
            Screen.ShowSubtitle("~y~Checking weight of vehicle...", 15000);
            await Delay(15000);
            
            API.PlaySoundFrontend(-1, "Found_Target", "POLICE_CHOPPER_CAM_SOUNDS", false);
            API.PlaySoundFrontend(-1, "Found_Target", "POLICE_CHOPPER_CAM_SOUNDS", false);
            API.PlaySoundFrontend(-1, "Found_Target", "POLICE_CHOPPER_CAM_SOUNDS", false);

            if (isLegal)
            {
                Screen.ShowSubtitle($"~y~Vehicle weight is: ~g~{weightKg}kg! ~y~This is a ~g~legal ~y~weight!", 15000);
                await Delay(500);
                API.ExecuteCommand("e c");
                usingWeigh = false;
                return;
            }

            Screen.ShowSubtitle($"~y~Vehicle weight is: ~r~{weightKg}kg! ~y~This is an ~r~illegal ~y~weight!", 15000);
            await Delay(500);
            API.ExecuteCommand("e c");
            usingWeigh = false;
        }
        #endregion
    }
}