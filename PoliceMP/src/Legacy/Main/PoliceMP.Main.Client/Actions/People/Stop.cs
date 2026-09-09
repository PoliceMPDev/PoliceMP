using CitizenFX.Core;
using CitizenFX.Core.Native;
using CitizenFX.Core.UI;
using PoliceMP.Main.Client.Extensions;
using PoliceMP.Main.Client.Managers;
using PoliceMP.Main.Client.Util;
using PoliceMP.Main.Core.Client;
using PoliceMP.Main.Core.Client.Extensions;
using PoliceMP.Main.Shared.Events;

namespace PoliceMP.Main.Client.Actions.People
{
    public class Stop : BaseScript
    {
        /// <summary>
        ///     The command used to execute the action.
        /// </summary>
        [Command(Commands.Stop)]
        private void Command()
        {
            if (PersonSelector.SelectedPerson == null)
            {
                ClientFunctions.SendErrorMessage("You need to select a person to do this.");
                return;
            }

            TriggerEvent(ClientEvents.ACTION_STOP, PersonSelector.SelectedPerson.EntityId());
        }

        [EventHandler(ClientEvents.ACTION_STOP)]
        private void Execute(int pedHandle)
        {
            if (!ActionCooldown.Check()) return;

            var ped = (Ped)Entity.FromHandle(pedHandle);

            if (API.IsPedCuffed(ped.Handle))
            {
                return;
            }

            Screen.ShowSubtitle("~b~You: ~w~Stop right there!");

            ped.Stop();
        }
    }
}