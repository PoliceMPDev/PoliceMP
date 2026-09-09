using System.Collections.Generic;
using PoliceMP.Core.Mediator;
using PoliceMP.Core.Shared.Models;

namespace PoliceMP.Shared.NetworkMessages.WorldEvents.Notifications
{
    /// <summary>
    /// Triggered whenever a vehicle collides with another vehicle
    /// </summary>
    public class CarCrashOccurredEvent : INotification
    {
        public int VictimVehicleNetworkId { get; set; }
        public int VictimDriverNetworkId { get; set; }
        public int AttackerVehicleNetworkId { get; set; }
        public int AttackerDriverNetworkId { get; set; }
        public float Damage { get; set; }
        public bool VictimVehicleDestroyed { get; set; }
        public PmpVector3 Position { get; set; }
        public Dictionary<int, float> DamagePerComponent { get; set; }

    }
}