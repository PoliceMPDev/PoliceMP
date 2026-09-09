using System;
using CitizenFX.Core;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using PoliceMP.Main.Core.Server.Enums;
using PoliceMP.Main.Core.Shared;
using PoliceMP.Main.Core.Shared.Constants;

namespace PoliceMP.Callouts.Server.Models.CalloutTypes
{
    public class GangWar : Callout
    {
        // Grove Street locations for each gang
        /*        private readonly Vector3 GREEN_GANG_LOCATION = new Vector3(76, -1847, 25);
                private readonly Vector3 PURPLE_GANG_LOCATION = new Vector3(116, -1933, 21);
                private readonly Vector3 ARRIVED_LOCATION = new Vector3(62, -1904, 22);*/

        // {Green Gang Location, Purple Gang Location, Arrival Location}
        private static readonly List<List<Vector3>> GangWarLocations = new List<List<Vector3>>() {
            new List<Vector3>() { new Vector3(76, -1847, 25), new Vector3(116, -1933, 21), new Vector3(62, -1904, 22)},// Purple home
            new List<Vector3>() { new Vector3(-214, -1604, 35), new Vector3(-149, -1648, 33), new Vector3(-136, -1544, 35)}, // Green home
            new List<Vector3>() { new Vector3(1182, -3035, 6), new Vector3(1082, -3046, 6), new Vector3(1050, -2960, 6)}, // Docks
            new List<Vector3>() { new Vector3(94, -1220, 30), new Vector3 (151, -1199, 30), new Vector3(129, -1246, 30)} // Homeless camp
        };

        private readonly int _gangLocationIndex = PoliceMpRandom.Next(GangWarLocations.Count);
        private readonly Vector3 _greenGangLocation = new Vector3(0, 0, 0);
        private readonly Vector3 _purpleGangLocation = new Vector3(0, 0, 0);
        private readonly Vector3 _arrivedLocation = new Vector3(0, 0, 0);


        private readonly string GANG_ONE_STRING = "Green Gang";
        private readonly string GANG_TWO_STRING = "Purple Gang";

        public GangWar(int id) : base(id)
        {
            // Setup randomized location
            _greenGangLocation = GangWarLocations[_gangLocationIndex][0];
            _purpleGangLocation = GangWarLocations[_gangLocationIndex][1];
            _arrivedLocation = GangWarLocations[_gangLocationIndex][2];


            Title = "Gang War"; // Do not change this whilst location setting is manual.
            Grade = 0;
            Description = "A gang war has broken out between rival gangs, Armed Response required!";
            LocationSetting = LocationSetting.Manual;
            Location = _arrivedLocation;

        }

        public override void Setup()
        {
            try
            {
                // Add 10 of each Ped
                for (var i = 0; i < 4; ++i)
                {
                    var greenPed = new CalloutPed()
                    {
                        Key = $"{GANG_ONE_STRING} {i}",
                        Hash = (PedHash)(uint)PedModels.GetRandomGreenGang(),
                        BlipColor = BlipColor.Red,
                        HasBlip = true,
                        SpawnLocation = _greenGangLocation
                    };

                    var purplePed = new CalloutPed()
                    {
                        Key = $"{GANG_TWO_STRING} {i}",
                        Hash = (PedHash)(uint)PedModels.GetRandomPurpleGang(),
                        BlipColor = BlipColor.Red,
                        HasBlip = true,
                        SpawnLocation = _purpleGangLocation
                    };

                    WeaponHash weapon = WeaponHash.Pistol;

                    var index = PoliceMpRandom.Next(15);
                    if (index == 2) weapon = WeaponHash.Pistol50;
                    else if (index == 3) weapon = WeaponHash.PistolMk2;
                    else if (index == 4) weapon = WeaponHash.SMG;
                    else if (index == 5) weapon = WeaponHash.SawnOffShotgun;

                    greenPed.WeaponHash = weapon;
                    purplePed.WeaponHash = weapon;

                    AddEntity(greenPed);
                    AddEntity(purplePed);
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine("Error @ Setup");
                Debug.WriteLine(e.ToString());
            }
        }

        protected override void OnFirstPlayerArrived(Player player)
        {
            try
            {
                base.OnFirstPlayerArrived(player);

                // Poplate Green and Purple Gang ped lists
                var entities = _entities.Values.ToList();

                List<int> greenPeds = new List<int>();
                List<int> purplePeds = new List<int>();
                foreach (var entity in entities)
                {
                    if (entity is CalloutPed ped)
                    {
                        if (ped.Key.Contains(GANG_ONE_STRING))
                        {
                            greenPeds.Add(ped.NetworkId);
                        }
                        else if (ped.Key.Contains(GANG_TWO_STRING))
                        {
                            purplePeds.Add(ped.NetworkId);
                        }
                        else
                        {
                            Debug.WriteLine("Something went wrong in the Callout API for GangWar Ped assignment");
                        }
                    }
                }

                var pGroupString = JsonConvert.SerializeObject(purplePeds);
                var gGroupString = JsonConvert.SerializeObject(greenPeds);
                ClientEventAPI.RunTheGangWar(player, pGroupString, gGroupString);
            }
            catch (Exception e)
            {
                Debug.WriteLine("Error @ OnFirstPlayerArrived");
                Debug.WriteLine(e.ToString());
            }
        }
    }
}
