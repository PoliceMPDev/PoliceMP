using CitizenFX.Core;
using CitizenFX.Core.UI;
using PoliceMP.Main.Client.Extensions;
using PoliceMP.Main.Client.Managers;
using PoliceMP.Main.Client.Menus;
using PoliceMP.Main.Core.Client.Extensions;
using PoliceMP.Main.Shared.Events;

namespace PoliceMP.Main.Client.Actions.Cars.DriverInteractions
{
    public class ReleaseCar : BaseScript
    {
        [Command("releasecar")]
        private void Command()
        {
            TriggerEvent(ClientEvents.ACTION_RELEASE_CAR);
        }

        [EventHandler(ClientEvents.ACTION_RELEASE_CAR)]
        private void Execute()
        {
            if (CarSelector.SelectedCar == null) return;

            if (!Game.PlayerPed.IsAtVehicleDriverDoor(CarSelector.SelectedCar.Vehicle()))
            {
                Screen.ShowSubtitle("~r~You must be at the driver's door to do this.");
                return;
            }

            Screen.ShowSubtitle("~b~You: ~w~Alright, you're free to go.");

            CarSelector.SelectedCar.Vehicle().AreLightsOn = true;
            CarSelector.UnselectCar();

            MenuManager.DriverInteractionMenu.CloseMenu();
        }
    }
}