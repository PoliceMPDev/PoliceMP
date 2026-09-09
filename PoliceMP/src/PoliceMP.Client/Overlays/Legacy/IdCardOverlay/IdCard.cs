using System.Collections.Generic;

namespace PoliceMP.Client.Overlays.Legacy.IdCardOverlay
{
    public class IdCard
    {
        public IdCardType Type { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string DateOfBirth { get; set; }
        public bool IsMale { get; set; }
        public string Height { get; set; }
        public List<string> DrivingLicenseTypes { get; set; }
    }

    public enum IdCardType
    {
        None,
        Driver,
        Weapon
    }
}