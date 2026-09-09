using PoliceMP.Core.Mediator;

namespace PoliceMP.Shared.NetworkMessages.Game.Notifications
{
    public class EntityCreatedEvent : INotification
    {
        public int NetworkId { get; set; }
    }
}