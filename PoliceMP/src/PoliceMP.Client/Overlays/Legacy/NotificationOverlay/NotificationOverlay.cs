using PoliceMP.Core.Client.Interface;
using PoliceMP.Core.Shared;

namespace PoliceMP.Client.Overlays.Legacy.NotificationOverlay
{
    public class NotificationOverlay : LegacyOverlay
    {
        private const int DefaultDelayMs = 10000;

        public NotificationOverlay(ILegacyNuiManager nuiManager, ILogger<LegacyOverlay> logger) : base(nuiManager, logger)
        {
        }

        public void Success(string title, string message, int delayMs = DefaultDelayMs)
        {
            Emit("showToast", new { type = "success", title, message, delayMs });
        }

        public void Warning(string title, string message, int delayMs = DefaultDelayMs)
        {
            Emit("showToast", new { type = "warning", title, message, delayMs });
        }

        public void Error(string title, string message, int delayMs = DefaultDelayMs)
        {
            Emit("showToast", new { type = "error", title, message, delayMs });
        }

        public void Info(string title, string message, int delayMs = DefaultDelayMs)
        {
            Emit("showToast", new { type = "info", title, message, delayMs });
        }

        protected override void OnReady()
        {
            Show();
        }
    }
}
