namespace PoliceMP.Computer.Client.Util
{
    public class Logger : ILogger
    {
        private bool _isProduction;

        public Logger(bool isProduction = false)
        {
            _isProduction = isProduction;
        }

        public void Error(string message)
        {
            if (_isProduction) return;

            CitizenFX.Core.Debug.WriteLine($"^1[Computer] [Error] {message}^7");
        }

        public void Log(string message)
        {
            if (_isProduction) return;

            CitizenFX.Core.Debug.WriteLine($"^0[Computer] [Log] {message}^7");
        }

        public void Warn(string message)
        {
            if (_isProduction) return;

            CitizenFX.Core.Debug.WriteLine($"^3[Computer] [Warn] {message}^7");
        }
    }
}
