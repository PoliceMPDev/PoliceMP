using CitizenFX.Core;

namespace PoliceMP.Core.Server.Interfaces.Services
{
    public interface INotificationService
    {
        void Success(Player player, string title, string message);
        void Warning(Player player, string title, string message);
        void Error(Player player, string title, string message);
        void Info(Player player, string title, string message);
    }
}