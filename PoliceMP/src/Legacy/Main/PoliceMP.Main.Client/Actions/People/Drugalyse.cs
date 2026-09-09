using CitizenFX.Core;
using CitizenFX.Core.Native;
using CitizenFX.Core.UI;
using PoliceMP.Main.Client.Extensions;
using PoliceMP.Main.Client.Managers;
using PoliceMP.Main.Client.Retrievers;
using PoliceMP.Main.Client.Util;
using PoliceMP.Main.Core.Client;
using PoliceMP.Main.Shared.Events;
using PoliceMP.Main.Shared.Models;
using System.Text;
using System.Threading.Tasks;
using PoliceMP.Main.Core.Client.Extensions;

namespace PoliceMP.Main.Client.Actions.People
{
    /// <summary>
    ///     Used to drugalyse people.
    /// </summary>
    public class Drugalyse : BaseScript
    {
        /// <summary>
        ///     The command used to execute the action.
        /// </summary>
        [Command(Commands.Drugalyse)]
        private void Command()
        {
            if (PersonSelector.SelectedPerson == null)
            {
                ClientFunctions.SendErrorMessage("You need to select a person to do this.");
                return;
            }

            TriggerEvent(ClientEvents.ACTION_DRUGALYSE, PersonSelector.SelectedPerson.EntityId());
        }

        /// <summary>
        ///     Handles the DRUGALYSE_PERSON event.
        /// </summary>
        /// <param name="entityId"></param>
        [EventHandler(ClientEvents.ACTION_DRUGALYSE)]
        private async void Execute(int pedHandle)
        {
            if (!ActionCooldown.Check()) return;

            // Try and retrieve the person
            var person = await PersonRetriever.GetPerson(pedHandle);
            if (person == null)
            {
                ClientFunctions.SendErrorMessage("Could not retrieve the person.");
                return;
            }

            // Make sure they are close enough
            if (!Game.PlayerPed.IsNearEntity(person.Ped(), new Vector3(2f, 2f, 2f)))
            {
                ClientFunctions.SendErrorMessage("You are not close enough to the person.");
                return;
            }

            // Make sure the person isn't in a vehicle
            if (person.Ped().IsInVehicle())
            {
                ClientFunctions.SendErrorMessage("Cannot drugalyse someone who is in a vehicle.");
                return;
            }

            await DrugalysePerson(person);
        }

        /// <summary>
        ///     Makes the player drugalyse the person.
        /// </summary>
        /// <param name="person">The person.</param>
        /// <returns></returns>
        private static async Task DrugalysePerson(Person person)
        {
            person.Ped().Task.TurnTo(Game.PlayerPed);

            API.FreezeEntityPosition(Game.PlayerPed.Handle, true);
            API.FreezeEntityPosition(person.EntityId(), true);

            Screen.ShowSubtitle("~b~You: ~w~I'm going to give you a quick drugalyser test...");
            
            SoundPlayer.PlaySound(SoundPlayer.Inhaler, 5);
            await Game.PlayerPed.PlayBreathalyserAnimAsync();


            var builder = new StringBuilder();
            builder.Append(person.OnCocaine 
                ? "<b>Cocaine:</b> <span class='text-danger>POSITIVE</span><br>" 
                : "<b>Cocaine:</b> <span class='text-success'>NEGATIVE</span><br>");
            builder.Append(person.SmokedCannabis 
                ? "<b>Cannabis:</b> <span class='text-danger>POSITIVE</span><br>"
                : "<b>Cannabis:</b> <span class='text-success'>NEGATIVE</span><br>");

            ClientFunctions.ShowToast("Drugalyser", builder.ToString(), "info");

            API.ClearPedTasks(Game.PlayerPed.Handle);

            API.FreezeEntityPosition(Game.PlayerPed.Handle, false);
            API.FreezeEntityPosition(person.EntityId(), false);
        }
    }
}