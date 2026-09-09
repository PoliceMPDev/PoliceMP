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
    public class Kneel : BaseScript
    {
        /// <summary>
        ///     The command used to execute the action.
        /// </summary>
        [Command(Commands.Kneel)]
        private void Command()
        {
            if (PersonSelector.SelectedPerson == null)
            {
                ClientFunctions.SendErrorMessage("You need to select a person to do this.");
                return;
            }

            TriggerEvent(ClientEvents.ACTION_KNEEL, PersonSelector.SelectedPerson.EntityId());
        }

        [EventHandler(ClientEvents.ACTION_KNEEL)]
        private void Execute(int pedHandle)
        {
            var ped = (Ped)Entity.FromHandle(pedHandle);
            if (API.IsPedCuffed(ped.Handle))
            {
                return;
            }

            Screen.ShowSubtitle("~b~You: ~w~On your knees!");


            ped.Kneel();

        }
    }
}