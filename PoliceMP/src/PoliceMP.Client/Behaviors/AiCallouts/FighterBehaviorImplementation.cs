using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CitizenFX.Core;
using CitizenFX.Core.Native;
using PoliceMP.Core.Client.Abstraction;
using PoliceMP.Core.Client.Extensions;
using PoliceMP.Core.Client.Interface;
using PoliceMP.Core.Client.Scripts;
using PoliceMP.Shared.Behaviors.AiCallouts;
using PoliceMP.Shared.Models;

namespace PoliceMP.Client.Behaviors.AiCallouts
{
    public class FighterBehaviorImplementation : PedBehavior<FighterBehavior>
    {
        // ReSharper disable once ConvertToPrimaryConstructor
        public FighterBehaviorImplementation(ITickManager ticks) : base(ticks)
        {
        }

        private Random _random = new Random();
        private double _nextStateChangeTime;


        public override void Initialize()
        {
            Blackboard.Set(bb => bb.StartTime, ServerInfo.GetServerTime().TotalSeconds);
            Blackboard.Set(bb => bb.State, FighterState.Aggresive);
        }

        protected override async void Start()
        {
            API.RequestAnimDict("switch@michael@argue_with_amanda");
            API.RequestClipSet("MOVE_F@TOUGH_GUY@");
        }

        protected override async Task Think()
        {
            var pos = Blackboard.Get(bb => bb.InitialPosition);
            API.SetPedMovementClipset(ThePed.Handle, "MOVE_F@TOUGH_GUY@", 1f);

            if (Blackboard.Get(bb => bb.State) == FighterState.Aggresive)
            {
                ThePed.Task.StartScenario("WORLD_HUMAN_STAND_IMPATIENT_CLUBHOUSE", ThePed.Position);
            }
            if (ServerInfo.GetServerTime().TotalSeconds - Blackboard.Get(bb => bb.StartTime) > 1000 &&
                Blackboard.Get(bb => bb.State) == FighterState.Aggresive)
            {
                Blackboard.Set(bb => bb.State, FighterState.Threatening);
                ThePed.Task.PlayAnimation("switch@michael@argue_with_amanda", "argue_with_amanda_loop_michael", 1f, -1, AnimationFlags.Loop);
            }
            else if (ServerInfo.GetServerTime().TotalSeconds - Blackboard.Get(bb => bb.StartTime) > 2000 &&
                     Blackboard.Get(bb => bb.State) == FighterState.Threatening)
            {
                Blackboard.Set(bb => bb.State, FighterState.Attacking);
                var targetPed = GetClosestPed(ThePed.Position, 10.0f);
                if (targetPed != null)
                {
                    API.SetCurrentPedWeapon(ThePed.Handle, (uint)WeaponHash.Unarmed, true);
                    ThePed.Task.FightAgainst(targetPed);
                }
            }
        }

        private Ped GetClosestPed(Vector3 position, float maxDistance)
        {
            Ped closestPed = null;
            float closestDistance = maxDistance;

            foreach (var ped in World.GetAllPeds())
            {
                if (!ped.IsPlayer && ped.Handle != ThePed.Handle)
                {
                    var distance = ped.Position.DistanceToSquared(position);
                    if (distance < closestDistance)
                    {
                        closestDistance = distance;
                        closestPed = ped;
                    }
                }
            }

            return closestPed;
        }
    }
}