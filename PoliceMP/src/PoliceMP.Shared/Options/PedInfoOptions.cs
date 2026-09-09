using System.Collections.Generic;

namespace PoliceMP.Shared.Options
{
    public class PedInfoOptions
    {
        public int DrunkChance { get; set; }
        public int CocaineChance { get; set; }
        public int CannabisChance { get; set; }
        public int EcstasyChance { get; set; }
        public int HeroinChance { get; set; }
        public int WeaponChance { get; set; }
        public int MarkerChance { get; set; }
        public int WarrantChance { get; set; }
        public int ChargeChance { get; set; }
        public int MultipleChargesChance { get; set; }
        public int MaxCharges { get; set; }
        public int HasDrivingLicenseChance { get; set; }
        public int HasHgvLicenseChance { get; set; }
        public int HasMotorcycleLicenseChance { get; set; }
        public int IsBannedFromDrivingChance { get; set; }
        public int HasPointsChance { get; set; }
        public int NoSeatbeltChance { get; set; }
        public int UnemployedChance { get; set; }

        public int HasLegalItemsChance { get; set; }
        public int HasIllegalItemsChance { get; set; }
        public int MaxLegalItems { get; set; }
        public int MaxIllegalItems { get; set; }

        public List<string> CriminalMarkers { get; set; }
        public List<string> Charges { get; set; }
        public List<string> Warrants { get; set; }
    }
}