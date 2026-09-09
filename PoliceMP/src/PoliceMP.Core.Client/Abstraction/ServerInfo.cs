using System;
using CitizenFX.Core.Native;

namespace PoliceMP.Core.Client.Abstraction
{
    public static class ServerInfo
    {
        public static TimeSpan GetServerTime()
        {
            int hours = 0;
            int minutes = 0;
            int seconds = 0;
            API.NetworkGetServerTime(ref hours, ref minutes, ref seconds);

            return new TimeSpan(hours, minutes, seconds);
        }
    }
}