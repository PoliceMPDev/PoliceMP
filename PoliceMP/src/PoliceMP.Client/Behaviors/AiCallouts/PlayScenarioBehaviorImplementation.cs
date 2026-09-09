using System;
using System.Threading.Tasks;
using CitizenFX.Core;
using PoliceMP.Client.Services.Interfaces;
using PoliceMP.Core.Client.Interface;
using PoliceMP.Core.Client.Scripts;
using PoliceMP.Shared.Behaviors.AiCallouts;

namespace PoliceMP.Client.Behaviors.AiCallouts
{
    public class PlayScenarioBehaviorImplementation : PedBehavior<PlayScenarioBehavior>
    {
        private readonly ISpeechService _speechService;
        private readonly Random _random;
        private int _tickCount;
        private int _ambientSoundTickCount;

        public PlayScenarioBehaviorImplementation(ITickManager ticks, ISpeechService speechService) : base(ticks)
        {
            _speechService = speechService;
            _tickCount = int.MaxValue;
            _ambientSoundTickCount = int.MaxValue;
            _random = new Random();
        }

        protected override void Start()
        {
            Blackboard.Set(bb => bb.AmbientTicksInterval, 100);
        }

        protected override Task Think()
        {
            var scenarioName = Blackboard.Get(bb => bb.ScenarioName);
            if (scenarioName == "") return Task.FromResult(0);


            if (Blackboard.Get(bb => bb.CurrentScenarioName) != scenarioName)
            {
                Blackboard.Set(bb => bb.CurrentScenarioName, scenarioName);
                ThePed.Task.StartScenario(scenarioName, ThePed.Position);
            }

            var ambientSounds = Blackboard.Get(bb => bb.AmbientSounds);
            if (Blackboard.Get(bb => bb.CurrentAmbientSounds) != ambientSounds && ambientSounds.Count > 0)
            {
                var ambientSound = ambientSounds[_random.Next(ambientSounds.Count)];
                Blackboard.Set(bb => bb.CurrentAmbientSounds, ambientSounds);
                ThePed.PlayAmbientSpeech(ambientSound);
            }

            var speechTicksInterval = Blackboard.Get(bb => bb.SpeechTicksInterval);
            if (speechTicksInterval == 0)
            {
                if (_tickCount > 1000)
                {
                    Speak();
                    _tickCount = 0;
                }

                return Task.FromResult(0);
            }

            if (_tickCount >= speechTicksInterval)
            {
                _tickCount = 0;
                Speak();
            }

            if (_ambientSoundTickCount >= Blackboard.Get(bb => bb.AmbientTicksInterval) && ambientSounds.Count > 0)
            {
                var ambientSound = ambientSounds[_random.Next(ambientSounds.Count)];
                ThePed.PlayAmbientSpeech(ambientSound);
                _ambientSoundTickCount = 0;
            }

            _tickCount++;
            _ambientSoundTickCount++;
            return Task.FromResult(0);
        }

        private void Speak()
        {
            if (ThePed.IsDead) return;

            var isInteracting = ThePed.State.Get("isInteracting") ?? false;
            if (isInteracting) return;

            var speeches = Blackboard.Get(bb => bb.Speeches);
            if (speeches.Count == 0) return;
            var speech = speeches[_random.Next(speeches.Count)];
            _speechService.Say(ThePed, speech, 10000);
        }
    }
}