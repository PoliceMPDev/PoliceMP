using System;
using PoliceMP.Core.Mediator;
using PoliceMP.Core.Shared.Scripts;

namespace PoliceMP.Shared.NetworkMessages.Behaviors.Common.Commands
{
    public class SetPedBehaviorCommand : IClientRequest<bool>
    {
        private Type _behaviorType;
        public int PedNetworkId { get; set; }

        public Type BehaviorType
        {
            get => _behaviorType;
            set
            {
                if (!value.IsSubclassOf(typeof(PedBehaviorDefinition)))
                {
                    throw new ArgumentException($"Value must be subclass of {nameof(PedBehaviorDefinition)}.",
                        nameof(BehaviorType));
                }

                _behaviorType = value;
            }
        }
    }
}