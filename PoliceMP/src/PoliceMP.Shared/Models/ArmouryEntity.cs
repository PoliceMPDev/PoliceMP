namespace PoliceMP.Shared.Models
{
    public class ArmouryEntity
    {
        public string ArmouryName { get; set; }
        public string[] AceGroupsRequired { get; set; }
        public int BlipType { get; set; }
        public int BlipColour { get; set; }
        public float xEntry { get; set; }
        public float yEntry { get; set; }
        public float zEntry { get; set; }
        public float xWalkExit { get; set; }
        public float yWalkExit { get; set; }
        public float zWalkExit { get; set; }
    }
}
