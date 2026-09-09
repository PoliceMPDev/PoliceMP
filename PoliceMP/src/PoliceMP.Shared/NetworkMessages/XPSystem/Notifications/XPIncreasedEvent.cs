using PoliceMP.Core.Mediator;
namespace PoliceMP.Shared.NetworkMessages.XPSystem.Notifications
{
    public class XPIncreasedEvent: INotification
    {
        public int XpIncrease { get; set; }
        public string Reason { get; set; }
    }
}