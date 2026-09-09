using System;
using System.Collections.Generic;
using System.Linq;

namespace PoliceMP.Shared.Models
{
    public class VehicleInfo
    {
        public int NetworkId { get; set; }
        public string Plate { get; set; }
        public string OwnerName { get; set; }

        public string VIN { get; set; }

        public DateTime MotExpiryDate { get; set; }
        public DateTime TaxExpiryDate { get; set; }
        public DateTime InsuranceExpiryDate { get; set; }

        public bool IsMotExpired => MotExpiryDate < DateTime.Now.Date;
        public bool IsTaxExpired => TaxExpiryDate < DateTime.Now.Date;
        public bool IsInsuranceExpired => InsuranceExpiryDate < DateTime.Now.Date;

        public List<string> Markers { get; set; }
        public bool HasMarker(string marker) => Markers.Contains(marker);

        public List<Item> Items { get; set; }
        public bool HasIllegalItems => Items.Any(item => item.IsIllegal);

        public bool IsIllegal =>
            IsMotExpired ||
            IsTaxExpired ||
            IsInsuranceExpired ||
            Markers.Any();
    }
}