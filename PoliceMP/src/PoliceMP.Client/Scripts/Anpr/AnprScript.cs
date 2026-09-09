using CitizenFX.Core;
using CitizenFX.Core.Native;
using PoliceMP.Client.Services.Interfaces;
using PoliceMP.Core.Client.Commands.Interfaces;
using PoliceMP.Core.Client.Extensions;
using PoliceMP.Core.Client.Interface;
using PoliceMP.Shared.Constants;
using System;
using System.Threading.Tasks;
using CitizenFX.Core.UI;
using PoliceMP.Client.Overlays.Legacy.AnprOverlay;
using PoliceMP.Core.Client.Scripts;
using System.Media;


namespace PoliceMP.Client.Scripts.Anpr
{
    public class AnprScript : Script
    {
        private const Control ToggleLockControl = Control.Jump;

        private AnprState _state;
        private Vehicle _currentVehicle;

        private readonly ICommandManager _commands;
        private readonly IVehicleInfoService _vehicleInfoService;
        private readonly ITickManager _ticks;
        private readonly AnprOverlay _overlay;
        private readonly IInstructionalButtonsService _instructionalButtons;
        private readonly ISoundService _soundService;

        private Vector3 OffsetToRead = new Vector3(0,10,0);
        
        public AnprScript(ICommandManager commands,
            ITickManager ticks,
            AnprOverlay overlay,
            IVehicleInfoService vehicleInfoService, 
            IInstructionalButtonsService instructionalButtons,
            ISoundService soundService)

        {
            _commands = commands;
            _ticks = ticks;
            _overlay = overlay;
            _vehicleInfoService = vehicleInfoService;
            _instructionalButtons = instructionalButtons;
            _soundService = soundService;
            _state = AnprState.Off;
        }

        protected override Task OnStartAsync()
        {
            _commands.Register("anpr").WithHandler(ToggleAnpr);
            API.AddTextEntry(TextEntries.UnlockAnpr, "Unlock ANPR");
            return Task.FromResult(0);
        }

        private void ToggleAnpr()
        {
            if (_state == AnprState.Off)
            {
                _state = AnprState.Positioning;
                _overlay.Hide();
                _ticks.On(AnprTick);
            }
            else if (_state == AnprState.Positioning)
            {
                _state = AnprState.Active;
                _overlay.Show();
                _ticks.On(AnprTick);
            }
            else
            {
                _state = AnprState.Off;
                _overlay.Hide();
                _ticks.Off(AnprTick);
            }
        }

        private async Task LockAnpr(bool shouldLock)
        {
            if (shouldLock)
            {
                await _instructionalButtons.AddInstructionalButton(TextEntries.UnlockAnpr, ToggleLockControl);
                _state = AnprState.Locked;
                _overlay.Lock(true);
            }
            else
            {
                await _instructionalButtons.RemoveInstructionalButton(TextEntries.UnlockAnpr);
                _currentVehicle = null;
                _overlay.Clear();
                _state = AnprState.Active;
                _overlay.Lock(false);
            }
        }

        private async Task AnprTick()
        {
            switch (_state)
            {
                case AnprState.Active:
                    await HandleActiveState();
                    break;
                
                case AnprState.Positioning:
                    await HandlePositioningState();
                    break;

                case AnprState.ActiveWithVehicleInFront:
                    await HandleActiveWithVehicleInFrontState();
                    break;

                case AnprState.Locked:
                    await HandleLockedState();
                    break;
            }
        }

        private async Task HandleActiveWithVehicleInFrontState()
        {
            if (!Game.PlayerPed.IsDrivingEmergencyVehicle())
            {
                ToggleAnpr();
                return;
            }

            var playerVehicle = Game.PlayerPed.CurrentVehicle;
            var vehicleInFront = playerVehicle.GetVehicleInFront();
            if (vehicleInFront == null || _currentVehicle == null || vehicleInFront.Handle != _currentVehicle.Handle)
            {
                _currentVehicle = null;
                _overlay.Clear();
                _state = AnprState.Active;
                return;
            }

            await Delay(1000);
        }

