using System.Threading.Tasks;
using CitizenFX.Core.Native;
using PoliceMP.Client.Services.Interfaces;
using PoliceMP.Core.Client.Interface;
using PoliceMP.Core.Client.Scripts;
using PoliceMP.Core.Shared;
using PoliceMP.Shared.Behaviors.Cuff;
namespace PoliceMP.Client.Behaviors
{
    public class CuffedBehaviourImplementation : PedBehavior<CuffedBehaviour>
    {
        private readonly ILogger<CuffedBehaviourImplementation> _logger;
        private readonly IAnimationService _anims;

        public CuffedBehaviourImplementation(
            ITickManager ticks,
            ILogger<CuffedBehaviourImplementation> logger,
            IAnimationService anims
        ) : base(ticks)
        {
            _logger = logger;
            _anims = anims;
        }
        
        protected override async Task Think()
        {
            if (!API.IsEntityPlayingAnim(ThePed.Handle, "mp_arresting", "idle", 3))
                await _anims.Play(ThePed, "mp_arresting", "idle", 8f, flag: 50, duration: -1);
            
            API.SetPedAlternateWalkAnim(
                ThePed.Handle,
                "mp_arresting",
                "walk",
                8.0f,
                true
            );
            API.SetPedAlternateMovementAnim(
                ThePed.Handle,
                0, // idle
                "mp_arresting",
                "idle",
                8.0f,
                true
            );
            API.SetPedAlternateMovementAnim(
                ThePed.Handle,
                1, // walk
                "mp_arresting",
                "walk",
                8.0f,
                true
            );
            API.SetPedAlternateMovementAnim(
                ThePed.Handle,
                2, // running
                "mp_arresting",
                "sprint",
                8.0f,
                true
            );
            
            await Script.Delay(100);
        }
    }
}