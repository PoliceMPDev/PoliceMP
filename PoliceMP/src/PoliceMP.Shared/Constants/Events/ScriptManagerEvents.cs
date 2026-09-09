using System.Security.Policy;

namespace PoliceMP.Shared.Constants.Events
{
    public static class ScriptManagerEvents
    {
        public static class Client
        {
        }

        public static class Server
        {
            public const string SuccessfullyStarted = "PoliceMP:ScriptManagerEvents:Server:SuccessfullyStarted";
            public const string FailedToStart = "PoliceMP:ScriptManagerEvents:Server:FailedToStart";
        }
    }
}
