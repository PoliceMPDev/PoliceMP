using PoliceMP.Core.Shared.Models;
using PoliceMP.Core.Shared.Scripts;

namespace PoliceMP.Shared.Behaviors.AiCallouts
{
    public enum FighterState
    {
        Aggresive,
        Threatening,
        Attacking,
    }

    public class FighterBehavior: PedBehaviorDefinition
    {
        public double StartTime { get; set; }
        public PmpVector3 InitialPosition { get; set; }
        public FighterState State { get; set; }
    }
}