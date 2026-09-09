using CitizenFX.Core;
using CitizenFX.Core.Native;
using MenuAPI;
using PoliceMP.Main.Client.Extensions;
using PoliceMP.Main.Client.Managers;
using PoliceMP.Main.Core.Client.Extensions;

namespace PoliceMP.Main.Client.Menus
{
    public class DriverInteractionMenu : BaseScript
    {
        private Menu _menu;

        public void OpenMenu()
        {
            if (_menu == null) CreateMenu();

            _menu.OpenMenu();
        }

        public void CloseMenu()
        {
            _menu?.CloseMenu();
        }

        private void CreateMenu()
        {
            MenuController.MenuAlignment = MenuController.MenuAlignmentOption.Right;

            _menu = new Menu("Driver Interaction", "Main Menu");
            MenuController.AddMenu(_menu);

            var warning = new MenuItem("Issue Warning", "Issue a warning to the person.");
            var id = new MenuItem("Ask for ID", "Ask the person to provide some form of identification.");
            var stepOut = new MenuItem("Ask Driver to Step Out", "Ask the driver to get out of the vehicle.");
            var release = new MenuItem("~b~Release", "Release the vehicle.");

            TicketMenu.AddToMenu(_menu);
            _menu.AddMenuItem(warning);
            QuestionMenu.AddToMenu(_menu);
            _menu.AddMenuItem(id);
            _menu.AddMenuItem(stepOut);
            _menu.AddMenuItem(release);

            _menu.OnItemSelect += (menu, item, index) =>
            {
                if (item == id) API.ExecuteCommand("askforid");
                else if (item == stepOut) API.ExecuteCommand("stepout");
                else if (item == release) API.ExecuteCommand("releasecar");
                else if (item == warning) API.ExecuteCommand("warning");
            };

            _menu.OnMenuOpen += menu =>
            {
                if (CarSelector.SelectedCar == null || !CarSelector.SelectedCar.Vehicle().CanPlayerInteractWithDriver())
                    _menu.CloseMenu();
            };
        }
    }
}