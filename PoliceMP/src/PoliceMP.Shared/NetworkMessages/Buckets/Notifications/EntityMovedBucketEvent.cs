using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PoliceMP.Core.Mediator;
using PoliceMP.Shared.Enums;

namespace PoliceMP.Shared.NetworkMessages.Buckets.Notifications
{
    /// <summary>
    /// Raised when an entity moves to a new bucket
    /// </summary>
    public class EntityMovedBucketEvent : INotification
    {
        public int EntityServerHandle { get; set; }
        public RoutingBucket OldBucket { get; set; }
        public RoutingBucket NewBucket { get; set; }
    }
}
