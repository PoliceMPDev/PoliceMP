using System.Threading.Tasks;
using CitizenFX.Core;
using CitizenFX.Core.Native;
using PoliceMP.Client.Overlays.NewNotification;
using PoliceMP.Client.Services.Interfaces;
using PoliceMP.Core.Client.Abstraction;
using PoliceMP.Core.Client.Interface;
using PoliceMP.Core.Client.Scripts;

namespace PoliceMP.Client.Scripts.Npas
{
    public class NoExitHelicopterScript : Script
    {
        private readonly IGameInputManager _input;
        private readonly ITickManager _ticks;
        private readonly INotificationService _notificationService;

        public NoExitHelicopterScript(IGameInputManager input, ITickManager ticks, INotificationService notificationService)
        {
            _input = input;
            _ticks = ticks;
            _notificationService = notificationService;
        }

        protected override Task OnStartAsync()
        {
            _ticks.On(NoExitHelicopterTick);
            return Task.FromResult(0);
        }

        private Task NoExitHelicopterTick()
        {
            if (!Game.PlayerPed.IsInHeli) { return Task.FromResult(0); }

            Game.DisableControlThisFrame(32, Control.VehicleExit);

            if (!_input.IsJustBeingHeld(Control.VehicleExit))
            {
                return Task.FromResult(0);
            }

            if (!Game.PlayerPed.CurrentVehicle.IsEngineRunning)
            {
                Game.PlayerPed.Task.LeaveVehicle();
                return Task.FromResult(0);
            }

            if (Game.PlayerPed.CurrentVehicle.HeightAboveGround >= 4f)
            {
                _notificationService.Error("Land The Helicopter", "If you get out at this height you end up as flat as the earth.");
                return Task.FromResult(0);
            }

            Game.PlayerPed.Task.LeaveVehicle();
            return Task.FromResult(0);
        }
    }
}