using System;
using PoliceMP.Main.Core.Server.Enums;

namespace PoliceMP.Callouts.Server.Models
{
    public class CalloutPed : CalloutEntity
    {
        public PedHash Hash { get; set; }
        public WeaponHash WeaponHash { get; set; } = WeaponHash.Unarmed;

        public bool GenerateInfoManual { get; set; } = false;

        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public DateTime BirthDate { get; set; } = default;
        public float AlcoholLevel { get; set; } = 0f;
        public bool OnCocaine { get; set; } = false;
        public bool OnCannabis { get; set; } = false;
        public bool OnHeroin { get; set; } = false;
        public bool OnEcstasy { get; set; } = false;
        public bool HasDrivingLicense { get; set; } = true;
        public bool IsBannedFromDriving { get; set; } = false;
        public int DrivingLicensePoints { get; set; } = 0;
        public bool WearsSeatbelt { get; set; } = true;
        public string[] Items { get; set; } = new string[0];
        public string[] Warrants { get; set; } = new string[0];
        public string[] Charges { get; set; } = new string[0];
        public string[] Markers { get; set; } = new string[0];
    }
}
