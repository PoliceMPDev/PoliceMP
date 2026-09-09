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
    public class Handsup : BaseScript
    {
        /// <summary>
        ///     The command used to execute the action.
        /// </summary>
        [Command(Commands.Handsup)]
        private void Command()
        {
            if (PersonSelector.SelectedPerson == null)
            {
                ClientFunctions.SendErrorMessage("You need to select a person to do this.");
                return;
            }

            TriggerEvent(ClientEvents.ACTION_HANDSUP, PersonSelector.SelectedPerson.EntityId());
        }

        [EventHandler(ClientEvents.ACTION_HANDSUP)]
        private void Execute(int pedHandle)
        {
            if (!ActionCooldown.Check()) return;

            var ped = (Ped)Entity.FromHandle(pedHandle);

            if (!ped.Exists())
            {
                ClientFunctions.SendErrorMessage("The ped does not exist.");
                return;
            }
            if (API.IsPedCuffed(ped.Handle))
            {
                return;
            }

            Screen.ShowSubtitle("~b~You: ~w~Get your hands in the air now!");

            ped.PlayHandsupAnimAsync();
        }
    }
}