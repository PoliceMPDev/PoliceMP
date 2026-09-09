using PoliceMP.Core.Mediator;

namespace PoliceMP.Shared.NetworkMessages.Game.Notifications
{
    public class PedTaseredEvent : INotification
    {
        public int VictimNetworkId { get; set; }
        public int AttackerNetworkId { get; set; }
    }
}