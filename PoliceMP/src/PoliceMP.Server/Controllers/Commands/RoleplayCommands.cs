using System;
using System.Linq;
using System.Threading.Tasks;
using CitizenFX.Core;
using PoliceMP.Core.Server.Commands.Interfaces;
using PoliceMP.Core.Server.Communications.Interfaces;
using PoliceMP.Core.Server.Interfaces.Services;
using PoliceMP.Core.Server.Networking;
using PoliceMP.Core.Shared;
using PoliceMP.Server.Services;
using PoliceMP.Shared.Constants;
using PoliceMP.Client.Services.Interfaces;

namespace PoliceMP.Server.Controllers.Commands
{
    public class RoleplayCommands : Controller
    {
        private readonly ILogger<RoleplayCommands> _logger;
        private readonly ICommandManager _commands;
        private readonly ILegacyServerCommunicationsManager _comms;
        private readonly INotificationService _notifications;
        private readonly PlayerList _players;
        private readonly IPermissionService _perms;

        private readonly float ChatDistance = 10f;

        public RoleplayCommands(ILogger<RoleplayCommands> logger,
            ICommandManager commands,
            ILegacyServerCommunicationsManager comms,
            INotificationService notifications,
            IPermissionService perms,
            PlayerList players)
        {
            _logger = logger;
            _commands = commands;
            _perms = perms;
            _comms = comms;
            _notifications = notifications;
            _players = players;
        }

        private enum RoleplayCommandType {
            ME, DO
        }

        public override Task Started()
        {
            _comms.On(ServerEvents.SendMeCommandToServer, (Player player, string message) => RoleplayCommand(player, message, RoleplayCommandType.ME));
            _comms.On(ServerEvents.SendDoCommandToServer, (Player player, string message) => RoleplayCommand(player, message, RoleplayCommandType.DO));

            _comms.On(ServerEvents.CreateScent, (Player player, string message, string playerName, Vector3 civPosition) =>
            {
                CreateNewScent(player, message, playerName, civPosition);
            });
            
            return Task.FromResult(0);
        }

        private async void CreateNewScent(Player player, string message, string playerName, Vector3 civPosition)
        {
            TriggerEvent("txaLogger:CommandExecuted", $"[INFO] {playerName} Sent created a DSU scent: ''{message}''!");
            
            foreach (var ped in _players.ToArray())
            {
                if (ped == null || ped.Character == null) continue;
                var aces = await _perms.GetUserAces(ped);
                var isDSU = aces.IsDsuTrained;

                var beingCheckedName = player.Name;
                if (!isDSU) continue;
                
                _comms.ToClient(ped, ClientEvents.SendNewScentToClient, message, playerName, civPosition);
            }
        }        
        
        private async void RoleplayCommand(Player player, string message, RoleplayCommandType roleplayCommandType)
        {
            try
            {
                if (string.IsNullOrEmpty(message)) return;
                string trimmedMessage = message.Trim();

                Vector3 playerPosition = player.Character.Position;

                string formattedString = roleplayCommandType == RoleplayCommandType.ME ? $"^6* {player.Name} {trimmedMessage}" : $"^6* {trimmedMessage} (( {player.Name} ))";

                foreach (var ped in _players.ToArray())
                {
                    if (ped == null || ped.Character == null) continue;

                    Vector3 pedPosition = ped.Character.Position;

                    if (pedPosition == default(Vector3)) continue;

                    if (pedPosition.Distance(playerPosition) > ChatDistance) continue;

                    ped.TriggerEvent("chat:addMessage", formattedString);

                    _comms.ToClient(ped,
                        ClientEvents.ReplicateDoPedSpeech,
                        player.Character.NetworkId,
                        trimmedMessage,
                        5000);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return;
            }
        }
    }
}