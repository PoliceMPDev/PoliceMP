using CitizenFX.Core;
using CitizenFX.Core.UI;
using PoliceMP.Callouts.Shared.Events;

namespace PoliceMP.Callouts.Client
{
    // This is here to handle events from the server
    // The server can't send chat messages or show subtitles for the player, so the
    // workaround is to trigger an event on the client.
    // This would be suitable in the PoliceMP.Main.Core.Client project, however I want to
    // wait until we get our communications system in place
    public class NotificationEventHandler : BaseScript
    {
        [EventHandler(ClientEvents.SEND_CHAT_MESSAGE)]
        private void Event_SendChatMessage(string message, bool error = false)
        {
            if (error)
                PoliceMP.Main.Core.Client.ClientFunctions.SendErrorMessage(message);
            else
                PoliceMP.Main.Core.Client.ClientFunctions.SendChatMessage(message);
        }

        [EventHandler(ClientEvents.SHOW_SUBTITLE)]
        private void Events_ShowSubtitle(string message, int duration)
        {
            Screen.ShowSubtitle(message, duration);
        }
    }
}
