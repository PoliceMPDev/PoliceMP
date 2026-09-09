using PoliceMP.Main.Core.Server.Enums;
using System;
using PoliceMP.Main.Core.Shared;

namespace PoliceMP.Main.Core.Server
{
    public static class ServerFunctions
    {
        public static VehicleColor GetRandomVehicleColor()
        {
            var values = Enum.GetValues(typeof(VehicleColor));
            return (VehicleColor)values.GetValue(PoliceMpRandom.Next(values.Length));
        }

        public static VehicleHash GetRandomVehicleHash()
        {
            var values = Enum.GetValues(typeof(SuitableVehicleHash));
            return (VehicleHash)values.GetValue(PoliceMpRandom.Next(values.Length));
        }

        public static PedHash GetRandomPedHash()
        {
            var values = Enum.GetValues(typeof(PedHash));
            return (PedHash)values.GetValue(PoliceMpRandom.Next(values.Length));
        }
    }
}
