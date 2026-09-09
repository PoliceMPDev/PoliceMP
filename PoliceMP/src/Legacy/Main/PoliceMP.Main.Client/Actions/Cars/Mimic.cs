using CitizenFX.Core;
using CitizenFX.Core.Native;
using CitizenFX.Core.UI;
using PoliceMP.Main.Client.Extensions;
using PoliceMP.Main.Client.Managers;
using PoliceMP.Main.Core.Client;
using PoliceMP.Main.Shared.Events;
using System.Threading.Tasks;

namespace PoliceMP.Main.Client.Actions.Cars
{
    public class Mimic : BaseScript
    {
        private static int _driver;
        private static bool _mimicking;

        [Command("mimic")]
        private void Command()
        {
            TriggerEvent(ClientEvents.ACTION_MIMIC);
        }

        [EventHandler(ClientEvents.ACTION_MIMIC)]
        private void Execute()
        {
            if (CarSelector.SelectedCar == null || !Pullover.Stopped || Pullover.KeepGoing) return;

            if (_mimicking)
                StopMimic();
            else
                StartMimic();
        }

        private static bool CheckMimicStillValid()
        {
            if (CarSelector.SelectedCar == null || !Pullover.Stopped || Pullover.KeepGoing)
            {
                StopMimic();
                return false;
            }

            var selectedVehicle = CarSelector.SelectedCar.Vehicle();

            if (selectedVehicle.IsDriveable &&
                Game.PlayerPed.CurrentVehicle != null) return true;


            StopMimic();
            return false;
        }

        private static void StartMimic()
        {
            _mimicking = true;

            var vehicle = CarSelector.SelectedCar.Vehicle();

            _driver = vehicle.Driver.Handle;
            API.SetPedIntoVehicle(vehicle.Driver.Handle, vehicle.Handle, 0);
        }

        private static void StopMimic()
        {
            _mimicking = false;

            if (CarSelector.SelectedCar != null)
            {
                var vehicle = CarSelector.SelectedCar.Vehicle();
                API.SetPedIntoVehicle(_driver, vehicle.Handle, -1);
            }

            _driver = -1;

            ClientFunctions.SendChatMessage("The vehicle is no longer mimicking you.", "Mimic");
        }

        private static void MakeCarMimicPlayer()
        {
            var selectedVehicle = CarSelector.SelectedCar.Vehicle();

            var playerVehicle = Game.PlayerPed.CurrentVehicle;
            var speedVector = API.GetEntitySpeedVector(playerVehicle.Handle, true);

            if (speedVector.Y > 0f)
                selectedVehicle.Speed = playerVehicle.Speed;
            else if (speedVector.Y < 0f)
                selectedVehicle.Speed = -1 * playerVehicle.Speed;

            selectedVehicle.SteeringAngle = playerVehicle.SteeringAngle;
        }

        [Tick]
        private Task OnTick()
        {
            if (!_mimicking || !CheckMimicStillValid()) return Task.FromResult(0); ;

            if (CarSelector.SelectedCar == null)
            {
                StopMimic();
                return Task.FromResult(0); ;
            }

            Screen.ShowSubtitle($"The ~b~{CarSelector.SelectedCar.Vehicle().LocalizedName} ~w~is now mimicking you.");

            MakeCarMimicPlayer();

            return Task.FromResult(0);
        }
    }
}