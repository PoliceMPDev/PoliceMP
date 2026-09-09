using CitizenFX.Core;
using CitizenFX.Core.Native;
using MenuAPI;
using PoliceMP.Main.Client.Actions.Cars;
using PoliceMP.Main.Client.Managers;

namespace PoliceMP.Main.Client.Menus
{
    public class CarMenu : BaseScript
    {
        private Menu _controlMenu;
        private Menu _menu;

        private Menu _selectedCarMenu;
        private MenuItem _selectedCarMenuBtn;

        public void OpenMenu()
        {
            if (_menu == null) CreateMenu();
            _menu.OpenMenu();
        }

        public void CloseMenu()
        {
            if (_menu == null) return;

            _menu.CloseMenu();
            _controlMenu.CloseMenu();
        }

        private void CreateMenu()
        {
            MenuController.MenuAlignment = MenuController.MenuAlignmentOption.Right;

            _menu = new Menu("Vehicle Menu", "Main Menu");
            MenuController.AddMenu(_menu);

            CreateSelectedCarSubmenu();

            var anpr = new MenuItem("Toggle ANPR", "Toggle whether Automatic Number Plate Recognition is running.");
            var runplate = new MenuItem("Run Plate", "Enter a number plate to receive details about it.");
            var pingAnpr = new MenuItem("Ping ANPR", "Enter a VRN and scan ANPR Cameras for a hit.");

            var computer = new MenuItem("Access Police Computer", "Login to the Police Computer to view active callouts and access database information.");
            computer.LeftIcon = MenuItem.Icon.STAR;

            _menu.AddMenuItem(anpr);
            _menu.AddMenuItem(runplate);
            _menu.AddMenuItem(pingAnpr);
            _menu.AddMenuItem(computer);

            _menu.OnItemSelect += (menu, item, index) =>
            {
                if (item == anpr) API.ExecuteCommand("anpr");
                else if (item == runplate) API.ExecuteCommand("runplate");
                else if (item == runplate) API.ExecuteCommand("pinganpr");
                else if (item == computer)
                {
                    TriggerEvent("PoliceComputer:Toggle", true);
                    _menu.CloseMenu();
                }
            };

            _menu.OnMenuOpen += menu =>
            {
                if (Game.PlayerPed.CurrentVehicle == null ||
                    Game.PlayerPed.CurrentVehicle.ClassType != VehicleClass.Emergency)
                    return;

                if (CarSelector.SelectedCar == null || !Pullover.Stopped)
                {
                    _selectedCarMenuBtn.Enabled = false;
                    _selectedCarMenuBtn.LeftIcon = MenuItem.Icon.LOCK;
                }
                else
                {
                    _selectedCarMenuBtn.Enabled = true;
                    _selectedCarMenuBtn.LeftIcon = MenuItem.Icon.TICK;
                }
            };
        }

        private void CreateSelectedCarSubmenu()
        {
            _selectedCarMenu = new Menu("Vehicle Menu", "Stopped Vehicle Interaction");
            MenuController.AddSubmenu(_menu, _selectedCarMenu);

            _selectedCarMenuBtn =
                new MenuItem("Stopped Vehicle Interaction", "Open the stopped vehicle interaction sub-menu.")
                { Label = "→→→" };
            _menu.AddMenuItem(_selectedCarMenuBtn);
            MenuController.BindMenuItem(_menu, _selectedCarMenu, _selectedCarMenuBtn);

            var release = new MenuItem("~b~Release", "Release the vehicle.");

            CreateControlSubmenu();

            _selectedCarMenu.AddMenuItem(release);

            _selectedCarMenu.OnItemSelect += (menu, item, index) =>
            {
                if (item == release) API.ExecuteCommand("releasecar");
            };
        }

        private void CreateControlSubmenu()
        {
            _controlMenu = new Menu("Vehicle Menu", "Stopped Vehicle Commands");
            MenuController.AddSubmenu(_menu, _controlMenu);

            var controlMenuBtn = new MenuItem("Controls", "Open the controls sub-menu.") { Label = "→→→" };
            _selectedCarMenu.AddMenuItem(controlMenuBtn);
            MenuController.BindMenuItem(_selectedCarMenu, _controlMenu, controlMenuBtn);

            var mimic = new MenuItem("Mimic",
                "Make the vehicle mimic you so you can put them in a better stopping location.");

            _controlMenu.AddMenuItem(mimic);

            _controlMenu.OnItemSelect += (menu, item, index) =>
            {
                if (item == mimic) API.ExecuteCommand("mimic");
            };
        }
    }
}