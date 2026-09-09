using PoliceMP.Core.Mediator;

namespace PoliceMP.Shared.NetworkMessages.Dog.Notifications
{
    public class PlayerDogSmelledNothingOnPedEvent : INotification
    {
        public int PlayerServerHandle { get; set; }
        public int DogNetworkId { get; set; }
        public int PedNetworkId { get; set; }
    }
}