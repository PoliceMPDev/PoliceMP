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
using System.Threading.Tasks;

namespace PoliceMP.Main.Client.Actions.People
{
    /// <summary>
    ///     Handles the player action to ask a ped for ID.
    /// </summary>
    public class AskForId : BaseScript
    {
        /// <summary>
        ///     The command used to execute the action.
        /// </summary>
        [Command(Commands.AskForId)]
        private void Command()
        {
            var pedHandle = -1;

            if (PersonSelector.SelectedPerson != null)
                pedHandle = PersonSelector.SelectedPerson.EntityId();
            else if (CarSelector.SelectedCar != null)
                pedHandle = CarSelector.SelectedCar.Vehicle().Driver.Handle;

            if (pedHandle == -1)
            {
                ClientFunctions.SendErrorMessage("Could not find the person to ask for ID.");
                return;
            }

            TriggerEvent(ClientEvents.ACTION_ASK_FOR_ID, pedHandle);
        }

        /// <summary>
        ///     Executes the action.
        /// </summary>
        /// <param name="ped">The ped to be asked for ID.</param>
        [EventHandler(ClientEvents.ACTION_ASK_FOR_ID)]
        private async void Execute(int pedHandle)
        {
            if (!ActionCooldown.Check()) return;

            var person = await PersonRetriever.GetPerson(pedHandle);
            if (person == null)
            {
                ClientFunctions.SendErrorMessage("Could not retrieve the person.");
                return;
            }

            await Start(person);
        }

        /// <summary>
        ///     Starts the action.
        /// </summary>
        /// <param name="person">The person.</param>
        private static async Task Start(Person person)
        {
            Game.PlayerPed.Task.TurnTo(person.Ped());

            Screen.ShowSubtitle("~b~You: ~w~Have you got any ID on you?");

            Game.PlayerPed.Task.PlayAnimation("special_ped@baygor@michael_2@michael_2c",
                "hey_how_you_doing2_2");

            if (!person.Ped().IsInVehicle())
            {
                await Delay(1000);
                person.Ped().Task.TurnTo(Game.PlayerPed);
            }

            await Delay(1000);

            // Using the API way because the anim didn't play how we wanted with fivem oop for some reason
            await person.Ped().PlayAnimAsync("misslester1b", "michael_phone_detonate_press");

            await Delay(1000);

            ClientFunctions.ShowToast(person.FullName + "'s Identification",
                $"<b>Forename:</b> {person.FirstName}<br>" +
                $"<b>Surname:</b> {person.LastName}<br>" +
                $"<b>Date of Birth:</b> {person.DateOfBirth:dd MMMM yyyy}<br>" +
                $"<b>Address:</b> {person.Street}<br>" +
                "<b>Nationality:</b> British<br>" +
                (person.HasDrivingLicense
                    ? "<b>Driving License:</b> <span class='text-success'>Valid</span>"
                    : "<b>Driving License:</b> <span class='text-warning'>None</span>"), "info");
        }
    }
}