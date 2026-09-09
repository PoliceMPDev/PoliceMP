using CitizenFX.Core;
using CitizenFX.Core.Native;
using CitizenFX.Core.UI;
using PoliceMP.Main.Client.Extensions;
using PoliceMP.Main.Client.Managers;
using PoliceMP.Main.Core.Client;
using PoliceMP.Main.Core.Client.Extensions;
using PoliceMP.Main.Shared.Events;

namespace PoliceMP.Main.Client.Actions.Cars.DriverInteractions
{
    public class StepOut : BaseScript
    {
        [Command("stepout")]
        private void Command()
        {
            TriggerEvent(ClientEvents.ACTION_STEP_OUT);
        }

        [EventHandler(ClientEvents.ACTION_STEP_OUT)]
        private async void Execute()
        {
            if (CarSelector.SelectedCar == null) return;

            if (!CarSelector.SelectedCar.Vehicle().CanPlayerInteractWithDriver())
            {
                ClientFunctions.SendErrorMessage("You must be at the driver's door to do this.");
                return;
            }

            Screen.ShowSubtitle("~b~You: ~w~Can you step out of the vehicle for me please?");

            var vehicle = CarSelector.SelectedCar.Vehicle();
            var ped = vehicle.Driver;
            API.TaskLeaveVehicle(ped.Handle, vehicle.Handle, 0);
            await Delay(1000);

            API.TaskStandStill(ped.Handle, -1);
        }
    }
}