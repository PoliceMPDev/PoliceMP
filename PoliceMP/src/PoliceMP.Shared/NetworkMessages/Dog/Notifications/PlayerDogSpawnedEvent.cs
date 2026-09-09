using PoliceMP.Core.Mediator;

namespace PoliceMP.Shared.NetworkMessages.Dog.Notifications
{
    public class PlayerDogSpawnedEvent : INotification
    {
        public int PlayerServerHandle { get; set; }
        public int DogNetworkId { get; set; }
    }
}