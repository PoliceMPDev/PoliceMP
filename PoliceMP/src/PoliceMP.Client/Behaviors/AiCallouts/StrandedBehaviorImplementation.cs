using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using PoliceMP.Client.Services.Interfaces;
using PoliceMP.Core.Client.Interface;
using PoliceMP.Core.Client.Scripts;
using PoliceMP.Shared.Behaviors.AiCallouts;

namespace PoliceMP.Client.Behaviors.AiCallouts
{
    public class StrandedBehaviorImplementation : PedBehavior<StrandedBehavior>
    {
        private readonly ISpeechService _speechService;
        private readonly Random _random;
        private int _tickCount;

        private readonly List<string> _strandedSpeeches = new List<string>
        {
            "Help, someone!",
            "I'm stranded!",
            "Where am I?",
            "What's going on? I'm confused...",
        };

        public StrandedBehaviorImplementation(ITickManager ticks, ISpeechService speechService) : base(ticks)
        {
            _speechService = speechService;
            _tickCount = int.MaxValue;
            _random = new Random();
        }

        protected override void Start()
        {
            ThePed.Task.WanderAround(ThePed.Position, 1f);
            ThePed.Task.StartScenario(new Random().Next(0, 2) == 0 ? "WORLD_HUMAN_STAND_MOBILE" : "WORLD_HUMAN_TOURIST_MOBILE", ThePed.Position);
        }

        protected override Task Think()
        {
            if (_tickCount >= 300)
            {
                _tickCount = 0;
                _speechService.Say(ThePed, _strandedSpeeches[_random.Next(_strandedSpeeches.Count)], 10000);
            }

            _tickCount++;
            return Task.FromResult(0);
        }
    }
}