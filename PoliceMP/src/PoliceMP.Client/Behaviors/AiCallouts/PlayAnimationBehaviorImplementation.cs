using System;
using System.Threading.Tasks;
using CitizenFX.Core;
using PoliceMP.Client.Services.Interfaces;
using PoliceMP.Core.Client.Interface;
using PoliceMP.Core.Client.Scripts;
using PoliceMP.Shared.Behaviors.AiCallouts;

namespace PoliceMP.Client.Behaviors.AiCallouts
{
    public class PlayAnimationBehaviorImplementation : PedBehavior<PlayAnimationBehavior>
    {
        private readonly ISpeechService _speechService;
        private readonly Random _random;
        private int _tickCount;
        private int _animationTickCount;
        private int _ambientSoundTickCount;

        public PlayAnimationBehaviorImplementation(ITickManager ticks, ISpeechService speechService) : base(ticks)
        {
            _speechService = speechService;
            _tickCount = int.MaxValue;
            _animationTickCount = int.MaxValue;
            _ambientSoundTickCount = int.MaxValue;
            _random = new Random();
        }

        protected override void Start()
        {
            Blackboard.Set(bb => bb.AmbientTicksInterval, 100);
        }

        protected override Task Think()
        {
            var animationsTupleList = Blackboard.Get(bb => bb.AnimationsTupleList);

            if (Blackboard.Get(bb => bb.CurrentAnimationsTupleList).Count != animationsTupleList.Count)
            {
                Blackboard.Set(bb => bb.CurrentAnimationsTupleList, animationsTupleList);

                var animationTuple = animationsTupleList[_random.Next(animationsTupleList.Count)];

                var animationDict = animationTuple.Item1;
                if (animationDict == "") return Task.FromResult(0);

                var animationName = animationTuple.Item2;
                if (animationName == "") return Task.FromResult(0);
                ThePed.Task.PlayAnimation(animationDict, animationName);
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

            if (_animationTickCount >= 400)
            {
                var animationTuple = animationsTupleList[_random.Next(animationsTupleList.Count)];

                var animationDict = animationTuple.Item1;
                if (animationDict == "") return Task.FromResult(0);

                var animationName = animationTuple.Item2;
                if (animationName == "") return Task.FromResult(0);
                ThePed.Task.PlayAnimation(animationDict, animationName);
                _animationTickCount = 0;
            }

            _tickCount++;
            _animationTickCount++;
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