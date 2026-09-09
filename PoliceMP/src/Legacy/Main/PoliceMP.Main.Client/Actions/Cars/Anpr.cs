using CitizenFX.Core;
using CitizenFX.Core.Native;
using PoliceMP.Client.Services.Interfaces;
using PoliceMP.Main.Client.Extensions;
using PoliceMP.Main.Client.Retrievers;
using PoliceMP.Main.Client.Util;
using PoliceMP.Main.Core.Client.Extensions;
using PoliceMP.Main.Shared.Events;
using PoliceMP.Main.Shared.Models;
using System.Media;
using System.Threading.Tasks;
using SoundPlayer = PoliceMP.Main.Client.Util.SoundPlayer;

namespace PoliceMP.Main.Client.Actions.Cars
{
    public class Anpr : BaseScript
    {
        public static Car CurrentCar;

        private static bool _enabled;
        private static bool _locked;
        private static float _lockedSpeed;
        private readonly ISoundService _sound;



        private void Command()
        {
            TriggerEvent(ClientEvents.ENABLE_ANPR);
        }

        [EventHandler(ClientEvents.ENABLE_ANPR)]
        private void Execute()
        {
            if (Game.PlayerPed.CurrentVehicle != null &&
                Game.PlayerPed.CurrentVehicle.ClassType == VehicleClass.Emergency)
                Enable(!_enabled);
        }

        [Tick]
        private async Task Update()
        {
            await Delay(100);

            // Disable ANPR if they are not in an emergency vehicle.
            if (Game.PlayerPed.CurrentVehicle == null ||
                Game.PlayerPed.CurrentVehicle.ClassType != VehicleClass.Emergency)
                Enable(false);

            // If it has been enabled...
            if (_enabled)
            {
                // If it's locked don't try to get new car
                if (_locked) return;

                // Check if the current car entity still exists.
                if (CurrentCar != null && !API.DoesEntityExist(CurrentCar.EntityId()))
                    CurrentCar = null;

                // Update the interface.
                UpdateNUI();

                // Get the car in front of the player.
                var vehicle = Game.PlayerPed.GetVehicleInFront(40f);
                if (vehicle == null)
                {
                    CurrentCar = null;
                    return;
                }

                // Check if the current car is the same as last time
                if (CurrentCar != null && CurrentCar.EntityId() == vehicle.Handle) return;

                // If not get the car
                CurrentCar = await CarRetriever.GetCar(vehicle.Handle);

                if (CurrentCar.IsIllegal)
                {
                    _sound.Play("firemdtsound.wav"); // doesnt seem to matter 
                    UpdateNUI();
                    Lock();
                }
            }
        }

        [Tick]
        private Task CheckControls()
        {
            if (!_enabled) return Task.FromResult(0);
            if (API.IsControlJustPressed(0, 36) || API.IsDisabledControlJustPressed(0, 36))
            {
                if (_locked)
                    Unlock();
                else
                    Lock();
            }           

            return Task.FromResult(0);
        }

        private void Enable(bool toggle)
        {
            _enabled = toggle;

            if (!toggle)
            {
                CurrentCar = null;
                HideNUI();
                Unlock();
                return;
            }

            ShowNUI();
        }

        private void ShowNUI()
        {
            API.SendNuiMessage("{\"anpr\": \"on\"}");
            API.SetNuiFocus(false, false);
        }

        private void HideNUI()
        {
            API.SendNuiMessage("{\"anpr\": \"off\"}");
        }

        private void Lock()
        {
            _locked = true;
            if (CurrentCar != null) _lockedSpeed = CurrentCar.Vehicle().GetMph();
            API.SendNuiMessage("{\"lock_anpr\": \"on\"}");
        }

        private void Unlock()
        {
            _locked = false;
            _lockedSpeed = 0f;
            API.SendNuiMessage("{\"lock_anpr\": \"off\"}");
        }

        private void UpdateNUI()
        {
            if (CurrentCar == null)
            {
                API.SendNuiMessage("{\"receive_anpr_data\": \"off\"}");
                return;
            }

            var colour = CurrentCar.Vehicle().Mods.PrimaryColor.ToString().ToUpper();
            if (colour.Contains("METALLIC"))
                colour = colour.Substring(8);

            float speed;
            var vehicleInFront = Game.PlayerPed.GetVehicleInFront(40f);
            if (_locked && vehicleInFront != null && vehicleInFront.Handle != CurrentCar.EntityId()) speed = _lockedSpeed;
            else
            {
                speed = CurrentCar.Vehicle().GetMph();
                _lockedSpeed = speed;
            }

            API.SendNuiMessage("{\"receive_anpr_data\": \"on\", " +
                               "\"plate\": \"" + CurrentCar.Plate + "\", " +
                               "\"speed\": \"" + speed + "\", " +
                               "\"model\": \"" + CurrentCar.Vehicle().LocalizedName.ToUpper() + "\", " +
                               "\"colour\": \"" + colour + "\", " +

                               "\"mot\": \"" + CurrentCar.HasMot + "\", " +
                               "\"tax\": \"" + CurrentCar.HasTax + "\", " +
                               "\"insurance\": \"" + CurrentCar.HasInsurance + "\", " +

                               "\"drugsintel\": \"" + !CurrentCar.HasMarker("Drugs Intel") + "\", " +
                               "\"weaponsintel\": \"" + !CurrentCar.HasMarker("Weapons Intel") + "\", " +
                               "\"failtostop\": \"" + !CurrentCar.HasMarker("Fail to Stop") + "\", " +
                               "\"outstandingcrime\": \"" + !CurrentCar.HasMarker("Outstanding Crime") + "\", " +
                               "\"stolen\": \"" + !CurrentCar.HasMarker("Stolen") + "\", " +
                               "\"writtenoff\": \"" + !CurrentCar.HasMarker("Written Off") + "\", " +
                               "\"scrapped\": \"" + !CurrentCar.HasMarker("Scrapped") + "\", " +
                               "\"exported\": \"" + !CurrentCar.HasMarker("Exported") + "\"}");
        }
    }
}