namespace PoliceMP.Shared.Models
{
    public class Offence
    {
        public string Category { get; set; }
        public string Name { get; set; }
        public int Points { get; set; }
        public int Fine { get; set; }
        public bool ShouldSeizeVehicle { get; set; }
    }
}