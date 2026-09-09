using PoliceMP.Core.Shared.Models;

namespace PoliceMP.Shared.Options
{
    public class SpawnOptions
    {
        public PmpVector3 Position { get; set; }
        public string Model { get; set; }
        public int RespawnDelayMs { get; set; }
    }
}
