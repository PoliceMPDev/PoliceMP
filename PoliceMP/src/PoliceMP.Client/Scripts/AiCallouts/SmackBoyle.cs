using System;
using System.Collections.Generic;
using CitizenFX.Core;
using CitizenFX.Core.Native;
using PoliceMP.Core.Client.Scripts;

namespace PoliceMP.Client.Scripts.AiCallouts
{
    public class SmackBoyle : Script
    {
        public SmackBoyle()
        {
            API.RegisterCommand("smackboyle", new Action(BoyleSmacker), false);
        }

        private readonly Random _random = new Random();

        private static readonly List<Vector3> PedLocations = new List<Vector3>()
        {
            new((float)-1109.99, (float)-847.01, (float)19.31)
        };

        private List<Ped> _peds = new List<Ped>();

        private async void BoyleSmacker()
        {
            var speeches = new List<string>()
            {
                "GENERIC_CURSE_HIGH",
                "GENERIC_FUCK_YOU",
                "DYING_HELP",
                "COVER_ME",
                "GENERIC_FRIGHTENED_HIGH",
                "GENERIC_FRIGHTENED_MED",
                "GENERIC_INSULT_HIGH",
                "GENERIC_WAR_CRY",
                "RELOADING",
                "SHOOT",
                "TAKE_COVER",
                "STAY_DOWN"
            };


            var weapons = new List<uint>()
            {
                (uint)WeaponHash.Nightstick
            };
            _peds = new List<Ped>()
            {
                await World.CreatePed(PedHash.ChemSec01SMM, PedLocations[0])
            };

            foreach (var ped in _peds)
            {
                var player = Game.Player.Character;
                API.RequestModel(ped.Model);
                while (!API.HasModelLoaded(ped.Model))
                {
                    Debug.WriteLine(("Waiting for model to load"));
                    await BaseScript.Delay(100);
                }

                ped.Task.ShootAt(player);
                API.SetPedAsGroupMember(ped.Handle, API.GetPedGroupIndex(player.Handle));
                API.SetPedCombatAbility(ped.Handle, 2);
                API.GiveWeaponToPed(ped.Handle, weapons[_random.Next(0, weapons.Count)], 1000, false, true);
                ped.PlayAmbientSpeech(speeches[_random.Next(0, speeches.Count)]);
            }
        }
    }
}