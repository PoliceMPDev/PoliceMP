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

namespace PoliceMP.Server.Controllers.EmergencyCalls
{
    public class EmergencyCallController : Controller
    {
        private readonly ILogger<EmergencyCallController> _logger;
        private readonly ICommandManager _commands;
        private readonly ILegacyServerCommunicationsManager _comms;
        private readonly INotificationService _notifications;
        private readonly PlayerList _players;

        private readonly float ChatDistance = 10f;

        public EmergencyCallController(ILogger<EmergencyCallController> logger,
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

        private enum EmergencyCallType {
            EMERGENCY, CIVIL
        }

        public override Task Started()
        {
            _comms.On(ServerEvents.Send999CallToServer, (Player player, string message) =>
            {
                SendCall(player, message, EmergencyCallType.EMERGENCY);
            });

            _comms.On(ServerEvents.Send101CallToServer, (Player player, string message) =>
            {
                SendCall(player, message, EmergencyCallType.CIVIL);
            });

            _comms.On(ServerEvents.SendForceInfoToServer, (Player player, string message, string div) =>
            {
                SendForceInfo(player, message, div);
            });

            return Task.FromResult(0);
        }

        // 999/101 Call
        private async void SendCall(Player player, string message, EmergencyCallType emergencyCallType)
        {
            try
            {
                if (string.IsNullOrEmpty(message)) return;
                string trimmedMessage = message.Trim();

                if (emergencyCallType == EmergencyCallType.EMERGENCY)
                    TriggerEvent("txaLogger:CommandExecuted", $"[999 CALL] {player.Name} has made 999 Call: {message}");
                else
                    TriggerEvent("txaLogger:CommandExecuted", $"[101 CALL] {player.Name} has made 101 Call: {message}");


                foreach (var ped in _players.ToArray())
                {
                    if (ped == null || ped.Character == null) continue;

                    string formattedString = emergencyCallType == EmergencyCallType.EMERGENCY ? $"^*~r~[999 CALL] 📞:^r ~r~{trimmedMessage}" : $"^*~b~[101 CALL] 📞:^r ~b~{trimmedMessage}";
                    ped.TriggerEvent("chat:addMessage", formattedString);
                    ped.TriggerEvent("callhistory:add", formattedString);

                }
                return;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return;
            }
        }

        // Force Info
        private async void SendForceInfo(Player player, string message, string div)
        {
            try
            {
                if (string.IsNullOrEmpty(message)) return;
                string trimmedMessage = message.Trim();

                TriggerEvent("txaLogger:CommandExecuted", $"[Force Info] {player.Name} has sent Force Info: {message}");


                foreach (var ped in _players.ToArray())
                {
                    if (ped == null || ped.Character == null) continue;
                    var formattedString = "";

                    switch (div)
                    {
                        case "Police":
                            formattedString = $"^*~b~[Force Info] 🔵:^r ~b~{trimmedMessage}";
                            break;
                        case "Nhs":
                             formattedString = $"^*~g~[Force Info] 🟢:^r ~g~{trimmedMessage}";
                            break;
                        case "Fire":
                            formattedString = $"^*~r~[Force Info] 🔴:^r ~r~{trimmedMessage}";
                            break;
                        case "Control":
                            formattedString = $"^*~r~[Force Info] 🔴:^r ~r~{trimmedMessage}";
                            break;
                        case "Highways":
                            formattedString = $"^*~y~[Force Info] 🟡:^r ~y~{trimmedMessage}";
                            break;
                    }
                    
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