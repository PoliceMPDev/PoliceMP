using System.Drawing.Text;
using System.Threading.Tasks;
using CitizenFX.Core;
using CitizenFX.Core.Native;
using CitizenFX.Core.UI;
using PoliceMP.Client.Overlays.NewNotification;
using PoliceMP.Client.Services.Interfaces;
using PoliceMP.Core.Client.Commands.Interfaces;
using PoliceMP.Core.Client.Communications.Interfaces;
using PoliceMP.Core.Client.Interface;
using PoliceMP.Core.Client.Scripts;
using PoliceMP.Core.Shared;
using PoliceMP.Shared.Constants;
using PoliceMP.Shared.Enums;
using PoliceMP.Shared.Models;

namespace PoliceMP.Client.Scripts.RoleplayCommands
{
    public class RoleplayCommands : Script
    {
        private readonly ILegacyClientCommunicationsManager _comms;
        private readonly ILogger<RoleplayCommands> _logger;
        private readonly IPermissionService _permissionService;
        private readonly ICommandManager _commands;
        private readonly INotificationService _notifications;
        private readonly ITickManager _ticks;
        private readonly ISpeechService _speech;
        private readonly INewNotificationOverlay _newNotificationOverlay;
        private readonly IGameInputManager _input;
        
        
        public RoleplayCommands(ILogger<RoleplayCommands> logger, ITickManager ticks, ISpeechService speech, IPermissionService permissionService, ICommandManager commands, ILegacyClientCommunicationsManager comms, INotificationService notifications, INewNotificationOverlay newNotificationOverlay, IGameInputManager input)
        {
            _logger = logger;
            _permissionService = permissionService;
            _commands = commands;
            _comms = comms;
            _notifications = notifications;
            _ticks = ticks;
            _speech = speech;
            _newNotificationOverlay = newNotificationOverlay;
            _input = input;
            
        }

        protected override Task OnStartAsync()
        {
            _comms.On(ClientEvents.SendNewScentToClient, (string message, string playerName, Vector3 civPosition) => 
            { 
                ScentFromServer(message, playerName, civPosition);
            });
            
            
            _ticks.On(Ragdoll);
            _ticks.On(FlipVehicle);

            _commands.Register("createscent").WithHandler(OnNewScent);
            
            _commands.Register("me").HasGreedyArgs().WithHandler((message) =>
            {
                try
                {
                    if (string.IsNullOrEmpty(message) || message == "" || message == string.Empty)
                    {
                        _notifications.Error("Command Error", "/me [Emote Text]");
                        return;
                    }
                    _comms.ToServer(ServerEvents.SendMeCommandToServer, message);
                }
                catch
                {
                    return;
                }
            });

            _commands.Register("do").HasGreedyArgs().WithHandler((message) =>
            {
                try
                {
                    if (string.IsNullOrEmpty(message) || message == "" || message == string.Empty)
                    {
                        _notifications.Error("Command Error", "/do [Emote Text]");
                        return;
                    }
                    _comms.ToServer(ServerEvents.SendDoCommandToServer, message);
                }
                catch
                {
                    return;
                }
            });

            _commands.Register("kill").WithHandler(async () =>
            {
                var userAces = await _permissionService.GetUserAces();
                var currentUserRole = _permissionService.CurrentUserRole;
                if (currentUserRole.Branch == UserBranch.Civ)
                {
                    _notifications.Error("Kill Command", "Stop right there!! Civs can not use this command.");
                    return;
                }
        
                try
                {
                    API.SetEntityHealth(Game.PlayerPed.Handle, 0);
                    API.ExecuteCommand(
                        "[ADMIN WARNING] ------------------------------------------- [ADMIN WARNING]");
                    API.ExecuteCommand(
                        "[ADMIN WARNING] THIS PLAYER HAS JUST USED THE /KILL COMMAND [ADMIN WARNING]");
                    API.ExecuteCommand(
                        "[ADMIN WARNING] ------------------------------------------- [ADMIN WARNING]");
                }
                catch
                {
                    return;
                }
            });

            return Task.FromResult(0);

        }

