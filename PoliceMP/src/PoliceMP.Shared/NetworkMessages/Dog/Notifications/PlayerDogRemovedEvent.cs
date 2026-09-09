using PoliceMP.Core.Mediator;

namespace PoliceMP.Shared.NetworkMessages.Dog.Notifications
{
    public class PlayerDogRemovedEvent : INotification
    {
        public int PlayerServerHandle { get; set; }
        public int PlayerPedNetworkId { get; set; }
    }
}