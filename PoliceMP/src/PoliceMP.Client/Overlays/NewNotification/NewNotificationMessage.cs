namespace PoliceMP.Client.Overlays.NewNotification
{
    public class NewNotificationMessage
    {
        public NewNotificationMessage(string title, string type, string message = null,
            NewNotificationMessageContent[] multiMessage = null, int? autoClose = null, string imageUrl = null)
        {
            Title = title;
            Type = type;
            Message = message;
            MultiMessage = multiMessage;
            AutoClose = autoClose;
            ImageUrl = imageUrl;
        }

        public string Title { get; set; }
        public string Type { get; set; }
        public string Message { get; set; }
        public NewNotificationMessageContent[] MultiMessage { get; set; }
        public int? AutoClose { get; set; }
        public string? ImageUrl { get; set; }
    }
}