using PoliceMP.Core.Mediator;
using PoliceMP.Core.Shared.Models;

namespace PoliceMP.Shared.NetworkMessages.Dog.Notifications
{
    public class DogShouldTeleportEvent : INotification
    {
        public int DogNetworkId { get; set; }
        public PmpVector3 Position { get; set; }
    }
}