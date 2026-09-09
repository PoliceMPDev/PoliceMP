using System.Threading.Tasks;
using CitizenFX.Core;
using CitizenFX.Core.Native;
using PoliceMP.Core.Client.Interface;
using PoliceMP.Core.Client.Scripts;
using PoliceMP.Shared.Behaviors.Writhe;

namespace PoliceMP.Client.Behaviors
{

    public class WritheBrainBehaviorImplementation : PedBehavior<WritheBrainBehavior>
    {
        public WritheBrainBehaviorImplementation(ITickManager ticks) 
            : base(ticks)
        {
        }
        
        public override void Initialize()
        {
            //while (Ped.IsRagdoll)
            //{
            //    await Script.Delay(0);
            //}

            //Ped.Task.ClearAllImmediately();
        }

        protected override Task Think()
        {
            if (!API.IsPedInWrithe(ThePed.Handle))
            {
                API.TaskWrithe(ThePed.Handle, Game.PlayerPed.Handle, 1, 1000);
            }

            return Task.FromResult(0);
        }

        
    }
}
