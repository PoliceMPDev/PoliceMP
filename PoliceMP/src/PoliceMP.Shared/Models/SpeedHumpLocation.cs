using PoliceMP.Core.Shared.Models;

namespace PoliceMP.Shared.Models
{
    public class SpeedHumpLocation
    {
        public PmpVector3 Position { get; set; }
        public float Heading { get; set; }
        public int Handle { get; set; }
    }
}