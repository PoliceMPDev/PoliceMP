using CitizenFX.Core;
using System.Linq;
using PoliceMP.Main.Core.Server.Enums;
using PoliceMP.Main.Core.Server.Extensions;
using PoliceMP.Main.Core.Shared;
using PoliceMP.Main.Core.Shared.Constants;

namespace PoliceMP.Callouts.Server.Models.CalloutTypes
{
    class PersonWithKnife : Callout
    {
        public PersonWithKnife(int id) : base(id)
        {
            Title = "Person With A Knife"; // Do not change this whilst location setting is manual.
            Grade = 0;
            Description = "A person has been spotted with a knife.";
            Location = Locations.GetRandomCallout().ToCitizenVector3();
        }

        public override void Setup()
        {
            var perp = new CalloutPed()
            {
                Key = $"KnifePerp",
                Hash = (PedHash)(uint)PedModels.GetRandomHuman(),
                BlipColor = BlipColor.Red,
                HasBlip = true,
                SpawnLocation = Location
            };

            perp.WeaponHash = WeaponHash.Knife;

            AddEntity(perp);
        }

        public override void OnPlayerArrived(Player player)
        {
            base.OnPlayerArrived(player);
            ClientEventAPI.ShowSubtitle(player, "You have arrived. Arrest the ~r~person with a knife~w~!", 5000);
        }

        protected override void OnFirstPlayerArrived(Player player)
        {
            base.OnFirstPlayerArrived(player);
            foreach (var entity in _entities.Values.ToList())
            {
                if (entity is CalloutPed ped)
                {
                    var r = PoliceMpRandom.Next(4);
                    if (r <= 2)
                    {
                        ClientEventAPI.SetPedAttackPlayer(player, ped.NetworkId);
                    }
                    else
                    {
                        ClientEventAPI.SetPedFleeFromPlayer(player, ped.NetworkId);
                    }
                }
            }
        }
    }
}
