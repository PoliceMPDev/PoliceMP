using CitizenFX.Core;
using PoliceMP.Client.Services.Interfaces;
using PoliceMP.Core.Client.Abstraction;
using PoliceMP.Core.Client.Communications.Interfaces;
using PoliceMP.Core.Client.Interface;
using PoliceMP.Core.Client.Options.Interfaces;
using PoliceMP.Core.Shared.Extensions;
using PoliceMP.Core.Shared.Models;
using PoliceMP.Shared.Constants;
using PoliceMP.Shared.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CitizenFX.Core.UI;

namespace PoliceMP.Client.Services
{
    public class SpeechService : ISpeechService
    {
        private readonly ITickManager _ticks;
        private readonly ILegacyClientCommunicationsManager _comms;
        private readonly List<SpeechData> _speechData = new List<SpeechData>();
        private readonly SpeechOptions _options;

        public SpeechService(ITickManager ticks,
            ILegacyClientCommunicationsManager comms,
            IOptionsManager optionsManager)
        {
            _ticks = ticks;
            _comms = comms;
            _options = optionsManager.Options.Speech;

            _comms.On<int, string, int>(ClientEvents.ReplicateSayPedSpeech, OnReplicateSay);
            _comms.On<int, string, int>(ClientEvents.ReplicateDoPedSpeech, OnReplicateDo);
        }

        public void Say(Ped ped, string speech, int durationMs, bool replicate = true)
        {
            if (ped == null) return;
            Add(ped, speech, durationMs, Color.White);

            if (!replicate) return;

            _comms.ToServer(ServerEvents.ReplicateSayPedSpeech, ped.NetworkId, speech, durationMs);
        }

        public void SayRandomGreeting(Ped ped, int durationMs, bool replicate = true)
        {
            string greeting = _options.Greetings.GetRandom();
            Say(ped, greeting, durationMs, replicate);
        }

        public void SayRandomFarewell(Ped ped, int durationMs, bool replicate = true)
        {
            string farewell = _options.Farewells.GetRandom();
            Say(ped, farewell, durationMs, replicate);
        }

        public void SayRandomInsult(Ped ped, int durationMs, bool replicate = true)
        {
            string insult = _options.Insults.GetRandom();
            Say(ped, insult, durationMs, replicate);
        }

        public void Do(Ped ped, string action, int durationMs, bool replicate = true)
        {
            if (ped == null) return;
            Add(ped, $"*{action}*", durationMs, Color.Purple);

            if (!replicate) return;

            _comms.ToServer(ServerEvents.ReplicateDoPedSpeech, ped.NetworkId, action, durationMs);
        }

        private void OnReplicateDo(int pedNetworkId, string action, int durationMs)
        {
            var ped = (Ped)Entity.FromNetworkId(pedNetworkId);
            if (ped == null) return;

            Do(ped, action, durationMs, false);
        }

        private void OnReplicateSay(int pedNetworkId, string speech, int durationMs)
        {
            var ped = (Ped)Entity.FromNetworkId(pedNetworkId);
            if (ped == null) return;

            Say(ped, speech, durationMs, false);
        }

        private void Add(Ped ped, string text, int durationMs, Color color)
        {
            bool shouldAddOnTick = !_speechData.Any();

            _speechData.RemoveAll(x => x.Ped.NetworkId == ped.NetworkId);

            if (Game.PlayerPed == ped)
            {
                Screen.ShowSubtitle($"~r~You: ~w~\"{text}\"", durationMs);
            }
            else
            {
                var textDraw = new PedTextDraw(ped, text, color);
                var speechData = new SpeechData(ped, textDraw, durationMs);
                _speechData.Add(speechData);
            }

            if (shouldAddOnTick)
            {
                _ticks.On(SpeechServiceTick);
            }
        }

        private Task SpeechServiceTick()
        {
            if (!_speechData.Any())
            {
                _ticks.Off(SpeechServiceTick);
                return Task.FromResult(0);
            }

            foreach (var speechData in _speechData.ToList())
            {
                if (Game.GameTime - speechData.TimeStarted > speechData.DurationMs
                    || speechData.Ped?.Exists() == false)
                {
                    _speechData.Remove(speechData);
                    continue;
                }

                if(Game.PlayerPed.Position.DistanceToSquared(speechData.Ped.Position) <= _options.MaxDistance)
                {
                    speechData.PedTextDraw.Draw();
                }
            }

            return Task.FromResult(0);
        }

        private class SpeechData
        {
            public Ped Ped { get; }
            public PedTextDraw PedTextDraw { get; }
            public int TimeStarted { get; }
            public int DurationMs { get; }

            public SpeechData(Ped ped, PedTextDraw pedTextDraw, int durationMs)
            {
                Ped = ped;
                PedTextDraw = pedTextDraw;
                DurationMs = durationMs;
                TimeStarted = Game.GameTime;
            }
        }
    }
}