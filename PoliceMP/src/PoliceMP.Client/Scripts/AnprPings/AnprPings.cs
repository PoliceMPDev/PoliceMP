using System;
using System.Collections.Generic;
using System.Drawing.Text;
using System.Net;
using System.Threading.Tasks;
using CitizenFX.Core;
using CitizenFX.Core.Native;
using CitizenFX.Core.UI;
using PoliceMP.Client.Services.Interfaces;
using PoliceMP.Core.Client.Commands.Interfaces;
using PoliceMP.Core.Client.Communications.Interfaces;
using PoliceMP.Core.Client.Interface;
using PoliceMP.Core.Client.Scripts;
using PoliceMP.Core.Shared;
using PoliceMP.Shared.Constants;
using PoliceMP.Shared.Enums;
using PoliceMP.Shared.Models;

namespace PoliceMP.Client.Scripts.AnprPings
{
    public class AnprPings : Script
    {
        private readonly ILegacyClientCommunicationsManager _comms;
        private readonly ILogger<AnprPings> _logger;
        private readonly IPermissionService _permissionService;
        private readonly ICommandManager _commands;
        private readonly INotificationService _notifications;
        private readonly ITickManager _ticks;

        private UserAces _userAces;
        private string secondPart;
        private string firstPart;

        public AnprPings(ILogger<AnprPings> logger, ITickManager ticks, IPermissionService permissionService, ICommandManager commands, ILegacyClientCommunicationsManager comms, INotificationService notifications)
        {
            _logger = logger;
            _permissionService = permissionService;
            _commands = commands;
            _comms = comms;
            _notifications = notifications;
            _ticks = ticks;
        }


        protected override async Task OnStartAsync()
        {
            /*
            _comms.On(ClientEvents.AnprPingResponse, (Vector3 targetCoords, string targetPlate, string requesterName) =>
            {
                AnprPingResponse(targetCoords, targetPlate, requesterName);
            });

            _comms.On(ClientEvents.CheckForPlateOnClient, (string requesterName, string targetPlate) =>
            {
                CheckForPlateOnClient(requesterName, targetPlate);
            });
            */

            //_commands.Register("pinganpr").WithHandler(OnPingedANPR);
        }

