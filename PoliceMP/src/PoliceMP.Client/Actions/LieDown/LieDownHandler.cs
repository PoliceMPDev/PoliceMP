using CitizenFX.Core;
using PoliceMP.Client.Services.Interfaces;
using PoliceMP.Core.Client.Actions;
using PoliceMP.Core.Client.Extensions;
using PoliceMP.Shared.Constants.States;
using System.Threading.Tasks;
using CitizenFX.Core.Native;

namespace PoliceMP.Client.Actions.LieDown
{
    public class LieDownHandler : ActionHandler<LieDown>
    {
        private readonly ISpeechService _speech;

        public LieDownHandler(ISpeechService speech)
        {
            _speech = speech;
        }

        protected override async Task<bool> Handle(LieDown action)
        {
            _speech.Say(Game.PlayerPed, "Get on the ground now!");

            var sequence = new TaskSequence();
            await sequence.AddTask.PlayAnimation("amb@lo_res_idles@", "lying_face_down_lo_res_base", 8f, -8f, -1, AnimationFlags.Loop, 0f);
            sequence.Close();
            action.Target.AlwaysKeepTask = true;
            action.Target.Task.PerformSequence(sequence);

            action.Target.State.Set<bool>(PedStates.IsLyingDown, true);
            action.Target.State.Set<bool>(PedStates.IsKneeling, false);

            _speech.Do(action.Target, "Gets on the ground.");

            return true;
        }

        public static bool IsPedOnGround(Ped ped)
        {
            return API.IsEntityPlayingAnim(ped.Handle, "amb@lo_res_idles@", "lying_face_down_lo_res_base", 3);
        }
    }
}