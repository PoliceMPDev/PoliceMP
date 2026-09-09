using CitizenFX.Core;
using PoliceMP.Main.Core.Server.Enums;
using PoliceMP.Main.Core.Server.Extensions;
using PoliceMP.Main.Core.Shared.Constants;

namespace PoliceMP.Callouts.Server.Models.CalloutTypes
{
    public class MissingPerson : Callout
    {
        public MissingPerson(int id) : base(id)
        {
            Title = "Lost Person";
            Grade = 2;
            IsSearch = true;
            Location = Locations.GetRandomRural().ToCitizenVector3();
        }

        public override void Setup()
        {
            AddEntity(new CalloutPed()
            {
                Key = "MissingPerson",
                Hash = (PedHash)(uint)PedModels.GetRandomHuman(),
                HasBlip = true,
                SpawnLocation = Location
            });
        }

        public override void OnPlayerArrived(Player player)
        {
            base.OnPlayerArrived(player);

            ClientEventAPI.ShowSubtitle(player, "~y~Missing Person: ~w~Oh my god, you found me!");
        }
    }
}
