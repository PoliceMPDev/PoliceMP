using System.Drawing;
using System.Threading.Tasks;
using CitizenFX.Core;
using CitizenFX.Core.Native;
using PoliceMP.Core.Client.Interface;
using PoliceMP.Core.Client.Scripts;
using PoliceMP.Shared.Behaviors.Test;

namespace PoliceMP.Client.Behaviors
{
    public class TestBehaviorImplementation : PedBehavior<TestBehavior>
    {
        public TestBehaviorImplementation(ITickManager ticks) : base(ticks)
        {
        }

        protected override Task Think()
        {
            return Task.FromResult(0);
        }

        protected override void OnDrawDebug()
        {
            base.OnDrawDebug();

            DrawDebugText($"Health: {ThePed.HealthFloat}");
            DrawDebugText($"FatallyInjured: {API.IsPedFatallyInjured(ThePed.Handle)}");
            DrawDebugText($"Dead: {API.IsEntityDead(ThePed.Handle)}");
        }
    }
}