using System.Linq;
using PoliceMP.Main.Core.Server.Enums;
using PoliceMP.Main.Core.Server.Extensions;
using PoliceMP.Main.Core.Shared;
using PoliceMP.Main.Core.Shared.Constants;

namespace PoliceMP.Callouts.Server.Models.CalloutTypes
{
    public class Misuse999 : Callout
    {
        public Misuse999(int id) : base(id)
        {
            Title = "Misuse Of 999";
            Grade = 3;
            Location = Locations.GetRandomCallout().ToCitizenVector3();
        }

        public override void Setup()
        {
            for (var i = 0; i < PoliceMpRandom.Next(1, 2); i++)
            {
                AddEntity(new CalloutPed()
                {
                    Key = $"Ped{i}",
                    Hash = (PedHash)(uint)PedModels.GetRandomHuman(),
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
                    ClientEventAPI.SetPedFleeFromPlayer(GetHost().Player, ped.NetworkId);
                }
            }
        }
    }
}
