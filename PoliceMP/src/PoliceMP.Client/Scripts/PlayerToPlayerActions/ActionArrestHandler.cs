using System.Threading.Tasks;
using CitizenFX.Core;
using CitizenFX.Core.Native;
using PoliceMP.Client.Actions.Cuff;
using PoliceMP.Core.Client.Actions.Interfaces;
using PoliceMP.Core.Client.Communications.Interfaces;
using PoliceMP.Core.Client.Scripts;
using PoliceMP.Core.Shared;
using PoliceMP.Shared.Constants;

namespace PoliceMP.Client.Scripts.PlayerToPlayerActions
{
    public class ActionArrestHandler : Script
    {
        private readonly ILegacyClientCommunicationsManager _comms;
        private readonly IActionManager _actions;
        private readonly ILogger<ActionArrestHandler> _logger;
        
        public ActionArrestHandler(ILegacyClientCommunicationsManager comms, IActionManager actions, ILogger<ActionArrestHandler> logger)
        {
            _comms = comms;
            _actions = actions;
            _logger = logger;
        }

        protected override Task OnStartAsync()
        {
            _comms.On<int>(ClientEvents.ArrestPlayerPed, HandleArrestPed);
            _comms.On<int>(ClientEvents.BeArrestedPlayerPed, HandleBeArrestedBy);
            return Task.FromResult(0);
        }

        private async Task HandleArrestPed(int targetNetworkId)
        {
            var target = Entity.FromNetworkId(targetNetworkId);
            if (target == null)
            {
                _logger.Error($"Failed to arrest ped. Could not find entity with network Id {targetNetworkId}!");
                return;
            }

            if (target is not Ped targetPed)
            {
                _logger.Error($"Failed to arrest ped. Entity is not a ped!");
                return;
            }

            await _actions.Execute(new Cuff(Game.PlayerPed, targetPed, true), true);
        }

        private async Task HandleBeArrestedBy(int arresterNetworkId)
        {
            var tackler = Entity.FromNetworkId(arresterNetworkId);
            if (tackler == null)
            {
                _logger.Error($"Failed to arrest ped. Could not find entity with network Id {arresterNetworkId}!");
                return;
            }

            if (tackler is not Ped tacklerPed)
            {
                _logger.Error($"Failed to arrest ped. Entity is not a ped!");
                return;
            }

            await _actions.Execute(new Cuff(tacklerPed, Game.PlayerPed, true), true);
        }
    }
}