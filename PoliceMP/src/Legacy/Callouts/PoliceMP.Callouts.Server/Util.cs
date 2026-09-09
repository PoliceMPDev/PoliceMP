using Bogus;
using CitizenFX.Core;
using Newtonsoft.Json;
using PoliceMP.Callouts.Server.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using PoliceMP.Main.Core.Shared;

namespace PoliceMP.Callouts.Server
{
    public static class Util
    {
        public static readonly Faker Faker = new Faker();

        private static string HOUSES_FILE = "Houses.json";
        private static string INTERIOR_FILE = "Interiors.json";
        private static List<House> houses = null;
        private static List<Interior> interiors = null;

        // Must be called before housing data is used.
        private static void ReadHousingList()
        {
            if (houses == null || interiors == null)
            {
                try
                {
                    using (var file = File.OpenText(HOUSES_FILE))
                    {
                        string jsonHouses = file.ReadToEnd();
                        houses = JsonConvert.DeserializeObject<List<House>>(jsonHouses);
                    }

                    using (var file = File.OpenText(INTERIOR_FILE))
                    {
                        string jsonInteriors = file.ReadToEnd();
                        interiors = JsonConvert.DeserializeObject<List<Interior>>(jsonInteriors);
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine("Havin a wabbler mate " + ex);
                }
            }
        }

        internal static House GetRandomHouse()
        {
            ReadHousingList();
            return houses[PoliceMpRandom.Next(houses.Count - 1)];
        }

        internal static Interior GetInteriorFromHouse(House house)
        {
            ReadHousingList();
            foreach (var interior in interiors)
            {
                if (interior.ID == house.InteriorID)
                {
                    return interior;
                }
            }
            // Fail safe
            return interiors[0];
        }

        private static string[] _SeriousCrimes = new string[]
        {
            "Posession With Intent to Supply",
            "Armed Robbery",
            "Murder",
            "Manslaughter",
            "Hit and run",
            "Being mean to Welsh People"
        };

        internal static String GetRandomHouseRaidCrime()
        {
            return _SeriousCrimes[PoliceMpRandom.Next(_SeriousCrimes.Count() - 1)];
        }
    }
}
