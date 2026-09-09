using System;
using System.Linq;
using System.Threading.Tasks;
using CitizenFX.Core;
using CitizenFX.Core.Native;
using PoliceMP.Core.Server.Commands.Interfaces;
using PoliceMP.Core.Server.Communications.Interfaces;
using PoliceMP.Core.Server.Interfaces.Services;
using PoliceMP.Core.Server.Networking;
using PoliceMP.Core.Shared;
using PoliceMP.Server.Services;
using PoliceMP.Shared.Constants;

namespace PoliceMP.Server.Controllers.RemoteBlipManage
{
    public class RemoteBlipManage : Controller
    {
        private readonly ILogger<RemoteBlipManage> _logger;
        private readonly ILegacyServerCommunicationsManager _comms;
        private readonly PlayerList _players;

        public RemoteBlipManage(ILogger<RemoteBlipManage> logger,
            ILegacyServerCommunicationsManager comms,
            PlayerList players)
        {
            _logger = logger;
            _comms = comms;
            _players = players;
        }

        public override Task Started()
        {
            _comms.On(ServerEvents.RemoteHideBlipsToServer, (string targetPlayerName) =>
            {
                RemoteHideBlipsToServer(targetPlayerName);
            });

            _comms.On(ServerEvents.RemoteShowBlipsToServer, (string targetPlayerName) =>
            {
                RemoteShowBlipsToServer(targetPlayerName);
            });

            return Task.FromResult(0);
        }

        private void RemoteHideBlipsToServer(string targetPlayerName)
        {
            try
            {
                foreach (var ped in _players.ToArray())
                {
                    if (ped == null || ped.Character == null) continue;

                    _comms.ToClient(ped, ClientEvents.RemoteHideBlipsToClient, targetPlayerName);

                }
                return;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return;
            }
        }

        private void RemoteShowBlipsToServer(string targetPlayerName)
        {
            try
            {
                foreach (var ped in _players.ToArray())
                {
                    if (ped == null || ped.Character == null) continue;

                    _comms.ToClient(ped, ClientEvents.RemoteShowBlipsToClient, targetPlayerName);
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