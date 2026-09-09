using CitizenFX.Core;

namespace PoliceMP.Client.Overlays.NewNotification
{
    public class NewNotificationMessageContent
	{
        public string Label { get; set; }
        public string Content { get; set; }

        public NewNotificationMessageContent(string label, string content)
        {
			Label = label;
			Content = content;
        }
    }
}