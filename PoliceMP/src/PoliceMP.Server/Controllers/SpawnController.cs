using System.Linq;
using CitizenFX.Core;
using Microsoft.Extensions.Options;
using PoliceMP.Core.Server.Communications.Interfaces;
using PoliceMP.Core.Server.Networking;
using PoliceMP.Core.Shared;
using PoliceMP.Shared.Constants;
using PoliceMP.Shared.Options;
using System.Threading.Tasks;

namespace PoliceMP.Server.Controllers
{
    public class SpawnController : Controller
    {
        private readonly ILegacyServerCommunicationsManager _comms;
        private readonly ILogger<SpawnController> _logger;
        private readonly SpawnOptions _options;
        private readonly PlayerList _players;

        public SpawnController(ILegacyServerCommunicationsManager comms,
            ILogger<SpawnController> logger,
            IOptions<SpawnOptions> options, PlayerList players)
        {
            _comms = comms;
            _logger = logger;
            _players = players;
            _options = options.Value;

            _comms.OnRequest(ServerEvents.SpawnGetOptions, OnGetOptions);
            _comms.On<int>(ServerEvents.SpawnNotifyPlayerToRevive, TellPlayerToRevive);
        }

        private Task<SpawnOptions> OnGetOptions(Player player)
        {
            return Task.FromResult(_options);
        }
        
        private void TellPlayerToRevive(int playerCharNetId)
        {
            var player = _players.FirstOrDefault(i => i.Character?.NetworkId == playerCharNetId);
            if (player == null) return;
            _comms.ToClient(player, ClientEvents.SpawnToldToRevive);
        }
    }
}