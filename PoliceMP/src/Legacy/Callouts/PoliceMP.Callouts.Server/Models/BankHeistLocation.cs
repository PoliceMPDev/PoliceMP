using CitizenFX.Core;
using System.Collections.Generic;

namespace PoliceMP.Callouts.Server.Models
{
    class BankHeistLocation
    {
        public List<Vector3> BadGuyLocations { get; set; }

        public List<Vector3> CivilianLocations { get; set; }

        public Vector3 ArrivalLocation { get; set; }
    }
}
