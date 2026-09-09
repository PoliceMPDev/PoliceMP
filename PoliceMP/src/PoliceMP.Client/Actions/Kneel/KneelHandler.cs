using CitizenFX.Core;
using PoliceMP.Client.Services.Interfaces;
using PoliceMP.Core.Client.Actions;
using PoliceMP.Core.Client.Extensions;
using PoliceMP.Shared.Constants.States;
using System.Threading.Tasks;

namespace PoliceMP.Client.Actions.Kneel
{
    public class KneelHandler : ActionHandler<Kneel>
    {
        private readonly ISpeechService _speech;

        public KneelHandler(ISpeechService speech)
        {
            _speech = speech;
        }

        protected override async Task<bool> Handle(Kneel action)
        {
            _speech.Say(Game.PlayerPed, "Get on your knees!");

            var sequence = new TaskSequence();
            await sequence.AddTask.PlayAnimation("random@arrests", "kneeling_arrest_idle", 8f, -8f, -1, AnimationFlags.Loop, 0f);
            sequence.Close();
            action.Target.AlwaysKeepTask = true;
            action.Target.Task.PerformSequence(sequence);

            action.Target.State.Set<bool>(PedStates.IsKneeling, true);
            action.Target.State.Set<bool>(PedStates.IsLyingDown, false);

            _speech.Do(action.Target, "Gets down on their knees.");

            return true;
        }
    }
}