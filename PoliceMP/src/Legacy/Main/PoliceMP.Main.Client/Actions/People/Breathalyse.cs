using CitizenFX.Core;
using CitizenFX.Core.UI;
using PoliceMP.Main.Client.Extensions;
using PoliceMP.Main.Client.Managers;
using PoliceMP.Main.Client.Retrievers;
using PoliceMP.Main.Client.Util;
using PoliceMP.Main.Core.Client;
using PoliceMP.Main.Core.Client.Extensions;
using PoliceMP.Main.Shared.Events;
using PoliceMP.Main.Shared.Models;
using System.Text;
using System.Threading.Tasks;

namespace PoliceMP.Main.Client.Actions.People
{
    /// <summary>
    ///     Used to breathalyse people.
    /// </summary>
    public class Breathalyse : BaseScript
    {
        /// <summary>
        ///     The blood alcohol limit.
        /// </summary>
        public const float BLOOD_ALCOHOL_LIMIT = 0.08f;

        /// <summary>
        ///     The command used to execute the action.
        /// </summary>
        [Command(Commands.Breathalyse)]
        private void Command()
        {
            if (PersonSelector.SelectedPerson == null)
            {
                ClientFunctions.SendErrorMessage("You need to select a person to do this.");
                return;
            }

            TriggerEvent(ClientEvents.ACTION_BREATHALYSE, PersonSelector.SelectedPerson.EntityId());
        }

        /// <summary>
        ///     Handles the BREATHALYSE_PERSON event.
        /// </summary>
        /// <param name="entityId">The ped entity ID.</param>
        [EventHandler(ClientEvents.ACTION_BREATHALYSE)]
        private async void Execute(int pedHandle)
        {
            if (!ActionCooldown.Check()) return;

            // Try and retrieve the person.
            var person = await PersonRetriever.GetPerson(pedHandle);
            if (person == null)
            {
                ClientFunctions.SendErrorMessage("Could not retrieve the person.");
                return;
            }

            // Make sure the player is close enough.
            if (!Game.PlayerPed.IsNearEntity(person.Ped(), new Vector3(2f, 2f, 2f)))
            {
                ClientFunctions.SendErrorMessage("You are not close enough to the person.");
                return;
            }

            // Make sure the person isn't in a vehicle.
            if (person.Ped().IsInVehicle())
            {
                ClientFunctions.SendErrorMessage("You cannot breathalyse someone who is in a vehicle.");
                return;
            }

            await BreathalysePerson(person);
        }

        /// <summary>
        ///     Makes the player breathalyse the person.
        /// </summary>
        /// <param name="person">The person</param>
        private static async Task BreathalysePerson(Person person)
        {
            // Make sure they are facing each other and then freeze position
            person.Ped().Task.TurnTo(Game.PlayerPed);
            Game.PlayerPed.IsPositionFrozen = true;
            person.Ped().IsPositionFrozen = true;

            Screen.ShowSubtitle("<span class='text-primary'>You: <b>I'm going to give you a quick breathalyser test...");
            
            // Make the player play the breathalyse anim
            SoundPlayer.PlaySound(SoundPlayer.Inhaler, 5);
            await Game.PlayerPed.PlayBreathalyserAnimAsync();

            // Create the string for the breathalyser result.
            var builder = new StringBuilder();
            builder.Append($"<b>Legal Limit:</b> {BLOOD_ALCOHOL_LIMIT}<br>");

            if (person.AlcoholLevel > BLOOD_ALCOHOL_LIMIT)
            {
                builder.Append($"<b>Reading:</b> <span class='text-danger'>{person.AlcoholLevel}</span><br>");
                builder.Append("<b>Result:</b> <span class='text-danger'>FAIL</span>");
            }
            else
            {
                builder.Append($"<b>Reading:</b> <span class='text-success'>{person.AlcoholLevel}</span><br>");
                builder.Append("<b>Result:</b> <span class='text-success'>PASS</span>");
            }
            
            ClientFunctions.ShowToast("Breathalyser", builder.ToString(), "info");

            // Make sure the player has stopped the animation and unfreeze them.
            Game.PlayerPed.Task.ClearAll();
            Game.PlayerPed.IsPositionFrozen = false;
            person.Ped().IsPositionFrozen = false;
        }
    }
}