using CitizenFX.Core;

namespace PoliceMP.Main.Core.Client.Extensions
{
    /// <summary>
    /// Useful extension methods.
    /// </summary>
    public static class BaseScriptExtensions
    {
        /// <summary>
        /// Sends a chat message to the client.
        /// </summary>
        /// <param name="baseScript">This BaseScript object.</param>
        /// <param name="message">The message to send.</param>
        /// <param name="title">The title of the message.</param>
        public static void SendChatMessage(this BaseScript baseScript, string message, string title = "PoliceMP")
        {
            ClientFunctions.SendChatMessage(message, title);
        }
    }
}