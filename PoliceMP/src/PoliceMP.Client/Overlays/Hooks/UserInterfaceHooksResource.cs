using System.Threading.Tasks;
using CitizenFX.Core;
using PoliceMP.Core.Client;
using PoliceMP.Core.Client.Interface;
using PoliceMP.Core.Client.Scripts;
using PoliceMP.Core.Shared;

namespace PoliceMP.Client.Overlays.Hooks
{

    public class UserInterfaceHooksResource : Script
    {
        private readonly ILogger<UserInterfaceHooksResource> _logger;
        private readonly INuiManager _nuiManager;
        private readonly ITickManager _ticks;
        private InputMode _inputMode;

        public UserInterfaceHooksResource(ILogger<UserInterfaceHooksResource> logger, INuiManager nuiManager, ITickManager ticks)
        {
            _logger = logger;
            _nuiManager = nuiManager;
            _ticks = ticks;
        }

        protected override Task OnStartAsync()
        {
            _inputMode = Game.CurrentInputMode;
            EmitInputMode();

            _ticks.On(OnTickAsync);
            return Task.FromResult(0);
        }

        protected Task OnTickAsync()
        {
            var currentInputMode = Game.CurrentInputMode;
            if (currentInputMode != _inputMode)
            {
                _inputMode = currentInputMode;
                EmitInputMode();
            }

            return Task.FromResult(0);
        }

        private void EmitInputMode() =>
            _nuiManager.Emit("SetInputMode", _inputMode);
    }
}
