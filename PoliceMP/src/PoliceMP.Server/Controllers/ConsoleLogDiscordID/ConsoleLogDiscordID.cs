using System;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
using CitizenFX.Core;
using CitizenFX.Core.Native;
using PoliceMP.Core.Server.Commands.Interfaces;
using PoliceMP.Core.Server.Communications.Interfaces;
using PoliceMP.Core.Server.Interfaces.Services;
using PoliceMP.Core.Server.Networking;
using PoliceMP.Core.Shared;
using PoliceMP.Core.Shared.Constants;
using PoliceMP.Server.Services;
using PoliceMP.Shared.Constants;

namespace PoliceMP.Server.Controllers.ConsoleLogDiscordID
{
    public class ConsoleLogDiscordID : Controller
    {
        private readonly ILogger<ConsoleLogDiscordID> _logger;
        private readonly ILegacyServerCommunicationsManager _comms;
        private readonly PlayerList _players;

        public ConsoleLogDiscordID(ILogger<ConsoleLogDiscordID> logger,
            ILegacyServerCommunicationsManager comms,
            PlayerList players)
        {
            _logger = logger;
            _comms = comms;
            _players = players;
        }

        public override Task Started()
        {
            _comms.On(ServerEvents.LogDiscordIDToServer, (Player player, string playerName, string playerSrc) =>
            {
                LogDiscordIDToServer(player, playerName, playerSrc);
            });

            return Task.FromResult(0);
        }

        private void LogDiscordIDToServer(Player player, string playerName, string playerSrc)
        {
            try
            {
                int numIdentifiers = API.GetNumPlayerIdentifiers(player.Handle);

                Console.WriteLine(" ");
                Console.WriteLine($"\x1b[34m[PoliceMP ID Logger] \x1b[33mPLAYER: \x1b[31m'{playerName}' \x1b[33mHAS JOINED THE GAME WITH IDENTIFIERS \x1b[34m[PoliceMP ID Logger]");

                for (int i = 0; i < numIdentifiers; i++)
                {
                    var identifier = API.GetPlayerIdentifier(player.Handle, i);
                    if (!string.IsNullOrWhiteSpace(identifier) && !identifier.Contains("ip:"))
                    {
                        Console.WriteLine($"\x1b[34m[PoliceMP ID Logger] \x1b[33mPLAYER: \x1b[31m'{playerName}': {identifier}");
                    }
                }

                Console.WriteLine(" ");
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