using System.Linq;
using PoliceMP.Main.Core.Server.Enums;
using PoliceMP.Main.Core.Server.Extensions;
using PoliceMP.Main.Core.Shared;
using PoliceMP.Main.Core.Shared.Constants;

namespace PoliceMP.Callouts.Server.Models.CalloutTypes
{
    public class PublicNudity : Callout
    {
        public PublicNudity(int id) : base(id)
        {
            Title = "Public Nudity";
            Grade = 3;
            Location = Locations.GetRandomCallout().ToCitizenVector3();
        }

        public override void Setup()
        {
            for (var i = 0; i < PoliceMpRandom.Next(1, 4); i++)
            {
                AddEntity(new CalloutPed()
                {
                    Key = $"Nude{i}",
                    Hash = (PedHash)(uint)PedModels.GetRandomNaked(),
                    HasBlip = true,
                    SpawnLocation = Location
                });
            }
        }

        protected override void OnEntitiesReady()
        {
            foreach (var entity in _entities.Values.ToList())
            {
                if (entity is CalloutPed ped)
                {
                    ClientEventAPI.SetPedRandomIdleAnim(GetHost().Player, ped.NetworkId);
                }
            }
        }
    }
}
