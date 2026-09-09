using CitizenFX.Core;
using Microsoft.EntityFrameworkCore.Storage;
using PoliceMP.Core.Server.Communications.Interfaces;
using PoliceMP.Core.Server.Networking;
using PoliceMP.Shared.Constants;

namespace PoliceMP.Server.Controllers
{
    public class SpeechController : Controller
    {
        private readonly ILegacyServerCommunicationsManager _comms;
        private readonly PlayerList _players;

        private readonly float MAX_DISTANCE_TO_REPLICATE = 10f;

        public SpeechController(ILegacyServerCommunicationsManager comms, PlayerList players)
        {
            _comms = comms;
            _players = players;

            _comms.On<int, string, int>(ServerEvents.ReplicateSayPedSpeech, OnReplicateSay);
            _comms.On<int, string, int>(ServerEvents.ReplicateDoPedSpeech, OnReplicateDo);
        }

        private void OnReplicateDo(Player player, int pedNetworkId, string action, int durationMs)
        {
            var sourcePosition = player?.Character?.Position;
            if (sourcePosition == null || sourcePosition == Vector3.Zero)
                return;

            foreach (var replicateToPlayer in _players)
            {
                var targetPosition = replicateToPlayer?.Character?.Position;
                if (targetPosition == null || targetPosition == Vector3.Zero)
                    continue;

                if (replicateToPlayer.Handle == player.Handle)
                    continue;

                var distance = Vector3.Distance(targetPosition.Value, sourcePosition.Value);
                if (distance >= MAX_DISTANCE_TO_REPLICATE)
                    continue;

                _comms.ToClient(replicateToPlayer,
                    ClientEvents.ReplicateDoPedSpeech,
                    pedNetworkId,
                    action,
                    durationMs);
            }
        }

        private void OnReplicateSay(Player player, int pedNetworkId, string speech, int durationMs)
        {
            var playerPosition = player?.Character?.Position;
            if (playerPosition == null || playerPosition == Vector3.Zero)
                return;

            foreach (var replicateToPlayer in _players)
            {
                var targetPosition = replicateToPlayer?.Character?.Position;
                if (targetPosition == null || targetPosition == Vector3.Zero)
                    continue;

                if (replicateToPlayer.Handle == player.Handle)
                    continue;

                var distance = Vector3.Distance(targetPosition.Value, playerPosition.Value);
                if (distance >= MAX_DISTANCE_TO_REPLICATE)
                    continue;

                _comms.ToClient(replicateToPlayer,
                    ClientEvents.ReplicateSayPedSpeech,
                    pedNetworkId,
                    speech,
                    durationMs);
            }
        }
    }
}