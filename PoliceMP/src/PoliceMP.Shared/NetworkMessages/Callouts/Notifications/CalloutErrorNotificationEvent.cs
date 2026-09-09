using PoliceMP.Core.Mediator;

namespace PoliceMP.Shared.NetworkMessages.Callouts.Notifications
{
    public class CalloutErrorNotificationEvent : INotification
    {
        public int CalloutId { get; set; }
        public string Title { get; set; }
        public string Body { get; set; }
        public string Subtitle { get; set; }
    }
}