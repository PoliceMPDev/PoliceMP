using CitizenFX.Core;
using CitizenFX.Core.Native;
using CitizenFX.Core.UI;
using PoliceMP.Client.Services.Interfaces;
using PoliceMP.Core.Client;
using PoliceMP.Core.Client.Extensions;
using PoliceMP.Core.Client.Interface;
using PoliceMP.Shared.Constants;
using PoliceMP.Shared.Constants.States;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using PoliceMP.Client.Actions.RunPlate;
using PoliceMP.Core.Client.Actions.Interfaces;
using PoliceMP.Core.Client.Communications.Interfaces;
using PoliceMP.Core.Client.Scripts;
using PoliceMP.Core.Shared.Enums;
using PoliceMP.Core.Shared.Models;
using PoliceMP.Shared.Behaviors.FailToStop;
using PoliceMP.Shared.Constants.Decors;
using Vector3 = CitizenFX.Core.Vector3;
using PoliceMP.Client.Overlays.NewNotification;
using PoliceMP.Shared.Enums;

namespace PoliceMP.Client.Scripts.Pullover
{
    public class PulloverScript : Script, IPulloverScript
    {
        private const Control DeselectVehicleControl = Control.Sprint;
        private const Control SelectVehicleControl = Control.Sprint;
        private const Control PulloverControl = Control.ContextSecondary;
        private const Control PulloverDifferentPlaceControl = Control.VehicleCinCam;
        private const Control KeepDrivingControl = Control.ContextSecondary;
        private const Control RunRedLightControl = Control.Pickup;

        private readonly ITickManager _ticks;
		private readonly INewNotificationOverlay _newNotificationOverlay;
		private readonly IInstructionalButtonsService _instructionalButtons;
        private readonly IActionManager _actions;
        private readonly IPersonalityService _personality;
        private readonly ILegacyClientCommunicationsManager _comms;
        private readonly IGameInputManager _input;
        private readonly IBehaviorService _behaviorService;
        private readonly IPermissionService _permissionService;
        public Vehicle PulledVehicle { get; private set; }
        public PulloverState State { get; private set; }
        public bool PlayerIsInCopVehicle { get; private set; }

        private Task _pulloverTask;
        private CancellationTokenSource _pulloverCancellationToken;

        public PulloverScript(ITickManager ticks,
			INewNotificationOverlay newNotificationOverlay,
            IInstructionalButtonsService instructionalButtons,
            IActionManager actions,
            IPersonalityService personality,
            ILegacyClientCommunicationsManager comms,
            IGameInputManager input, 
            IBehaviorService behaviorService,
            IPermissionService permissionService
        ) {
            _ticks = ticks;
			_newNotificationOverlay = newNotificationOverlay;
			_instructionalButtons = instructionalButtons;
            _actions = actions;
            _personality = personality;
            _comms = comms;
            _input = input;
            _behaviorService = behaviorService;
            _permissionService = permissionService;
            State = PulloverState.None; 

            RegisterDecor(PedDecors.ALWAYS_FLEE, DecorType.Bool);
        }

        protected override Task OnStartAsync()
        {
            _ticks.On(PulloverTick);
            API.AddTextEntry(TextEntries.DeselectVehicle, "Deselect vehicle");
            API.AddTextEntry(TextEntries.PullOverVehicle, "Pull vehicle over");
            API.AddTextEntry(TextEntries.PullOverRunRedLight, "Run red light");
            API.AddTextEntry(TextEntries.ReleaseVehicle, "Release vehicle");
            API.AddTextEntry(TextEntries.PullOverKeepDriving, "Keep driving");
            API.AddTextEntry(TextEntries.PullOverVehicleDifferentPlace, "Pull over in different place");
            API.AddTextEntry(TextEntries.SelectVehicleInFront, "Select vehicle in front");
            return Task.FromResult(0);
        }

        private async Task PulloverTick()
        {
            if (State != PulloverState.None)
            {
                if (PlayerIsInCopVehicle && !Game.PlayerPed.IsInEmergencyVehicle())
                {
                    await ClearInstructionalButtons();
                    PlayerIsInCopVehicle = false;
                }
                else
                {
                    PlayerIsInCopVehicle = Game.PlayerPed.IsInEmergencyVehicle();
                }
            }

            if (State != PulloverState.None && !IsPulledVehicleStillValid())
            {
                await InvalidReset();
                return;
            }

            switch (State)
            {
                case PulloverState.None:
                    await HandleNoneState();
                    break;

                case PulloverState.Selected:
                    await HandleSelectedState();
                    break;

                case PulloverState.PullingOver:
                    await HandlePullingOverState();
                    break;

                case PulloverState.PulledOver:
                    await HandlePulledOverState();
                    break;
            }
        }

