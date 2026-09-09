using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CitizenFX.Core;
using CitizenFX.Core.Native;
using PoliceMP.Core.Server.Communications.Interfaces;
using PoliceMP.Core.Server.Networking;
using PoliceMP.Core.Shared;
using PoliceMP.Core.Shared.Communications.Interfaces;
using PoliceMP.Shared.Constants;
using PoliceMP.Shared.Models;
using PoliceMP.Shared.NetworkMessages.Game.Notifications;

namespace PoliceMP.Server.Controllers
{
    public class PlayerServiceController : Controller
    {
        private readonly ILegacyServerCommunicationsManager _legacyComms;
        private readonly IServerCommunicationsManager _comms;
        private readonly ILogger<PlayerServiceController> _logger;
        private readonly PlayerList _playerList;
        private readonly IFiveEventManager _fiveEvents;

        public PlayerServiceController(ILegacyServerCommunicationsManager legacyComms, IServerCommunicationsManager comms, ILogger<PlayerServiceController> logger,
            PlayerList playerList, IFiveEventManager fiveEvents)
        {
            _legacyComms = legacyComms;
            _comms = comms;
            _logger = logger;
            _playerList = playerList;
            _fiveEvents = fiveEvents;
        }

        public override Task Started()
        {
            _legacyComms.OnRequest(ServerEvents.FetchAllNetworkIdsFromServer, SendPlayersToClient);
            _legacyComms.OnRequest<int, string>(ServerEvents.FetchPlayerNameFromNetworkId, FetchNameFromNetworkId);

            // Add forwarding events
            _fiveEvents.On("playerJoining", new Action<Player>(HandlePlayerJoining));
            _fiveEvents.On("playerDropped", new Action<Player, string>(HandlePlayerDropped));
            
            return Task.FromResult(0);
        }

        private void HandlePlayerJoining([FromSource] Player player)
        {
            _comms.PublishAll(new PlayerJoinedEvent
            {
                ServerHandle = int.Parse(player.Handle),
                PlayerName = player.Name
            });
        }

        private void HandlePlayerDropped([FromSource] Player player, string arg2)
        {
            _comms.PublishAll(new PlayerDroppedEvent
            {
                ServerHandle = int.Parse(player.Handle),
                PlayerName = player.Name
            });
        }


        private async Task<List<int>> SendPlayersToClient(Player player)
        {
            List<int> networkIds = new List<int>();

            foreach (var targetPlayer in _playerList)
            {
                if (targetPlayer == null) continue;

                if (targetPlayer.Character == null) continue;
                
                networkIds.Add(targetPlayer.Character.NetworkId);
            }

            _logger.Debug($"Found {networkIds.Count()} Players");
            
            return networkIds;
        }

        private async Task<string> FetchNameFromNetworkId(Player player, int networkId)
        {
            var targetPlayer = _playerList.FirstOrDefault(x => x.Character?.NetworkId == networkId);

            return targetPlayer == null ? string.Empty : targetPlayer.Name;
        }
    }
}