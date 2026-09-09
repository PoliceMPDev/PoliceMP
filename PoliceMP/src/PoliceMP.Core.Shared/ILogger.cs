using System;

namespace PoliceMP.Core.Shared
{
    public interface ILogger
    {
        void Debug(string message);
        void Trace(string message);
        void Warn(string message);
        void Error(string message);
        void Error(string message, Exception exception);
    }

    public interface ILogger<T> : ILogger
    {
    }
}
