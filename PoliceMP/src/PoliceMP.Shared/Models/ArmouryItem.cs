using System.Collections.Generic;

namespace PoliceMP.Shared.Models
{
    public class ArmouryItem
    {
        public string WeaponName { get; set; }
        public string WeaponClass { get; set; }
        public string[] AceGroupsRequired { get; set; }
        public List<WeaponComponent> Components { get; set; }
    }

    public class WeaponComponent
    {
        public string ComponentName { get; set; }
        public string ComponentClass { get; set; }
    }
}
