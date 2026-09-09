using System;

namespace PoliceMP.Core.Client.Communications
{
    public class ClientMediatorException : Exception
    {
        public ClientMediatorException(string message) : base(message)
        {
        }
    }
}