        private async Task HandleNoneState()
        {
            var playerVehicle = Game.PlayerPed.CurrentVehicle;

            if (playerVehicle == null || playerVehicle.ClassType != VehicleClass.Emergency || !(_permissionService.CurrentUserRole.Branch == UserBranch.Police || _permissionService.CurrentUserRole.Branch == UserBranch.Highways))
            {
                await ClearInstructionalButtons();
                return;
            }

            var result = World.RaycastCapsule(playerVehicle.Position,
                playerVehicle.ForwardVector,
                20f + playerVehicle.Speed * 5,
                4f,
                IntersectOptions.MissionEntities, playerVehicle);

            if (!result.DitHitEntity 
                || result.HitEntity is not Vehicle vehicleInFront
                || vehicleInFront.Driver is null 
                || !API.DoesEntityExist(vehicleInFront.Driver.Handle))
            {
                await ClearInstructionalButtons();
                return;
            }

            //World.DrawLine(playerVehicle.Position, hitVehicle.Position, System.Drawing.Color.FromArgb(255, 0, 255, 0));

            await _instructionalButtons.AddInstructionalButton(TextEntries.SelectVehicleInFront, SelectVehicleControl);

            if (!_input.IsJustPressed(SelectVehicleControl)) return;

            if (vehicleInFront.State.Get<bool>(VehicleStates.IsSelected))
            {
				_newNotificationOverlay.SendNotification(new NewNotificationMessage("Pullover", "error", "The vehicle you are trying to select is already selected by someone else.", new NewNotificationMessageContent[0]));
				return;
            }

            PulledVehicle = vehicleInFront;
            PulledVehicle.AttachBlip();
            PulledVehicle.State.Set<bool>(VehicleStates.IsSelected, true);
			_newNotificationOverlay.SendNotification(new NewNotificationMessage("Pullover", "info", $"You have selected the {PulledVehicle.LocalizedName}.", new NewNotificationMessageContent[0]));

			await SetState(PulloverState.Selected);
            await _actions.Execute(new RunPlate(PulledVehicle.GetPlateText()));

            Game.Player.State.Set<string>(PlayerStates.LastPlateFromPullover, PulledVehicle.GetPlateText());
        }

        private async Task HandleSelectedState()
        {
            var playerVehicle = Game.PlayerPed.CurrentVehicle;

            if (playerVehicle == null || playerVehicle.ClassType != VehicleClass.Emergency)
            {
				_newNotificationOverlay.SendNotification(new NewNotificationMessage("Pullover", "info", $"The {PulledVehicle.LocalizedName} was deselected because you left your vehicle.", new NewNotificationMessageContent[0]));
				await ReleaseVehicle();
                return;
            }

            await _instructionalButtons.AddInstructionalButton(TextEntries.DeselectVehicle, DeselectVehicleControl);
            await _instructionalButtons.AddInstructionalButton(TextEntries.PullOverVehicle, PulloverControl);
            if (PulledVehicle.IsStoppedAtTrafficLights)
                await _instructionalButtons.AddInstructionalButton(TextEntries.PullOverRunRedLight, RunRedLightControl);
            else
                await _instructionalButtons.RemoveInstructionalButton(TextEntries.PullOverRunRedLight);

            if (_input.IsJustPressed(DeselectVehicleControl))
            {
                await ReleaseVehicle();
                return;
            }

            if (PulledVehicle.Driver?.IsPlayer == true)
            {
                return;
            }

            if (playerVehicle.IsSirenActive)
            {
				_newNotificationOverlay.SendNotification(new NewNotificationMessage("Pullover", "info", $"You have ordered the {PulledVehicle.LocalizedName} to pull over.", new NewNotificationMessageContent[0]));
				await SetState(PulloverState.PullingOver);
                PulledVehicle.IsPersistent = true;
                return;
            }

            if (PulledVehicle.IsStoppedAtTrafficLights && _input.IsJustPressed(RunRedLightControl))
            {
				_newNotificationOverlay.SendNotification(new NewNotificationMessage("Pullover", "info", $"You have ordered the {PulledVehicle.LocalizedName} to run the red light.", new NewNotificationMessageContent[0]));
				PulledVehicle.Driver.TaskDriveWander(drivingStyle: 7);
            }
        }

