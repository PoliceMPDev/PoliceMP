using CitizenFX.Core;
using PoliceMP.Client.Services.Interfaces;
using PoliceMP.Core.Client.Actions;
using System.Threading.Tasks;
using PoliceMP.Core.Client.Scripts;

namespace PoliceMP.Client.Actions.WarnPed
{
    public class WarnPedHandler : ActionHandler<WarnPed>
    {
        private readonly IAnimationService _anims;
        private readonly ISpeechService _speech;

        public WarnPedHandler(IAnimationService anims, ISpeechService speech)
        {
            _anims = anims;
            _speech = speech;
        }

        protected override async Task<bool> Handle(WarnPed action)
        {
            _speech.Say(Game.PlayerPed, "I'm going to let you off with a warning. Don't do that again.");
            action.Subject.Task.TurnTo(action.Target);
            await Script.Delay(500);
            await _anims.GesturePoint(action.Subject);
            await Script.Delay(1000);
            await _anims.FacePalm(action.Target);
            _speech.Do(action.Target, "Facepalms.");
            return true;
        }
    }
}