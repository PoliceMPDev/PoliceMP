using PoliceMP.Core.Shared.Models;
using PoliceMP.Core.Shared.Scripts;

namespace PoliceMP.Shared.Behaviors.AiCallouts
{
    public class WanderingRadiusBehaviour : PedBehaviorDefinition
    {
        public PmpVector3 Position { get; set; }
        public float Radius { get; set; }
        public double LastMoveTime { get; set; }
    }
}