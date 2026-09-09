using System.Collections.Generic;

namespace PoliceMP.Shared.Models
{
    public class GarageEntity
    {
        public string GarageName { get; set; }
        public string[] AceGroupsRequired { get; set; }
        public int BlipType { get; set; }
        public int BlipColour { get; set; }
        public float xEntry { get; set; }
        public float? xEntryEA { get; set; }
        public float yEntry { get; set; }
        public float? yEntryEA { get; set; }
        public float zEntry { get; set; }
        public float? zEntryEA { get; set; }
        public float xWalkExit { get; set; }
        public float? xWalkExitEA { get; set; }
        public float yWalkExit { get; set; }
        public float? yWalkExitEA { get; set; }
        public float zWalkExit { get; set; }
        public float? zWalkExitEA { get; set; }
        public string[] GarageCategories { get; set; }
        public List<ExitPoint> ExitPoints { get; set; }
        public List<ExitPoint> ExitPointsEA { get; set; }
    }

    public class ExitPoint
    {
        public float xExit { get; set; }
        public float yExit { get; set; }
        public float zExit { get; set; }
        public float hExit { get; set; }
    }
}
