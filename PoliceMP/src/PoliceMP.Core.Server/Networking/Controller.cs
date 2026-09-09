using System;
using System.Threading.Tasks;
using CitizenFX.Core;

namespace PoliceMP.Core.Server.Networking
{
    public abstract class Controller : BaseScript
    {
        public virtual Task Started() => Task.CompletedTask;
        public async Task TickAsync()
        {
            await ControllerTick();
        }

        protected virtual Task ControllerTick()
        {
            return Task.FromResult(0);
        }
        protected async Task Delay(int ms)
        {
            await BaseScript.Delay(ms);
        }

        protected async Task Delay(TimeSpan delay)
        {
            await BaseScript.Delay((int)delay.TotalMilliseconds);
        }
        
        
    }
}