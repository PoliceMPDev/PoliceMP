using PoliceMP.Core.Mediator;

namespace PoliceMP.Shared.NetworkMessages.Dog.Notifications
{
    public class DogTookDownPlayerEvent : INotification
    {
        public int PlayerServerHandle { get; set; }
        public int OwnerNetworkId { get; set; }
        public int DogNetworkId { get; set; }
        public string AnimDict { get; set; }
        public string AnimName { get; set; }
    }
}