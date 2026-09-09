using System.Linq;
using CitizenFX.Core;
using CitizenFX.Core.Native;
using PoliceMP.Core.Server.Communications.Interfaces;
using PoliceMP.Core.Server.Networking;
using PoliceMP.Core.Shared;
using PoliceMP.Core.Shared.Communications.Interfaces;
using PoliceMP.Shared.Constants;
using System.Threading.Tasks;

namespace PoliceMP.Server.Controllers
{
    public class AdminToolsController : Controller
    {
        private readonly ILogger<AdminToolsController> _logger;
        private readonly PlayerList _players;
        private readonly ILegacyServerCommunicationsManager _comms;

        public AdminToolsController(IFiveEventManager fiveEvents,
            ILogger<AdminToolsController> logger,
            PlayerList players,
            ILegacyServerCommunicationsManager comms
            )
        {
            _logger = logger;
            _players = players;
            _comms = comms;

            comms.OnRequest<bool>(ServerEvents.RequestRayPerms, GetSetInteraction);
            comms.On<int>(ServerEvents.RequestPlayerToFling, RequestPlayerToFling);
            comms.On<int>(ServerEvents.AdminYeetEntity, YeetEntity);
            comms.On<int, int>(ServerEvents.AdminYeetEntityLog, YeetEntityLogOnly);
            comms.OnRequest<int, string>(ServerEvents.AdminGetNetworkFirstOwner, GetNetworkFirstOwner);
        }

        private void RequestPlayerToFling(int playerCharNetID)
        {
            lock (_players)
            {
                foreach (Player pl in _players)
                {
                    if (pl.Character.NetworkId != playerCharNetID) { continue; }
                    _comms.ToClient(pl, ClientEvents.TellPlayerToFling);
                }
            }
        }

        private Task<bool> GetSetInteraction(Player player)
        {
            if (API.IsPlayerAceAllowed(player.Handle, "The.Main.Man")) return Task.FromResult(true);
            return Task.FromResult(false);
        }

        private Task YeetEntity(Player player, int networkId)
        {
            var entity = Entity.FromNetworkId(networkId);
            if (entity == null)
            {
                _logger.Error($"{player.Name} requested deletion of entity {networkId} but it does not exist!");
                return Task.FromResult(0); 
            }

            var owner = entity.Owner;
            _logger.Debug($"{player.Name} is requesting deletion of entity {networkId}... {owner.Name} is the owner.");

            if (API.DoesEntityExist(entity.Handle))
            {
                API.DeleteEntity(entity.Handle);
            }
            return Task.FromResult(0); 
        }

        private void YeetEntityLogOnly(Player player, int entityNetId, int originalOwnerNetId)
        {
            var owner = Players[originalOwnerNetId];
            _logger.Debug($"{player.Name} deleted entity {entityNetId}.");
        }

        private async Task<string> GetNetworkFirstOwner(Player player, int entityNetworkId)
        {
            var entity = Entity.FromNetworkId(entityNetworkId);

            if (entity == null)
            {
                _logger.Debug($"Player requested owner of entity but server could not find the entity. {entityNetworkId}");
                return null;
            }

            var ownerId = API.NetworkGetFirstEntityOwner(entity.Handle);
            var owner = Players[ownerId];

            return owner?.Name;
        }
    }
}