        private async Task HandlePullingOverState()
        {

            await _instructionalButtons.AddInstructionalButton(TextEntries.ReleaseVehicle, DeselectVehicleControl);
            await _instructionalButtons.AddInstructionalButton(TextEntries.PullOverVehicleDifferentPlace,
                PulloverDifferentPlaceControl);

            if (_pulloverTask == null)
            {
                var cannotPullOver = PulledVehicle.Driver.GetBoolDecor(PedDecors.ALWAYS_FLEE);
                var flee = cannotPullOver || await _personality.WillPedResist(PulledVehicle.Driver);

                if (flee)
                {
                    var driver = PulledVehicle.Driver;
                    Screen.ShowSubtitle($"~r~The driver of the {PulledVehicle.LocalizedName} has driven off!");
					_newNotificationOverlay.SendNotification(new NewNotificationMessage("Pullover", "warning", $"The {PulledVehicle.LocalizedName} has failed to stop!", new NewNotificationMessageContent[0]));
					PulledVehicle.Driver.SetBoolDecor(PedDecors.ALWAYS_FLEE, true);
                    await ReleaseVehicle();
                    // await driver.DesperatelyFlee(Game.PlayerPed);

                    if (await driver.TryRequestNetworkEntityControl())
                    {
                        _behaviorService.SetPedBehavior<FailToStopBehavior>(driver);
                    }
                }
                else
                {
                    _pulloverCancellationToken = new CancellationTokenSource();
                    _pulloverTask = PulledVehicle.PullOver(_pulloverCancellationToken.Token);
                }
                
            }
            else
            {
                Screen.DisplayHelpTextThisFrame($"Please wait while the ~y~{PulledVehicle.LocalizedName} ~w~pulls over.");

                if (_pulloverTask.IsCompleted)
                {
                    await SetState(PulloverState.PulledOver);
                    _pulloverTask = null;
                    return;
                }

                if (_input.IsJustPressed(SelectVehicleControl))
                {
                    _pulloverCancellationToken?.Cancel();

                    while (_pulloverTask.IsCompleted == false
                           && _pulloverTask.IsCanceled == false
                           && _pulloverTask.IsFaulted == false)
                    {
                        await Delay(0);
                    }

                    _pulloverTask = null;
                }

                if (_input.IsJustPressed(PulloverDifferentPlaceControl))
                {
                    _pulloverCancellationToken?.Cancel();
                    _pulloverCancellationToken = new CancellationTokenSource();
                    _pulloverTask = PulledVehicle.PullOver(_pulloverCancellationToken.Token);
                }

                if (_pulloverCancellationToken?.IsCancellationRequested == true)
                {
                    await ReleaseVehicle();
                    _pulloverTask = null;
                }
            }
        }

        private async Task HandlePulledOverState()
        {
            var playerVehicle = Game.PlayerPed.CurrentVehicle;

            PulledVehicle.SetEngineState(false);

            if (playerVehicle == null || playerVehicle.ClassType != VehicleClass.Emergency)
                return;

            await _instructionalButtons.AddInstructionalButton(TextEntries.ReleaseVehicle, DeselectVehicleControl);
            await _instructionalButtons.AddInstructionalButton(TextEntries.PullOverKeepDriving, KeepDrivingControl);
            await _instructionalButtons.AddInstructionalButton(TextEntries.PullOverVehicleDifferentPlace, PulloverDifferentPlaceControl);

            if (_input.IsJustPressed(DeselectVehicleControl))
            {
				_newNotificationOverlay.SendNotification(new NewNotificationMessage("Pullover", "info", $"You have released the {PulledVehicle.LocalizedName}.", new NewNotificationMessageContent[0]));
				await ReleaseVehicle();
                return;
            }

            if (_input.IsJustPressed(PulloverDifferentPlaceControl))
            {
				_newNotificationOverlay.SendNotification(new NewNotificationMessage("Pullover", "info", $"You have told the {PulledVehicle.LocalizedName} to pull over in a different place.", new NewNotificationMessageContent[0]));
				await SetState(PulloverState.PullingOver);
            }
        }

        private async Task ReleaseVehicle()
        {
            if (PulledVehicle != null && API.DoesEntityExist(PulledVehicle.Handle))
            {
                PulledVehicle.AttachedBlip.Delete();
                PulledVehicle.SetEngineState(true);
                PulledVehicle.Driver?.Task.ClearSecondary();

                var driver = PulledVehicle.Driver;
                if (driver != null) driver.TaskDriveWander();

                PulledVehicle.State.Set<bool>(VehicleStates.IsSelected, false);
                PulledVehicle.IsPersistent = false;
                PulledVehicle = null;
            }

            await SetState(PulloverState.None);
        }

        private async Task SetState(PulloverState state)
        {
            await ClearInstructionalButtons();
            State = state;
        }

        private async Task InvalidReset()
        {
            _newNotificationOverlay.SendNotification(new NewNotificationMessage("Pullover", "warning", "The vehicle has been deselected/released as it was no longer valid", new NewNotificationMessageContent[0]));
			if (PulledVehicle == null) await SetState(PulloverState.None);
            else await ReleaseVehicle();
        }

        private bool IsPulledVehicleStillValid()
        {
            if (PulledVehicle == null) return false;
            if (!API.DoesEntityExist(PulledVehicle.Handle)) return false;
            if (!Game.PlayerPed.IsCloseEnoughToEntity(PulledVehicle, 100f)) return false;
            return true;
        }

        private bool IsDriverStillValid()
        {
            if (PulledVehicle?.Driver == null) return false;
            if (!API.DoesEntityExist(PulledVehicle.Driver.Handle)) return false;
            if (PulledVehicle.Driver.IsDead) return false;
            return true;
        }

        private async Task ClearInstructionalButtons()
        {
            await _instructionalButtons.RemoveInstructionalButtons(new List<string>
            {
                TextEntries.DeselectVehicle,
                TextEntries.PullOverVehicle,
                TextEntries.PullOverRunRedLight,
                TextEntries.ReleaseVehicle,
                TextEntries.PullOverKeepDriving,
                TextEntries.PullOverVehicleDifferentPlace,
                TextEntries.SelectVehicleInFront
            });
        }

        public async Task ForceRelease()
        {
            await ReleaseVehicle();
            await SetState(PulloverState.None);
        }
    }
}