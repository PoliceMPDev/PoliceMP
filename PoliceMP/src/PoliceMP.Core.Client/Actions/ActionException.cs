using System;
using System.Runtime.Serialization;

namespace PoliceMP.Core.Client.Actions
{
    public class ActionException : Exception
    {
        public ActionException()
        {
        }

        protected ActionException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        public ActionException(string message) : base(message)
        {
        }

        public ActionException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}