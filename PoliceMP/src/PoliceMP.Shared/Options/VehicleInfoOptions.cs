using System.Collections.Generic;

namespace PoliceMP.Shared.Options
{
    public class VehicleInfoOptions
    {
        public int MotExpiredChance { get; set; }
        public int TaxExpiredChance { get; set; }
        public int InsuranceExpiredChance { get; set; }
        public int MarkerChance { get; set; }
        public int MultipleMarkersChance { get; set; }
        public int MaxMarkers { get; set; }
        public List<string> Markers { get; set; }

        public int HasIllegalItemsChance { get; set; }
        public int MaxIllegalItems { get; set; }
        public int HasLegalItemsChance { get; set; }
        public int MaxLegalItems { get; set; }
    }
}