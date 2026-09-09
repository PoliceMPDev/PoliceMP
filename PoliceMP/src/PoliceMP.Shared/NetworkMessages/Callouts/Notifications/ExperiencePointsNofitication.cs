using PoliceMP.Core.Mediator;

namespace PoliceMP.Shared.NetworkMessages.Callouts.Notifications
{
    public class ExperiencePointsNofitication : INotification
    {
        public int CalloutId { get; set; }
        public string Title { get; set; }
        public string Body { get; set; }
        public string Subtitle { get; set; }

        public int ExperiencePoints { get; set; }

        //@todo Got to update this for future reference for points
    }
}