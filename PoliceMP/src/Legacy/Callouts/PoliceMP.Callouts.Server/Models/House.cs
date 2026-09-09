namespace PoliceMP.Callouts.Server.Models
{
    class House
    {
        public int HouseID { get; set; }
        public string HouseNameOrNumber { get; set; }
        public string HouseStreet { get; set; }
        public int InteriorID { get; set; }
        public float FrontDoorX { get; set; }
        public float FrontDoorY { get; set; }
        public float FrontDoorZ { get; set; }
        public string HouseType { get; set; }
    }
}