using CitizenFX.Core;
using CitizenFX.Core.Native;
using CitizenFX.Core.UI;
using PoliceMP.Main.Client.Extensions;
using PoliceMP.Main.Client.Managers;
using System.Threading.Tasks;

namespace PoliceMP.Main.Client.Actions.Cars
{
    public class Pullover : BaseScript
    {
        public static bool Stopped { get; private set; }

        public static bool KeepGoing { get; private set; }
        
        private async Task CheckControls()
        {
            if (CarSelector.SelectedCar == null)
            {
                Stopped = false;
                KeepGoing = false;
            }

            // Check if the vehicle is stopped and make sure they're stopped
            if (Stopped)
            {
                if (!KeepGoing)
                {
                    API.SetVehicleEngineOn(CarSelector.SelectedCar.EntityId(), false, false, true);
                    await CheckPlayerCommandVehicleToKeepGoing();
                }
                else
                {
                    await CheckPlayerCommandVehicleToStopAgain();
                    await CheckPlayerCommandVehicleGoThroughRedLight();
                }
            }
            else
            {
                await CheckPlayerCommandVehicleToStop();
            }
        }

        private async Task CheckPlayerCommandVehicleToStop()
        {
            if (Game.PlayerPed.CurrentVehicle == null ||
                Game.PlayerPed.CurrentVehicle.ClassType != VehicleClass.Emergency) return;

            var car = CarSelector.SelectedCar;

            if (car == null) return;

            if (!Stopped && Game.PlayerPed.CurrentVehicle.IsSirenActive)
            {
                if (Resist.Check(car)) return;

                await DoTheStop(car.EntityId());

                API.RollDownWindows(car.EntityId());
                Stopped = true;
            }
        }

        private Vehicle vHistorical;
        private async Task DoTheStop(int _entityID)
        {
            Vehicle v = (Vehicle)Entity.FromHandle(_entityID);
            vHistorical = v;
            Vector3 offset = new Vector3(5f, 20f, 0);               //15f in front of the car
            Vector3 worldcoords = v.GetOffsetPosition(offset);


            //This gets the vehicle node heading
            Vector3 outPosition = new Vector3();
            float outHeading = new float();
            int unk = 0;
            bool f = API.GetNthClosestVehicleNodeWithHeading(v.Position.X, v.Position.Y, v.Position.Z, 1, ref outPosition, ref outHeading, ref unk, 9, 3.0f, 2.5f);


            //Find spawn in relation to offset and vehicle node heading
            Vector3 outpos = new Vector3();
            bool found = API.GetRoadSidePointWithHeading(worldcoords.X, worldcoords.Y, worldcoords.Z, outHeading, ref outpos);

            //Start moving
            if (found)
            {
                Screen.ShowSubtitle("Vehicle is moving to an appropriate stopping point.");

                v.Driver.Task.DriveTo(v, outpos, 0f, 5);

                Vector3 distanceto = API.GetOffsetFromEntityGivenWorldCoords(v.Handle, outpos.X, outpos.Y, outpos.Z);

                //While not near keep driving
                bool near = false;
                int tickCounter = 0;
                while (!near && tickCounter < 1000)
                {
                    if (tickCounter >= 1000) { v.Driver.Task.Wait(10); return; }

                    Vector3 dynamicdistanceto = API.GetOffsetFromEntityGivenWorldCoords(v.Handle, outpos.X, outpos.Y, outpos.Z);
                    bool xclose = false, yclose = false;

                    if ((dynamicdistanceto.X <= 0.2f && dynamicdistanceto.X >= 0)
                        || (dynamicdistanceto.X >= -0.2f && dynamicdistanceto.X <= 0))
                    {
                        xclose = true;
                    }

                    if ((dynamicdistanceto.Y <= 0.2f && dynamicdistanceto.Y >= 0)
                        || (dynamicdistanceto.Y >= -0.2f && dynamicdistanceto.Y <= 0))
                    {
                        yclose = true;
                    }

                    if (xclose && yclose)
                    {
                        near = true;
                        v.Driver.Task.Wait(10);
                    }

                    tickCounter++;
                    await Delay(1);
                }

                //Once near stop
                Stopped = true;
                KeepGoing = false;
            }
        }

        private Task CheckPlayerCommandVehicleToKeepGoing()
        {
            if (Game.PlayerPed.CurrentVehicle == null ||
                Game.PlayerPed.CurrentVehicle.ClassType != VehicleClass.Emergency) return Task.FromResult(0);

            if (Stopped && !KeepGoing && API.IsControlPressed(0, 38))
            {
                KeepGoing = true;
                API.TaskVehicleDriveWander(CarSelector.SelectedCar.Vehicle().Driver.Handle,
                    CarSelector.SelectedCar.Vehicle().Handle, 15f, 447);

                Game.PlayerPed.CurrentVehicle.IsSirenActive = false;
            }

            return Task.FromResult(0);
        }

        private async Task CheckPlayerCommandVehicleToStopAgain()
        {
            if (Game.PlayerPed.CurrentVehicle == null ||
                Game.PlayerPed.CurrentVehicle.ClassType != VehicleClass.Emergency) 
                return;

            await DoTheStop(vHistorical.Handle);

            if (Stopped && KeepGoing && Game.PlayerPed.CurrentVehicle.IsSirenActive) KeepGoing = false;
        }

        private Task CheckPlayerCommandVehicleGoThroughRedLight()
        {
            if (Game.PlayerPed.CurrentVehicle == null ||
                Game.PlayerPed.CurrentVehicle.ClassType != VehicleClass.Emergency) return Task.FromResult(0);

            if (API.IsControlPressed(0, 38) && CarSelector.SelectedCar.Vehicle().IsStoppedAtTrafficLights) // E
            {
                KeepGoing = true;
                API.TaskVehicleDriveWander(CarSelector.SelectedCar.Vehicle().Driver.Handle,
                    CarSelector.SelectedCar.Vehicle().Handle, 15f, 7);
            }

            return Task.FromResult(0);
        }

        [Tick]
        private Task UpdateScreen()
        {
            if (CarSelector.SelectedCar == null || Game.PlayerPed.CurrentVehicle == null ||
                Game.PlayerPed.CurrentVehicle.ClassType != VehicleClass.Emergency)
                return Task.FromResult(0);

            if (!Stopped)
                Screen.DisplayHelpTextThisFrame("Press ~INPUT_VEH_RADIO_WHEEL~ ~w~to turn on your lights and stop the vehicle.");
            else if (!KeepGoing)
                Screen.DisplayHelpTextThisFrame("Press ~INPUT_PICKUP~ ~w~to command the vehicle to keep moving.");
            else if (CarSelector.SelectedCar.Vehicle().IsStoppedAtTrafficLights)
                Screen.DisplayHelpTextThisFrame("Press ~INPUT_PICKUP~ ~w~to tell the vehicle to run the red light.");
            else if (KeepGoing)
                Screen.DisplayHelpTextThisFrame("Press ~INPUT_VEH_RADIO_WHEEL~ ~w~to turn on your lights and stop the vehicle again.");

            return Task.FromResult(0);
        }
    }
}