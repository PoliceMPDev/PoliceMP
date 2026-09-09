using System;
using System.Threading.Tasks;
using CitizenFX.Core;
using CitizenFX.Core.Native;
using PoliceMP.Core.Client.Abstraction;
using PoliceMP.Core.Client.Extensions;
using PoliceMP.Core.Client.Interface;
using PoliceMP.Core.Client.Scripts;
using PoliceMP.Shared.Behaviors.AiCallouts;

namespace PoliceMP.Client.Behaviors.AiCallouts
{
    public class HandsUpBehaviourImplementation : PedBehavior<DrunkBehavior>
    {
        // ReSharper disable once ConvertToPrimaryConstructor
        public HandsUpBehaviourImplementation(ITickManager ticks) : base(ticks)
        {
        }

        public override void Initialize()
        {
            Blackboard.Set(bb => bb.StartTime, ServerInfo.GetServerTime().TotalSeconds);
            Blackboard.Set(bb => bb.State, DrunkState.Slightly);
        }

        protected override async void Start()
        {
            API.RequestClipSet("random@arrests");

            while (!API.HasAnimSetLoaded("random@arrests"))
            {
                await BaseScript.Delay(100);
            }

            API.SetPedToRagdoll(ThePed.Handle, 2000, 2000, 0, true, true, false);
            ThePed.Task.StartScenario(new Random().Next(0, 2) == 0 ? "idle_c" : "idle_2_hands_up",
                ThePed.Position);
            API.TaskWanderInArea(ThePed.Handle, ThePed.Position.X, ThePed.Position.Y, ThePed.Position.Z, 100f, 10f, 5f);

            Blackboard.Set(bb => bb.InitialPosition, ThePed.Position.ToPmpVector3());
        }

        protected override async Task Think()
        {
            var pos = Blackboard.Get(bb => bb.InitialPosition);
            API.SetPedMovementClipset(ThePed.Handle, "random@arrests", 1f);

            if (ServerInfo.GetServerTime().TotalSeconds - Blackboard.Get(bb => bb.StartTime) > 1000 &&
                Blackboard.Get(bb => bb.State) == DrunkState.Slightly)
            {
                Blackboard.Set(bb => bb.State, DrunkState.Buzzed);

                API.SetPedToRagdoll(ThePed.Handle, 3000, 3000, 0, true, true, false);
            }
            else if (ServerInfo.GetServerTime().TotalSeconds - Blackboard.Get(bb => bb.StartTime) > 2000 &&
                     Blackboard.Get(bb => bb.State) == DrunkState.Buzzed)
            {
                Blackboard.Set(bb => bb.State, DrunkState.Very);

                API.SetPedToRagdoll(ThePed.Handle, 5000, 5000, 0, true, true, false);
                API.TaskWanderStandard(ThePed.Handle, 100f, 10);
            }
            else if (ServerInfo.GetServerTime().TotalSeconds - Blackboard.Get(bb => bb.StartTime) > 7000 &&
                     Blackboard.Get(bb => bb.State) == DrunkState.Very)
            {
                if (Blackboard.Get(bb => bb.WillBecomeAggressive))
                {
                    // AGGRESSIVE STUFF HERE
                }
            }
        }
    }
}