        private async void OnNewScent()
        {
            var userAces = await _permissionService.GetUserAces();
            var currentUserRole = _permissionService.CurrentUserRole;
            if (currentUserRole.Branch != UserBranch.Civ & !userAces.IsDeveloper)
            {
                _notifications.Error("Create Scent", "You must be a civ to use this!");
                return;
            }
            
            API.AddTextEntry("FMMC_KEY_TIP1", "What the Dog picks up");
            API.DisplayOnscreenKeyboard(0, "FMMC_KEY_TIP1", "", "", "", "", "", 60);

            API.UpdateOnscreenKeyboard();

            while (API.UpdateOnscreenKeyboard() == 0)
            {
                API.DisableControlAction(0, (int)Control.MpTextChatAll, true); // stop text chat input :)
                await Delay(10);
                API.UpdateOnscreenKeyboard();
            }
            
            API.EnableControlAction(0, (int)Control.MpTextChatAll, true);

            var player = Game.PlayerPed.Handle;
            var civPosition = API.GetEntityCoords(player, false);
            
            var playerID = API.PlayerId();
            var playerName = API.GetPlayerName(playerID);
            
            var newMessage = API.GetOnscreenKeyboardResult();
            _comms.ToServer(ServerEvents.CreateScent, newMessage, playerName, civPosition);
            Debug.WriteLine($"Sent to server, message being {newMessage}!");
            Debug.WriteLine($"Sent to server, playerName being {playerName}!");
            Debug.WriteLine($"Sent to server, civPosition being {civPosition}!");

        }

        private async void ScentFromServer(string message, string playerName, Vector3 civPosition)
        {
            var currentUserRole = _permissionService.CurrentUserRole;
            if (currentUserRole.Division != UserDivision.Dsu) return;
            var localPlayer = Game.PlayerPed.Handle;
            var localPlayerPos = API.GetEntityCoords(localPlayer, false);
            var distanceFromCiv = Vector3.Distance(localPlayerPos, civPosition);
            if (distanceFromCiv > 50f) return;

            Ped closestDog = null;
            var closestDistance = 15f;
            Vector3 dogPos = new Vector3(0, 0, 0);

            var allPeds = World.GetAllPeds();
            foreach (var ped in allPeds)
            {
                if (ped.Model == (Model)"a_c_shepherd" || ped.Model == (Model)"a_c_retriever")
                {
                    dogPos = ped.Position;
                    float distanceToPed = Vector3.Distance(localPlayerPos, dogPos);
                    if (distanceToPed < closestDistance)
                    {
                        closestDog = ped;
                        closestDistance = distanceToPed;
                    }
                }
            }

            if (closestDog == null) return;

            var direction = civPosition - dogPos;
            direction.Normalize();

            _speech.Say(closestDog, $"~r~*Indicates: {message}*", 25000);

            for (int tickCount = 0; tickCount < 1500; tickCount++)
            {
                dogPos = closestDog.Position;
                direction = civPosition - dogPos;
                direction.Normalize();
                await Delay(2);
                API.DrawMarker(20, dogPos.X, dogPos.Y, dogPos.Z + 1.0f, direction.X, direction.Y, direction.Z, 90f, 0f,
                    0f, 0.55f, 0.55f, 1f, 255, 0, 0, 110, false, false, 2, false, null, null, false);
            }
        }
        

        private async Task Ragdoll()
        {
            var currentUserRole = _permissionService.CurrentUserRole;

            if (API.IsControlPressed(0, 168)) // F7 Key
            {
                if (currentUserRole == null) 
                {
                    await Delay(1000);
                    return;
                }

                if (currentUserRole.Branch != UserBranch.Civ)
                {
                    return;
                }

                API.SetPedToRagdoll(Game.PlayerPed.Handle, 1000, 1000, 0, true, true, false);
                await Delay(10);
            }
        }
        private async Task FlipVehicle()
        {
            var currentUserRole = _permissionService.CurrentUserRole;

            // Ensure only keyboard input triggers this
            if (API.IsInputDisabled(2)) 
            {
                if (_input.IsBeingHeld(Control.VehicleDuck, holdMs: 1000))
                {
                    if (currentUserRole == null)
                    {
                        await Delay(1000);
                        return;
                    }

                    if (currentUserRole.Branch != UserBranch.Civ)
                    {
                        return;
                    }

                    var playerPed = Game.PlayerPed;
                    var vehicle = API.GetVehiclePedIsIn(playerPed.Handle, false);

                    if (vehicle != 0 && API.DoesEntityExist(vehicle))
                    {
                        int vehicleClass = API.GetVehicleClass(vehicle);
                        
                        if (vehicleClass == 8 || vehicleClass == 13)
                        {
                            return;
                        }

                        Vector3 vehicleRotation = API.GetEntityRotation(vehicle, 2);
                        Vector3 flippedRotation = new Vector3(vehicleRotation.X + 180.0f, vehicleRotation.Y, vehicleRotation.Z + 180.0f); // Flips upright
                        API.SetEntityRotation(vehicle, flippedRotation.X, flippedRotation.Y, flippedRotation.Z, 2, true);
                        _newNotificationOverlay.SendNotification(new NewNotificationMessage(
                            "Vehicle", "success", "Vehicle Flip Successful!", new NewNotificationMessageContent[0]
                        ));
                        await Delay(1000);
                    }
                }
            }
        }

    }
}