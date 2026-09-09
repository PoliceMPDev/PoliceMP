using CitizenFX.Core;
using PoliceMP.Core.Server.Commands.Interfaces;
using PoliceMP.Core.Server.Communications.Interfaces;
using PoliceMP.Core.Server.Interfaces.Services;
using PoliceMP.Core.Server.Networking;
using PoliceMP.Core.Shared;
using PoliceMP.Server.Services;
using PoliceMP.Shared.Constants;
using System.Threading;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PoliceMP.Client.Services.Interfaces;

namespace PoliceMP.Server.Controllers.ControlNotify
{
    public class ControlNotifyController : Controller
    {
        private readonly ILogger<ControlNotifyController> _logger;
        private readonly ICommandManager _commands;
        private readonly ILegacyServerCommunicationsManager _comms;
        private readonly INotificationService _notifications;
        private readonly IPermissionService _perms;

        private readonly PlayerList _players;

        private bool _isControlActive = false;
        private bool _isCROActive = false;

        private readonly Dictionary<Player, bool> _modChatPreferences = new Dictionary<Player, bool>();

        private readonly float ChatDistance = 10f;

        public ControlNotifyController(ILogger<ControlNotifyController> logger,
            ICommandManager commands,
            ILegacyServerCommunicationsManager comms,
            INotificationService notifications,
            IPermissionService perms,

            PlayerList players)
        {
            _logger = logger;
            _comms = comms;
            commands.Register("togglemodc").WithHandler(ToggleModChatCommand);
            _perms = perms;
            _notifications = notifications;
            _players = players;
        }

        public override Task Started()
        {
            _comms.On(ServerEvents.SendControlNotificationToServer, (Player player, string message) =>
            {
                ControlNotify(player, message);
            });

            _comms.On(ServerEvents.ControlStatus, async (Player _, bool isActive) =>
            {
                _isControlActive = isActive;
                foreach (var ped in _players.ToArray())
                {
                    _comms.ToClient(ped, ClientEvents.ControlStatus, isActive);
                    await BaseScript.Delay(10);
                }
            });

            _comms.On(ServerEvents.CROStatus, async (Player _, bool isActive) =>
            {
                _isCROActive = isActive;
                foreach (var ped in _players.ToArray())
                {
                    _comms.ToClient(ped, ClientEvents.CROStatus, isActive);
                    await BaseScript.Delay(10);
                }
            });

            _comms.On(ServerEvents.SendModMessageToServer, (Player player, string message, string playerName) =>
            {
                ModMessage(player, message, playerName);
            });
            
            _comms.OnRequest(ServerEvents.GetControlStatus, () => Task.FromResult(_isControlActive));

            _comms.OnRequest(ServerEvents.GetCROStatus, () => Task.FromResult(_isCROActive));

            return Task.FromResult(0);
        }

        private void ControlNotify(Player player, string message)
        {
            try
            {
                TriggerEvent("txaLogger:CommandExecuted", $"[CONTROL NOTIFY] {player.Name} has notified units of recent Channel Status!");

                foreach (var ped in _players.ToArray())
                {
                    if (ped == null || ped.Character == null) continue;

                    string formattedString = $"~r~[CONTROL STATUS] 🔊: {message}";
                    ped.TriggerEvent("chat:addMessage", formattedString);
                }
                return;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return;
            }
        }

        private async void ModMessage(Player player, string message, string playerName)
        {
            TriggerEvent("txaLogger:CommandExecuted", $"[MOD CHAT] {playerName} Sent mod chat: ''{message}''!");
            var playerAces = await _perms.GetUserAces(player);
            if (!playerAces.IsModerator && !playerAces.IsAdmin && !playerAces.IsDeveloper) return;

            foreach (var ped in _players.ToArray())
            {
                if (ped == null || ped.Character == null) continue;
                var aces = await _perms.GetUserAces(ped);
                if (!aces.IsModerator && !aces.IsAdmin && !aces.IsDeveloper) continue;
                if (_modChatPreferences.ContainsKey(ped) && !_modChatPreferences[ped]) continue;
                string formattedString = $"~r~[MOD CHAT] from: {playerName}⚠️: {message}";
                ped.TriggerEvent("chat:addMessage", formattedString);
            }
        }

        private void ToggleModChatCommand(Player player)
        {
            var playerAces = _perms.GetUserAces(player).Result;
            if (!playerAces.IsModerator && !playerAces.IsAdmin && !playerAces.IsDeveloper) return;

            if (_modChatPreferences.ContainsKey(player))
            {
                _modChatPreferences[player] = !_modChatPreferences[player];
            }
            else
            {
                _modChatPreferences[player] = false;
            }

            string status = _modChatPreferences[player] ? "enabled" : "disabled";
            player.TriggerEvent("chat:addMessage", $"~r~[MOD CHAT] Visibility has been {status}.");
        }
    }
}