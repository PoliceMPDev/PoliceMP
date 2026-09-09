using PoliceMP.Core.Mediator;
namespace PoliceMP.Shared.NetworkMessages.XPSystem.Notifications
{
    public class XPDecreasedEvent: INotification
    {
        public int XpDecrease { get; set; }
        public string Reason { get; set; }
    }
}