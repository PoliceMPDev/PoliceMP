using CitizenFX.Core;
using CitizenFX.Core.Native;
using CitizenFX.Core.UI;
using PoliceMP.Main.Client.Actions.Cars;
using PoliceMP.Main.Client.Actions.Cars.DriverInteractions;
using PoliceMP.Main.Client.Extensions;
using PoliceMP.Main.Client.Menus;
using PoliceMP.Main.Client.Retrievers;
using PoliceMP.Main.Core.Client.Extensions;
using PoliceMP.Main.Shared.Models;
using System.Threading.Tasks;

namespace PoliceMP.Main.Client.Managers
{
    /// <summary>
    ///     Handles selecting cars.
    /// </summary>
    public class CarSelector : BaseScript
    {
        /// <summary>
        ///     The max distance the player can be from the selected car
        ///     before it gets unselected.
        /// </summary>
        public const float MAX_DISTANCE_FROM_SELECTED_CAR = 100f;

        /// <summary>
        ///     The selected car blip ID.
        /// </summary>
        private static int _blip = -1;

        /// <summary>
        ///     The vehicle in front of the player.
        /// </summary>
        private static Vehicle _vehicleInFront;

        /// <summary>
        ///     The selected car.
        /// </summary>
        public static Car SelectedCar { get; private set; }

        /// <summary>
        ///     Selects the given car.
        /// </summary>
        /// <param name="car">The car.</param>
        //public static void SelectCar(Car car)
        //{
        //    SelectedCar = car;

        //    API.NetworkRequestControlOfEntity(car.EntityId());

        //    _blip = API.AddBlipForEntity(SelectedCar.EntityId());
        //    RunPlate.Execute(SelectedCar.Plate);

        //    TicketPerson.ClearCurrentTicket();

        //    SelectedCar.Vehicle().IsPersistent = true;
        //}

        /// <summary>
        ///     Unselects the currently selected car.
        /// </summary>
        public static void UnselectCar()
        {
            //if (SelectedCar.Vehicle() != null) SelectedCar.Vehicle().IsPersistent = false;

            //SelectedCar = null;
            //var blip = _blip;
            //API.RemoveBlip(ref blip);
            //_blip = -1;

            //MenuManager.CarMenu.CloseMenu();
        }

        /// <summary>
        ///     Checks for player input.
        /// </summary>
        //[Tick]
        //private async Task CheckControls()
        //{
        //    if (Game.PlayerPed.CurrentVehicle == null ||
        //        Game.PlayerPed.CurrentVehicle.ClassType != VehicleClass.Emergency)
        //    {
        //        MenuManager.CarMenu.CloseMenu();
        //        return;
        //    }

        //    // Check if the player is trying to open the menu.
        //    if (API.IsControlPressed(0, 244)) // M
        //        MenuManager.CarMenu.OpenMenu();

        //    // Check if the player is trying to select a vehicle
        //    if (API.IsControlJustPressed(0, 21)) // Left shift
        //    {
        //        if (SelectedCar != null)
        //        {
        //            UnselectCar();
        //            return;
        //        }

        //        if (_vehicleInFront != null)
        //        {
        //            var car = await CarRetriever.GetCar(_vehicleInFront.Handle);
        //            if (car == null) return;

        //            SelectCar(car);
        //        }
        //    }
        //}

        ///// <summary>
        /////     Checks if the selected car is still valid.
        ///// </summary>
        //[Tick]
        //private Task CheckCarStillValid()
        //{
        //    if (SelectedCar == null) return Task.FromResult(0);

        //    if (!API.DoesEntityExist(SelectedCar.EntityId()) ||
        //        !SelectedCar.Vehicle().IsDriveable ||
        //        !Game.PlayerPed.IsCloseEnoughToEntity(SelectedCar.Vehicle(), MAX_DISTANCE_FROM_SELECTED_CAR))
        //        UnselectCar();

        //    return Task.FromResult(0);
        //}

        ///// <summary>
        /////     Updates the screen with help messages where appropriate.
        ///// </summary>
        //[Tick]
        //private Task UpdateScreen()
        //{
        //    if (SelectedCar != null) return Task.FromResult(0);

        //    var playerVehicle = Game.PlayerPed.CurrentVehicle;
        //    if (playerVehicle != null && playerVehicle.Speed < 2f && playerVehicle.ClassType == VehicleClass.Emergency)
        //        Screen.DisplayHelpTextThisFrame("Press ~INPUT_INTERACTION_MENU~ ~w~to open the vehicle menu.");

        //    if (Game.PlayerPed.CurrentVehicle == null ||
        //        Game.PlayerPed.CurrentVehicle.ClassType != VehicleClass.Emergency) return Task.FromResult(0);

        //    var vehicle = Game.PlayerPed.GetVehicleInFront();
        //    if (vehicle == null)
        //    {
        //        _vehicleInFront = null;
        //        return Task.FromResult(0);
        //    }

        //    if (_vehicleInFront == null || vehicle.Handle != _vehicleInFront.Handle)
        //    {
        //        if (API.GetPedInVehicleSeat(vehicle.Handle, -1) < 1) return Task.FromResult(0);

        //        if (vehicle.ClassType == VehicleClass.Emergency)
        //        {
        //            _vehicleInFront = null;
        //            return Task.FromResult(0);
        //        }

        //        _vehicleInFront = vehicle;
        //    }

        //    Screen.DisplayHelpTextThisFrame("Press ~INPUT_SPRINT~ ~w~to select the vehicle in front.");

        //    return Task.FromResult(0);
        //}
    }
}