using PoliceMP.Core.Mediator;
using PoliceMP.Shared.Enums;

namespace PoliceMP.Shared.NetworkMessages.Buckets.Notifications
{
    /// <summary>
    /// Raised when a player moves to a new bucket
    /// </summary>
    public class PlayerMovedBucketEvent : INotification
    {
        public int PlayerServerHandle { get; set; }
        public string PlayerName { get; set; }
        public RoutingBucket OldBucket { get; set; }
        public RoutingBucket NewBucket { get; set; }
    }
}