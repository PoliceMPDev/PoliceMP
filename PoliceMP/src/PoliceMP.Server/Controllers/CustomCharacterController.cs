using System;
using System.Threading.Tasks;
using CitizenFX.Core;
using CitizenFX.Core.Native;
using Microsoft.EntityFrameworkCore.Internal;
using PoliceMP.Client.Services.Interfaces;
using PoliceMP.Core.Server.Commands.Interfaces;
using PoliceMP.Core.Server.Communications.Interfaces;
using PoliceMP.Core.Server.Interfaces.Services;
using PoliceMP.Core.Server.Networking;
using PoliceMP.Core.Shared;
using PoliceMP.Core.Shared.Communications.Interfaces;
using PoliceMP.Shared.Constants;
using PoliceMP.Shared.Models;

namespace PoliceMP.Server.Controllers
{
    public class CustomCharacterController : Controller
    {
        private readonly ILegacyServerCommunicationsManager _comms;
        private readonly ICommandManager _commands;
        private readonly IFiveEventManager _fiveEvent;
        private readonly ILogger<CustomCharacterController> _logger;
        private readonly PlayerList _playerList;
        private readonly IPermissionService _permission;
        private readonly INotificationService _notification;

        public CustomCharacterController(
            ILegacyServerCommunicationsManager comms,
            ICommandManager commands,
            IFiveEventManager fiveEvent,
            ILogger<CustomCharacterController> logger,
            PlayerList playerList,
            IPermissionService permission // ✅ add this
        )

        {
            _logger = logger;
            _comms = comms;
            _fiveEvent = fiveEvent ?? throw new ArgumentNullException(nameof(fiveEvent));
            _playerList = playerList;
            _commands = commands;
            _permission = permission;
            _comms.On<Player, int>(ServerEvents.StartCharacterCustomisation, OnStartCustomisation);
            _comms.On<Player, string>(ServerEvents.SetPlayerCharacterCustomisation, OnSetAppearance);

            fiveEvent.On<Player, string>("charcreator:charactercreated", OnCharacterCreated);
            fiveEvent.On<Player, string>("charcreator:appearance:applied", OnCharacterApplied);
            if (_commands != null)
            {
                _commands.Register("wipeoutfits").WithHandler(async (player, args) => await HandleDeleteKvp(player, args));
            }
            else
            {
                _logger.Warn("ICommandManager is null! Command /wipeoutfits not registered.");
            }
        }
        private async Task HandleDeleteKvp(Player sourcePlayer, string args)
        {
            var aces = await _permission.GetUserAces(sourcePlayer);
            if (!aces.IsDeveloper && !aces.IsAdmin) return;

            if (!int.TryParse(args, out int targetId))
            {
                _comms.ToClient(sourcePlayer, "client:notify:kvpError", "Usage: /wipeoutfits <player_id>");
                return;
            }

            var targetPlayer = _playerList[targetId];
            if (targetPlayer == null || string.IsNullOrWhiteSpace(targetPlayer.Name))
            {
                _comms.ToClient(sourcePlayer, "client:notify:kvpError", $"No player found with ID {targetId}");
                return;
            }

            _comms.ToClient(targetPlayer, "client:delete:kvp");
            _comms.ToClient(sourcePlayer, "client:notify:kvpSuccess", $"Triggered outfits deletion for player {targetPlayer.Name} (ID {targetId})");

        }

        public override Task Started()
        {
            return Task.FromResult(0);
        }

        private async Task OnCharacterApplied([FromSource] Player player, string json)
        {
            _comms.ToClient(player, ServerEvents.OnCharacterAppearanceApplied);
        }

        private async Task OnCharacterCreated([FromSource] Player player, string json)
        {
            _logger.Debug($"Custom Character saved for: {player.Name}");
            _comms.ToClient(player, ServerEvents.OnFinishCharacterCustomisation, json);
        }

        private async Task OnStartCustomisation(Player player, int gender)
        {
            Exports["character-creator"].startCreation(player.Name, gender);
        }

        private void OnSetAppearance(Player player, string appearance)
        {
            Exports["character-creator"].applyAppearanace(player.Name, appearance);
        }
    }
}