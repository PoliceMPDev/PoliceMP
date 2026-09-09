using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;
using CitizenFX.Core;
using CitizenFX.Core.Native;
using PoliceMP.Client.Services.Interfaces;
using PoliceMP.Core.Server.Interfaces.Services;
using PoliceMP.Core.Server.Networking;
using PoliceMP.Core.Shared;
using PoliceMP.Core.Shared.Communications.Interfaces;
using PoliceMP.Server.Services;
using PoliceMP.Shared.Constants.States;
using PoliceMP.Shared.Enums;
using PoliceMP.Shared.Models;
using Enumerable = System.Linq.Enumerable;

namespace PoliceMP.Server.Controllers
{
    public class ChatController : Controller
    {

        private readonly ILogger<ChatController> _logger;
        private readonly INotificationService _notification;
        private readonly IPermissionService _permission;
        private readonly PlayerList _playerList;
        private Timer TenSecs = new Timer(10000);

        public ChatController(IFiveEventManager fiveEvent, ILogger<ChatController> logger, INotificationService notification, PlayerList playerList, IPermissionService permission)
        {
            _logger = logger;
            _notification = notification;
            _permission = permission;
            _playerList = playerList;

            fiveEvent.On<int, string, string>("chatMessage", OnChatAddMessage);
        }

        private async Task OnChatAddMessage(int pl, string author, string message)
        {

            API.CancelEvent();

            if (string.IsNullOrEmpty(message) || string.IsNullOrWhiteSpace(author))
            {
                return;
            }

            var player = _playerList.FirstOrDefault(p => p.Handle == pl.ToString());
            
            if (message[0] == '/')
            {
                return;
            }

            if (message.Length >= 1000)
            {
                _notification.Warning(player, "Chat", "Message not sent, Your message is over 1000 characters long");
                return;
            }

            //Player name has colour in it
            if (author.Contains('^') || author.Contains('~'))
            {
                _notification.Warning(player, "Chat", "Message not added to chat because you have attempted to use colour in your name. Please remove any ^ from your name.");
                return;
            }
            var aces = await _permission.GetUserAces(player);
            var role = await _permission.GetUserRole(player);

            bool displayCallsign = true;

            if (aces.IsDeveloper || aces.IsTierTwo)
            {
                author += "🔧";
            }
            else if (aces.IsModerator)
            {
                author += "🔨";
            }
            else if (role.Division == UserDivision.Rpu)
            {
                author += "🚔";
            }
            else if (role.Division == UserDivision.Cid)
            {
                author += "🔍";
            }
            else if (role.Division == UserDivision.Afo)
            {
                author += "🔫";
            }
            else if (role.Division == UserDivision.Dsu)
            {
                author += "🐕";
            }
            else if (role.Division == UserDivision.Tsg)
            {
                author += "🚐";
            }
            else if (role.Division == UserDivision.Npas)
            {
                author += "🚁";
            }
            else if (role.Division == UserDivision.HemsDoctor || role.Division == UserDivision.BeepDoctor)
            {
                author += "🩺";
            }
            else if (role.Branch == UserBranch.Nhs)
            {
                author += "🚑";
            }
            else if (role.Branch == UserBranch.Fire)
            {
                author += "🚒";
            }
            else if (role.Branch == UserBranch.Highways)
            {
                author += "🚧";
            }
            else if (role.Branch == UserBranch.Control)
            {
                author += "🎧";
            }
            else if (role.Branch == UserBranch.Civ)
            {
                author += "🎭";
            }
            else if (role.Division == UserDivision.Ert)
            {
                author += "👮‍♂️";
            }
            else if (aces.IsProDonator)
            {
                author += "🌟";
            }
            else if (aces.IsBasicDonator)
            {
                author += "⭐";
            }
            
            if (DateTime.Now.Hour is >= 7 and <= 10)
            {
                author += "🍳";
            }
            if (DateTime.Now.Hour is >= 1 and <= 4)
            {
                author += "🦉";
            }


            var callsign = "Set Callsign";
            var c = (string)player.State.Get(PlayerStates.CallSign);


            if ((aces.IsTierTwo || aces.IsAdmin || aces.IsDeveloper || aces.IsBandFour) && string.IsNullOrEmpty(c))
            {
                displayCallsign = false;
            }

            if (string.IsNullOrEmpty(c) || c.Contains("null"))
            {
                callsign = "Set Callsign";
            }
            else
            {
                callsign = c;
            }

            int[] nameColor;
            switch (role.Branch)
            {
                case UserBranch.Police:
                    author = !displayCallsign ? author : $"[{callsign}] " + author;
                    nameColor = new[] { 66, 126, 245 };
                    break;
                case UserBranch.Fire:
                    author = !displayCallsign ? author : $"[{callsign}] " + author;
                    nameColor = new[] { 179, 98, 23 };
                    break;
                case UserBranch.Nhs:
                    author = !displayCallsign ? author : $"[{callsign}] " + author;
                    nameColor = new[] { 15, 189, 85 };
                    break;
                case UserBranch.Civ:
                    author = author;
                    nameColor = new[] { 119, 46, 230 };
                    break;
                case UserBranch.Highways:
                    author = !displayCallsign ? author : $"[{callsign}] " + author;
                    nameColor = new[] { 165, 176, 12 };
                    break;
                case UserBranch.Control:
                    author = !displayCallsign ? author : $"[{callsign}] " + author;
                    nameColor = new[] { 204, 0, 0 };
                    break;
                default:
                    nameColor = new[] { 66, 126, 245 };
                    break;
            }
 
            SendMessage(author, message, nameColor);
        }


        private void SendMessage(string title, string text, int[] tagColor)
        {
            foreach (var player in _playerList)
            {
                player.TriggerEvent("chat:addMessage");
            }

            TriggerClientEvent("chat:addMessage", new
            {
                color = tagColor,
                multiline = true,
                args = new[] { title, text }
            });
        }
    }
}