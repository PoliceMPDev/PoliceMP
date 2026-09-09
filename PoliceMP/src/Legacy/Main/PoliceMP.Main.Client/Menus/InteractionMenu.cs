using CitizenFX.Core;
using CitizenFX.Core.Native;
using MenuAPI;
using PoliceMP.Main.Client.Managers;
using PoliceMP.Main.Shared.Events;

namespace PoliceMP.Main.Client.Menus
{
    public class InteractionMenu : BaseScript
    {
        private Menu _arrestsMenu;
        private Menu _interactionsMenu;
        private Menu _menu;

        public void OpenMenu()
        {
            if (_menu == null) CreateMenu();

            _menu.OpenMenu();
        }

        public void CloseMenu()
        {
            _menu?.CloseMenu();
            _arrestsMenu?.CloseMenu();
            _interactionsMenu?.CloseMenu();
        }

        private void CreateMenu()
        {
            MenuController.MenuAlignment = MenuController.MenuAlignmentOption.Right;

            // Create the parent menu
            _menu = new Menu("Person Interaction", "Main Menu");
            MenuController.AddMenu(_menu);

            CreateArrestsSubmenu();
            CreateInteractionsSubmenu();

            var warning = new MenuItem("Issue Warning", "Issue a warning to the person.");
            var release = new MenuItem("~b~Release", "Release the person.");

            TicketMenu.AddToMenu(_menu);
            _menu.AddMenuItem(warning);
            QuestionMenu.AddToMenu(_menu);
            _menu.AddMenuItem(release);

            _menu.OnItemSelect += (menu, item, index) =>
            {
                if (item == release) API.ExecuteCommand("release");
                else if (item == warning) API.ExecuteCommand("warning");
            };

            _menu.OnMenuOpen += menu =>
            {
                if (PersonSelector.SelectedPerson == null) _menu.CloseMenu();
            };

            _menu.OnMenuClose += menu => { TriggerEvent(ClientEvents.ON_INTERACTION_MENU_CLOSED); };
        }

        private void CreateInteractionsSubmenu()
        {
            _interactionsMenu = new Menu("Person Interaction", "Interactions");
            MenuController.AddSubmenu(_menu, _interactionsMenu);

            var interactionsMenuBtn = new MenuItem("Interactions", "Open the interactions sub-menu.") { Label = "→→→" };
            _menu.AddMenuItem(interactionsMenuBtn);
            MenuController.BindMenuItem(_menu, _interactionsMenu, interactionsMenuBtn);

            var id = new MenuItem("Ask for ID", "Ask the person to provide some form of identification.");
            var breathalyse = new MenuItem("Breathalyse",
                "Breathalyse the person to see whether they are below the drink drive limit.");
            var drugalyse = new MenuItem("Drugalyse",
                "Drugalyse the person to see whether they have consumed any illegal substances.");
            var search = new MenuItem("Search", "Pat the person down to find out what they are carrying.");


            _interactionsMenu.AddMenuItem(id);
            _interactionsMenu.AddMenuItem(breathalyse);
            _interactionsMenu.AddMenuItem(drugalyse);
            _interactionsMenu.AddMenuItem(search);

            _interactionsMenu.OnItemSelect += (menu, item, index) =>
            {
                if (item == id) API.ExecuteCommand("askforid");
                else if (item == breathalyse) API.ExecuteCommand("breathalyse");
                else if (item == drugalyse) API.ExecuteCommand("drugalyse");
                else if (item == search) API.ExecuteCommand("search");
            };
        }

        private void CreateArrestsSubmenu()
        {
            _arrestsMenu = new Menu("Person Interaction", "Arrests");
            MenuController.AddSubmenu(_menu, _arrestsMenu);

            var arrestsMenuBtn = new MenuItem("Arrests", "Open the arrests sub-menu.") { Label = "→→→" };
            _menu.AddMenuItem(arrestsMenuBtn);
            MenuController.BindMenuItem(_menu, _arrestsMenu, arrestsMenuBtn);

            var handcuff = new MenuItem("Handcuff", "Handcuff the person.");
            var book = new MenuItem("Book", "Book the person in jail.");
            var grab = new MenuItem("Grab", "Grab the person.");
            var follow = new MenuItem("Follow", "Command the person to follow you.");
            var kneel = new MenuItem("Kneel", "Command the person to kneel.");
            var handsup = new MenuItem("Hands Up", "Command the person to put their hands up.");
            var liedown = new MenuItem("Lie Down", "Command the person to lie on the ground.");
            var stand = new MenuItem("Stand Up", "Command the person to stand up.");
            var takePedOutside = new MenuItem("Take Outside", "Move the person out the house");


            _arrestsMenu.AddMenuItem(handcuff);
            _arrestsMenu.AddMenuItem(book);
            _arrestsMenu.AddMenuItem(grab);
            _arrestsMenu.AddMenuItem(follow);
            _arrestsMenu.AddMenuItem(kneel);
            _arrestsMenu.AddMenuItem(handsup);
            _arrestsMenu.AddMenuItem(liedown);
            _arrestsMenu.AddMenuItem(stand);
            _arrestsMenu.AddMenuItem(takePedOutside);

            _arrestsMenu.OnItemSelect += (menu, item, index) =>
            {
                if (item == handcuff) API.ExecuteCommand("cuff");
                else if (item == grab) API.ExecuteCommand("grab");
                else if (item == follow) API.ExecuteCommand("follow");
                else if (item == kneel) API.ExecuteCommand("kneel");
                else if (item == handsup) API.ExecuteCommand("handsup");
                else if (item == liedown) API.ExecuteCommand("liedown");
                else if (item == stand) API.ExecuteCommand("stand");
                else if (item == book) API.ExecuteCommand("book");
                else if (item == takePedOutside) API.ExecuteCommand("movepedoutside");
            };
        }
    }
}