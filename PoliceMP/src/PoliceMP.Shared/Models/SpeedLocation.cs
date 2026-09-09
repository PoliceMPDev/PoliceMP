using PoliceMP.Core.Shared.Models;

namespace PoliceMP.Shared.Models
{
    public class SpeedLocation
    {
        public float Limit { get; set; }
        public PmpVector3 Coords { get; set; }
        public float Heading { get; set; }

        public int Prop { get; set; }

        public int OrangeActive { get; set; }
        public int Orange1 { get; set; }
        public int Orange2 { get; set; }
        public int Warning { get; set; }

        public bool Active { get; set; }
        public bool Timeout { get; set; }

        public SpeedLocation(float limit, PmpVector3 coords, float heading)
        {
            Limit = limit;
            Coords = coords;
            Heading = heading;
        }
    }
}