using System;
using System.Linq;
using System.Threading.Tasks;
using CitizenFX.Core.Native;
using PoliceMP.Client.Services.Interfaces;
using PoliceMP.Core.Client.Abstraction;
using PoliceMP.Core.Client.Interface;
using PoliceMP.Core.Client.Scripts;
using PoliceMP.Shared.Behaviors.AiCallouts;

namespace PoliceMP.Client.Behaviors.AiCallouts
{
    public class MisperBehaviorImplementation : PedBehavior<MisperBehavior>
    {
        private readonly ISpeechService _speech;
        // ReSharper disable once ConvertToPrimaryConstructor
        public MisperBehaviorImplementation(
            ITickManager ticks,
            ISpeechService speech
        ) : base(ticks)
        {
            _speech = speech;
        }

        private Random _random = new Random();
        private double _nextStateChangeTime;


        public override void Initialize()
        {
            Blackboard.Set(bb => bb.State, MisperState.Map);
            _nextStateChangeTime = ServerInfo.GetServerTime().TotalSeconds + 1000;
        }

        protected override async void Start()
        {
            await ChangeMisperState();
        }

        protected override async Task Think()
        {
            var currentTime = ServerInfo.GetServerTime().TotalSeconds;
            if (currentTime >= _nextStateChangeTime)
            {
                await ChangeMisperState();
                _nextStateChangeTime = currentTime + 1000;
            }

            switch (Blackboard.Get(bb => bb.State))
            {
                case MisperState.Searching:
                    _speech.Say(ThePed, "Where the hell am I?");
                    break;
                case MisperState.Map:
                    _speech.Say(ThePed, "Where is Paleto Police Station on 'ere?");
                    break;
                case MisperState.Sitting:
                    _speech.Say(ThePed, "I need to take a break a sec.");
                    break;
            }
        }

        private async Task ChangeMisperState()
        {
            var states = Enum.GetValues(typeof(MisperState)).Cast<MisperState>().ToList();
            var newState = states[_random.Next(states.Count)];
            Blackboard.Set(bb => bb.State, newState);

            ThePed.Task.ClearAll();

            switch (newState)
            {
                case MisperState.Searching:
                    ThePed.Task.StartScenario("WORLD_HUMAN_BINOCULARS", ThePed.Position);
                    break;
                case MisperState.Map:
                    ThePed.Task.StartScenario("WORLD_HUMAN_TOURIST_MAP", ThePed.Position);
                    break;
                case MisperState.Wandering:
                    API.TaskWanderStandard(ThePed.Handle, 20f, 10);
                    break;
                case MisperState.Sitting:
                    ThePed.Task.StartScenario("WORLD_HUMAN_PICNIC", ThePed.Position);
                    break;
                case MisperState.LayingDown:
                    ThePed.Task.StartScenario("WORLD_HUMAN_SUNBATHE", ThePed.Position);
                    break;
            }
        }
    }
}