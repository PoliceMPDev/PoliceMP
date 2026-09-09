using CitizenFX.Core;
using CitizenFX.Core.Native;
using CitizenFX.Core.UI;
using PoliceMP.Main.Client.Extensions;
using PoliceMP.Main.Client.Managers;
using PoliceMP.Main.Client.Menus;
using PoliceMP.Main.Core.Client.Extensions;
using PoliceMP.Main.Shared.Events;
using System.Threading.Tasks;

namespace PoliceMP.Main.Client.Actions.Cars.DriverInteractions
{
    public class DriverInteractions : BaseScript
    {
        [Tick]
        private async Task OnTick()
        {
            var selectedCar = CarSelector.SelectedCar;

            if (selectedCar == null || Game.PlayerPed.CurrentVehicle != null || selectedCar.Vehicle() == null) return;

            // If its a bike or truck we cant check if they are at the drviers door
            if (selectedCar.Vehicle().CanPlayerInteractWithDriver())
            {
                Screen.DisplayHelpTextThisFrame("Press ~INPUT_PICKUP~ ~w~to talk to the driver.");
                if (API.IsControlJustPressed(0, 38))
                {
                    MenuManager.DriverInteractionMenu.OpenMenu();
                    TriggerEvent(ClientEvents.ON_INTERACT_WITH_DRIVER);
                    await Delay(1000);
                }
            }
            else
            {
                MenuManager.DriverInteractionMenu.CloseMenu();
            }
        }
    }
}