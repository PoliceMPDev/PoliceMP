using CitizenFX.Core;
using CitizenFX.Core.Native;
using MenuAPI;
using PoliceMP.Client.Actions.RunName;
using PoliceMP.Client.Actions.RunPlate;
using PoliceMP.Core.Client;
using PoliceMP.Core.Client.Actions.Interfaces;
using PoliceMP.Core.Client.Extensions;
using PoliceMP.Core.Client.Interface;
using System.Threading.Tasks;
using PoliceMP.Core.Client.Commands.Interfaces;
using PoliceMP.Core.Client.Scripts;

namespace PoliceMP.Client.Scripts.RadioMenu
{
    public class RadioMenuScript : Script
    {
        private readonly ITickManager _ticks;
        private readonly IActionManager _actions;
        private Menu _radioMenu;

        public RadioMenuScript(ITickManager ticks, IActionManager actions, ICommandManager commandManager)
        {
            _ticks = ticks;
            _actions = actions;
            commandManager.Register("radiomenu").WithHandler(() =>
            {
                if (_radioMenu.Visible)
                {
                    _radioMenu.CloseMenu();
                    return;
                }

                if (MenuController.IsAnyMenuOpen()) return;
                _radioMenu.OpenMenu();
            });
            
            API.RegisterKeyMapping("radiomenu", "The Police Radio menu", "keyboard", "g");
        }

        protected override Task OnStartAsync()
        {
            CreateMenu();
            return Task.FromResult(0);
        }

        private void CreateMenu()
        {
            _radioMenu = new Menu("Police Radio", "Main Menu");
            MenuController.AddMenu(_radioMenu);

            var runPlate = new MenuItem("Run Plate", "Run a number plate to get more information about a vehicle.");
            var runName = new MenuItem("Run Name", "Run a name to get more information about a person.");
            var callTowTruck = new MenuItem("Call Tow Truck",
                "Call for a tow truck to pick up the vehicle that you are facing.");
            var callPrisonerTransport = new MenuItem("Call Prisoner Transport",
                "Call for a vehicle to pickup the prisoner that you are facing.");
            var callCoroner = new MenuItem("Call Coroner",
                "Call for a coroner to collect dead people.");
            var callEms = new MenuItem("Call LHS",
                "Call for EMS for people needing medical assistance.");
            var callTaxi = new MenuItem("Call Taxi", "Call for a taxi to pick somebody up. Useful for if they are stranded without a vehicle.");
            var callFire = new MenuItem("Call Fire Brigade", "Call for the fire brigade to put out any fires.");

            _radioMenu.AddMenuItem(runPlate);
            _radioMenu.AddMenuItem(runName);
            _radioMenu.AddMenuItem(callTowTruck);
            _radioMenu.AddMenuItem(callPrisonerTransport);
            //_radioMenu.AddMenuItem(callCoroner);
            //_radioMenu.AddMenuItem(callEms);
            _radioMenu.AddMenuItem(callTaxi);
            //_radioMenu.AddMenuItem(callFire);

            _radioMenu.OnItemSelect += async (menu, item, index) =>
            {
                if (item == runPlate) await _actions.Execute(new RunPlate());
                else if (item == runName) await _actions.Execute(new RunName());
                else if (item == callTowTruck) API.ExecuteCommand("towtruck");
                else if (item == callPrisonerTransport) CallPoliceTransport();
                else if (item == callEms) API.ExecuteCommand("ems");
                else if (item == callTaxi) CallTaxi();
                else if (item == callFire) API.ExecuteCommand("fire");
            };
        }

        private void CallPoliceTransport()
        {
            var ped = Game.PlayerPed.GetPedInFront();
            if (ped != null)
            {
                BaseScript.TriggerEvent("PoliceMPSummon:Transport:AssignTransport", ped.Handle);
            }
        }

        private void CallTaxi()
        {
            var ped = Game.PlayerPed.GetPedInFront();
            if (ped != null)
            {
                BaseScript.TriggerEvent("PoliceMPSummon:Taxi:AssignTaxi", ped.Handle);
            }
        }
    }
}