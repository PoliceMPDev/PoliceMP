namespace PoliceMP.Main.Shared.Models
{
    /// <summary>
    ///     Represents an Offence.
    /// </summary>
    public class Offence
    {
        /// <summary>
        ///     The category of offence. (e.g. Traffic, Drugs, and Disorders)
        /// </summary>
        public string Category { get; set; }

        /// <summary>
        ///     The name of the offence.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        ///     How many points that the driver receives for this offence.
        /// </summary>
        public string Points { get; set; }

        /// <summary>
        ///     The fine amount in pounds sterling to be paid for the offence.
        /// </summary>
        public string Fine { get; set; }

        /// <summary>
        ///     Whether the vehicle should be seized for the offence.
        /// </summary>
        public bool SeizeVehicle { get; set; }
    }
}