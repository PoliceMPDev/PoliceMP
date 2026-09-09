using PoliceMP.Core.Mediator;

namespace PoliceMP.Shared.NetworkMessages.Game.Notifications
{
    public class PlayerJoinedEvent : INotification
    {
        public int ServerHandle { get; set; }
        public string PlayerName { get; set; }
    }
}