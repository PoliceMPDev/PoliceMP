namespace PoliceMP.Client.Overlays.Legacy.AnprOverlay
{
    public class AnprViewModel
    {
        public string Plate { get; set; }
        public int Mph { get; set; }
        public string Model { get; set; }
        public string Colour { get; set; }
        public bool MOT { get; set; }
        public bool Insurance { get; set; }
        public bool Tax { get; set; }
        public bool DrugsIntel { get; set; }
        public bool WeaponsIntel { get; set; }
        public bool FailToStop { get; set; }
        public bool Stolen { get; set; }
        public bool OutstandingCrime { get; set; }
        public bool Other { get; set; }
    }
}