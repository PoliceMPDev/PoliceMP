namespace PoliceMP.Computer.Client.Util
{
    public interface ILogger
    {
        void Log(string message);
        void Error(string message);
        void Warn(string message);
    }
}
