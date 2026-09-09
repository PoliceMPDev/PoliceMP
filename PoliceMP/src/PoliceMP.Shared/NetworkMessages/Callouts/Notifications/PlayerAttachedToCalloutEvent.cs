using System.Collections.Generic;
using PoliceMP.Core.Mediator;
using PoliceMP.Core.Shared.Models;

namespace PoliceMP.Shared.NetworkMessages.Callouts.Notifications
{
    public class PlayerAttachedToCalloutEvent : INotification
    {
        public string CalloutTitle { get; set; }
        public string CalloutBody { get; set; }
        public string CalloutSubtitle { get; set; }
        public List<string> CalloutAttendingUnits { get; set; }
        public int CalloutId { get; set; }
        public int PlayerServerHandle { get; set; }
        public string PlayerName { get; set; }
        public string PlayerCallsign { get; set; }
        public PmpVector2 CalloutLocation { get; set; }
        public int? CalloutIcon { get; set; }
        public float? CalloutBlipRadius { get; set; }
    }
}