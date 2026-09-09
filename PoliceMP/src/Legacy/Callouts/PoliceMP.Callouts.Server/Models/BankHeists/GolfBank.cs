using CitizenFX.Core;
using System.Collections.Generic;

namespace PoliceMP.Callouts.Server.Models.BankHeists
{
    class GolfBank : BankHeistLocation
    {
        public GolfBank()
        {
            BadGuyLocations = new List<Vector3>()
            {
                new Vector3(-1211.7f, -329.06f, 37.78f),
                new Vector3(-1215.77f, -330.41f, 37.78f),
                new Vector3(-1215.65f, -337.96f, 37.78f),
                new Vector3(-1211.93f, -335.1f, 37.78f)
            };

            CivilianLocations = new List<Vector3>()
            {
                new Vector3(-1216.02f, -335.96f, 37.78f),
                new Vector3(-1218.52f, -331.39f, 37.78f)
            };

            ArrivalLocation = new Vector3(-1217.27f, -321.87f, 37.67f);
        }
    }
}
