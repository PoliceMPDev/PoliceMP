using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using PoliceMP.Shared.Enums;

namespace PoliceMP.Shared.Models
{
    public class BootItem
    {
        public string DisplayName { get; set; }
        
        [JsonConverter(typeof(StringEnumConverter))]  
        public Weapon Weapon { get; set; } = Weapon.None;
        
        [JsonConverter(typeof(StringEnumConverter))]  
        public Prop Prop { get; set; } = Prop.None;
        
        
    }
}