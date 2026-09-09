using CitizenFX.Core;
using PoliceMP.Main.Client.Extensions;
using PoliceMP.Main.Client.Managers;
using PoliceMP.Main.Client.Util;
using PoliceMP.Main.Core.Client;
using PoliceMP.Main.Core.Client.Extensions;
using PoliceMP.Main.Shared.Events;

namespace PoliceMP.Main.Client.Actions.People
{
    public class Release : BaseScript
    {
        /// <summary>
        ///     The command used to execute the action.
        /// </summary>
        [Command(Commands.Release)]
        private void Command()
        {
            if (PersonSelector.SelectedPerson == null)
            {
                ClientFunctions.SendErrorMessage("You need to select a person to do this.");
                return;
            }

            TriggerEvent(ClientEvents.ACTION_RELEASE, PersonSelector.SelectedPerson.EntityId());
        }

        [EventHandler(ClientEvents.ACTION_RELEASE)]
        private async void Execute(int pedHandle)
        {
            if (!ActionCooldown.Check()) return;

            var ped = (Ped)Entity.FromHandle(pedHandle);

            if (ped.IsCuffed)
            {
                ClientFunctions.SendErrorMessage("You must uncuff the person before releasing them.");
                return;
            }

            PersonSelector.UnselectPerson();

            ped.BlockPermanentEvents = false;
            ped.CanPlayGestures = true;

            // If they were in a vehicle, put them back in it
            var vehicle = ped.LastVehicle;
            if (vehicle != null)
            {
                var seat = VehicleSeat.Any;
                // If someone's already in the driver's seat, shove them
                // in one of the passenger seats or make them walk away
                // if it's full
                if (vehicle.Driver.Handle != 0)
                {
                    if (vehicle.IsSeatFree(VehicleSeat.RightFront))
                        seat = VehicleSeat.RightFront;
                    else if (vehicle.IsSeatFree(VehicleSeat.RightRear))
                        seat = VehicleSeat.RightRear;
                    else if (vehicle.IsSeatFree(VehicleSeat.LeftRear))
                        seat = VehicleSeat.LeftRear;
                    else
                        ped.Task.WanderAround();
                }

                ped.Task.ClearAll();

                await Delay(1000);

                ped.Task.EnterVehicle(vehicle, seat);

                if (CarSelector.SelectedCar != null && CarSelector.SelectedCar.Vehicle().Handle == vehicle.Handle)
                    CarSelector.UnselectCar();

                return;
            }

            ped.Task.ClearAll();
            await Delay(1000);

            ped.Stop(false);
            ped.Task.WanderAround();
        }
    }
}