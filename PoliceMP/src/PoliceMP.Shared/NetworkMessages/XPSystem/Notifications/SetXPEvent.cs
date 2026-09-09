using PoliceMP.Core.Mediator;
namespace PoliceMP.Shared.NetworkMessages.XPSystem.Notifications
{
    public class SetXPEvent : INotification
    {
        public int XpValue { get; set; }
    }
}