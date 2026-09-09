using System.Threading.Tasks;
using CitizenFX.Core;
using Microsoft.Extensions.Logging;
using PoliceMP.Client.Services.Interfaces;
using PoliceMP.Core.Server.Commands.Interfaces;
using PoliceMP.Core.Server.Communications.Interfaces;
using PoliceMP.Core.Server.Networking;
using PoliceMP.Shared.Constants;

namespace PoliceMP.Server.Controllers
{
    public class DevToolsController : Controller
    {
        private readonly ILogger<DevToolsController> _log;
        private readonly ILegacyServerCommunicationsManager _legacyComms;
        private readonly IPermissionService _permissions;

        public DevToolsController(ILogger<DevToolsController> log, ILegacyServerCommunicationsManager legacyComms, ICommandManager commands, IPermissionService permissions)
        {
            _log = log;
            _legacyComms = legacyComms;
            _permissions = permissions;
            
            commands.Register("toggledebug").WithHandler(ToggleDebug);
        }

        private async void ToggleDebug(Player player)
        {
            var aces = await _permissions.GetUserAces(player);
            if (!aces.IsTierTwo && !aces.IsDeveloper) return;
            _log.LogInformation("Player {PlayerName} has toggled debug!", player.Name);
            _legacyComms.ToClient(player, ClientEvents.ToggleDebug);
        }
    }
}