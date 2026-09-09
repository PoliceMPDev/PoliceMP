using CitizenFX.Core;
using System;
using System.Collections.Generic;

namespace PoliceMP.Computer.Client.Models
{
    public class CalloutData
    {
        public int Id { get; set; }
        public int Grade { get; set; }
        public string Rank { get; set; }
        public string Title { get; set; }
        public Vector3 LocationVector { get; set; }
        public string Location { get; set; }
        public string LatLong { get; set; }
        public DateTime Time { get; set; }
        public List<CalloutPlayer> Players { get; set; }

        public bool LocalPlayerIsOn { get; set; }
        public bool FirstPlayerArrived { get; set; }

        public DateTime TimeOfArrival { get; set; }
    }
}
