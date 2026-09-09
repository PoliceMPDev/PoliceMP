using CitizenFX.Core;
using System.Linq;
using PoliceMP.Main.Core.Server.Enums;
using PoliceMP.Main.Core.Server.Extensions;
using PoliceMP.Main.Core.Shared;
using PoliceMP.Main.Core.Shared.Constants;

namespace PoliceMP.Callouts.Server.Models.CalloutTypes
{
    public class ShopLifter : Callout
    {
        public ShopLifter(int id) : base(id)
        {
            Title = "Shop Lifter";
            Grade = 3;
            Description = "Some fucker has shoplifted";
            Location = Locations.GetRandomShop().ToCitizenVector3();
        }

        public override void Setup()
        {
            var liftersCount = PoliceMpRandom.Next(1, 3);

            for (var i = 0; i < liftersCount; i++)
            {
                AddEntity(new CalloutPed()
                {
                    Key = $"ShopLifter{i}",
                    Hash = (PedHash)(uint)PedModels.GetRandomHuman(),
                    BlipColor = BlipColor.Red,
                    HasBlip = true,
                    SpawnLocation = Location
                });
            }
        }

        public override void OnPlayerArrived(Player player)
        {
            base.OnPlayerArrived(player);
            ClientEventAPI.ShowSubtitle(player, "You have arrived. Catch the ~r~shop lifters~w~!", 5000);
        }

        protected override void OnFirstPlayerArrived(Player player)
        {
            base.OnFirstPlayerArrived(player);
            foreach (var entity in _entities.Values.ToList())
            {
                if (entity is CalloutPed ped)
                {
                    ClientEventAPI.SetPedAttackPlayer(player, ped.NetworkId);
                }
            }
        }
    }
}
