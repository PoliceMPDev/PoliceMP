namespace PoliceMP.Shared.Models
{
    public class PedOutfit
    {
        public string Name { get; set; }
        public string[] AceGroupsRequired { get; set; }
        
        public string LockerType { get;set; }
        public bool MaleOutfit { get; set; }
        public string Category { get; set; }

        //Components
        public Component HEAD { get; set; }  // Head
        public Component BERD { get; set; }  // Beard
        public Component HAIR { get; set; }  // Hair
        public Component UPPR { get; set; }  // Arms
        public Component LOWR { get; set; }  // Pants
        public Component HAND { get; set; }  // Parachute, bag, etc...
        public Component FEET { get; set; }  // Foots
        public Component TEEF { get; set; }  // Tie, scarf, necklace, etc..
        public Component ACCS { get; set; }  // T-Shirt
        public Component TASK { get; set; }  // Bulletproof, bag, etc...
        public Component DECL { get; set; } // Decals
        public Component JBIB { get; set; } // Vest, sweat, jacket, etc...

        //Props
        public Component headProp { get; set; }  // head
        public Component EYES { get; set; }  // eyes
        public Component EARS { get; set; }  // ears
        public Component MOUTH { get; set; }  // mouth
        public Component LEFT_HAND { get; set; }  // lhand
        public Component RIGHT_HAND { get; set; }  // rhand
        public Component LEFT_WRIST { get; set; }  // lwrist
        public Component RIGHT_WRIST { get; set; } // rwrist
        public Component HIP { get; set; }  // hip
        public Component LEFT_FOOT { get; set; } // lfoot
        public Component RIGHT_FOOT { get; set; } // rfoot
        public Component UNK_604819740 { get; set; } // ???
        public Component UNK_2358626934 { get; set; } // ???
    }

    public class Component
    {
        public int DrawableID { get; set; }
        public int TextureID { get; set; }
        public int PaletteID { get; set; }
    }
}