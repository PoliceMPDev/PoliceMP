using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.Drawing;
using System.Drawing.Imaging;
using System.Net;
using System.Threading.Tasks;
using CitizenFX.Core;
using CitizenFX.Core.Native;
using CitizenFX.Core.UI;
using PoliceMP.Client.Scripts.Admin;
using PoliceMP.Client.Scripts.ELS;
using PoliceMP.Client.Services.Interfaces;
using PoliceMP.Core.Client;
using PoliceMP.Core.Client.Commands.Interfaces;
using PoliceMP.Core.Client.Communications.Interfaces;
using PoliceMP.Core.Client.Extensions;
using PoliceMP.Core.Client.Interface;
using PoliceMP.Core.Client.Scripts;
using PoliceMP.Core.Shared;
using PoliceMP.Shared.Constants;
using PoliceMP.Shared.Constants.States;
using PoliceMP.Shared.Enums;
using PoliceMP.Shared.Models;

namespace PoliceMP.Client.Scripts.RemoteBlips
{
    public class RemoteBlips : Script
    {
        private readonly ILogger<RemoteBlips> _logger;
        private readonly ITickManager _ticks;
        private readonly ILegacyClientCommunicationsManager _comms;
        private readonly ICommandManager _commands;
        private readonly IPermissionService _useraces;
        private readonly ISoundService _soundService;
        private readonly INotificationService _notification;
        private readonly IFeatureService _featureService;
        private readonly IGameInputManager _input;

        public RemoteBlips(ILogger<RemoteBlips> logger, ITickManager ticks, ILegacyClientCommunicationsManager comms,
            ICommandManager commands, IPermissionService useraces, ISoundService soundService, INotificationService notificationService, IFeatureService featureService, IGameInputManager input)
        {
            _logger = logger;
            _ticks = ticks;
            _comms = comms;
            _commands = commands;
            _useraces = useraces;
            _soundService = soundService;
            _notification = notificationService;
            _featureService = featureService;
            _input = input;
        }

        protected override async Task OnStartAsync()
        {
            _comms.On(ClientEvents.RemoteHideBlipsToClient, (string targetPlayerName) =>
            {
                RemoteHideBlipsToClient(targetPlayerName);
            });

            _comms.On(ClientEvents.RemoteShowBlipsToClient, (string targetPlayerName) =>
            {
                RemoteShowBlipsToClient(targetPlayerName);
            });

            _commands.Register("remotehideblips").WithHandler(OnRemoteHideBlips);
            _commands.Register("remoteshowblips").WithHandler(OnRemoteShowBlips);
        }

        private async void OnRemoteHideBlips()
        {
            var userAces = await _useraces.GetUserAces();
            if (!userAces.IsDeveloper && !userAces.IsAdmin && !userAces.IsCivCommand) return;

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

            _comms.ToServer(ServerEvents.RemoteHideBlipsToServer, targetPlayerName);
            _notification.Success("Remote Hide Blips", $"You have hidden {targetPlayerName}'s blips! Remember to show them again using '/remoteshowblips'!");
        }

        private async void OnRemoteShowBlips()
        {
            var userAces = await _useraces.GetUserAces();
            if (!userAces.IsDeveloper && !userAces.IsAdmin) return;

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

            _comms.ToServer(ServerEvents.RemoteShowBlipsToServer, targetPlayerName);
            _notification.Success("Remote Hide Blips", $"You have shown {targetPlayerName}'s blips!");
        }


        private async void RemoteHideBlipsToClient(string targetPlayerName)
        {
            var playerID = API.PlayerId();
            var playerName = API.GetPlayerName(playerID);
            var nameMatch = playerName.Trim().Equals(targetPlayerName.Trim());
            if (!nameMatch) return;

            Game.Player.State.Set(PlayerStates.HideBlipState, true, true);
            _notification.Success("Remote Hide Blips", $"Your blips have been remotely hidden by a Dev/Admin!");
        }

        private async void RemoteShowBlipsToClient(string targetPlayerName)
        {
            var playerID = API.PlayerId();
            var playerName = API.GetPlayerName(playerID);
            var nameMatch = playerName.Trim().Equals(targetPlayerName.Trim());
            if (!nameMatch) return;

            Game.Player.State.Set(PlayerStates.HideBlipState, false, true);
            _notification.Success("Remote Hide Blips", $"Your blips have been remotely shown by a Dev/Admin!");
        }
    }
}