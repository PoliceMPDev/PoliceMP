using CitizenFX.Core;
using CitizenFX.Core.UI;
using PoliceMP.Main.Client.Extensions;
using PoliceMP.Main.Client.Managers;
using PoliceMP.Main.Client.Models;
using PoliceMP.Main.Client.Retrievers;
using PoliceMP.Main.Core.Client;
using PoliceMP.Main.Core.Shared.Constants;
using PoliceMP.Main.Shared.Events;
using PoliceMP.Main.Shared.Models;
using System;
using System.Text;
using System.Threading.Tasks;

namespace PoliceMP.Main.Client.Actions.Cars.DriverInteractions
{
    public class TicketPerson : BaseScript
    {
        public static Ticket CurrentTicket;

        public static void ClearCurrentTicket()
        {
            CurrentTicket = null;
        }

        [Command("ticket")]
        private void Command()
        {
            TriggerEvent(ClientEvents.ACTION_TICKET);
        }

        [EventHandler(ClientEvents.ACTION_TICKET)]
        private async void Execute()
        {
            // If a person is selected, then the player is trying to ticket the person
            // because you can't select a person who is in a car. So if they aren't selected,
            // then check if the car is selected.
            if (PersonSelector.SelectedPerson != null)
                await ExecuteTicketPerson();
            else if (CarSelector.SelectedCar != null)
                await ExecuteTicketCar();
        }

        private static async Task ExecuteTicketCar()
        {
            if (!ActionCooldown.Check()) return;

            if (CurrentTicket == null) return;

            if (CurrentTicket.GetFine() == 0 && CurrentTicket.GetPoints() == 0) return;

            var car = CarSelector.SelectedCar;

            if (car == null) return;

            var driver = car.Vehicle().Driver;
            if (driver == null) return;

            var person = await PersonRetriever.GetPerson(driver.Handle);

            if (person == null) return;

            await Start(person);

            PersonSelector.UnselectPerson();
        }

        private static async Task ExecuteTicketPerson()
        {
            if (!ActionCooldown.Check()) return;

            if (CurrentTicket == null) return;

            if (CurrentTicket.GetFine() == 0 && CurrentTicket.GetPoints() == 0) return;

            var person = PersonSelector.SelectedPerson;

            if (person == null) return;

            await Start(person);

            PersonSelector.UnselectPerson();
        }

        private static async Task Start(Person person)
        {
            Screen.ShowSubtitle("~b~You: ~w~I'm going to write you a Fixed Penalty Notice...");

            await Delay(1000);

            Game.PlayerPed.Task.PlayAnimation("veh@busted_low", "issue_ticket_cop");

            await Delay(2000);

            person.Ped().Task.PlayAnimation("anim@mp_player_intcelebrationfemale@face_palm",
                "face_palm");

            Game.PlayerPed.IsPositionFrozen = false;

            var builder = new StringBuilder();

            builder.Append($"<b>Name:</b> {person.FullName}<br>");
            builder.Append($"<b>Date:</b> {DateTime.Now:D}<br>");
            builder.Append($"<b>Points:</b> <span class='text-warning'>{CurrentTicket.GetPoints()}</span><br>");
            builder.Append($"<b>Fine:</b> <span class='text-warning'>£{CurrentTicket.GetFine()}</span><br>");

            builder.Append("<br><b>Offences</b><br><br>");

            foreach (var offence in CurrentTicket.Offences)
                builder.Append($"- {offence}<br>");

            if (CurrentTicket.GetPoints() >= 12)
                builder.Append(
                    "<br><span class='text-danger'>The vehicle should be towed because driver has received 12 points on their license.</span>");

            ClientFunctions.ShowToast("Ticket", builder.ToString(), "info");

            ClearCurrentTicket();
        }
    }
}