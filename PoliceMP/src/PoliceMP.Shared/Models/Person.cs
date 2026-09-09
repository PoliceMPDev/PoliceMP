using System;

namespace PoliceMP.Shared.Models
{
    public class Person
    {
        public int NetworkId { get; set; }

        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string FullName => $"{FirstName} {LastName}";
        public DateTime DateOfBirth { get; set; }

        public string Street { get; set; }
        public string Company { get; set; }
        public string JobTitle { get; set; }

        public float AlcoholLevel { get; set; }
        public bool IsOnCocaine { get; set; }
        public bool IsOnCannabis { get; set; }
        public bool IsOnHeroin { get; set; }
        public bool IsOnEcstasy { get; set; }
        public bool HasDrivingLicense { get; set; }
        public bool IsBannedFromDriving { get; set; }
        public int DrivingLicensePoints { get; set; }
        public bool IsWearingSeatbelt { get; set; }
    }
}
