using PoliceMP.Client.Services.Interfaces;
using PoliceMP.Core.Client.Communications.Interfaces;
using PoliceMP.Core.Shared.Communications.Interfaces;
using PoliceMP.Shared.Constants;
using System.Threading.Tasks;
using PoliceMP.Client.Overlays.Legacy.NotificationOverlay;

namespace PoliceMP.Client.Services
{
    public class NotificationService : INotificationService
    {
        private readonly NotificationOverlay _notificationOverlay;
        private readonly ILegacyClientCommunicationsManager _comms;

        public NotificationService(
            NotificationOverlay notificationOverlay,
            IFiveEventManager fiveEvents,
            ILegacyClientCommunicationsManager comms)
        {
            _notificationOverlay = notificationOverlay;
            _comms = comms;

            fiveEvents.On<string, string, string>("PoliceMP:ShowNotification", OnShowNotificationExternal);
            _comms.On<string, string, string>(ClientEvents.ShowNotification, OnShowNotificationInternal);
        }

        private async Task OnShowNotificationInternal(string title, string message, string type)
        {
            await OnShowNotificationExternal(title, message, type);
        }

        private Task OnShowNotificationExternal(string title, string message, string type)
        {
            switch (type.ToLower())
            {
                case "success":
                    Success(title, message);
                    break;

                case "warning":
                    Warning(title, message);
                    break;

                case "error":
                    Error(title, message);
                    break;

                case "info":
                    Info(title, message);
                    break;
            }

            return Task.FromResult(0);
        }

        public void Success(string title, string message)
        {
            _notificationOverlay.Success(title, message);
        }

        public void Warning(string title, string message)
        {
            _notificationOverlay.Warning(title, message);
        }

        public void Error(string title, string message)
        {
            _notificationOverlay.Error(title, message);
        }

        public void Info(string title, string message)
        {
            _notificationOverlay.Info(title, message);
        }
    }
}