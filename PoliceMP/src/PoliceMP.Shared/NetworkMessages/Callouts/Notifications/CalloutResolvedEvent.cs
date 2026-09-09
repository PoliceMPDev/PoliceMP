using PoliceMP.Core.Mediator;

namespace PoliceMP.Shared.NetworkMessages.Callouts.Notifications
{
    public class CalloutResolvedEvent : INotification
    {
        public int CalloutId { get; set; }
    }
}