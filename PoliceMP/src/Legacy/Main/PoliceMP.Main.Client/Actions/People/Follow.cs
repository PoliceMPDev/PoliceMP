using CitizenFX.Core;
using CitizenFX.Core.Native;
using CitizenFX.Core.UI;
using PoliceMP.Main.Client.Extensions;
using PoliceMP.Main.Client.Managers;
using PoliceMP.Main.Client.Retrievers;
using PoliceMP.Main.Client.Util;
using PoliceMP.Main.Core.Client;
using PoliceMP.Main.Core.Client.Extensions;
using PoliceMP.Main.Shared.Events;

namespace PoliceMP.Main.Client.Actions.People
{
    /// <summary>
    ///     Used to make a person follow the player.
    /// </summary>
    public class Follow : BaseScript
    {
        /// <summary>
        ///     The command used to execute the action.
        /// </summary>
        [Command(Commands.Follow)]
        private void Command()
        {
            if (PersonSelector.SelectedPerson == null)
            {
                ClientFunctions.SendErrorMessage("You need to select a person to do this.");
                return;
            }

            TriggerEvent(ClientEvents.ACTION_FOLLOW, PersonSelector.SelectedPerson.EntityId());
        }

        /// <summary>
        ///     Handles the FOLLOW_PERSON event.
        /// </summary>
        [EventHandler(ClientEvents.ACTION_FOLLOW)]
        private async void Execute(int pedHandle)
        {
            if (!ActionCooldown.Check()) return;

            var person = await PersonRetriever.GetPerson(pedHandle);
            if (person == null)
            {
                ClientFunctions.SendErrorMessage("Could not retrieve the person.");
                return;
            }

            // If already in player group, make them unfollow.
            var playerGroup = API.GetPlayerGroup(Game.PlayerPed.Handle);
            if (API.IsPedGroupMember(person.EntityId(), playerGroup))
            {
                Screen.ShowSubtitle("~b~You: ~w~Stay there!");
                person.Ped().Unfollow(Game.PlayerPed);
                return;
            }

            // Make them follow the player.
            Screen.ShowSubtitle("~b~You: ~w~Come with me!");
            person.Ped().Follow(Game.PlayerPed);
        }
    }
}