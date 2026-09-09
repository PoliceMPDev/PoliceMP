using System.Threading.Tasks;
using CitizenFX.Core;
using PoliceMP.Core.Client.Extensions;
using PoliceMP.Core.Client.Interface;
using PoliceMP.Core.Client.Scripts;
using PoliceMP.Shared.Behaviors.AiCallouts;

namespace PoliceMP.Client.Behaviors.AiCallouts
{
    public class AgroAtEveryoneBehaviorImplementation : PedBehavior<AgroAtEveryoneBehavior>
    {
        public AgroAtEveryoneBehaviorImplementation(ITickManager ticks) : base(ticks)
        {
        }

        protected override void Start()
        {
            ThePed.Task.ClearAll();
        }

        protected override Task Think()
        {
            if (ThePed.GetActiveTasks().Length == 0)
                ThePed.Task.ShootAt(ThePed.Position + ThePed.ForwardVector * 10f, 1000, FiringPattern.FullAuto);
            return Task.FromResult(0);
        }
    }
}