using PoliceMP.Core.Shared.Models;

namespace PoliceMP.Shared.Models
{
    public class RoadManagementItem
    {
        public int ModelHash { get; set; }
        public PmpVector3 Position { get; set; }
        public PmpVector3 Rotation { get; set; }
        public int ObjectNetId { get; set; }
        public int RatNetId { get; set; }
        public int Speed { get; set; }
        public PmpVector3 NodePosition { get; set; }
        public int PlacedByNetId { get; set; }
    }
}