using System.Threading.Tasks;
using CitizenFX.Core;
using PoliceMP.Client.Actions.CPR;
using PoliceMP.Client.Actions.Cuff;
using PoliceMP.Core.Client.Actions.Interfaces;
using PoliceMP.Core.Client.Communications.Interfaces;
using PoliceMP.Core.Client.Scripts;
using PoliceMP.Core.Shared;
using PoliceMP.Shared.Constants;

namespace PoliceMP.Client.Scripts.PlayerToPlayerActions
{
    public class ActionCPRHandler : Script
    {
        private readonly ILegacyClientCommunicationsManager _comms;
        private readonly IActionManager _actions;
        private readonly ILogger<ActionCPRHandler> _logger;
        
        public ActionCPRHandler(ILegacyClientCommunicationsManager comms, IActionManager actions, ILogger<ActionCPRHandler> logger)
        {
            _comms = comms;
            _actions = actions;
            _logger = logger;
        }

        protected override Task OnStartAsync()
        {
            _comms.On<int>(ClientEvents.CPRPlayerPed, HandleCPRPed);
            _comms.On<int>(ClientEvents.BeCPRByPlayerPed, HandleBeCPRedBy);
            return Task.FromResult(0);
        }

        private async Task HandleCPRPed(int targetNetworkId)
        {
            var target = Entity.FromNetworkId(targetNetworkId);
            if (target == null)
            {
                _logger.Error($"Failed to CPR ped. Could not find entity with network Id {targetNetworkId}!");
                return;
            }

            if (target is not Ped targetPed)
            {
                _logger.Error($"Failed to CPR ped. Entity is not a ped!");
                return;
            }

            await _actions.Execute(new CPR(targetPed, Game.PlayerPed, true), true);
        }

        private async Task HandleBeCPRedBy(int arresterNetworkId)
        {
            var tackler = Entity.FromNetworkId(arresterNetworkId);
            if (tackler == null)
            {
                _logger.Error($"Failed to CPR ped. Could not find entity with network Id {arresterNetworkId}!");
                return;
            }

            if (tackler is not Ped tacklerPed)
            {
                _logger.Error($"Failed to arrest ped. Entity is not a ped!");
                return;
            }

            await _actions.Execute(new CPR(Game.PlayerPed, tacklerPed, true), true);
        }
    }
}