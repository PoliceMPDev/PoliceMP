using System.Threading.Tasks;
using CitizenFX.Core;
using PoliceMP.Core.Client.Interface;

namespace PoliceMP.Core.Client.Scripts
{
    public abstract class EntityScript : Script
    {
        private ITickManager _ticks;
        public Entity AttachedEntity { get; private set; }
        public bool IsAlive { get; private set; }

        internal void Initialise(ITickManager ticks, Entity entity)
        {
            _ticks = ticks;
            _ticks.On(Heartbeat);
            AttachedEntity = entity;
        }

        protected override Task OnStartAsync()
        {
            IsAlive = true;
            _ticks.On(Think);
            return Task.FromResult(0);
        }

        private Task Heartbeat()
        {
            if (AttachedEntity == null ||
                !AttachedEntity.Exists())
            {
                IsAlive = false;
                _ticks.Off(Think);
            }
            return Task.FromResult(0);
        }

        protected abstract Task Think();

    }
}