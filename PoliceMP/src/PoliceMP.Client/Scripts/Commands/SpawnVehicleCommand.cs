using System.Collections.Generic;
using System.Threading.Tasks;
using CitizenFX.Core;
using CitizenFX.Core.Native;
using PoliceMP.Client.Overlays.NewNotification;
using PoliceMP.Client.Services.Interfaces;
using PoliceMP.Core.Client.Commands.Interfaces;
using PoliceMP.Core.Client.Communications.Interfaces;
using PoliceMP.Core.Client.Interface;
using PoliceMP.Core.Client.Scripts;
using PoliceMP.Core.Shared;
using PoliceMP.Shared.Constants;
using PoliceMP.Shared.Constants.Decors;

namespace PoliceMP.Client.Scripts.Commands
{
    public class SpawnVehicleCommand : Script
    {
        private readonly ILogger<SpawnVehicleCommand> _logger;
        private readonly ITickManager _ticks;
        private readonly ILegacyClientCommunicationsManager _comms;
        private readonly ICommandManager _commands;
        private readonly IPermissionService _useraces;
        private readonly ISoundService _soundService;
        private readonly INewNotificationOverlay _newNotificationOverlay;
        private readonly IFeatureService _featureService;
        private readonly IGameInputManager _input;

        public static bool spawnWithELS = false;

        public SpawnVehicleCommand(ILogger<SpawnVehicleCommand> logger, ITickManager ticks,
            ILegacyClientCommunicationsManager comms,
            ICommandManager commands, IPermissionService useraces, ISoundService soundService,
            INewNotificationOverlay newNotificationOverlay, IFeatureService featureService, IGameInputManager input)
        {
            _logger = logger;
            _ticks = ticks;
            _comms = comms;
            _commands = commands;
            _useraces = useraces;
            _soundService = soundService;
            _newNotificationOverlay = newNotificationOverlay;
            _featureService = featureService;
            _input = input;
        }

