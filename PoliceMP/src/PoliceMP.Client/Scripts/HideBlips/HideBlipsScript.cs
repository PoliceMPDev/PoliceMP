using System.Threading.Tasks;
using CitizenFX.Core;
using CitizenFX.Core.Native;
using CitizenFX.Core.UI;
using PoliceMP.Client.Services.Interfaces;
using PoliceMP.Core.Client.Commands.Interfaces;
using PoliceMP.Core.Client.Extensions;
using PoliceMP.Core.Client.Interface;
using PoliceMP.Core.Client.Scripts;
using PoliceMP.Core.Shared;
using PoliceMP.Shared.Constants.States;
using PoliceMP.Shared.Enums;

namespace PoliceMP.Client.Scripts.HideBlips
{
    public class HideBlipsScript : Script, IHideBlipsScript
    {
        private readonly ILogger<HideBlipsScript> _logger;
        private readonly ICommandManager _commands;
        private readonly IPermissionService _perms;
        private readonly ITickManager _ticks;

        public HideBlipsScript(ILogger<HideBlipsScript> logger, ICommandManager commands, IPermissionService perms,
            ITickManager ticks)
        {
            _logger = logger;
            _commands = commands;
            _perms = perms;
            _ticks = ticks;
        }

        protected override async Task OnStartAsync()
        {
            var aces = await _perms.GetUserAces();
            if (aces == null) return; // prevent console spam
            if (aces.IsAdmin || aces.IsModerator || aces.IsCivTrained || aces.IsDeveloper)
            {
                _commands.Register("hideblips").WithHandler(HideBlips);
                _commands.Register("showblips").WithHandler(ShowBlips);
            }

            _ticks.On(OnBlipsHideTick);
        }

        private Task OnBlipsHideTick()
        {
            if (_perms.CurrentUserRole == null) return Task.FromResult(0);

            if (Game.Player.State.Get<bool>(PlayerStates.HideBlipState))
            {
                Screen.ShowSubtitle("~m~[hidden]", 0);
                API.SetBlipColour(API.GetMainPlayerBlipId(), 40);
            }
            else
            {
                API.SetBlipColour(API.GetMainPlayerBlipId(), 0);
                if (_perms.CurrentUserRole.Branch == UserBranch.Civ) Screen.ShowSubtitle("~r~[NOT hidden]", 0);
            }

            return Task.FromResult(0);
        }

        private void HideBlips()
        {
            _logger.Debug("Hiding player blips...");
            Game.Player.State.Set(PlayerStates.HideBlipState, true, true);
        }

        private void ShowBlips()
        {
            _logger.Debug("Showing player blips...");
            Game.Player.State.Set(PlayerStates.HideBlipState, false, true);
        }

        public bool AreBlipsHiddenForPed(Player player)
        {
            return player.State.Get<bool>(PlayerStates.HideBlipState);
        }
    }
}