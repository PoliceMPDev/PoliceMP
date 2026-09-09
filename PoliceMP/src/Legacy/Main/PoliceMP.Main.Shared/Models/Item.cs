namespace PoliceMP.Main.Shared.Models
{
    /// <summary>
    ///     Represents an Item.
    /// </summary>
    public class Item
    {
        /// <summary>
        ///     The Item Id.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        ///     The name of the Item.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        ///     Whether the Item is legal.
        /// </summary>
        public bool Legal { get; set; }

        /// <summary>
        ///     The description of the Item.
        /// </summary>
        public string Desc { get; set; }

        /// <summary>
        ///     The chance of this item appearing.
        /// </summary>
        public int Chance { get; set; }
    }
}