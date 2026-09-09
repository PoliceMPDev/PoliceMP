using System.Collections.Generic;
using PoliceMP.Core.Mediator;

namespace PoliceMP.Shared.NetworkMessages.Callouts.Commands
{
    public class BreakVehicleCommand : IClientRequest
    {
        public int VehicleNetworkId { get; set; }
        public List<int> BreakWheels { get; set; }

    }
}