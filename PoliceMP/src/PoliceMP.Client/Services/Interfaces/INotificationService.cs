namespace PoliceMP.Client.Services.Interfaces
{
    public interface INotificationService
    {
        void Success(string title, string message);
        void Warning(string title, string message);
        void Error(string title, string message);
        void Info(string title, string message);
    }
}
