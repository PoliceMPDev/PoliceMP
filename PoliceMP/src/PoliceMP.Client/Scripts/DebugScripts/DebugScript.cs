using System.Threading.Tasks;
using PoliceMP.Client.Services.Interfaces;
using PoliceMP.Client.Utils.CameraUtils;
using PoliceMP.Core.Client;
using PoliceMP.Core.Client.Commands.Interfaces;
using PoliceMP.Core.Client.Communications.Interfaces;
using PoliceMP.Core.Client.Interface;
using PoliceMP.Core.Client.Scripts;
using PoliceMP.Shared.Constants;

namespace PoliceMP.Client.Scripts.DebugScripts
{
    public class DebugScript : Script
    {
        private readonly ICommandManager _commands;
        private readonly ILegacyClientCommunicationsManager _legacyComms;
        private readonly INotificationService _notifications;
        private readonly ITickManager _ticks;
        private SpringArmCamera _camera = null;

        public DebugScript(ICommandManager commands,
            ILegacyClientCommunicationsManager legacyComms,
            INotificationService notifications, ITickManager ticks)
        {
            _commands = commands;
            _legacyComms = legacyComms;
            _notifications = notifications;
            _ticks = ticks;


            //_commands.Register("testcamera").WithHandler(Handler);
        }

        // private void Handler()
        // {
        //     _camera = new SpringArmCamera(_ticks, Game.PlayerPed.Position, new Vector3(-90f, 0f, 0f), 15f, 1f,
        //         raycastCheck: false, smoothMoveModifier: 3f);
        //     _camera.AttachTo(Game.PlayerPed);
        //     _camera.Enable();
        // }

        protected override Task OnStartAsync()
        {
            _legacyComms.On(ClientEvents.ToggleDebug, Toggle);
            return Task.FromResult(0);
        }

        private void Toggle()
        {
            DebugUtils.DebugEnabled = !DebugUtils.DebugEnabled;

            var message = DebugUtils.DebugEnabled ? "Debug Enabled!" : "Debug Disabled!";
            _notifications.Info(message, message);
        }
    }
}