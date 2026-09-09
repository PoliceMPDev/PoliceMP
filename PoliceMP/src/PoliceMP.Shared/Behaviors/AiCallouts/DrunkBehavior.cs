using PoliceMP.Core.Shared.Models;
using PoliceMP.Core.Shared.Scripts;

namespace PoliceMP.Shared.Behaviors.AiCallouts
{
    public enum DrunkState
    {
        Slightly,
        Buzzed,
        Very,
    }
    
    public class DrunkBehavior: PedBehaviorDefinition
    {
        public double StartTime { get; set; }
        public PmpVector3 InitialPosition { get; set; }
        public DrunkState State { get; set; }
        public bool WillBecomeAggressive { get; set; }
    }
}