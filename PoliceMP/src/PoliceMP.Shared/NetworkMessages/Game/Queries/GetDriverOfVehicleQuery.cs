using PoliceMP.Core.Mediator;

namespace PoliceMP.Shared.NetworkMessages.Game.Queries
{
    /// <summary>
    /// Returns the network id of the vehicle driver, or 0 if there is no driver
    /// </summary>
    public class GetDriverOfVehicleQuery : IClientRequest<int>
    {
        public int VehicleNetworkId { get; set; }
    }
}