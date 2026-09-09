using CitizenFX.Core;
using System.Linq;
using PoliceMP.Main.Core.Server.Enums;
using PoliceMP.Main.Core.Server.Extensions;
using PoliceMP.Main.Core.Shared;
using PoliceMP.Main.Core.Shared.Constants;

namespace PoliceMP.Callouts.Server.Models.CalloutTypes
{
    public class ArmedRobbery : Callout
    {
        public ArmedRobbery(int id) : base(id)
        {
            Title = "Armed Robbery";
            Grade = 1;
            Description = "Armed robbery in progress at a shop";
            Location = Locations.GetRandomShop().ToCitizenVector3();
        }

        public override void Setup()
        {
            var perp = new CalloutPed()
            {
                Key = $"Perp",
                Hash = (PedHash)(uint)PedModels.GetRandomHuman(),
                BlipColor = BlipColor.Red,
                HasBlip = true,
                SpawnLocation = Location
            };

            AddEntity(perp);
        }

        public override void OnPlayerArrived(Player player)
        {
            base.OnPlayerArrived(player);
            ClientEventAPI.ShowSubtitle(player, "You have arrived. Catch the ~r~robbers~w~!", 5000);
        }

        protected override void OnFirstPlayerArrived(Player player)
        {
            base.OnFirstPlayerArrived(player);
            foreach (var entity in _entities.Values.ToList())
            {
                if (entity is CalloutPed ped)
                {
                    ClientEventAPI.SetPedAttackPlayer(player, ped.NetworkId);

                    var random = PoliceMpRandom.Next(10);
                    if (random >= 1 && random <= 5)
                    {
                        //Mele
                        var randomWeapon = PoliceMpRandom.Next(5);
                        if (randomWeapon == 1)
                        {
                            ped.WeaponHash = WeaponHash.Ball;
                        }
                        else if (randomWeapon == 2)
                        {
                            ped.WeaponHash = WeaponHash.Crowbar;
                        }
                        else if (randomWeapon == 3)
                        {
                            ped.WeaponHash = WeaponHash.BattleAxe;
                        }
                        else if (randomWeapon == 4)
                        {
                            ped.WeaponHash = WeaponHash.Dagger;
                        }
                        else
                        {
                            ped.WeaponHash = WeaponHash.Knife;
                        }
                    }
                    else if (random >= 6 && random <= 9)
                    {
                        //Pistol
                        ped.WeaponHash = WeaponHash.Pistol;
                    }
                    else
                    {
                        //Big gat
                        ped.WeaponHash = WeaponHash.AssaultRifle;
                    }
                }
            }
        }
    }
}
