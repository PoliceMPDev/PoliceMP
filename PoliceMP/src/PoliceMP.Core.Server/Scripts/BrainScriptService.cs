using System;
using System.Collections.Generic;
using System.Text;
using CitizenFX.Core;
using PoliceMP.Core.Server.Communications.Interfaces;
using PoliceMP.Core.Shared;
using PoliceMP.Core.Shared.Constants;
using PoliceMP.Shared.Constants.States;

namespace PoliceMP.Core.Server.Scripts
{
    public class BrainScriptService
    {
        private readonly ILogger<BrainScriptService> _log;
        private readonly ILegacyServerCommunicationsManager _comms;

        public BrainScriptService(ILogger<BrainScriptService> log, ILegacyServerCommunicationsManager comms)
        {
            _log = log;
            _comms = comms;

            comms.On<Player, int>(PedStates.AttachedBrain, TransferBrainOwnership);
        }

        private void TransferBrainOwnership(Player player, int networkId)
        {
            var entity = Entity.FromNetworkId(networkId);
            if (entity?.Owner is null)
            {
                _log.Error($"Cannot transfer brain ownership for entity {networkId}");
                return;
            }

            _log.Trace($"Brain is being transferred from \"{player.Name}\" to \"{entity.Owner.Name}\" for entity {networkId}.");
            _comms.ToClient(entity.Owner, PedStates.AttachedBrain, networkId);
        }
    }
}
