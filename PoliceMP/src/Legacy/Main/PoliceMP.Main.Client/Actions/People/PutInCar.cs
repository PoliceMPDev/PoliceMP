using CitizenFX.Core;
using CitizenFX.Core.UI;
using PoliceMP.Main.Client.Extensions;
using PoliceMP.Main.Client.Managers;
using PoliceMP.Main.Core.Client.Extensions;
using PoliceMP.Main.Shared.Events;
using PoliceMP.Main.Shared.Models;
using System.Threading.Tasks;

namespace PoliceMP.Main.Client.Actions.People
{
    public class PutInCar : BaseScript
    {
        [Tick]
        private async Task OnTick()
        {
            if (PersonSelector.SelectedPerson == null)
                await CheckTryingToRemoveFromCar();
            else
                await CheckTryingToPutInCar();
        }

        private async Task CheckTryingToPutInCar()
        {
            // Check if player is grabbing someone
            if (PersonSelector.SelectedPerson == null) return;

            var person = PersonSelector.SelectedPerson;
            if (person.Ped().IsAttachedTo(Game.PlayerPed))
            {
                var vehicle = Game.PlayerPed.GetVehicleInFront();
                if (vehicle == null) return;

                var door = Game.PlayerPed.GetVehicleDoorIsLookingAt(vehicle);
                if (door == VehicleDoorIndex.Trunk) return;

                Screen.DisplayHelpTextThisFrame("Press ~INPUT_PICKUP~ ~w~to put the person in the vehicle.");

                if (Game.IsControlPressed(0, Control.Pickup)) await PutPedInCar(person, vehicle, door);
            }
        }

        private async Task CheckTryingToRemoveFromCar()
        {
            if (PersonSelector.SelectedPerson != null) return;

            var vehicle = Game.PlayerPed.GetVehicleInFront();
            if (vehicle == null) return;

            var door = Game.PlayerPed.GetVehicleDoorIsLookingAt(vehicle);
            if (door == VehicleDoorIndex.Trunk) return;

            Ped ped = null;
            var seat = VehicleSeat.Any;

            switch (door)
            {
                case VehicleDoorIndex.BackLeftDoor:
                    seat = VehicleSeat.LeftRear;
                    break;
                case VehicleDoorIndex.BackRightDoor:
                    seat = VehicleSeat.RightRear;
                    break;
                case VehicleDoorIndex.FrontRightDoor:
                    seat = VehicleSeat.RightFront;
                    break;
            }

            ped = vehicle.GetPedOnSeat(seat);
            if (seat == VehicleSeat.Any || vehicle.IsSeatFree(seat)) return;

            Screen.DisplayHelpTextThisFrame("Press ~INPUT_PICKUP~ ~w~to remove the person from the vehicle.");

            if (Game.IsControlPressed(0, Control.Pickup))
            {
                ped.Task.LeaveVehicle();
                await Delay(500);
                ped.Task.StandStill(-1);
            }
        }

        private async Task PutPedInCar(Person person, Vehicle vehicle, VehicleDoorIndex door)
        {
            TriggerEvent(ClientEvents.ACTION_GRAB, person.EntityId());

            var seat = VehicleSeat.Any;
            switch (door)
            {
                case VehicleDoorIndex.BackLeftDoor:
                    seat = VehicleSeat.LeftRear;
                    break;
                case VehicleDoorIndex.BackRightDoor:
                    seat = VehicleSeat.RightRear;
                    break;
                case VehicleDoorIndex.FrontRightDoor:
                    seat = VehicleSeat.RightFront;
                    break;
            }

            vehicle.Doors[door].Open();

            while (!person.Ped().IsInVehicle(vehicle))
            {
                person.Ped().Task.EnterVehicle(vehicle, seat);

                await Delay(1000);
            }

            vehicle.Doors[door].Close();
        }
    }
}