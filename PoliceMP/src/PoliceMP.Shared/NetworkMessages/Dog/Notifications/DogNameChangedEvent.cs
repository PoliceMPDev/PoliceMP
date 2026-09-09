using PoliceMP.Core.Mediator;

namespace PoliceMP.Shared.NetworkMessages.Dog.Notifications
{
    public class DogNameChangedEvent : INotification
    {
        public int DogNetworkId { get; set; }
        public string OldName { get; set; }
        public string NewName { get; set; }
    }
}