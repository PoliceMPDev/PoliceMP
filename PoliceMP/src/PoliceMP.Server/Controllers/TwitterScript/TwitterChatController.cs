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

namespace PoliceMP.Server.Controllers.TwitterScript
{
    public class TwitterChatController : Controller
    {
        private readonly ILogger<TwitterChatController> _logger;
        private readonly ICommandManager _commands;
        private readonly ILegacyServerCommunicationsManager _comms;
        private readonly INotificationService _notifications;
        private readonly PlayerList _players;

        private readonly float ChatDistance = 10f;

        public TwitterChatController(ILogger<TwitterChatController> logger,
            ICommandManager commands,
            ILegacyServerCommunicationsManager comms,
            INotificationService notifications,
            PlayerList players)
        {
            _logger = logger;
            _commands = commands;
            _comms = comms;
            _notifications = notifications;
            _players = players;
        }

        public override Task Started()
        {
            _comms.On(ServerEvents.SendTweetCommandToServer, (Player player, string message) =>
            {
                RoleplayTweetCommand(player, message);
            });

            return Task.FromResult(0);
        }


        private async void RoleplayTweetCommand(Player player, string message)
        {
            try
            {
                if (string.IsNullOrEmpty(message)) return;
                string trimmedMessage = message.Trim();

                TriggerEvent("txaLogger:CommandExecuted", $"[INTEL] {player.Name} has made the Intel: {message}");


                foreach (var ped in _players.ToArray())
                {
                    if (ped == null || ped.Character == null) continue;

                    string formattedString = $"~b~[INTEL 📝]: {trimmedMessage}";
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
    }
}