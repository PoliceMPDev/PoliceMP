using CitizenFX.Core;
using System.Collections.Generic;

namespace PoliceMP.Callouts.Server.Models.BankHeists
{
    class PoliceStationBank : BankHeistLocation
    {
        public PoliceStationBank()
        {
            BadGuyLocations = new List<Vector3>()
            {
                new Vector3(150.22f, -1040.18f, 29.37f),
                new Vector3(145.56f, -1039.37f, 29.37f),
                new Vector3(145.84f, -1045.33f, 29.38f),
                new Vector3(143.71f, -1042.92f, 29.37f)
            };

            CivilianLocations = new List<Vector3>()
            {
                new Vector3(151.04f, -1040.2f, 29.37f),
                new Vector3(145.4f, -1037.55f, 29.37f),
                new Vector3(142.27f, -1043.94f, 29.37f)
            };

            ArrivalLocation = new Vector3(152.16f, -1033.52f, 29.34f);
        }
    }
}
