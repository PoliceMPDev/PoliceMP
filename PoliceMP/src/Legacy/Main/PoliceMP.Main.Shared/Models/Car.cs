using System;
using System.Collections.Generic;
using System.Linq;

namespace PoliceMP.Main.Shared.Models
{
    /// <summary>
    ///     Represents a Car.
    /// </summary>
    public class Car
    {
        /// <summary>
        ///     The network ID of the vehicle.
        /// </summary>
        public int NetworkId { get; set; }

        /// <summary>
        ///     The registration number.
        /// </summary>
        public string Plate { get; set; }

        /// <summary>
        /// The vehicle identification number.
        /// </summary>
        public string Vin { get; set; }

        /// <summary>
        ///     The owner of the vehicle's full name.
        /// </summary>
        public string Owner { get; set; }

        /// <summary>
        ///     Gets whether the Car has insurance.
        /// </summary>
        public bool HasInsurance => InsuranceDue > DateTime.Now;

        /// <summary>
        ///     Gets whether the Car has tax.
        /// </summary>
        public bool HasTax => TaxDue > DateTime.Now;

        /// <summary>
        ///     Gets whether the Car has MOT.
        /// </summary>
        public bool HasMot => MotDue > DateTime.Now;

        /// <summary>
        ///     The date that the Car's insurance is due to be renewed.
        /// </summary>
        public DateTime InsuranceDue { get; set; }

        /// <summary>
        ///     The date that the Car's tax is due to be renewed.
        /// </summary>
        public DateTime TaxDue { get; set; }

        /// <summary>
        ///     The date that the Car's MOT is due to be renewed.
        /// </summary>
        public DateTime MotDue { get; set; }

        /// <summary>
        ///     The criminal markers on the Car.
        /// </summary>
        public List<string> Markers { get; set; }

        /// <summary>
        ///     Gets whether the Car has any criminal markers.
        /// </summary>
        public bool HasMarkers => Markers.Count > 0;

        public bool HasMarker(string marker) => Markers.Contains(marker);

        public bool HasIllegalItems => Items.FirstOrDefault(i => !i.Legal) != null;

        public bool IsIllegal => HasMarkers || !HasMot || !HasTax || !HasInsurance;

        /// <summary>
        ///     The Items that are inside the Car.
        /// </summary>
        public List<Item> Items { get; set; }
    }
}