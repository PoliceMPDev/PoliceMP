using System;

namespace PoliceMP.Core.Client.Scripts
{
    public class BrainScriptException : Exception
    {
        public BrainScriptException(string message) : base(message)
        {
        }
    }
}