        protected override async Task OnStartAsync()
        {
            _comms.On(ClientEvents.SendVehicleToClient,
                (string spawnCode, string localPlayerName, bool elsEnabled) =>
                {
                    VehicleFromServer(spawnCode, localPlayerName, elsEnabled);
                });

            _commands.Register("svtp").WithHandler(SendVehicleToPlayer);
            _commands.Register("sv").HasGreedyArgs().WithHandler(async spawnCode =>
            {
                #region Local Variables

                var isEaServer = _featureService.IsFeatureEnabled(FeatureToggle.IsEarlyAccess);
                var isDevServer = _featureService.IsFeatureEnabled(FeatureToggle.IsDevelopment);
                var userAces = await _useraces.GetUserAces();

                var player = Game.PlayerPed.Handle;
                var playerPos = API.GetEntityCoords(player, true);
                var playerHeading = API.GetEntityHeading(player);
                Vector3
                    codyRoadPos =
                        new Vector3(-598.0912f, -2170.831f,
                            6.927242f); // Location of HART Base due to controlling distance of Polaris spawn from base.

                var logSpawn = true;
                spawnWithELS = false;

                #endregion

                #region Lists / Spawn Code lists

                List<string> collegeCodes = new List<string>
                {
                    "adddrivertrain",
                    "addpolsmart",
                    "trainingambo",
                    "HGVtraining",
                    "collegeTruck",
                    "trainervolvo"
                };

                List<string> nhsCodes = new List<string>
                {
                    "hart5" // NHS Hart Polaris
                };

                List<string> policeCodes = new List<string>
                {
                    "addpolpbus" // Police CAT A Transport Bus
                };

                #endregion


                #region Vehicle Permission Management

                if (collegeCodes.Contains(spawnCode))
                {
                    if (!userAces.IsDeveloper && !userAces.IsAdmin && !userAces.IsCivCommand &&
                        !userAces.IsSeniorModerator && !userAces.IsCollegeStaff)
                    {
                        _newNotificationOverlay.SendNotification(new NewNotificationMessage("Action Blocked", "error",
                            "You do not have the required roles for this feature, Unlucky!",
                            new NewNotificationMessageContent[0]));
                        return;
                    }

                    spawnWithELS = true;
                }

                else if (nhsCodes.Contains(spawnCode))
                {
                    if (!userAces.IsDeveloper && !userAces.IsAdmin && !userAces.IsCivCommand &&
                        !userAces.IsSeniorModerator && !userAces.IsHartTrained)
                    {
                        _newNotificationOverlay.SendNotification(new NewNotificationMessage("Action Blocked", "error",
                            "You do not have the required roles for this feature, Unlucky!",
                            new NewNotificationMessageContent[0]));
                        return;
                    }

                    var distanceFromBase = API.GetDistanceBetweenCoords(playerPos.X, playerPos.Y, playerPos.Z,
                        codyRoadPos.X, codyRoadPos.Y, codyRoadPos.Z, false);
                    if (distanceFromBase > 100f && !userAces.IsDeveloper && !userAces.IsAdmin &&
                        !userAces.IsCivCommand && !userAces.IsSeniorModerator)
                    {
                        _newNotificationOverlay.SendNotification(new NewNotificationMessage("Action Blocked", "error",
                            "You are too far away from the HRT Base to spawn the Polaris!",
                            new NewNotificationMessageContent[0]));
                        return;
                    }

                    spawnWithELS = true;
                }

                else if (policeCodes.Contains(spawnCode))
                {
                    if (!userAces.IsDeveloper && !userAces.IsAdmin && !userAces.IsCivCommand &&
                        !userAces.IsSeniorModerator && !userAces.IsBandTwo)
                    {
                        _newNotificationOverlay.SendNotification(new NewNotificationMessage("Action Blocked", "error",
                            "You do not have the required roles for this feature, Unlucky!",
                            new NewNotificationMessageContent[0]));
                        return;
                    }

                    spawnWithELS = true;
                }

                else
                {
                    if (!userAces.IsDeveloper && !userAces.IsAdmin && !userAces.IsCivCommand &&
                        !userAces.IsSeniorModerator && !userAces.IsTierTwo && !userAces.IsDevFunNight)
                    {
                        _newNotificationOverlay.SendNotification(new NewNotificationMessage("Action Blocked", "error",
                            "You do not have the required roles for this feature, Unlucky!",
                            new NewNotificationMessageContent[0]));
                        return;
                    }

                    if (userAces.IsDeveloper || userAces.IsAdmin || userAces.IsDevFunNight)
                    {
                        logSpawn = false;
                    }
                }

                #endregion

                #region Loading and Spawning vehicle Model

                var modelHash = (uint)API.GetHashKey(spawnCode);

                if (!API.IsModelValid(modelHash))
                {
                    _newNotificationOverlay.SendNotification(new NewNotificationMessage("Vehicle Spawn", "error",
                        "Invalid spawncode, check spelling and try again!", new NewNotificationMessageContent[0]));
                    return;
                }

                API.RequestModel(modelHash);
                while (!API.HasModelLoaded(modelHash))
                {
                    await Delay(100);
                }

                if (API.IsPedInAnyVehicle(player, false))
                {
                    var vehicleIn = API.GetVehiclePedIsIn(player, false);
                    API.DeleteEntity(ref vehicleIn);
                }

                var newVehicle = API.CreateVehicle(modelHash, playerPos.X, playerPos.Y, playerPos.Z, playerHeading,
                    true, false);

                if (newVehicle == 0)
                {
                    _newNotificationOverlay.SendNotification(new NewNotificationMessage("Vehicle Spawn", "error",
                        "Valid spawncode used, however vehicle has failed to spawn. Retry!",
                        new NewNotificationMessageContent[0]));
                    return;
                }

                API.SetPedIntoVehicle(player, newVehicle, -1);
                _newNotificationOverlay.SendNotification(new NewNotificationMessage("Vehicle Spawn", "success",
                    $"Your shiny new {spawnCode} has been spawned!", new NewNotificationMessageContent[0]));

                if (!userAces.IsDeveloper && !userAces.IsAdmin && logSpawn)
                {
                    API.ExecuteCommand("[PoliceMP Spawn System] This player has spawned '" + spawnCode +
                                       "' [PoliceMP Spawn System]");
                }

                #endregion

                #region Spawning with ELS

                if (spawnWithELS)
                {
                    await Delay(100);
                    API.ExecuteCommand("elsme");
                    await Delay(500);

                    // Check if the spawnCode is part of nhsCodes
                    if (nhsCodes.Contains(spawnCode) && newVehicle != 0)
                    {
                        API.DecorSetInt(newVehicle, ELSDecors.SIREN_TYPE_CODE, 2); // Override siren tone to 2
                    }
                }

                logSpawn = true;
                spawnWithELS = false;

                #endregion
            });
        }

