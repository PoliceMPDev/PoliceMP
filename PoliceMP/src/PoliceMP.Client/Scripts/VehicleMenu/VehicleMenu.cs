using System.Threading.Tasks;
using CitizenFX.Core;
using CitizenFX.Core.Native;
using MenuAPI;
using PoliceMP.Client.Scripts.Pullover;
using PoliceMP.Client.Services.Interfaces;
using PoliceMP.Core.Client;
using PoliceMP.Core.Client.Commands.Interfaces;
using PoliceMP.Core.Client.Interface;
using PoliceMP.Core.Client.Scripts;
using PoliceMP.Shared.Enums;

namespace PoliceMP.Client.Scripts.VehicleMenu
{
    public class VehicleMenu : Script
    {
        private readonly ITickManager _tickManager;
        private readonly IPermissionService _permission;
        
        private Menu _menu;

        public VehicleMenu(ITickManager tickManager, ICommandManager commandManager, IPermissionService permission)
        {
            _tickManager = tickManager;
            _permission = permission;
            CreateMenu();
            commandManager.Register("vehiclemenu").WithHandler(() =>
            {
                if (_menu.Visible)
                {
                    _menu.CloseMenu();
                    return;
                }
                var currentVehicle = Game.PlayerPed.CurrentVehicle;
                var currentUser = _permission.CurrentUserRole;

                if (currentVehicle == null || currentVehicle.ClassType != VehicleClass.Emergency || currentUser.Branch != UserBranch.Police) return;
                
                _menu.OpenMenu();
            });
            
            API.RegisterKeyMapping("vehiclemenu", "Vehicle Menu", "keyboard", "m");
        }

        protected override Task OnStartAsync()
        {
            return Task.FromResult(0);
        }

        private void CreateMenu()
        {
            MenuController.MenuAlignment = MenuController.MenuAlignmentOption.Right;

            _menu = new Menu("Vehicle Menu", "Main Menu");
            MenuController.AddMenu(_menu);
            var anprItem = new MenuItem("Toggle ANPR",
                "Toggle the state of the Automatic Number Plate Recognition system");
            var runPlateItem = new MenuItem("Run Plate", "Runs an entered Vehicle Number Plate");
            var anprPingItem = new MenuItem("Ping reg on ANPR", "Searches ANPR System for last seen vehicle location");
            /*var computerItem = new MenuItem("Access PNC", "Access the Police National Computer")
            {
                LeftIcon = MenuItem.Icon.STAR
            };*/

            _menu.AddMenuItem(anprItem);
            _menu.AddMenuItem(runPlateItem);
            _menu.AddMenuItem(anprPingItem);

            //_menu.AddMenuItem(computerItem);

            _menu.OnItemSelect += (menu, item, index) =>
            {
                if(item == anprItem)
                {
                    API.ExecuteCommand("anpr");
                }

                if (item == runPlateItem)
                {
                    API.ExecuteCommand("runplate");
                }

                if (item == anprPingItem)
                {
                    API.ExecuteCommand("pinganpr");
                }

                /*if (item == computerItem)
                {
                    BaseScript.TriggerEvent("PoliceComputer:Toggle", true);
                }*/

                _menu.CloseMenu();
            };
        }
    }
}