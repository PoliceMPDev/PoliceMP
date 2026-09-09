using System;
using PoliceMP.Core.Client.Options.Interfaces;
using PoliceMP.Core.Shared;
using PoliceMP.Core.Shared.Constants;

namespace PoliceMP.Core.Client
{
    public class Logger<T> : ILogger<T>
    {
        private LogLevel LogLevel => LogLevel.Debug;
        private readonly string _format = $"[{{3}}] [PoliceMP] {{2}}[{{0}}] [{typeof(T).Name}]^7 {{1}}";

        public void Debug(string message)
        {
            if (LogLevel >= LogLevel.Debug)
            {
                InternalLog(nameof(Debug), message, ConsoleColors.Cyan);
            }
        }

        public void Trace(string message)
        {
            if (LogLevel >= LogLevel.Trace)
            {
                InternalLog(nameof(Trace), message);
            }
        }

        public void Warn(string message)
        {
            InternalLog(nameof(Warn), message, ConsoleColors.Yellow);
        }

        public void Error(string message)
        {
            InternalLog(nameof(Error), message, ConsoleColors.Red);
        }

        public void Error(string message, Exception exception)
        {
            InternalLog(nameof(Error), $"{message}\n{exception.Message}\n{exception.StackTrace}", ConsoleColors.Red);
        }

        private void InternalLog(string level, string message, string color = ConsoleColors.White) =>
            CitizenFX.Core.Debug.WriteLine(_format, level, message, color, DateTime.Now);
    }
}