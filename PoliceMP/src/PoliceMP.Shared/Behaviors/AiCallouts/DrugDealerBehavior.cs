using PoliceMP.Core.Shared.Models;
using PoliceMP.Core.Shared.Scripts;

namespace PoliceMP.Shared.Behaviors.AiCallouts
{
    public enum DrugDealerState
    {
        Idle,
        Fleeing,
        Phone
    }
    
    public class DrugDealerBehavior : PedBehaviorDefinition
    {
        public DrugDealerState DrugDealerState { get; set; }
        public int LastStateChangeTime { get; set; }
    }
}