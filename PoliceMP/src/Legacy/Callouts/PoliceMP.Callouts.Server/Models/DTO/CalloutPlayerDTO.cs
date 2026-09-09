using System;

namespace PoliceMP.Callouts.Server.Models.DTO
{
    public class CalloutPlayerDTO
    {
        public string Name { get; set; }
        public string[] Ranks { get; set; }
        public bool HasArrived { get; set; }
        public DateTime TimeOfArrival { get; set; }
    }
}
