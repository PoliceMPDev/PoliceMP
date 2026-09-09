using CitizenFX.Core;
using CitizenFX.Core.Native;
using PoliceMP.Client.Services.Interfaces;
using PoliceMP.Core.Client.Actions;
using PoliceMP.Core.Client.Communications.Interfaces;
using PoliceMP.Core.Client.Extensions;
using PoliceMP.Core.Client.Interface;
using PoliceMP.Core.Shared;
using PoliceMP.Shared.Constants;
using System.Threading.Tasks;
using PoliceMP.Client.Utils;
using PoliceMP.Core.Client.Scripts;

namespace PoliceMP.Client.Actions.Grab
{
    public class GrabHandler : ActionHandler<Grab>
    {
        private Ped _grabbedPed;

        private readonly INotificationService _notifications;
        private readonly IAnimationService _anims;
        private readonly ITickManager _ticks;
        private readonly ILegacyClientCommunicationsManager _comms;
        private readonly ILogger<GrabHandler> _logger;
        private readonly ISpeechService _speech;
        private readonly ICommonFunctionsService _common;

        public GrabHandler(INotificationService notifications,
            IAnimationService anims,
            ITickManager ticks,
            ILegacyClientCommunicationsManager comms,
            ILogger<GrabHandler> logger,
            ISpeechService speech, ICommonFunctionsService common)
        {
            _notifications = notifications;
            _anims = anims;
            _ticks = ticks;
            _comms = comms;
            _logger = logger;
            _speech = speech;
            _common = common;
        }

        protected override async Task<bool> Handle(Grab action)
        {
            //_logger.Debug("Handling grab request...");

            if (_grabbedPed != null)
            {
                if (_grabbedPed.NetworkId != action.Target.NetworkId)
                {
                    _notifications.Error("Grab", "You are already grabbing a ped.");
                    return false;
                }

                return await Ungrab(action);
            }

            if (!action.OnlyUngrab)
                return await Grab(action);

            //_logger.Debug("Handling grab request... Failed!");

            return false;
        }

        private async Task<bool> Ungrab(Grab action)
        {
            _ticks.Off(GrabHandlerTick);

            await Delay(10);

            _grabbedPed.Detach();
            _grabbedPed.IsInvincible = false;

            _grabbedPed.Task.ClearAll();
            Game.PlayerPed.Task.ClearAll();

            _speech.Say(Game.PlayerPed, "Stay here.");

            API.SetEntityCollision(_grabbedPed.Handle, true, true);

            await _grabbedPed.StandStill();

            _comms.ToClient(ClientEvents.PedGrabbed, _grabbedPed, false);

            _grabbedPed = null;

            _speech.Do(action.Target, $"Stops being grabbed by {Game.Player.Name}.");

            await action.Target.StandStillFacingPlayer();

            return true;
        }

        private async Task<bool> Grab(Grab action)
        {
            _speech.Say(Game.PlayerPed, "Let me walk you over here...");

            var position = action.Target.GetOffsetPosition(new Vector3(0f, -1f, 0f));
            Game.PlayerPed.Task.GoTo(position);

            await Delay(1000);

            Game.PlayerPed.Task.AchieveHeading(action.Target.Heading);

            await Delay(1000);

            var timeout = Game.GameTime + 5000;

            action.Target.Task.ClearAllImmediately();
            action.Target.Task.ClearSecondary();
            await Delay(0);
            while (!action.Target.IsAttachedTo(Game.PlayerPed) && Game.GameTime <= timeout)
            {
                await Script.Delay(0);
                action.Target.AttachTo(Game.PlayerPed, new Vector3(-0.2f, 0.4f, 0f));

            }

            if (!action.Target.IsAttachedTo(Game.PlayerPed))
            {
                action.Target.Detach();
                return false;
            }

            await _anims.Play(Game.PlayerPed, "rcmnigel1d", "base_club_shoulder", 8f, flag: 50, duration: -1);

            action.Target.IsInvincible = true;
            action.Target.Task.StandStill(-1);

            _grabbedPed = action.Target;

            _speech.Do(action.Target, $"Gets grabbed by {Game.Player.Name}.");

            _ticks.On(GrabHandlerTick);

            _comms.ToClient(ClientEvents.PedGrabbed, _grabbedPed, true);
            _logger.Debug("Finished grabbing!");

            return true;
        }

        private async Task GrabHandlerTick()
        {
            if (_grabbedPed == null) return;

            if (!_common.RequestNetworkEntityControl(_grabbedPed.NetworkId, 10).Result)
            {
                _logger.Debug("Grabbing may of jobbied");
                _comms.ToClient(ClientEvents.PedGrabbed, _grabbedPed, false);
                _grabbedPed = null;
                return;
            }

            if (!_grabbedPed.IsAttachedTo(Game.PlayerPed))
                _grabbedPed.AttachTo(Game.PlayerPed, new Vector3(-0.2f, 0.4f, 0f));

            /*            
            if (!API.NetworkHasControlOfNetworkId(_grabbedPed.NetworkId))
            {
                API.NetworkRequestControlOfNetworkId(_grabbedPed.NetworkId);
            }
            */

            var posFront = _grabbedPed.GetOffsetPosition(new Vector3(20f, 0f, 0f));

            if (Game.PlayerPed.IsWalking)
                API.TaskGoStraightToCoord(_grabbedPed.Handle, posFront.X, posFront.Y, posFront.Z, 1f, -1,
                    Game.PlayerPed.Heading, 1f);
            else if (Game.PlayerPed.IsRunning)
                API.TaskGoStraightToCoord(_grabbedPed.Handle, posFront.X, posFront.Y, posFront.Z, 2f, -1,
                    Game.PlayerPed.Heading, 1f);
            else if (Game.PlayerPed.IsSprinting)
                API.TaskGoStraightToCoord(_grabbedPed.Handle, posFront.X, posFront.Y, posFront.Z, 3f, -1,
                    Game.PlayerPed.Heading, 1f);
            else
                _grabbedPed.Task.StandStill(-1);

            if (!API.IsEntityPlayingAnim(_grabbedPed.Handle, "mp_arresting", "idle", 3))
                await _grabbedPed.Task.PlayAnimation("mp_arresting", "idle", 8f, -8f, -1, (AnimationFlags)49, 0);
        }
    }
}