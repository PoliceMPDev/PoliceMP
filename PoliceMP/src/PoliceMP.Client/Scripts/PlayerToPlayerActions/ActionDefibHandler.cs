using System.Threading.Tasks;
using CitizenFX.Core;
using PoliceMP.Client.Actions.CPR;
using PoliceMP.Client.Actions.Defib;
using PoliceMP.Core.Client.Actions.Interfaces;
using PoliceMP.Core.Client.Communications.Interfaces;
using PoliceMP.Core.Client.Scripts;
using PoliceMP.Core.Shared;
using PoliceMP.Shared.Constants;

namespace PoliceMP.Client.Scripts.PlayerToPlayerActions
{
    public class ActionDefibHandler : Script
    {
        private readonly ILegacyClientCommunicationsManager _comms;
        private readonly IActionManager _actions;
        private readonly ILogger<ActionCPRHandler> _logger;
        
        public ActionDefibHandler(ILegacyClientCommunicationsManager comms, IActionManager actions, ILogger<ActionCPRHandler> logger)
        {
            _comms = comms;
            _actions = actions;
            _logger = logger;
        }

        protected override Task OnStartAsync()
        {
            _comms.On<int>(ClientEvents.DefibPlayerPed, HandleDefibPed);
            _comms.On<int>(ClientEvents.BeDefibByPlayerPed, HandleBeDefibedBy);
            return Task.FromResult(0);
        }

        private async Task HandleDefibPed(int targetNetworkId)
        {
            var target = Entity.FromNetworkId(targetNetworkId);
            if (target == null)
            {
                _logger.Error($"Failed to Defib ped. Could not find entity with network Id {targetNetworkId}!");
                return;
            }

            if (target is not Ped targetPed)
            {
                _logger.Error($"Failed to Defib ped. Entity is not a ped!");
                return;
            }

            await _actions.Execute(new Defib(targetPed, Game.PlayerPed, true), true);
        }

        private async Task HandleBeDefibedBy(int arresterNetworkId)
        {
            var tackler = Entity.FromNetworkId(arresterNetworkId);
            if (tackler == null)
            {
                _logger.Error($"Failed to Defib ped. Could not find entity with network Id {arresterNetworkId}!");
                return;
            }

            if (tackler is not Ped tacklerPed)
            {
                _logger.Error($"Failed to Defib ped. Entity is not a ped!");
                return;
            }

            await _actions.Execute(new Defib(Game.PlayerPed, tacklerPed, true), true);
        }
    }
}