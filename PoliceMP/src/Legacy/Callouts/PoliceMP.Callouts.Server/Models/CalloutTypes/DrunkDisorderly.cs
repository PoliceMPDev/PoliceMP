using System.Linq;
using PoliceMP.Main.Core.Server.Enums;
using PoliceMP.Main.Core.Server.Extensions;
using PoliceMP.Main.Core.Shared;
using PoliceMP.Main.Core.Shared.Constants;

namespace PoliceMP.Callouts.Server.Models.CalloutTypes
{
    public class DrunkDisorderly : Callout
    {
        public DrunkDisorderly(int id) : base(id)
        {
            Title = "Drunk and Disorderly";
            Grade = 2;
            Location = Locations.GetRandomCallout().ToCitizenVector3();
        }

        public override void Setup()
        {
            for (var i = 0; i < PoliceMpRandom.Next(1, 2); i++)
            {
                AddEntity(new CalloutPed()
                {
                    Key = $"Drunk{i}",
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
                    ClientEventAPI.SetPedRandomIdleAnim(GetHost().Player, ped.NetworkId);
                    ped.AlcoholLevel = 50;

                    var r = PoliceMpRandom.Next(5);
                    if (r == 1) { ped.OnCannabis = true; }
                    else if (r == 2) { ped.OnCocaine = true; }
                    else if (r == 3) { ped.OnEcstasy = true; }
                    else if (r == 4) { ped.OnHeroin = true; }
                    else if (r == 5) { ped.OnCocaine = true; ped.OnEcstasy = true; ped.OnHeroin = true; ped.OnHeroin = true; }
                }
            }
        }
    }
}
