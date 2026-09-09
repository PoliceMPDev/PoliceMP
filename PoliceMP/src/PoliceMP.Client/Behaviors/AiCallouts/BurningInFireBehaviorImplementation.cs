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
    public class BurningInFireBehaviorImplementation : PedBehavior<BurningInFireBehavior>
    {
        private readonly ISpeechService _speechService;
        private readonly Random _random;
        private int _tickCount;

        private readonly List<string> _fireSpeeches = new List<string>
        {
            "I'm burning!",
            "Help me!",
            "I'm on fire!"
        };

        public BurningInFireBehaviorImplementation(ITickManager ticks, ISpeechService speechService) : base(ticks)
        {
            _speechService = speechService;
            _tickCount = int.MaxValue;
            _random = new Random();
        }

        protected override void Start()
        {
            API.StartEntityFire(ThePed.Handle);
        }

        protected override Task Think()
        {
            ThePed.Health = ThePed.MaxHealth;

            if (!API.IsEntityOnFire(ThePed.Handle)) return Task.FromResult(0);
            if (_tickCount >= 100)
            {
                _tickCount = 0;
                _speechService.Say(ThePed, _fireSpeeches[_random.Next(_fireSpeeches.Count)], 15000);
            }

            _tickCount++;

            return Task.FromResult(0);
        }

        protected override void Stop()
        {
            API.StopEntityFire(ThePed.Handle);
        }
    }
}