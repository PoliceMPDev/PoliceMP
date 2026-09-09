using PoliceMP.Core.Shared.Models;

namespace PoliceMP.Shared.Models
{
    public class EntitySpawnVehicle
    {
        public string Name { get; set; }
        public PmpVector3 Position { get; set; }
        public float Heading { get; set; }
        public bool MissionEntity { get; set; }
    }

    public class EntitySpawnPed
    {
        public string Name { get; set; }
        public PmpVector3 Position { get; set; }
        public float Heading { get; set; }
        public bool ScriptHosted { get; set; }
    }

    public class EntitySpawnObject
    {
        public string Name { get; set; }
        public PmpVector3 Position { get; set; }
        public float Heading { get; set; }
        public bool MissionEntity { get; set; }
        
        public bool DoorFlag { get; set; }
    }
}