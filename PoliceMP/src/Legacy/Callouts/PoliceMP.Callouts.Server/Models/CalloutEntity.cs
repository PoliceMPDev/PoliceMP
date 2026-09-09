using CitizenFX.Core;
using System.Collections.Generic;
using PoliceMP.Main.Core.Server.Enums;

namespace PoliceMP.Callouts.Server.Models
{
    public abstract class CalloutEntity
    {
        public int NetworkId { get; set; }
        public string Key { get; set; }
        public Vector3 SpawnLocation { get; set; }
        public bool HasBlip { get; set; }
        public BlipSprite BlipSprite { get; set; } = BlipSprite.Standard;
        public BlipColor BlipColor { get; set; } = BlipColor.Yellow;
        public Dictionary<string, bool> Flags { get; set; } = new Dictionary<string, bool>();
    }
}
