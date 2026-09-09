using CitizenFX.Core;
using CitizenFX.Core.Native;
using PoliceMP.Client.Services.Interfaces;
using PoliceMP.Core.Client.Actions;
using PoliceMP.Core.Client.Extensions;
using PoliceMP.Shared.Constants.States;
using System.Threading.Tasks;

namespace PoliceMP.Client.Actions.Follow
{
    public class FollowHandler : ActionHandler<Follow>
    {
        private readonly ISpeechService _speech;

        public FollowHandler(ISpeechService speech)
        {
            _speech = speech;
        }

        protected override async Task<bool> Handle(Follow action)
        {
            bool isFollowing = action.Target.State.Get<bool>(PedStates.IsFollowingPlayer);

            if (isFollowing)
            {
                _speech.Say(Game.PlayerPed, "Stay there!");
                API.RemovePedFromGroup(action.Target.Handle);

                // Optionally also:
                API.ClearPedTasks(action.Target.Handle);
                await action.Target.StandStillFacingPlayer();

                action.Target.State.Set<bool>(PedStates.IsFollowingPlayer, false);
                _speech.Do(action.Target, $"Stops following {Game.Player.Name}.");
                return true;
            }

            _speech.Say(Game.PlayerPed, "Come with me!");
            int group = API.GetPlayerGroup(action.Subject.Handle);

            action.Target.Task.ClearAll();
            action.Target.AlwaysKeepTask = false;

            API.SetPedAsGroupMember(action.Target.Handle, group);
            API.SetPedCanTeleportToGroupLeader(action.Target.Handle, group, true);

            action.Target.State.Set<bool>(PedStates.IsFollowingPlayer, true);
            _speech.Do(action.Target, $"Follows {Game.Player.Name}.");
            return true;
        }
    }
}