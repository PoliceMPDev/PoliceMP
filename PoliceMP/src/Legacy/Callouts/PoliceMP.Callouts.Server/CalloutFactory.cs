using CitizenFX.Core;
using PoliceMP.Callouts.Server.Models;
using PoliceMP.Callouts.Server.Models.CalloutTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using PoliceMP.Main.Core.Shared;

namespace PoliceMP.Callouts.Server
{
    public static class CalloutFactory
    {
        // How to add a new callout to the factory!
        // { CalloutClassName, Probability callout appears }
        private readonly static Dictionary<string, int> calloutsAndProbabilities = new Dictionary<string, int>(){
            { "ShopLifter", 1 },
            { "AbandonedVehicle", 1 },
            { "StreetFight", 1 },
            { "ANPRHit", 1 },
            { "ArmedRobbery", 1 },
            { "DrinkDrive", 1 },
            { "DrunkDisorderly", 1 },
            { "StolenVehicle", 1 },
            { "IllegalImmigrants", 1 },
            { "MissingPerson", 1 },
            { "Misuse999", 1 },
            { "OngoingAssault", 1 },
            { "RTC", 1 },
            { "SuddenDeath", 1 },
            { "PublicNudity", 1 },
            { "StreetRace", 1 },
            { "GangWar", 0 },
            { "FTS", 1 },
            { "PersonWithKnife", 1 },
            { "WeaponsIntel", 1 },
            { "HouseRaid", 1 },
            { "BankHeist", 0 }
        };

        public static Callout GetSpecificCallout(string calloutName, int calloutId)
        {
            // Aslong as the callouts remain in the same assembly this will function, also the namespace stays similar for each.
            return calloutsAndProbabilities.ContainsKey(calloutName) ? GenerateCalloutObject(calloutName, calloutId) : new Misuse999(calloutId);
        }

        public static Callout Random(int calloutId)
        {
            try
            {
                int sum = calloutsAndProbabilities.Values.Sum();
                var index = PoliceMpRandom.Next(sum);
                foreach (var callout in calloutsAndProbabilities.Keys.ToList())
                {
                    int addition = calloutsAndProbabilities[callout];
                    index -= addition;
                    if (index <= 0)
                    {
                        return GenerateCalloutObject(callout, calloutId);
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed to spawn callout {0}", ex);
            }

            return null;
        }


        private static Callout GenerateCalloutObject(string calloutName, int calloutId)
        {
            // Aslong as the callouts remain in the same assembly this will function, also the namespace stays similar for each.
            object[] parameters = { calloutId };
            return (Callout)Activator.CreateInstance(Type.GetType("PoliceMP.Callouts.Server.Models.CalloutTypes." + calloutName), parameters);
        }
    }

}
