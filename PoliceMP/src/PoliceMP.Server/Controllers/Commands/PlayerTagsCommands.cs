using System;
using System.Collections.Generic;
using System.Text;
using CitizenFX.Core;
using PoliceMP.Core.Server.Commands.Interfaces;
using PoliceMP.Core.Server.Extensions;
using PoliceMP.Core.Server.Networking;
using PoliceMP.Server.Extensions;
using PoliceMP.Shared.Constants.States;
using PoliceMP.Shared.Enums;

namespace PoliceMP.Server.Controllers.Commands
{
    public class PlayerTagsCommands : Controller
    {
        public PlayerTagsCommands(ICommandManager commands)
        {
            commands.Register("playertags.togglemod")
                .Restrict()
                .WithHandler(ToggleMod);
        }

        private void ToggleMod(Player player)
        {
            var curValue = player.State.Get<bool>(PlayerStates.PlayerTags.ModOverride);
            player.State.Set(PlayerStates.PlayerTags.ModOverride, !curValue);
        }
    }
}
