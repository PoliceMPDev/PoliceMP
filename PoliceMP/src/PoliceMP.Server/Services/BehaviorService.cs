using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CitizenFX.Core;
using CitizenFX.Core.Native;
using PoliceMP.Core.Server.Communications.Interfaces;
using PoliceMP.Core.Server.Extensions;
using PoliceMP.Core.Server.Scripts;
using PoliceMP.Core.Shared;
using PoliceMP.Core.Shared.Scripts;
using PoliceMP.Server.Services.Interfaces;
using PoliceMP.Shared.Constants.States;
using PoliceMP.Shared.NetworkMessages.Behaviors.Common.Commands;

namespace PoliceMP.Server.Services
{
    public class BehaviorService : IBehaviorService
    {
        private readonly ILogger<BehaviorService> _log;
        private readonly IServerCommunicationsManager _comms;

        public BehaviorService(ILogger<BehaviorService> log, IServerCommunicationsManager comms)
        {
            _log = log;
            _comms = comms;
        }

        public async Task<Blackboard<T>?> SetPedBehavior<T>(Ped ped) where T : PedBehaviorDefinition
        {
            //TODO: fixed this!
            while (API.NetworkGetEntityOwner(ped.Handle) == -1)
            {
                await BaseScript.Delay(100);
            }

            var success = await _comms.SendToClient(ped.Owner, new SetPedBehaviorCommand
            {
                PedNetworkId = ped.NetworkId,
                BehaviorType = typeof(T)
            });

            return success ? Blackboard<T>.Get(ped) : null;
        }

        public Stack<Type> GetBehaviorTypes(Ped ped)
        {
            return ped.State.Get<Stack<Type>>(PedStates.AttachedBrain);
        }

        public async Task<bool> RemovePedBehaviors(Ped ped)
        {
            throw new NotImplementedException();
        }
    }
}