using CitizenFX.Core;
using MenuAPI;
using PoliceMP.Main.Client.Extensions;
using PoliceMP.Main.Client.Managers;
using PoliceMP.Main.Client.Retrievers;
using PoliceMP.Main.Core.Client;
using PoliceMP.Main.Shared.Events;

namespace PoliceMP.Main.Client.Menus
{
    public class QuestionMenu : BaseScript
    {
        public static void AddToMenu(Menu parentMenu)
        {
            CreateQuestionSubmenu(parentMenu);
        }

        private static void CreateQuestionSubmenu(Menu parentMenu)
        {
            Debug.WriteLine("FUNNYHAT WAS HERE");
            
            var questionMenu = new Menu("Person Interaction", "Questions");
            MenuController.AddSubmenu(parentMenu, questionMenu);

            var questionMenuBtn =
                new MenuItem("Questions", "Ask the person questions.");
            parentMenu.AddMenuItem(questionMenuBtn);
            MenuController.BindMenuItem(parentMenu, questionMenu, questionMenuBtn);
            
            Debug.WriteLine("Ped Selected Is: " + PersonSelector.SelectedPerson?.EntityId());
            
            var questions = QuestionRetriever.GetAll();
            foreach (var question in questions)
            {
                var item = new MenuItem(question.Text);
                questionMenu.AddMenuItem(item);
            }

            questionMenu.OnItemSelect += (menu, item, index) =>
            {
                Debug.WriteLine("WOODY WAS HERE");
                
                var pedHandle = -1;

                if (PersonSelector.SelectedPerson != null)
                {
                    pedHandle = PersonSelector.SelectedPerson.EntityId();
                }
                else if (CarSelector.SelectedCar != null)
                {
                    pedHandle = CarSelector.SelectedCar.Vehicle().Driver.Handle;
                }

                if (pedHandle == -1)
                {
                    ClientFunctions.SendErrorMessage("Could not find the ped to question.");
                    return;
                }

                TriggerEvent(ClientEvents.ACTION_QUESTION, pedHandle, item.Text);
            };
        }
    }
}