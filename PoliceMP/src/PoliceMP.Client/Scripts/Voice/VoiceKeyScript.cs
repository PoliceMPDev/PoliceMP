using System;
using System.Collections.Generic;
using System.Linq;
using CitizenFX.Core;
using CitizenFX.Core.Native;
using PoliceMP.Core.Client;
using PoliceMP.Core.Client.Communications.Interfaces;
using PoliceMP.Core.Client.Interface;
using PoliceMP.Core.Shared;
using PoliceMP.Shared.Constants;
using System.Threading.Tasks;
using System.Timers;
using CitizenFX.Core.UI;
using PoliceMP.Client.Services;
using PoliceMP.Client.Services.Interfaces;
using PoliceMP.Core.Client.Commands.Interfaces;
using PoliceMP.Core.Client.Communications;
using PoliceMP.Core.Client.Scripts;
using PoliceMP.Core.Shared.Communications.Interfaces;
using PoliceMP.Shared.Models;
using PoliceMP.Shared.Enums;

namespace PoliceMP.Client.Scripts.Voice
{
    public class VoiceKeyScript : Script
    {
        private readonly ILegacyClientCommunicationsManager _comms;
        private readonly ILogger<VoiceKeyScript> _logger;
        private readonly ITickManager _ticks;
        private readonly IPlayerService _playerService;
        private readonly ICommandManager _commands;
        private readonly IGameInputManager _gameInputManager;

        public VoiceKeyScript(ILegacyClientCommunicationsManager comms,
            ILogger<VoiceKeyScript> logger,
            ITickManager ticks,
            IFiveEventManager fiveEventManager,
            IPlayerService playerService, ICommandManager commands, IGameInputManager gameInputManager)
        {
            _comms = comms;
            _logger = logger;
            _ticks = ticks;
            _playerService = playerService;
            _commands = commands;
            _gameInputManager = gameInputManager;

            _ticks.On(CheckRadioKey);

            fiveEventManager.On<int, bool>("pma-voice:setTalkingOnRadio", RadioTalkToggle);
        }

        DateTime _lastPressed = DateTime.Now;
        private Task CheckRadioKey()
        {
            if (!API.HasModelLoaded((uint)API.GetHashKey("mdx_sc20")))
            {
                API.RequestModel((uint)API.GetHashKey("mdx_sc20"));
                return Task.FromResult(0);
            }

            TimeSpan timeDifference = DateTime.Now - _lastPressed;

            if (timeDifference <= TimeSpan.FromSeconds(5))
            {
                return Task.FromResult(0);
            }

            if (_gameInputManager.IsPressed(Control.Sprint) && _gameInputManager.IsPressed((Control)289))
            {
                _lastPressed = DateTime.Now;
                API.ExecuteCommand("radio1");
            }
            return Task.FromResult(0);
        }

        private List<int> TalkingServerIDs = new List<int>();
        private List<PlayerInfo> Players = new List<PlayerInfo>();
        private bool WhosTalking = true;
        private Task RadioTalkToggle(int ServerID, bool toggle)
        {
            lock (TalkingServerIDs)
            {
                if (toggle)
                {
                    TalkingServerIDs.Add(ServerID);
                }
                else
                {
                    if (!TalkingServerIDs.Contains(ServerID)) return Task.FromResult(0);
                    TalkingServerIDs.Remove(ServerID);
                }
            }
            return Task.FromResult(0);
        }

        protected override Task OnStartAsync()
        {
            _ticks.On(TalkingDisplay);
            _ticks.On(RetrievePlayers);
            
            _commands.Register("radiochatdisplay").WithHandler(ToggleWhosTalking);

            API.RequestModel((uint)API.GetHashKey("mdx_sc20"));
            
            return Task.FromResult(0);
        }

        private void ToggleWhosTalking()
        {
            WhosTalking = !WhosTalking;
            lock (TalkingServerIDs)
            {
                TalkingServerIDs.Clear();
            }
        }
        
        private async Task RetrievePlayers()
        {
            Players = await _playerService.FetchAllRecentPlayerInfo();
            await Delay(TimeSpan.FromSeconds(15));
        }

        private async Task TalkingDisplay()
        {
            API.DistantCopCarSirens(false);

            if (!WhosTalking) return;
            
            bool currentlyTalking = false;
            string outputText = "";
            
            foreach (int id in TalkingServerIDs.ToArray())
            {
                if (!currentlyTalking)
                {
                    outputText += "~s~Currently Talking: ~n~";
                    currentlyTalking = true;
                }

                var playerInfo = Players.FirstOrDefault(x => x.ServerHandle == id);
                if(playerInfo == null) continue;
                
                var name = $"{playerInfo.Name} [{playerInfo.Index}]";

                if (!string.IsNullOrEmpty(playerInfo.CallSign))
                {
                    if (playerInfo.ActiveBranch == UserBranch.Control)
					{
                        name = $"~r~[{playerInfo.CallSign}]~r~ {playerInfo.Name}";
					}
                    else
					{
                        name = $"~p~[{playerInfo.CallSign}]~b~ {playerInfo.Name}";
					}
                }
                
                outputText += name + "~n~";
            }
            DrawTextOnScreen(outputText, 0.5f, 0.00f, 0.5f, Alignment.Center, 6, false);
        }
        
        private static void DrawTextOnScreen(string text, float xPosition, float yPosition, float size, Alignment justification, int font, bool disableTextOutline)
        {
            API.SetTextFont(font);
            API.SetTextScale(1.0f, size);
            if (justification == Alignment.Right)
            {
                API.SetTextWrap(0f, xPosition);
            }
            API.SetTextJustification((int)justification);
            if (!disableTextOutline) { API.SetTextOutline(); }
            API.BeginTextCommandDisplayText("STRING");
            API.AddTextComponentSubstringPlayerName(text);
            API.EndTextCommandDisplayText(xPosition, yPosition);
        }
    }
}