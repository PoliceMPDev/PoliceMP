using System.Threading.Tasks;
using PoliceMP.Core.Client.Interface;
using PoliceMP.Core.Client.Scripts;
using PoliceMP.Shared.Behaviors.Jump;

namespace PoliceMP.Client.Behaviors
{
    public class JumpBehaviorImplementation : PedBehavior<JumpBehavior>
    {
        public JumpBehaviorImplementation(ITickManager ticks) : base(ticks)
        {
        }

        public override void Initialize()
        {
            Blackboard.Set(bb => bb.TimesJumped, 0);
        }

        protected override async Task Think()
        {
            if (!ThePed.IsJumping)
            {
                ThePed.Task.Jump();
                Blackboard.Set(bb => bb.TimesJumped, Blackboard.Get(bbb => bbb.TimesJumped) + 1);
            }

            await Script.Delay(100);
        }
    }
}