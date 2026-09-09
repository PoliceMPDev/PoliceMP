using System;
using System.Drawing.Imaging;
using System.Threading.Tasks;
using CitizenFX.Core;
using CitizenFX.Core.Native;
using PoliceMP.Core.Client.Abstraction;
using PoliceMP.Core.Client.Extensions;
using PoliceMP.Core.Client.Interface;
using PoliceMP.Core.Client.Scripts;
using PoliceMP.Core.Shared.Models;
using PoliceMP.Shared.Behaviors.AiCallouts;

namespace PoliceMP.Client.Behaviors.AiCallouts
{
    public class WanderInRadiusImplementation : PedBehavior<WanderingRadiusBehaviour>
    {
        private Random _random = new Random();

        public WanderInRadiusImplementation(ITickManager ticks) : base(ticks)
        {
        }

        public override void Initialize()
        {
            Blackboard.Set(bb => bb.Radius, 5f);
        }

        protected override Task Think()
        {
            var position = Blackboard.Get(bb => bb.Position).ToCitizenVector3();
            var radius = Blackboard.Get(bb => bb.Radius);
            var lastMoveTime = Blackboard.Get(bb => bb.LastMoveTime);
            var serverTime = ServerInfo.GetServerTime().TotalMilliseconds;

            if (serverTime - lastMoveTime > 5000)
            {
                var newposition = GetRandomPosition(position, radius);
                ThePed.Task.GoTo(newposition);
                Blackboard.Set(bb => bb.LastMoveTime, serverTime);
            }


            return Task.FromResult(0);
        }

        Vector3 GetRandomPosition(Vector3 position, float radius)
        {
            var x = (float)_random.NextDouble() * radius * 2 - radius;
            var y = (float)_random.NextDouble() * radius * 2 - radius;

            return new Vector3(x, y, position.Z);
        }

        protected override void OnDrawDebug()
        {
            var position = Blackboard.Get(bb => bb.Position).ToCitizenVector3();
            var radius = Blackboard.Get(bb => bb.Radius);

            base.OnDrawDebug();
            World.DrawMarker(MarkerType.DebugSphere, position, Vector3.Zero, Vector3.Zero, Vector3.One * radius,
                System.Drawing.Color.FromArgb(80, 80, 80, 255));
            DrawDebugText("Forbs is a cunt!");
        }
    }
}