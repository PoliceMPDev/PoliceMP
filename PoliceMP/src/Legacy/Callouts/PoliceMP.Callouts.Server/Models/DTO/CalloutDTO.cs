using CitizenFX.Core;
using System;
using System.Collections.Generic;

namespace PoliceMP.Callouts.Server.Models.DTO
{
    public class CalloutDTO
    {
        public int Id { get; set; }
        public int Grade { get; set; }
        public string Rank { get; set; }
        public string Title { get; set; }
        public Vector3 LocationVector { get; set; }
        public DateTime Time { get; set; }
        public DateTime TimeOfArrival { get; set; }
        public bool FirstPlayerArrived { get; set; }
        public List<CalloutPlayerDTO> Players { get; set; }
    }
}
