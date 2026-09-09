using PoliceMP.Core.Shared.Models;
using PoliceMP.Core.Shared.Scripts;

namespace PoliceMP.Shared.Behaviors.AiCallouts
{
    public enum MisperState
    {
        Searching,
        Map,
        Wandering,
        Sitting,
        LayingDown,
    }

    public class MisperBehavior: PedBehaviorDefinition
    {
        public double StartTime { get; set; }
        public PmpVector3 InitialPosition { get; set; }
        public MisperState State { get; set; }
    }
}