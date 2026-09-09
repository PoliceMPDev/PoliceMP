using System.Collections.Generic;

namespace PoliceMP.Shared.Models
{
    public class GarageCategory
    {
        public string CategoryName { get; set; }
        public string[] AceGroupsRequired { get; set; }
        public string[] OptionalAceGroups { get; set; }
        public List<GarageVehicles> Vehicles { get; set; }
    }
}