        private async Task HandleActiveState()
        {
            if (!Game.PlayerPed.IsDrivingEmergencyVehicle())
            {
                ToggleAnpr();
                return;
            }

            var worldpos = API.GetOffsetFromEntityInWorldCoords(Game.PlayerPed.CurrentVehicle.Handle, OffsetToRead.X,
                OffsetToRead.Y, OffsetToRead.Z);
            var subject = Game.PlayerPed.CurrentVehicle;
            float lastdistance = 100f;
            foreach (var vehicle in World.GetAllVehicles())
            {
                if (_state == AnprState.Locked || _state == AnprState.Off) return;

                if (vehicle == Game.PlayerPed.CurrentVehicle) continue;
                
                var distance = API.GetDistanceBetweenCoords(worldpos.X, worldpos.Y, worldpos.Z, vehicle.Position.X,
                    vehicle.Position.Y, vehicle.Position.Z, true);

                if (distance >= 5f) continue;
                
                if (distance < lastdistance)
                {
                    subject = vehicle;
                }
            }

            if (subject == null || subject == Game.PlayerPed.CurrentVehicle) return;

            var vehicleInfo = await _vehicleInfoService.GetByNetworkId(
                subject.NetworkId,
                subject.GetPlateText(),
                subject.GetDriverPedNetworkId());
            if (vehicleInfo == null) return;

            _currentVehicle = subject;
            _overlay.Update(new AnprViewModel
            {
                Plate = vehicleInfo.Plate,
                Mph = _currentVehicle.GetMph(),
                Model = _currentVehicle.LocalizedName,
                Colour = _currentVehicle.GetColourNameWithoutMetallic(),
                MOT = vehicleInfo.IsMotExpired,
                Insurance = vehicleInfo.IsInsuranceExpired,
                Tax = vehicleInfo.IsTaxExpired,
                DrugsIntel = vehicleInfo.HasMarker("Drugs Intel"),
                WeaponsIntel = vehicleInfo.HasMarker("Weapons Intel"),
                FailToStop = vehicleInfo.HasMarker("Fail to Stop"),
                Stolen = vehicleInfo.HasMarker("Stolen"),
                OutstandingCrime = vehicleInfo.HasMarker("Outstanding Crime"),
                Other = vehicleInfo.HasMarker("Scrapped") || vehicleInfo.HasMarker("Exported") || vehicleInfo.HasMarker("Written Off")
            });

            if (vehicleInfo.IsIllegal)
            {
                _soundService.Play("attention.wav");
                await LockAnpr(true);
            }
            else
            {
                _state = AnprState.ActiveWithVehicleInFront;
            }

            await Delay(TimeSpan.FromSeconds(1));
        }

        private async Task HandlePositioningState()
        {
            if (!Game.PlayerPed.IsDrivingEmergencyVehicle())
            {
                _state = AnprState.Off;
                return;
            }

            var worldpos = API.GetOffsetFromEntityInWorldCoords(Game.PlayerPed.CurrentVehicle.Handle, OffsetToRead.X,
                OffsetToRead.Y, OffsetToRead.Z);

            float STEPVALUE = 0.5f;
            
            API.DrawMarker(1,worldpos.X, worldpos.Y, worldpos.Z, 0, 0,0, 0, 0, 0, 2, 2, 2, 201, 163, 10, 50, false, true, 2, false, null, null, false);
            
            
            Screen.ShowSubtitle("~y~Marker ~s~shows the ANPR position." +
                                "~n~~c~NUM 8~s~ ANPR Forward" +
                                "~n~~c~NUM 4~s~ ANPR Left" +
                                "~n~~c~NUM 5~s~ ANPR Back" +
                                "~n~~c~NUM 6~s~ ANPR Right" +
                                "~n~~c~NUM Enter~s~ ANPR Reset ~n~~n~~n~~n~~n~" 
                                 ,0);
            
            
            //Check if the player is using keyboard
            if (!API.IsInputDisabled(2)) return;
            
            //Numpad 8 - MOVE FORWARDS
            if (Game.IsControlJustPressed(0, (Control) 111)) OffsetToRead.Y += STEPVALUE;

            //Numpad 4 - MOVE LEFT
            if (Game.IsControlJustPressed(0, (Control) 108)) OffsetToRead.X -= STEPVALUE;

            //Numpad 6 - MOVE RIGHT
            if (Game.IsControlJustPressed(0, (Control) 109)) OffsetToRead.X += STEPVALUE;

            //Numpad 5 - MOVE BACK
            if (Game.IsControlJustPressed(0, (Control) 110)) OffsetToRead.Y -= STEPVALUE;
            
            //Numpad Enter - RESET
            if (Game.IsControlJustPressed(0, (Control) 201)) OffsetToRead = new Vector3(0, 10, 0);
        }

        private async Task HandleLockedState()
        {
            if (Game.IsControlJustPressed(0, ToggleLockControl))
            {
                await LockAnpr(false);
            }
        }
    }
}