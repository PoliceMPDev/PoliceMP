using CitizenFX.Core;
using PoliceMP.Client.Services.Interfaces;
using PoliceMP.Core.Client.Actions;
using PoliceMP.Core.Client.Extensions;
using PoliceMP.Shared.Constants.States;
using System.Threading.Tasks;

namespace PoliceMP.Client.Actions.StandUp
{
    public class StandUpHandler : ActionHandler<StandUp>
    {
        private readonly ISpeechService _speech;

        public StandUpHandler(ISpeechService speech)
        {
            _speech = speech;
        }

        protected override async Task<bool> Handle(StandUp action)
        {
            _speech.Say(Game.PlayerPed, "Get up off the ground!");

            var sequence = new TaskSequence();
            await sequence.AddTask.PlayAnimation("random@arrests", "kneeling_arrest_get_up", 8f, -8f, -1, AnimationFlags.None, 0f);
            await sequence.AddTask.PlayAnimation("random@mugging5", "ig_2_guy_handsup_loop", 8f, -8f, -1, AnimationFlags.Loop, 0f);
            sequence.Close();
            action.Target.AlwaysKeepTask = true;
            action.Target.Task.PerformSequence(sequence);

            action.Target.State.Set<bool>(PedStates.IsKneeling, false);
            action.Target.State.Set<bool>(PedStates.IsLyingDown, false);

            _speech.Do(action.Target, "Stands up.");

            await Delay(2000);
            await action.Target.StandStillFacingPlayer();
            return true;
        }
    }
}