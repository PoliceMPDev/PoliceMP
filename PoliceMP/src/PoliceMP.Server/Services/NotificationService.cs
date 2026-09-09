using CitizenFX.Core;
using PoliceMP.Core.Server.Communications.Interfaces;
using PoliceMP.Core.Server.Interfaces.Services;
using PoliceMP.Shared.Constants;

namespace PoliceMP.Server.Services
{
    public class NotificationService : INotificationService
    {
        private readonly ILegacyServerCommunicationsManager _comms;

        public NotificationService(ILegacyServerCommunicationsManager comms)
        {
            _comms = comms;
        }

        public void Success(Player player, string title, string message)
        {
            _comms.ToClient(player, ClientEvents.ShowNotification, title, message, "success");
        }

        public void Warning(Player player, string title, string message)
        {
            _comms.ToClient(player, ClientEvents.ShowNotification, title, message, "warning");
        }

        public void Error(Player player, string title, string message)
        {
            _comms.ToClient(player, ClientEvents.ShowNotification, title, message, "error");
        }

        public void Info(Player player, string title, string message)
        {
            _comms.ToClient(player, ClientEvents.ShowNotification, title, message, "info");
        }
    }
}