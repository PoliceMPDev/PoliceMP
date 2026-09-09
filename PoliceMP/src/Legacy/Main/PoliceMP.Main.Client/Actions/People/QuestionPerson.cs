using CitizenFX.Core;
using CitizenFX.Core.UI;
using PoliceMP.Main.Client.Extensions;
using PoliceMP.Main.Client.Managers;
using PoliceMP.Main.Client.Retrievers;
using PoliceMP.Main.Core.Client;
using PoliceMP.Main.Core.Client.Extensions;
using PoliceMP.Main.Shared.Events;
using PoliceMP.Main.Shared.Models;
using System.Linq;
using System.Threading.Tasks;
using PoliceMP.Main.Core.Shared;

namespace PoliceMP.Main.Client.Actions.People
{
    public class QuestionPerson : BaseScript
    {
        [Command("question")]
        private void Command(string[] args)
        {
            if (args.Length != 1)
            {
                ClientFunctions.SendErrorMessage("Invalid arguments specified.");
                return;
            }

            if (PersonSelector.SelectedPerson == null)
            {
                ClientFunctions.SendErrorMessage("You need to select a person to do this.");
                return;
            }

            TriggerEvent(ClientEvents.ACTION_QUESTION, PersonSelector.SelectedPerson.EntityId(), args[0]);
        }

        [EventHandler(ClientEvents.ACTION_QUESTION)]
        private async void Execute(int pedHandle, string question)
        {
            if (!ActionCooldown.Check()) return;

            var person = await PersonRetriever.GetPerson(pedHandle);
            if (person == null)
            {
                ClientFunctions.SendErrorMessage("Could not retrieve the person.");
                return;
            }

            if (person.Ped().IsInVehicle() && !person.Ped().CurrentVehicle.CanPlayerInteractWithDriver())
            {
                ClientFunctions.SendErrorMessage("Can not interact with the driver of the vehicle.");
                return;
            }

            await Start(person, question);
        }

        private static async Task Start(Person person, string questionText)
        {
            // Get the question being asked
            var question = QuestionRetriever.GetAll().FirstOrDefault(q => q.Text.Equals(questionText));
            if (question == null) return;

            var positiveAnswer = true;

            // Get what answer should be given
            switch (question.Attribute)
            {
                case "Search":
                    positiveAnswer = !person.HasIllegalItems;
                    break;
                case "Drunk":
                    positiveAnswer = person.AlcoholLevel < 0.5f;
                    break;
                case "Drugs":
                    positiveAnswer = !person.OnAnyDrugs;
                    break;
                case "Attitude":
                    positiveAnswer = person.Attitude < 50;
                    break;
            }

            var answer = string.Empty;
            if (positiveAnswer)
            {
                var index = PoliceMpRandom.Next(question.PositiveAnswers.Count - 1);
                answer = question.PositiveAnswers[index];
            }
            else
            {
                var index = PoliceMpRandom.Next(question.NegativeAnswers.Count - 1);
                answer = question.NegativeAnswers[index];
            }

            Game.PlayerPed.Task.PlayAnimation("special_ped@baygor@michael_2@michael_2c",
                "hey_how_you_doing2_2");

            // Display the question
            Screen.ShowSubtitle($"~b~You: ~w~{question.Text}");

            await Delay(2000);

            if (!person.Ped().IsInVehicle())
            {
                person.Ped().Task.TurnTo(Game.PlayerPed);

                await Delay(1000);

                person.Ped().Task.PlayAnimation("special_ped@baygor@michael_2@michael_2c",
                    "hey_how_you_doing2_2");
            }

            // Display the answer
            Screen.ShowSubtitle($"~y~{person.FirstName}: ~w~{answer}");
        }
    }
}