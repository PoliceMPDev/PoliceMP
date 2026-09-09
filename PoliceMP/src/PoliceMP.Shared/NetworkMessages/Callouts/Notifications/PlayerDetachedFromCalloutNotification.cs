using System.Collections.Generic;
using PoliceMP.Core.Mediator;

namespace PoliceMP.Shared.NetworkMessages.Callouts.Notifications
{
    public class PlayerDetachedFromCalloutNotification : INotification
    {
        public int CalloutId { get; set; }
        public List<string> CalloutAttendingUnits { get; set; }
        public int PlayerServerHandle { get; set; }
        public string PlayerName { get; set; }
        public string PlayerCallsign { get; set; }
    }
}