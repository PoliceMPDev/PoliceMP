using PoliceMP.Main.Core.Server.Enums;
using PoliceMP.Main.Core.Server.Extensions;
using PoliceMP.Main.Core.Shared.Constants;

namespace PoliceMP.Callouts.Server.Models.CalloutTypes
{
    public class SuddenDeath : Callout
    {
        public SuddenDeath(int id) : base(id)
        {
            Title = "Sudden Death";
            Grade = 1;
            Location = Locations.GetRandomCallout().ToCitizenVector3();
        }

        public override void Setup()
        {
            AddEntity(new CalloutPed()
            {
                Key = "DeadPerson",
                Hash = (PedHash)(uint)PedModels.GetRandomHuman(),
                HasBlip = true,
                SpawnLocation = Location
            });
        }

        protected override void OnEntitiesReady()
        {
            base.OnEntitiesReady();

            var deadPerson = (CalloutPed)GetEntity("DeadPerson");

            ClientEventAPI.ApplyPedDamage(GetHost().Player, deadPerson.NetworkId, 5000);
        }
    }
}
