using System;

namespace PoliceMP.Computer.Client.Models
{
    public class CalloutPlayer
    {
        public string Name { get; set; }
        public string[] Ranks { get; set; }
        public bool HasArrived { get; set; }
        public DateTime TimeOfArrival { get; set; }
    }
}
