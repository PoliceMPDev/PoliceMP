using CitizenFX.Core;
using PoliceMP.Core.Server.Communications.Interfaces;
using PoliceMP.Core.Server.Networking;
using PoliceMP.Core.Shared;
using PoliceMP.Shared.Constants;
using System.Linq;

namespace PoliceMP.Server.Controllers
{
    public class FlashBangController : Controller
    {
        private readonly ILegacyServerCommunicationsManager _comms;
        private readonly ILogger<FlashBangController> _logger;
        private readonly PlayerList _players;

        private int stunTime = 8;
        private int afterTime = 8;
        private float range = 8.0f;

        private readonly float _MAX_DISTANCE_TO_EVENT = 20f;

        public FlashBangController(ILegacyServerCommunicationsManager comms, ILogger<FlashBangController> logger, PlayerList players)
        {
            _comms = comms;
            _logger = logger;
            _players = players;

            _comms.On<float, float, float, int>(ServerEvents.SendFlashBangEventToServer, OnReceiveFlashBangFromClient);
        }

        private void OnReceiveFlashBangFromClient(Player player, float posX, float posY, float posZ, int networkId)
        {
            if (player == null || player.Character == null)
            {
                _logger.Error($"[FlashBang] Sender player or their character is null. Player: {(player?.Name ?? "null")}");
                return;
            }

            foreach (var targetPlayer in _players.ToList())
            {
                if (targetPlayer == null || targetPlayer.Character == null)
                {
                    _logger.Warn($"[FlashBang] Skipping target player with null reference. Player: {(targetPlayer?.Name ?? "null")}");
                    continue;
                }

                var distance = Vector3.Distance(targetPlayer.Character.Position, player.Character.Position);
                if (distance >= _MAX_DISTANCE_TO_EVENT) continue;

                _comms.ToClient(targetPlayer, ClientEvents.SendFlashBangEventToClient, posX, posY, posZ, stunTime,
                    afterTime, range, networkId);
            }
        }
    }
}