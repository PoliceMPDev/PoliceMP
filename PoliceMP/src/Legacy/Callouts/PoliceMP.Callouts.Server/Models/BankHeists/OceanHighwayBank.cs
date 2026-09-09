using CitizenFX.Core;
using System.Collections.Generic;

namespace PoliceMP.Callouts.Server.Models.BankHeists
{
    class OceanHighwayBank : BankHeistLocation
    {
        public OceanHighwayBank()
        {
            BadGuyLocations = new List<Vector3>()
            {
                new Vector3(-2964.17f, 478f, 15.7f),
                new Vector3(-2961.06f, 477.71f, 15.7f),
                new Vector3(-2958.02f, 477.38f, 15.7f),
                new Vector3(-2958.33f, 481.48f, 15.7f)
            };

            CivilianLocations = new List<Vector3>()
            {
                new Vector3(-2963.99f, -480.83f, 15.7f),
                new Vector3(-2963.23f, -483.36f, 15.7f),
                new Vector3(-2963.88f, -485.17f, 15.7f)
            };

            ArrivalLocation = new Vector3(-2979.97f, 468.78f, 15.2f);
        }
    }
}
