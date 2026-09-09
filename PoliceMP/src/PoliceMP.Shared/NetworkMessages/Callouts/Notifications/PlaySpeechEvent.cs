using PoliceMP.Core.Mediator;

namespace PoliceMP.Shared.NetworkMessages.Callouts.Notifications
{
    public class PlaySpeechEvent : INotification
    {
        public int NetworkId { get; set; }
        public string Text { get; set; }
    }
}