using PoliceMP.Core.Shared.Models;

namespace PoliceMP.Shared.NetworkMessages.Game.Notifications.ShockingEvents
{
    public class EventShockingCarCrashOccurred
    {
        public int[] InvolvedPedNetworkIds { get; set; }
        public int SourcePedNetworkId { get; set; }
        public PmpVector3 Position { get; set; }
    }
}