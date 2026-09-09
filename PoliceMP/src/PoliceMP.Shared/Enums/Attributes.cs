using System;

namespace PoliceMP.Shared.Enums
{
    public class BackupItemAttribute : Attribute
    {
        public string Text { get; set; }
        public string Description { get; set; }
        public string Command { get; set; }
        public int Grade { get; set; }
    }
    
    public class WeaponAttribute : Attribute
    {
        public string Name { get; set; }
    }
}