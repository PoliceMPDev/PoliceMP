using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CitizenFX.Core;
using CitizenFX.Core.Native;
using PoliceMP.Core.Client.Abstraction;
using PoliceMP.Core.Client.Interface;
using PoliceMP.Core.Client.Scripts;
using PoliceMP.Core.Shared;
using PoliceMP.Shared.Behaviors.Tailgate;

namespace PoliceMP.Client.Behaviors
{
    public class TailgateBehaviorImplementation : PedBehavior<TailgateBehavior>
    {
        private readonly ILogger<TailgateBehavior> _log;
        private const int DefaultMinDistance = 5;

        public TailgateBehaviorImplementation(ITickManager ticks, ILogger<TailgateBehavior> log) : base(ticks)
        {
            _log = log;
        }

        protected override Task Think()
        {
            var hNetTarget = Blackboard.Get(bb => bb.TargetVehicleNetworkId);
            var minDistance = Blackboard.Get(bb => bb.MinDistance);

            if (Blackboard.Get(bb => bb.MinDistance) <= 0f)
                Blackboard.Set(bb => bb.MinDistance, DefaultMinDistance);

            if (Entity.FromNetworkId(hNetTarget) is not Vehicle targetVehicle)
            {
                _log.Warn($"Could not find tailgate target! {hNetTarget}");
                return Task.FromResult(0);
            }

            API.SetTaskVehicleChaseIdealPursuitDistance(ThePed.Handle, 5f);
            if (API.GetActiveVehicleMissionType(ThePed.CurrentVehicle.Handle) != (int) VehicleMissionType.Escort)
            {
                API.TaskVehicleEscort(ThePed.Handle, ThePed.CurrentVehicle.Handle, targetVehicle.Handle, -1,
                    API.GetVehicleMaxSpeed(ThePed.CurrentVehicle.Handle), 17301628, 2f, 0, 3f);
            }

            return Task.FromResult(0);
        }

        protected override void OnDrawDebug()
        {
            base.OnDrawDebug();

            DrawDebugText(
                $"VehicleMission: {(VehicleMissionType) API.GetActiveVehicleMissionType(ThePed.CurrentVehicle.Handle)}");
        }
    }
}