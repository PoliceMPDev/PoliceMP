using CitizenFX.Core;
using Newtonsoft.Json;
using PoliceMP.Callouts.Server.Models.BankHeists;
using System.Collections.Generic;
using System.Linq;
using PoliceMP.Main.Core.Server.Enums;
using PoliceMP.Main.Core.Shared;
using PoliceMP.Main.Core.Shared.Constants;

namespace PoliceMP.Callouts.Server.Models.CalloutTypes
{
    public class BankHeist : Callout
    {
        private string BAD_PED_STRING = "BadGuy";
        private string CIV_PED_STRING = "Civ";

        private List<BankHeistLocation> possibleLocations = new List<BankHeistLocation>()
        {
            new UpTownBankLocation(),
            new PoliceStationBank(),
            new PaletoBayBank(),
            new OceanHighwayBank(),
            new GolfBank()
        };
        private BankHeistLocation location;

        public BankHeist(int id) : base(id)
        {
            location = possibleLocations[PoliceMpRandom.Next(possibleLocations.Count)];

            Title = "Bank Heist"; // Do not change this whilst location setting is manual.
            Grade = 0;
            Description = "Bank Heist has started ";
            LocationSetting = LocationSetting.Manual;
            Location = location.ArrivalLocation;
        }

        public override void Setup()
        {
            // Add bad guys
            for (var i = 0; i < location.BadGuyLocations.Count; ++i)
            {
                var newBadGuy = new CalloutPed()
                {
                    Key = $"{BAD_PED_STRING}{i}",
                    Hash = (PedHash)(uint)PedModels.GetRandomMexicanGang(),
                    BlipColor = BlipColor.Red,
                    HasBlip = true,
                    SpawnLocation = location.BadGuyLocations[i]
                };

                var weapon = WeaponHash.AssaultRifle;
                var weaponIndex = PoliceMpRandom.Next(3);
                if (weaponIndex == 1) weapon = WeaponHash.AssaultShotgun;
                else if (weaponIndex == 2) weapon = WeaponHash.BullpupRifleMk2;

                newBadGuy.WeaponHash = weapon;

                AddEntity(newBadGuy);
            }

            // Add civs
            for (var i = 0; i < location.CivilianLocations.Count; ++i)
            {
                var civ = new CalloutPed()
                {
                    Key = $"{CIV_PED_STRING}{i}",
                    Hash = (PedHash)(uint)PedModels.GetRandomMediumClass(),
                    BlipColor = BlipColor.White,
                    HasBlip = true,
                    SpawnLocation = location.CivilianLocations[i]
                };

                AddEntity(civ);
            }
        }

        protected override void OnFirstPlayerArrived(Player player)
        {
            base.OnFirstPlayerArrived(player);

            // Populate civ and bad guys list
            var entities = _entities.Values.ToList();
            List<int> badPeds = new List<int>();
            List<int> civPeds = new List<int>();

            foreach (var entity in entities)
            {
                if (entity is CalloutPed ped)
                {
                    if (ped.Key.Contains(BAD_PED_STRING))
                    {
                        badPeds.Add(ped.NetworkId);
                    }
                    else if (ped.Key.Contains(CIV_PED_STRING))
                    {
                        civPeds.Add(ped.NetworkId);
                    }
                }
            }

            var badPedString = JsonConvert.SerializeObject(badPeds);
            var civPedString = JsonConvert.SerializeObject(civPeds);
            ClientEventAPI.RunBankHeist(player, badPedString, civPedString);
        }
    }
}
