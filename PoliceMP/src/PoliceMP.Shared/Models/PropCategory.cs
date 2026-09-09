using System.Collections.Generic;

namespace PoliceMP.Shared.Models
{
    public class PropCategory
    {
        public string CategoryName { get; set; }
        public string[] AceGroupsRequired { get; set; }
        public string[] AllowedBranches { get; set; }
        public List<PropItem> Props { get; set; }
    }
}
