using System;
using System.Runtime.Serialization;

namespace PoliceMP.Core.Shared.Commands
{
    public class CommandException : Exception
    {
        public CommandException()
        {
        }

        protected CommandException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        public CommandException(string message) : base(message)
        {
        }

        public CommandException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
