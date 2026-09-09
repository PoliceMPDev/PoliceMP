using CitizenFX.Core;
using System.Collections.Generic;

namespace PoliceMP.Callouts.Server.Models.BankHeists
{
    class PaletoBayBank : BankHeistLocation
    {
        public PaletoBayBank()
        {
            BadGuyLocations = new List<Vector3>()
            {
                new Vector3(-109.22f, 6464.59f, 31.63f),
                new Vector3(-102.63f, 6469.19f, 31.63f),
                new Vector3(-104.78f, 6471.3f, 31.63f),
                new Vector3(-100.61f, 6462.17f, 31.63f),
                new Vector3(-110.37f, 6470.05f, 31.63f),
                new Vector3(-113.2f, 6468.66f, 31.63f),
                new Vector3(-103.2f, 6476.93f, 31.63f)
            };

            CivilianLocations = new List<Vector3>()
            {
                new Vector3(-103.51f, 6465.43f, 31.63f),
                new Vector3(-115.2f, 6470.96f, 31.63f),
                new Vector3(-117.41f, 6469.27f, 31.63f),
                new Vector3(-113.52f, 6465.94f, 31.63f)
            };

            ArrivalLocation = new Vector3(-120.84f, 6453.08f, 31.47f);
        }
    }
}
