using CitizenFX.Core;
using PoliceMP.Client.Services.Interfaces;
using PoliceMP.Core.Client.Actions;
using PoliceMP.Core.Shared;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CitizenFX.Core.Native;

namespace PoliceMP.Client.Actions.HandsUp
{
    public class HandsUpHandler : ActionHandler<HandsUp>
    {
        private readonly INotificationService _notifications;
        private readonly ISpeechService _speech;

        private static Dictionary<string, string> _arrestAnims = new Dictionary<string, string>
        {
            {"random@mugging5", "ig_2_guy_handsup_loop"},
        };

        public HandsUpHandler(INotificationService notifications, ISpeechService speech)
        {
            _notifications = notifications;
            _speech = speech;
        }

        protected override async Task<bool> Handle(HandsUp action)
        {
            if (action.Target.IsCuffed)
            {
                _notifications.Warning("Hands Up", "You can't command the ped to put their hands up because they are cuffed.");
                return true;
            }

            _speech.Say(Game.PlayerPed, "Hands in the air!");

            var animKvp = _arrestAnims.ElementAt(AppRandom.Next(0, _arrestAnims.Count));

            var sequence = new TaskSequence();
            await sequence.AddTask.PlayAnimation(animKvp.Key, animKvp.Value, 8f, -8f, -1, AnimationFlags.Loop, 0f);
            sequence.Close();
            action.Target.AlwaysKeepTask = true;
            action.Target.Task.PerformSequence(sequence);

            _speech.Do(action.Target, "Puts hands up.");

            return true;
        }
        
        // Quick fix -- this will move to PedController when State: HandsUp
        public static bool DoesPedHaveHandsUp(Ped entity)
        {
            foreach (var kvp in _arrestAnims)
            {
                if (API.IsEntityPlayingAnim(entity.Handle, kvp.Key, kvp.Value, 3))
                    return true;
            }

            return false;
        }
    }
}