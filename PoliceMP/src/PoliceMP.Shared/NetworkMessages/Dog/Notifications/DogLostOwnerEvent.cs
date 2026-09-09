using PoliceMP.Core.Mediator;

namespace PoliceMP.Shared.NetworkMessages.Dog.Notifications
{
    public class DogLostOwnerEvent : INotification
    {
        public int DogNetworkId { get; set; }
        public int OwnerNetworkId { get; set; }
    }
}