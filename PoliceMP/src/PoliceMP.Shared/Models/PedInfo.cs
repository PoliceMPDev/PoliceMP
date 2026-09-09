using System;
using System.Collections.Generic;
using System.Linq;
using PoliceMP.Shared.Enums;

namespace PoliceMP.Shared.Models
{
    public class PedInfo
    {
        public int NetworkId { get; set; }

        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string FullName => $"{FirstName} {LastName}";
        public DateTime DateOfBirth { get; set; }
        public string Height { get; set; }
        public Gender Gender { get; set; }

        public string Street { get; set; }
        public string Company { get; set; }
        public string JobTitle { get; set; }

        public float AlcoholLevel { get; set; }
        public bool IsOnCocaine { get; set; }
        public bool IsOnCannabis { get; set; }
        public bool IsOnHeroin { get; set; }
        public bool IsOnEcstasy { get; set; }
        public bool IsOnAnyDrugs => IsOnCocaine || IsOnCannabis || IsOnHeroin || IsOnEcstasy;
        
        public bool HasIllegalItems => Items?.Any(item => item.IsIllegal) == true;

        public int Attitude { get; set; }

        public DateTime ReceivedDrivingLicenseDate { get; set; }
        public bool HasDrivingLicense { get; set; }

        public DateTime DrivingBanExpiryDate { get; set; }
        public bool IsBannedFromDriving { get; set; }
        public int DrivingLicensePoints { get; set; }
        public bool IsWearingSeatbelt { get; set; }
        public string OwnedVehiclePlate { get; set; }

        public List<string> CriminalMarkers { get; set; }
        public List<string> Charges { get; set; }
        public List<string> Warrants { get; set; }

        public List<Item> Items { get; set; }
        
        public string QuestionList { get; set; }
    }
}
