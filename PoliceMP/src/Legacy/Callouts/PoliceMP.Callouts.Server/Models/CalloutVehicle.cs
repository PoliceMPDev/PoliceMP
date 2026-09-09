using System.Collections.Generic;
using PoliceMP.Main.Core.Server.Enums;

namespace PoliceMP.Callouts.Server.Models
{
    public class CalloutVehicle : CalloutEntity
    {
        public VehicleHash Hash { get; set; }
        public VehicleColor Colour { get; set; } = VehicleColor.MatteWhite;

        public float BodyHealth { get; set; } = 1000f;
        public float EngineHealth { get; set; } = 1000f;
        public float PetrolTankHealth { get; set; } = 1000f;

        public List<string> Passengers { get; set; } = new List<string>();
        public string Driver { get; set; }

        public bool GenerateInfoManual { get; set; } = false;

        public string Plate { get; set; } = string.Empty;
        /// <summary>
        /// Not the entity key, must use full name of Person ie "Big Ben"
        /// </summary>
        public string Owner { get; set; } = string.Empty;
        public bool HasInsurance { get; set; } = true;
        public bool HasTax { get; set; } = true;
        public bool HasMot { get; set; } = true;
        public string[] Markers { get; set; } = new string[0];
        public string[] Items { get; set; } = new string[0];
    }
}
