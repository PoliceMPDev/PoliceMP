using System;
using System.Runtime.Serialization;

namespace PoliceMP.Core.Client.IoC
{
    public class ServiceContainerException : Exception
    {
        public ServiceContainerException()
        {
        }

        protected ServiceContainerException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        public ServiceContainerException(string message) : base(message)
        {
        }

        public ServiceContainerException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}