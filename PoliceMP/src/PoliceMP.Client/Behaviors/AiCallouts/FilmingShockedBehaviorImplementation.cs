using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CitizenFX.Core.Native;
using PoliceMP.Client.Services.Interfaces;
using PoliceMP.Core.Client.Interface;
using PoliceMP.Core.Client.Scripts;
using PoliceMP.Shared.Behaviors.AiCallouts;

namespace PoliceMP.Client.Behaviors.AiCallouts
{
    public class FilmingShockedBehaviorImplementation : PedBehavior<FilmingShockedBehavior>
    {
        private readonly ISpeechService _speechService;
        private readonly Random _random;
        private int _tickCount;

        public FilmingShockedBehaviorImplementation(ITickManager ticks, ISpeechService speechService) : base(ticks)
        {
            _speechService = speechService;
            _tickCount = int.MaxValue;
            _random = new Random();
        }

        private readonly List<string> _shockedSpeeches = new List<string>
        {
            "What's going on here?!",
            "Holy cow!!",
            "I can't believe this!",
            "This is insane!",
            "What just happened??",
        };

        protected override void Start()
        {
            ThePed.Task.StartScenario("WORLD_HUMAN_MOBILE_FILM_SHOCKING", ThePed.Position);
        }

        protected override Task Think()
        {
            if (_tickCount >= 300)
            {
                _tickCount = 0;
                _speechService.Say(ThePed, _shockedSpeeches[_random.Next(_shockedSpeeches.Count)], 10000);
            }

            _tickCount++;
            return Task.FromResult(0);
        }
    }
}