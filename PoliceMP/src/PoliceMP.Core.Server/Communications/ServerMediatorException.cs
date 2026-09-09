using System;

namespace PoliceMP.Core.Client.Communications
{
    public class ServerMediatorException : Exception
    {
        public ServerMediatorException(string message) : base(message)
        {
        }
    }
}