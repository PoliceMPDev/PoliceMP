using CitizenFX.Core;
using PoliceMP.Client.Services.Interfaces;
using PoliceMP.Core.Client.Actions;
using PoliceMP.Core.Shared;
using System.Threading.Tasks;
using CitizenFX.Core.Native;
using PoliceMP.Core.Client.Abstraction;
using PoliceMP.Core.Client.Actions.Interfaces;

namespace PoliceMP.Client.Actions.AskDriverToStepOut
{
    public class AskPedToStepOutHandler : ActionHandler<AskPedToStepOut>
    {
        private readonly ILogger<AskPedToStepOutHandler> _logger;
        private readonly ISpeechService _speech;
        private readonly IAnimationService _anims;
        private readonly IActionManager _actions;

        public AskPedToStepOutHandler(ILogger<AskPedToStepOutHandler> logger, 
            ISpeechService speech, 
            IAnimationService anims,
            IActionManager actions)
        {
            _logger = logger;
            _speech = speech;
            _anims = anims;
            _actions = actions;
        }

        protected override async Task<bool> Handle(AskPedToStepOut action)
        {
            var ped = action.Ped;
            ped.Task.ClearAll();
            ped.BlockPermanentEvents = false;

            _logger.Debug($"Asking ped {ped?.Handle} to step out of car!");

            if (!ped.IsInVehicle())
                return true;

            var speech = action.Aggressive
                ? "GET OUT THE VEHICLE, NOW!!!"
                : "Could you step out of the vehicle for me please?";

            _speech.Say(Game.PlayerPed, speech);

            if (action.Aggressive)
            {
                API.AddShockingEventForEntity((int) ShockingEventType.EventAcquaintancePedWanted, action.Ped.Handle,
                    10000);
            }

            var sequence = new TaskSequence();
            sequence.AddTask.LeaveVehicle();
            sequence.AddTask.TurnTo(Game.PlayerPed);
            sequence.AddTask.LookAt(Game.PlayerPed);
            sequence.Close();

            ped.AlwaysKeepTask = true;
            ped.BlockPermanentEvents = true;
            ped.Task.PerformSequence(sequence);

            if (ped.TaskSequenceProgress == -1)
            {
                await Delay(0);
                return true;
            }

            while (ped.TaskSequenceProgress < 0)
            {
                _logger.Debug($"{ped.TaskSequenceProgress} == {sequence.Count}");
                if (action.HandsUp && !API.IsEntityPlayingAnim(ped.Handle, "busted", "idle_b", 3))
                    await _anims.Play(ped, "random@mugging5", "ig_2_guy_handsup_loop");

                await Delay(0);
            }
            
            API.SetNetworkIdCanMigrate(ped.NetworkId, true);

            return true;
        }
    }
}