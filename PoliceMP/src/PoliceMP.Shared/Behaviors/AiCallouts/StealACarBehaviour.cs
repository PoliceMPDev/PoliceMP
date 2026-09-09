using PoliceMP.Core.Shared.Scripts;

namespace PoliceMP.Shared.Behaviors.AiCallouts
{
    public enum StealingState
    {
        Idle,
        Fleeing,
        EnterAnyVehicle,
        Stealing
    }

    public class StealACarBehaviour : PedBehaviorDefinition
    {
        public StealingState StealingState { get; set; }
    }
}