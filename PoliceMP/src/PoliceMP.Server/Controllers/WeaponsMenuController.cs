using System.Threading.Tasks;
using CitizenFX.Core;
using PoliceMP.Client.Services.Interfaces;
using PoliceMP.Core.Server.Communications.Interfaces;
using PoliceMP.Core.Server.Networking;
using PoliceMP.Core.Shared;
using PoliceMP.Shared.NetworkMessages.Game.Notifications;

namespace PoliceMP.Server.Controllers
{
    public class WeaponsMenuController: Controller
    {
        private readonly ILogger<WeaponsMenuController> _logger;
        private readonly IPermissionService _permissionService;
        private readonly IServerCommunicationsManager _comms;
        
        public WeaponsMenuController(
            ILogger<WeaponsMenuController> _logger,
            IPermissionService _permissionService,
            IServerCommunicationsManager _comms
        ){
            this._logger = _logger;
            this._permissionService = _permissionService;
            this._comms = _comms;
            
            _comms.AddNotificationHandler<PlayerSpawnedEvent>(HandlePlayerSpawned);
        }

        private async Task HandlePlayerSpawned(Player player, PlayerSpawnedEvent playerSpawnedEvent)
        {
            // Should player have access to weapons menu?
            var aces = await _permissionService.GetUserAces(player);
            var allowed = aces.IsAdmin || aces.IsDeveloper || aces.IsTierTwo;
            if (allowed)
            {
                _logger.Debug($"{player.Name} has weapons!");
                // Fire event to allow weapons menu to open
                _comms.PublishToClient(player, new EnableWeaponsEvent {});
            }
            else
            {
                _logger.Debug($"{player.Name} does not have weapons!");
            }
        }
    }
}