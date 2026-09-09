namespace PoliceMP.Shared.Models
{
    public class LockerItem
    {
        public string Name { get; set; }
        public string[] AceGroupsRequired { get; set; }
        public bool MaleItem { get; set; }
        public int ComponentID { get; set; }
        public int DrawableID { get; set; }
        public int TextureID { get; set; }
        public int PaletteID { get; set; }
        public bool IsProp { get; set; }
    }
}