        /*
        private async void OnPingedANPR()
        {
            var currentUserRole = _permissionService.CurrentUserRole;
            var player = Game.PlayerPed.Handle;

            if (currentUserRole.Branch != UserBranch.Police && currentUserRole.Branch != UserBranch.Control)
            {
                _notifications.Error("ANPR System", "Only Police and Control can use the ANPR System!");
                return;
            }

            if (currentUserRole.Branch != UserBranch.Control && !API.IsPedInAnyVehicle(player, false))
            {
                _notifications.Error("ANPR System", "You must be in a vehicle to use the ANPR System!");
                return;
            }

            if (currentlyPinging && currentUserRole.Branch != UserBranch.Control)
            {
                _notifications.Error("ANPR System", "Please wait until your current ANPR Ping has cleared before using again!");
                return;
            }

            var playerID = API.PlayerId();
            var requesterName = API.GetPlayerName(playerID);

            API.AddTextEntry("FMMC_KEY_TIP1", "INSERT TARGET REG PLATE");
            API.DisplayOnscreenKeyboard(0, "FMMC_KEY_TIP1", "", firstPart, secondPart, "", "", 10);
            API.UpdateOnscreenKeyboard();

            while (API.UpdateOnscreenKeyboard() == 0)
            {
                await Delay(10);
                API.UpdateOnscreenKeyboard();
            }
            if (API.UpdateOnscreenKeyboard() != 1)
            {
                return;
            }

            var targetPlate = API.GetOnscreenKeyboardResult();
            targetPlate = targetPlate.ToUpper();

            var parts = targetPlate.Split(' ');
            if (parts.Length == 2)
            {
                firstPart = parts[0] + " ";
                secondPart = parts[1];
            }

            _notifications.Info("ANPR System", $"Searching ANPR Cameras for a hit on {targetPlate}!");
            if (currentUserRole.Branch == UserBranch.Control) await Delay(3000);
            _comms.ToServer(ServerEvents.AnprPingRequest, targetPlate, requesterName);

            await Delay(7500);
            if (!plateFound)
            {
                _notifications.Error("ANPR System", $"No hit on {targetPlate} was found, this vehicle could have their plate obstructed or it has avoided ANPR Cameras!");

                API.PlaySoundFrontend(-1, "Hack_Failed", "DLC_HEIST_BIOLAB_PREP_HACKING_SOUNDS", false);
                API.PlaySoundFrontend(-1, "Hack_Failed", "DLC_HEIST_BIOLAB_PREP_HACKING_SOUNDS", false);
                API.PlaySoundFrontend(-1, "Hack_Failed", "DLC_HEIST_BIOLAB_PREP_HACKING_SOUNDS", false);

                return;
            }
        }


        private async void CheckForPlateOnClient(string targetPlate, string requesterName)
        {

            try
            {
                foreach (var vehicle in World.GetAllVehicles())
                {
                    var vehicleHandle = vehicle.Handle;
                    var currentPlate = API.GetVehicleNumberPlateText(vehicleHandle);

                    if (currentPlate == null) continue;
                    await Delay(10);
                    var plateMatch = targetPlate.Trim().Equals(currentPlate.Trim()); ;

                    if (!plateMatch) continue;

                    Vector3 targetCoords = API.GetEntityCoords(vehicleHandle, true);
                    _comms.ToServer(ServerEvents.AnprPingSendCoords, targetCoords, targetPlate, requesterName);

                    return;
                }
            }
            catch
            {
                return;
            }
        }

        private int recentBlip;
        private int recentRadiusBlip;
        private bool plateFound = false;
        private bool currentlyPinging = false;
        private Vector3 currentTargetCoords = new Vector3(0, 0, 0);

        private async void AnprPingResponse(Vector3 targetCoords, string targetPlate, string requesterName)
        {
            var random = new Random();

            try
            {
                var playerID = API.PlayerId();
                var playerName = API.GetPlayerName(playerID);

                var nameMatch = playerName.Trim().Equals(requesterName.Trim());

                if (!nameMatch) return;

                var blipExists = API.DoesBlipExist(recentBlip);
                var radiusBlipExists = API.DoesBlipExist(recentRadiusBlip);

                currentlyPinging = true;

                plateFound = true;
                if (blipExists && currentTargetCoords == targetCoords)
                {
                    API.RemoveBlip(ref recentBlip);
                    API.DeleteEntity(ref recentBlip);
                }
                if (radiusBlipExists && currentTargetCoords == targetCoords)
                {
                    API.RemoveBlip(ref recentRadiusBlip);
                    API.DeleteEntity(ref recentRadiusBlip);
                }

                currentTargetCoords = targetCoords;

                await Delay(1000);

                var offsetBlipRadius = 125f;
                var offsetX = (float)(random.NextDouble() * 2 * offsetBlipRadius - offsetBlipRadius);
                var offsetY = (float)(random.NextDouble() * 2 * offsetBlipRadius - offsetBlipRadius);

                targetCoords += new Vector3(offsetX, offsetY, 0);

                var pingRadiusBlip = API.AddBlipForRadius(targetCoords.X, targetCoords.Y, targetCoords.Z, 160f);
                var pingBlip = API.AddBlipForCoord(targetCoords.X, targetCoords.Y, targetCoords.Z);

                recentRadiusBlip = pingRadiusBlip;
                recentBlip = pingBlip;

                API.SetBlipColour(pingRadiusBlip, 29);
                API.SetBlipAlpha(pingRadiusBlip, 195);
                API.SetBlipSprite(pingBlip, 432);
                API.SetBlipScale(pingBlip, 2f);
                API.SetBlipColour(pingBlip, 1);
                API.SetBlipAlpha(pingBlip, 255);
                API.BeginTextCommandSetBlipName("STRING");
                API.AddTextComponentString($"ANPR Hit: {targetPlate}");
                API.EndTextCommandSetBlipName(pingBlip);

                API.PlaySoundFrontend(-1, "Found_Target", "POLICE_CHOPPER_CAM_SOUNDS", false);
                API.PlaySoundFrontend(-1, "Found_Target", "POLICE_CHOPPER_CAM_SOUNDS", false);
                API.PlaySoundFrontend(-1, "Found_Target", "POLICE_CHOPPER_CAM_SOUNDS", false);
                _notifications.Info("ANPR System", $"{targetPlate} has been found on ANPR and has been pinged on your map!");

                await Delay(35000);

                API.SetBlipFlashes(pingRadiusBlip, true);
                API.SetBlipFlashes(pingBlip, true);

                await Delay(5000);

                API.RemoveBlip(ref pingRadiusBlip);
                API.DeleteEntity(ref pingRadiusBlip);
                API.RemoveBlip(ref pingBlip);
                API.DeleteEntity(ref pingBlip);
                plateFound = false;
                currentlyPinging = false;
                currentTargetCoords = new Vector3(0, 0, 0);

            }
            catch
            {
                return;
            }
        }
        */
    }
}