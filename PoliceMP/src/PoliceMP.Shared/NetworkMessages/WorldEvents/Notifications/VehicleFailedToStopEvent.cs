using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PoliceMP.Core.Mediator;

namespace PoliceMP.Shared.NetworkMessages.WorldEvents.Notifications
{
    public class VehicleFailedToStopEvent : INotification
    {
        public int DriverNetworkId { get; set; }
    }
}
