namespace PoliceMP.Shared.Models
{
    public class Locker
    {
        public string Name { get; set; }
        public string LockerType { get; set; }
        public float xPos { get; set; }
        public float? xPosEA { get; set; }
        public float yPos { get; set; }
        public float? yPosEA { get; set; }
        public float zPos { get; set; }
        public float? zPosEA { get; set; }
        public float xPosExit { get; set; }
        public float? xPosExitEA { get; set; }
        public float yPosExit { get; set; }
        public float? yPosExitEA { get; set; }
        public float zPosExit { get; set; }
        public float? zPosExitEA { get; set; }
        public float exitHeading { get; set; }
    }
}