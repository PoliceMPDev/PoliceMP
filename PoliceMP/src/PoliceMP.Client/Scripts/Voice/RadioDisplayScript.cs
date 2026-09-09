using CitizenFX.Core;
using CitizenFX.Core.Native;
using PoliceMP.Client.Services.Interfaces;
using PoliceMP.Core.Client.Commands.Interfaces;
using PoliceMP.Core.Client.Interface;
using PoliceMP.Core.Client.Scripts;
using PoliceMP.Core.Shared;
using PoliceMP.Shared.Enums;
using PoliceMP.Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace PoliceMP.Client.Scripts.Voice
{
    public class RadioDisplayScript : Script
    {
        private readonly ITickManager _ticks;
        private readonly ICommandManager _commands;
        private readonly ILogger<RadioDisplayScript> _logger;
        private readonly IPlayerService _players;
        private readonly IPermissionService _permissionService;
        private UserAces _userAces;

        private bool _shouldDisplay = false;

        public RadioDisplayScript(ITickManager ticks, ICommandManager commands, ILogger<RadioDisplayScript> logger, IPlayerService players, IPermissionService permissionService)
        {
            _ticks = ticks;
            _commands = commands;
            _logger = logger;
            _players = players;
            _permissionService = permissionService;

            _commands.Register("radiodisplay").WithHandler(() =>
            {
                if (_userAces.IsBandTwo || _userAces.IsAdmin || _userAces.IsDeveloper || _userAces.IsTierTwo || _userAces.IsModerator)
                {
                    _shouldDisplay = !_shouldDisplay;
                }
            });

            _commands.Register("radioflash").WithHandler(async () =>
            {
                if (_userAces.IsBandTwo || _userAces.IsAdmin || _userAces.IsDeveloper || _userAces.IsTierTwo || _userAces.IsModerator)
                {
                    _shouldDisplay = true;
                    await Delay(TimeSpan.FromSeconds(5));
                    _shouldDisplay = false;
                }
            });
        }

        protected override async Task OnStartAsync()
        {
            _userAces = await _permissionService.GetUserAces();
            _ticks.On(RadioDisplay);
        }

        private async Task RadioDisplay()
        {
            if (!_shouldDisplay) return;

            List<PlayerInfo> playersInChannel = new List<PlayerInfo>();
            var players = await _players.FetchAllRecentPlayerInfo();
            foreach (var player in players)
            {
                if (player.RadioChannel != Game.Player.State["radioChannel"])
                {
                    continue;
                }
                // Only show civs if the current player is a Moderator+
                if (player.ActiveBranch == UserBranch.Civ && !(_userAces.IsAdmin || _userAces.IsDeveloper || _userAces.IsTierTwo || _userAces.IsModerator))
                {
                    continue;
                }
                playersInChannel.Add(player);
            }

            /* UNCOMMENT THIS TO ADD SOME TEST DATA
            for (int i = 0; i < 100; i++)
            {
                var randInfo = new PlayerInfo();
                randInfo.RadioChannel = Game.Player.State["radioChannel"];
                randInfo.Name = i.ToString();
                randInfo.CallSign = "FO " + i.ToString();
                playersInChannel.Add(randInfo);
            }
            */

            if (playersInChannel.Count == 0)
            {
                return;
            }

            float startX = 0.90f; // Right side of the screen
            float startY = 0.05f; // Adjust this to set the vertical position
            float textScale = 0.2f;
            float spacingY = 0.03f;

            int columns = 45; // Number of columns
            //int maxRows = 10; // Maximum number of rows per column
            int totalPlayers = playersInChannel.Count;
            int rows = (int)Math.Ceiling((double)totalPlayers / columns);
            int playersPerColumn = (int)Math.Ceiling((double)totalPlayers / rows);

            for (int i = 0; i < totalPlayers; i++)
            {
                var player = playersInChannel[i];
                
                int[] nameColor = GetPlayerNameColor(player.ActiveBranch);

                var text = $"[{player.CallSign}] {player.Name}";
                API.SetTextFont(0);
                API.SetTextScale(textScale, textScale);
                API.SetTextColour(nameColor[0], nameColor[1], nameColor[2], 255);
                API.SetTextDropShadow();
                API.SetTextCentre(false);
                API.SetTextEntry("STRING");
                API.AddTextComponentString(text);

                int column = i / playersPerColumn;
                int row = i % playersPerColumn;
                float x = startX - (column * 0.1f); // Adjust column spacing
                float y = startY + (row * spacingY);

                API.DrawText(x, y);
            }
        }

        private int[] GetPlayerNameColor(UserBranch branch)
        {
            switch (branch)
            {
                case UserBranch.Police:
                    return new int[] { 66, 126, 245 };
                case UserBranch.Fire:
                    return new int[] { 179, 98, 23 };
                case UserBranch.Nhs:
                    return new int[] { 15, 189, 85 };
                case UserBranch.Civ:
                    return new int[] { 119, 46, 230 };
                case UserBranch.Highways:
                    return new int[] { 165, 176, 12 };
                case UserBranch.Control:
                    return new int[]{204, 0, 0};
                default:
                    return new int[] { 66, 126, 245 };
            }
        }
    }
}
