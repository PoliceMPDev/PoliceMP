using System;
using System.Collections.Generic;
using System.Linq;

namespace PoliceMP.Main.Shared.Models
{
    /// <summary>
    ///     Represents a Person.
    /// </summary>
    public class Person
    {
        /// <summary>
        ///     The Ped network ID.
        /// </summary>
        public int NetworkId { get; set; }

        /// <summary>
        ///     The Person's first name.
        /// </summary>
        public string FirstName { get; set; }

        /// <summary>
        ///     The Person's last name.
        /// </summary>
        public string LastName { get; set; }

        /// <summary>
        ///     The Person's full name.
        /// </summary>
        public string FullName => $"{FirstName} {LastName}";

        /// <summary>
        ///     The Person's date of birth.
        /// </summary>
        public DateTime DateOfBirth { get; set; }

        /// <summary>
        ///     The Person's address.
        /// </summary>
        public string Street { get; set; }

        /// <summary>
        ///     The Person's phone number.
        /// </summary>
        public string PhoneNumber { get; set; }

        /// <summary>
        ///     The Person's company.
        /// </summary>
        public string Company { get; set; }

        /// <summary>
        /// The Person's job title.
        /// </summary>
        public string JobTitle { get; set; }

        /// <summary>
        ///     The Person's attitude. Effects whether they resist and other things.
        /// </summary>
        public int Attitude { get; set; }

        /// <summary>
        ///     The current alcohol level.
        /// </summary>
        public float AlcoholLevel { get; set; }

        /// <summary>
        ///     Gets whether the Person is on any drugs.
        /// </summary>
        public bool OnAnyDrugs => OnCocaine || SmokedCannabis || OnHeroin || OnEcstasy;

        /// <summary>
        ///     Gets whether the Person is on cocaine.
        /// </summary>
        public bool OnCocaine { get; set; }

        /// <summary>
        ///     Gets whether the Person has smoked cannabis.
        /// </summary>
        public bool SmokedCannabis { get; set; }

        /// <summary>
        ///     Gets whether the person is on heroin.
        /// </summary>
        public bool OnHeroin { get; set; }

        /// <summary>
        ///     Gets whether the person has taken ecstasy.
        /// </summary>
        public bool OnEcstasy { get; set; }

        /// <summary>
        /// Gets whether the person has a valid driving license.
        /// </summary>
        public bool HasDrivingLicense { get; set; }

        /// <summary>
        /// Gets whether the person has been banned from driving.
        /// </summary>
        public bool IsBannedFromDriving { get; set; }

        /// <summary>
        /// Gets the amount of points the person has.
        /// </summary>
        public int DrivingLicensePoints { get; set; }

        /// <summary>
        /// Gets whether the person will be wearing their seatbelt.
        /// </summary>
        public bool WearsSeatbelt { get; set; }

        /// <summary>
        ///     Gets the weapon that the person is carrying.
        /// </summary>
        public string Weapon { get; set; }

        /// <summary>
        ///     Gets whether the person has a weapon.
        /// </summary>
        public bool HasWeapon => !string.IsNullOrEmpty(Weapon);

        /// <summary>
        /// Gets whether the person has items.
        /// </summary>
        public bool HasItems => Items.Count > 0;

        public bool HasIllegalItems => Items.FirstOrDefault(i => !i.Legal) != null;

        /// <summary>
        /// Gets whether the person has warrants.
        /// </summary>
        public bool HasWarrants => Warrants.Count > 0;

        /// <summary>
        /// Gets whether the person has charges.
        /// </summary>
        public bool HasCharges => Charges.Count > 0;

        /// <summary>
        /// Gets whether the person has markers.
        /// </summary>
        public bool HasMarkers => Markers.Count > 0;

        /// <summary>
        ///     The items that the person is carrying.
        /// </summary>
        public IList<Item> Items { get; set; }

        /// <summary>
        ///     The outstanding warrants on the person.
        /// </summary>
        public IList<string> Warrants { get; set; }

        /// <summary>
        ///     Previous charges given to the person.
        /// </summary>
        public IList<string> Charges { get; set; }

        /// <summary>
        ///     The criminal markers that the person has.
        /// </summary>
        public IList<string> Markers { get; set; }
    }
}