using CitizenFX.Core;
using CitizenFX.Core.Native;
using CitizenFX.Core.UI;
using PoliceMP.Main.Client.Retrievers;
using PoliceMP.Main.Client.Util;
using PoliceMP.Main.Core.Client;
using PoliceMP.Main.Core.Client.Extensions;
using PoliceMP.Main.Core.Shared.Constants;
using PoliceMP.Main.Shared.Events;
using System.Text;
using System.Threading.Tasks;

namespace PoliceMP.Main.Client.Actions.Cars
{
    public class SearchCar : BaseScript
    {
        private static bool CanSearch = true;

        [Command("searchcar")]
        private void Command()
        {
            TriggerEvent(ClientEvents.ACTION_SEARCH_CAR);
        }

        [EventHandler(ClientEvents.ACTION_SEARCH_CAR)]
        private async void Execute()
        {
            if (!ActionCooldown.Check() || !CanSearch) return;

            if (Game.PlayerPed.CurrentVehicle != null)
                return;

            // Get the car in front of the player
            var vehicle = Game.PlayerPed.GetVehicleInFront();
            if (vehicle == null)
            {
                ClientFunctions.SendErrorMessage("You must be facing the vehicle that you want to search.");
                return;
            }

            if (!vehicle.IsNearEntity(Game.PlayerPed, new Vector3(2f, 2f, 2f)))
            {
                ClientFunctions.SendErrorMessage("You are not close enough to the vehicle.");
                return;
            }

            if (!VehicleCanBeSearched(vehicle)) return;
            CanSearch = false;

            // If it's not, open the doors and display findings
            foreach (var door in vehicle.Doors)
                door.Open();

            Screen.ShowSubtitle($"Searching the <span class='text-warning'>{vehicle.LocalizedName}...");

            Game.PlayerPed.IsPositionFrozen = true;
            vehicle.IsDriveable = false;

            SoundPlayer.PlaySound(SoundPlayer.SearchCar);
            Game.PlayerPed.Task.PlayAnimation("missexile3", "ex03_dingy_search_case_base_michael");

            await Delay(5000);

            vehicle.IsDriveable = true;
            Game.PlayerPed.IsPositionFrozen = false;

            foreach (var door in vehicle.Doors)
                door.Close();

            var car = await CarRetriever.GetCar(vehicle.Handle);

            if (car.Items.Count == 0)
            {
                ClientFunctions.ShowToast("Vehicle Search", "Nothing of interest was found.", "info");
            }
            else
            {
                var builder = new StringBuilder();

                builder.Append(car.HasIllegalItems
                    ? "<span class='text-danger'>Illegal Item(s) Found</span>"
                    : "<span class='text-success'>Clear</span>");

                foreach (var item in car.Items)
                    builder.Append(item.Legal ? $"- {item.Name}<br>" : $"- <span class='text-danger'>{item.Name}</span><br>");

                ClientFunctions.ShowToast("Vehicle Search", builder.ToString(), "info");
            }

            CanSearch = true;
        }

        private static bool VehicleCanBeSearched(Vehicle vehicle, bool displayHelpText = true)
        {
            if (vehicle == null || !CanSearch) return false;

            // See if it's police car
            if (vehicle.ClassType == VehicleClass.Emergency)
            {
                if (displayHelpText) ClientFunctions.SendErrorMessage("You cannot search that vehicle.");
                return false;
            }

            // See if it's locked
            if (vehicle.LockStatus == VehicleLockStatus.CanBeBrokenInto)
            {
                if (displayHelpText)
                    ClientFunctions.SendErrorMessage("The vehicle is locked. You should break into it first.");
                return false;
            }

            // See if there is someone inside
            if (vehicle.Occupants.Length > 0)
            {
                if (displayHelpText)
                    ClientFunctions.SendErrorMessage("You should ask all occupants to leave the vehicle first.");
                return false;
            }

            return true;
        }
    }
}