        private async void SendVehicleToPlayer()
        {
            var userAces = await _useraces.GetUserAces();
            if (!userAces.IsDeveloper && !userAces.IsAdmin) return;

            API.AddTextEntry("FMMC_KEY_TIP1", "INSERT SPAWN CODE");
            API.DisplayOnscreenKeyboard(0, "FMMC_KEY_TIP1", "", "", "", "", "", 20);
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

            var spawnCode = API.GetOnscreenKeyboardResult();
            await Delay(100);

            API.AddTextEntry("FMMC_KEY_TIP1", "INSERT PLAYER NAME");
            API.DisplayOnscreenKeyboard(0, "FMMC_KEY_TIP1", "", "", "", "", "", 20);
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

            var targetPlayerName = API.GetOnscreenKeyboardResult();
            await Delay(100);

            API.AddTextEntry("FMMC_KEY_TIP1", "SPAWN WITH ELS? 'yes'/'no'");
            API.DisplayOnscreenKeyboard(0, "FMMC_KEY_TIP1", "", "", "", "", "", 20);
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

            var elsAnswer = API.GetOnscreenKeyboardResult();
            var elsEnabled = false;
            if (elsAnswer == "yes") elsEnabled = true;
            await Delay(100);
            _comms.ToServer(ServerEvents.SendVehicleSpawnToServer, spawnCode, targetPlayerName, elsEnabled);
            _newNotificationOverlay.SendNotification(new NewNotificationMessage("Vehicle Spawn", "success",
                $"You have sent a {spawnCode} over the server to {targetPlayerName}, I will try my best to ensure it arrives safely!",
                new NewNotificationMessageContent[0]));
        }

        private async void VehicleFromServer(string spawnCode, string targetPlayerName, bool elsEnabled)
        {
            var playerID = API.PlayerId();
            var playerName = API.GetPlayerName(playerID);

            var nameMatch = playerName.Trim().Equals(targetPlayerName.Trim());
            if (!nameMatch) return;

            var player = Game.PlayerPed.Handle;
            var playerPos = API.GetEntityCoords(player, true);
            var playerHeading = API.GetEntityHeading(player);
            var modelHash = (uint)API.GetHashKey(spawnCode);

            if (!API.IsModelValid(modelHash))
            {
                return;
            }

            API.RequestModel(modelHash);
            while (!API.HasModelLoaded(modelHash))
            {
                await Delay(100);
            }

            if (API.IsPedInAnyVehicle(player, false))
            {
                var vehicleIn = API.GetVehiclePedIsIn(player, false);
                API.DeleteEntity(ref vehicleIn);
            }

            var newVehicle = API.CreateVehicle(modelHash, playerPos.X, playerPos.Y, playerPos.Z, playerHeading, true,
                false);

            if (newVehicle == 0)
            {
                return;
            }

            API.SetPedIntoVehicle(player, newVehicle, -1);
            _newNotificationOverlay.SendNotification(new NewNotificationMessage("Vehicle Spawn", "success",
                $"Your shiny new {spawnCode} has been spawned for you kindly by a Dev/Admin!",
                new NewNotificationMessageContent[0]));

            if (elsEnabled)
            {
                spawnWithELS = true;
                await Delay(100);
                API.ExecuteCommand("elsme");
                await Delay(500);
            }

            elsEnabled = false;
            spawnWithELS = false;
        }
    }
}