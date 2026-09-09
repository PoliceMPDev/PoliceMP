using CitizenFX.Core;
using PoliceMP.Core.Server.Communications.Interfaces;
using PoliceMP.Core.Server.Networking;
using PoliceMP.Core.Shared;
using PoliceMP.Shared.Constants.Events;

namespace PoliceMP.Server.Controllers
{
    public class ScriptManagerController : Controller
    {
        private readonly ILogger<ScriptManagerController> _log;

        public ScriptManagerController(ILogger<ScriptManagerController> log, ILegacyServerCommunicationsManager comms)
        {
            _log = log;

            comms.On<Player, string>(ScriptManagerEvents.Server.SuccessfullyStarted, HandleSuccess);
            comms.On<Player, string, string>(ScriptManagerEvents.Server.FailedToStart, HandleFailedToStart);
        }


        private void HandleSuccess(Player player, string name)
        {
            _log.Trace($"{player.Name} successfully started script {name}.");
        }

        private void HandleFailedToStart(Player player, string name, string reason)
{
            _log.Error($"{player.Name} failed to start script {name}. Reason: {reason}");
        }
    }
}