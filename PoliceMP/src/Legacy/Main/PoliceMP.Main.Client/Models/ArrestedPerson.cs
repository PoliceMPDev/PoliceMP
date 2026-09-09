namespace PoliceMP.Main.Client.Models
{
    /// <summary>
    ///     Represents an arrested person.
    /// </summary>
    public class ArrestedPerson
    {
        /// <summary>
        ///     Creates a new ArrestedPerson.
        /// </summary>
        /// <param name="entityId">The ped entity ID.</param>
        /// <param name="blipId">The ped blip ID.</param>
        public ArrestedPerson(int entityId, int blipId)
        {
            EntityId = entityId;
            BlipId = blipId;
        }

        /// <summary>
        ///     The entity ID.
        /// </summary>
        public int EntityId { get; set; }

        /// <summary>
        ///     The blip ID.
        /// </summary>
        public int BlipId { get; set; }
    }
}