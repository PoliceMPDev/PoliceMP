using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CitizenFX.Core;
using CitizenFX.Core.Native;
using PoliceMP.Client.Services.Interfaces;
using PoliceMP.Core.Client;
using PoliceMP.Core.Client.Abstraction;
using PoliceMP.Core.Client.Communications.Interfaces;
using PoliceMP.Core.Client.Extensions;
using PoliceMP.Core.Client.Interface;
using PoliceMP.Core.Client.Scripts;
using PoliceMP.Core.Shared;
using PoliceMP.Shared.Behaviors.FailToStop;
using PoliceMP.Shared.Constants.Decors;
using PoliceMP.Shared.NetworkMessages.WorldEvents.Notifications;
using Color = System.Drawing.Color;
using Vector3 = CitizenFX.Core.Vector3;

namespace PoliceMP.Client.Behaviors
{
    public class FailToStopBehaviorImplementation : PedBehavior<FailToStopBehavior>
    {
        private readonly ILogger<FailToStopBehavior> _log;
        private readonly IClientCommunicationsManager _comms;
        private readonly IBehaviorService _behaviors;
        private readonly Random _random = new Random();

        private const int DecampAttemptDelay = 200;
        private const int DecampStuckTimer = 1000;
        private const float TpacCheckDistance = 1f;
        private const float FlipSpeed = 10f;
        private const float FlipThreshold = 0.7f;
        private const float ChanceOfFlip = 0.1f;
        private const int DefaultDrivingStyle = 61;
        private const float DistanceToEscape = 1000f;
        private const float DistanceToForceDelete = 1500f;
        private const float HostMigrateThreshold = 10f;
        private const float ChanceOfDeathDueToFlip = 0.5f;
        private const int TimeToReckless = 1000 * 60 * 5;
        private const int TimeToKeepControlAfterMigrationSeconds = 1;
        private const IntersectOptions TpacIntersectOptions = IntersectOptions.MissionEntities
                                                              | IntersectOptions.Objects
                                                              | IntersectOptions.Map;

        private int HandlingHash = API.GetHashKey("sports_car");

        private int TookControlTime { get; set; }

        private Vector3 _lastHitRaycast = Vector3.Zero;

        private int _lastDecampAttempt = 0;

        private Vector3 _fleeFromDebugPos = default;

        public FailToStopBehaviorImplementation(
            ITickManager ticks,
            ILogger<FailToStopBehavior> log,
            IClientCommunicationsManager comms,
            IBehaviorService behaviors) : base(ticks)
        {
            _log = log;
            _comms = comms;
            _behaviors = behaviors;
        }

        public override void Initialize()
        {
            //_log.Debug($"Initializing ped {ThePed.NetworkId}...");
            var serverTime = ServerInfo.GetServerTime();
            Blackboard.Set(bb => bb.StartTime, serverTime.TotalSeconds);
            Blackboard.Set(bb => bb.State, FailToStopBehaviorState.Fleeing);
            Blackboard.Set(bb => bb.DrivingStyle, DefaultDrivingStyle);
            Blackboard.Set(bb => bb.Reckless, false);
            Blackboard.Set(bb => bb.StopForVehicles, false);
            SetMigrateInformation();
        }

        private void SetMigrateInformation()
        {
            var serverTime = ServerInfo.GetServerTime();
            Blackboard.Set(bb => bb.HostServerId, Game.Player.ServerId);
            Blackboard.Set(bb => bb.MigrateTime, serverTime.TotalSeconds);
            Blackboard.Set(bb => bb.NextAbleToMigrate, serverTime.TotalSeconds + TimeToKeepControlAfterMigrationSeconds);
        }

        protected override void Start()
        {
            //_aiEvents.AddShockingEventListener(ThePed, EventNames.ShockingSiren, TestSiren);
            _comms.AddNotificationHandler<CarCrashOccurredEvent>(CheckCarCrash);
            ThePed.Task.ClearSecondary();
            Blackboard.AddChangeHandler(bb => bb.DrivingStyle, SetDrivingStyle);
            Blackboard.AddChangeHandler(bb => bb.StopForVehicles, HandleStopForVehicleChange);
            Blackboard.AddChangeHandler(bb => bb.Reckless, HandleRecklessChange);

            API.NetworkUseHighPrecisionBlending(ThePed.Handle, true);
            //foreach (var vehicle in World.GetAllVehicles().OrderBy(v => World.GetDistance(ThePed.CurrentVehicle.Position, v.Position)))
            //{
            //    if (vehicle.HasDriver() && vehicle.Driver.Handle != ThePed.Handle && !vehicle.Driver.IsPlayer)
            //    {
            //        var tailgate = _behaviors.SetPedBehavior<TailgateBehavior>(vehicle.Driver);
            //        tailgate.Set(bb => bb.TargetVehicleNetworkId, ThePed.CurrentVehicle.NetworkId);
            //        break;
            //    }
            //}
        }

        protected override async Task Think()
        {
            var p = ThePed.Position;
            var shouldDecamp = Blackboard.Get(bb => bb.ShouldDecamp);
            var state = Blackboard.Get(bb => bb.State);
            var pedInVehicle = ThePed.IsInVehicle();
            var currentHost = Blackboard.Get(bb => bb.HostServerId);

            if (currentHost != Game.Player.ServerId)
            {
                SetMigrateInformation();
            }

            if (ThePed.IsDead)
            {
                //_log.Debug("Removing behavior from dead ped!");
                _behaviors.RemovePedBehaviors(ThePed);
                return;
            }

            if (ThePed.IsBeingStunned)
            {
                //_log.Debug("Removing flee behaviour from tasered ped");
                _behaviors.RemovePedBehaviors(ThePed);
                return;
            }

            // make sure collision is loaded around this entity
            API.SetEntityLoadCollisionFlag(ThePed.Handle, true);
            bool shouldBeMission =
                ThePed.HasNetworkControl()
                && World.GetDistance(ThePed.Position, Game.PlayerPed.Position) < DistanceToEscape;

            if (shouldBeMission)
            {
                if (!API.IsEntityAMissionEntity(ThePed.Handle))
                {
                    API.SetEntityAsMissionEntity(ThePed.Handle, false, false);
                }

                if (ThePed.CurrentVehicle != null
                    && !API.IsEntityAMissionEntity(ThePed.CurrentVehicle.Handle))
                {
                    API.SetEntityAsMissionEntity(ThePed.CurrentVehicle.Handle, false, false);
                }
            }
            else
            {
                if (API.IsEntityAMissionEntity(ThePed.Handle))
                {
                    int handle = ThePed.Handle;
                    API.SetEntityAsNoLongerNeeded(ref handle);
                }

                if (ThePed.CurrentVehicle != null
                    && API.IsEntityAMissionEntity(ThePed.CurrentVehicle.Handle))
                {
                    int handle = ThePed.CurrentVehicle.Handle;
                    API.SetEntityAsNoLongerNeeded(ref handle);
                    API.SetEntityCleanupByEngine(ThePed.Handle, true);
                }
            }

            if (World.GetDistance(Game.PlayerPed.Position, ThePed.Position) > DistanceToForceDelete)
            {
                if (ThePed.CurrentVehicle != null)
                {
                    ThePed.CurrentVehicle.Delete();
                }

                ThePed.Delete();
            }

            if (CheckTpac())
            {
                Blackboard.Set(bb => bb.DetectedTpac, true);
                Blackboard.Set(bb => bb.ShouldDecamp, true);
                shouldDecamp = true;
            }

            if (shouldDecamp)
            {
                if (Game.GameTime - _lastDecampAttempt < DecampAttemptDelay)
                {
                    return;
                }

                if (pedInVehicle
                    && ThePed.SeatIndex == VehicleSeat.Driver
                    && !API.GetIsTaskActive(ThePed.Handle, (int)TaskTypeIndex.CTaskExitVehicle)
                    && ThePed.TaskSequenceProgress == -1)
                {
                    //_log.Debug($"NetId:{ThePed.NetworkId} is decamping...");

                    if (ThePed.CurrentVehicle != null && ThePed.CurrentVehicle.Driver == ThePed)
                    {
                        foreach (var passenger in ThePed.CurrentVehicle.Passengers)
                        {
                            if (passenger.Handle == ThePed.Handle)
                            {
                                continue;
                            }

                            var passengerBlackboard = _behaviors.SetPedBehavior<FailToStopBehavior>(passenger);
                            passengerBlackboard.Set(bb => bb.ShouldDecamp, true);
                        }
                    }

                    var sequence = new TaskSequence();
                    API.TaskVehicleTempAction(0, 0, (int)VehicleTempAction.BrakeUntilStop, 5000);
                    sequence.AddTask.LeaveVehicle(LeaveVehicleFlags.LeaveDoorOpen);
                    sequence.Close();

                    ThePed.Task.PerformSequence(sequence);
                    return;
                }

                if (!pedInVehicle)
                {
                    Blackboard.Set(bb => bb.HasDecamped, true);
                }

                _lastDecampAttempt = Game.GameTime;
            }

            if (state == FailToStopBehaviorState.Fleeing)
            {
                if (ThePed.IsInVehicle())
                {
                    await DriveThink();
                }
                else
                {
                    await OnFootThink();
                }
            }
        }

        private async Task OnFootThink()
        {
            var dazed = Blackboard.Get(bb => bb.Dazed);

            if (dazed && !ThePed.IsRagdoll && ThePed.IsRunning && ThePed.Velocity.Length() > 2f)
            {
                API.SetPedToRagdoll(ThePed.Handle, 4000, 1000, 3, true, true, false);
                Blackboard.Set(bb => bb.Dazed, false);
            }

            if (!API.GetIsTaskActive(ThePed.Handle, (int)TaskType.CTaskSmartFlee))
            {
                API.TaskSmartFleePed(ThePed.Handle, Game.PlayerPed.Handle, 500f, -1, false, false);
            }
        }

        protected override async Task ThinkRemote()
        {
            var currentHost = API.NetworkGetEntityOwner(ThePed.Handle);
            var hostServerId = Blackboard.Get(bb => bb.HostServerId);
            var nextAbleToMigrate = Blackboard.Get(bb => bb.NextAbleToMigrate);
            var serverTime = ServerInfo.GetServerTime();

            if (nextAbleToMigrate > serverTime.TotalSeconds)
            {
                //_log.Debug($"Not checking migrate as GameTime is less than next migrate time {nextAbleToMigrate} > {serverTime.TotalSeconds}");
                return;
            }



            if (hostServerId != Game.Player.ServerId
                && nextAbleToMigrate < serverTime.TotalSeconds)
            {
                //_log.Debug($"CheckShouldMigrate enter: {currentHost} ({API.GetPlayerPed(currentHost)}) <{API.GetPlayerServerId(API.PlayerId())}>");
                var hostPed = Entity.FromHandle(API.GetPlayerPed(currentHost));
                var hostDistance = World.GetDistance(ThePed.Position, hostPed?.Position ?? Vector3.Zero);

                //_log.Debug($"CheckShouldMigrate HostDistance: {hostDistance}");
                if (hostDistance > HostMigrateThreshold)
                {
                    var distance = World.GetDistance(ThePed.Position, Game.PlayerPed.Position);
                    //_log.Debug($"CheckShouldMigrate MyDistance: {distance}");
                    if (distance < hostDistance)
                    {

                        //_log.Debug($"Requesting network control of {ThePed.NetworkId}");

                        if (ThePed.CurrentVehicle != null)
                        {
                            if (await ThePed.TryRequestNetworkEntityControl(false, 100)
                                && await ThePed.CurrentVehicle.TryRequestNetworkEntityControl(false, 100))
                            {
                                API.SetEntityAsMissionEntity(ThePed.Handle, true, true);
                                API.SetEntityAsMissionEntity(ThePed.CurrentVehicle.Handle, true, true);
                            }
                        }
                        else if (await ThePed.TryRequestNetworkEntityControl(false, 100))
                        {
                            API.SetEntityAsMissionEntity(ThePed.Handle, true, true);
                        }

                        SetMigrateInformation();

                        return;
                    }
                }
            }

            int handle = ThePed.Handle;
            API.SetEntityAsNoLongerNeeded(ref handle);
            if (ThePed.CurrentVehicle != null)
            {
                handle = ThePed.CurrentVehicle.Handle;
                API.SetEntityAsNoLongerNeeded(ref handle);
            }
        }

        private async Task DriveThink()
        {
            var pedPosition = ThePed.Position;
            var drivingStyle = Blackboard.Get(bb => bb.DrivingStyle);
            var startTime = Blackboard.Get(bb => bb.StartTime);
            var serverTime = ServerInfo.GetServerTime();

            if (ThePed.CurrentVehicle is null)
            {
                Blackboard.Set(bb => bb.HasDecamped, true);
                return;
            }

            if (serverTime.TotalSeconds - startTime > TimeToReckless)
            {
                Blackboard.Set(bb => bb.Reckless, true);
            }

            if (ThePed.CurrentVehicle.Speed > FlipSpeed
                && ThePed.CurrentVehicle.TryFlip(FlipThreshold, ChanceOfFlip))
            {
                var value = _random.Next(100);
                if (value < ChanceOfDeathDueToFlip)
                {
                    ThePed.Kill();
                }
                else
                {
                    Blackboard.Set(bb => bb.ShouldDecamp, true);
                    Blackboard.Set(bb => bb.Dazed, true);
                }

                ThePed.CurrentVehicle.EngineHealth = 0f;

                return;
            }

            if (ThePed.CurrentVehicle.EngineHealth == 0f ||
                ThePed.CurrentVehicle.PetrolTankHealth == 0f ||
                ThePed.CurrentVehicle.BodyHealth == 0f ||
                ThePed.CurrentVehicle.HasAnyBurstTire() ||
                API.IsVehicleStuckTimerUp(ThePed.CurrentVehicle.Handle, 0, DecampStuckTimer) ||
                API.IsVehicleStuckOnRoof(ThePed.CurrentVehicle.Handle))
            {
                Blackboard.Set(bb => bb.ShouldDecamp, true);
                return;
            }
            
            // Make sure ped stats are right
            ThePed.BlockPermanentEvents = false;
            ThePed.AlwaysKeepTask = true;
            API.SetDriveTaskMaxCruiseSpeed(ThePed.Handle, float.MaxValue);
            API.SetDriverAbility(ThePed.Handle, 1f);
            API.SetDriverAggressiveness(ThePed.Handle, 1f);
            API.SetNetworkIdCanMigrate(ThePed.NetworkId, serverTime.TotalSeconds - TookControlTime > TimeToKeepControlAfterMigrationSeconds);
            API.SetPlayerAngry(ThePed.Handle, false);
            API.DecorSetBool(ThePed.Handle, VehicleDecors.CalmVehicleDisabled, true);
            API.SetIgnoreSecondaryRouteNodes(false);
            API.SetVehicleUseAlternateHandling(ThePed.CurrentVehicle.Handle, true);
            API.SetVehicleHandlingHashForAi(ThePed.CurrentVehicle.Handle, HandlingHash);
            API.SetNetworkEnableVehiclePositionCorrection(ThePed.CurrentVehicle.Handle, true);
            //API.NetworkUseHighPrecisionVehicleBlending(ThePed.CurrentVehicle.NetworkId, true);
            if (ThePed.SeatIndex != VehicleSeat.Driver)
            {
                Blackboard.Set(bb => bb.ShouldDecamp, true);
                return;
            }

            var width = ThePed.CurrentVehicle.Speed * 2f;
            var velocity = ThePed.CurrentVehicle.Velocity;
            var movementForward = velocity;
            movementForward.Normalize();

            var movementOrtho = Vector3.Cross(movementForward, Vector3.Up);
            movementOrtho.Normalize();

            var midPoint = ThePed.CurrentVehicle.Position + movementForward * width / 2;
            midPoint.Z = ThePed.CurrentVehicle.Position.Z;
            var verticalOffset = movementForward.Z * 3;
            var velocityBasedZOffset = Math.Max(6f, Math.Abs(6f + velocity.Z * 3));
            var origin = midPoint + movementOrtho * 6f - Vector3.Up * velocityBasedZOffset;
            var extent = midPoint - movementOrtho * 6f + Vector3.Up * velocityBasedZOffset;

            if (verticalOffset > 0f)
            {
                extent += Vector3.Up * verticalOffset;
            }
            else
            {
                origin += Vector3.Up * verticalOffset;
            }

            if (DebugUtils.DebugEnabled)
            {
                DebugUtils.DrawDebugAngledArea(origin, extent, width, Color.FromArgb(100, 255, 0, 0));
            }

            foreach (var vehicle in World.GetAllVehicles())
            {
                if (vehicle == ThePed.CurrentVehicle || (vehicle.HasDriver() && vehicle.Driver.IsPlayer))
                    continue;

                if (API.IsEntityInAngledArea(vehicle.Handle,
                        origin.X, origin.Y, origin.Z,
                        extent.X, extent.Y, extent.Z,
                        width, true, true, 0))
                {
                    // API.SetPedIncreasedAvoidanceRadius(ThePed.Handle);
                    // API.SetPedIncreasedAvoidanceRadius(vehicle.Driver.Handle);
                    if (API.GetActiveVehicleMissionType(vehicle.Handle) != (int)VehicleMissionType.Stop)
                    {
                        Function.Call(Hash.TASK_VEHICLE_MISSION,
                            vehicle.Driver.Handle, // iPedID
                            vehicle.Handle, // iVehicleID
                            0, // iTargetVehicleID
                            (int)VehicleMissionType.Stop, // iMission
                            10f, // fCruiseSpeed
                            7791, // iDrivingStyle
                            -1.0f, // fTargetReached < 0 causes DEFAULT value
                            -5f, // fStraightLineDistance  < 0 causes DEFAULT value
                            false); // bDriveAgainstTraffic
                    }
                    //API.TaskVehicleTempAction(vehicle.Driver.Handle, vehicle.Handle, 24, 3000);
                }
            }

            var raycast = World.RaycastCapsule(ThePed.CurrentVehicle.Position,
                ThePed.CurrentVehicle.ForwardVector,
                20f + ThePed.CurrentVehicle.Speed * 2, 4f, TpacIntersectOptions, ThePed.CurrentVehicle);
            if (raycast is var result && result.DitHitEntity
                                      && result.HitEntity is Vehicle hitVehicle && hitVehicle.Driver?.IsPlayer == false)
            {
                _lastHitRaycast = raycast.HitPosition;

                // Only set ignore all paths to true if the vehicle speed is over 0
                // This allows the vehicle to continue even if above >200 meters from the
                // player, as ignore paths requires the ped to be within range.
                // Blackboard.Set(bb => bb.IgnoreAllPathing, true);

                var stopForVehicles = raycast.HitEntity is Vehicle vehicle
                                      && vehicle.ClassType == VehicleClass.Emergency
                                      && vehicle.Driver?.IsPlayer == false;
                Blackboard.Set(bb => bb.StopForVehicles, stopForVehicles);
                //Blackboard.Set(bb => bb.IgnoreRoads, true);
            }
            else
            {
                // Blackboard.Set(b => b.IgnoreAllPathing, false);
            }

            // If we're leaving a vehicle then don't do any more thinking
            if (API.GetIsTaskActive(ThePed.Handle, (int)TaskType.CTaskLeaveAnyCar))
            {
                Blackboard.Set(bb => bb.ShouldDecamp, true);
                return;
            }

            if (API.GetActiveVehicleMissionType(ThePed.CurrentVehicle.Handle) != (int)VehicleMissionType.Flee)
            {
                Function.Call(Hash.TASK_VEHICLE_MISSION_PED_TARGET,
                    ThePed.Handle,
                    ThePed.CurrentVehicle.Handle,
                    Game.PlayerPed.Handle,
                    (int)VehicleMissionType.Flee,
                    API.GetVehicleMaxSpeed(ThePed.CurrentVehicle.Handle),
                    drivingStyle,
                    -1f,
                    -1f,
                    false
                );
            }

            Vector3 nodePos = Vector3.Zero;
            int density = 0;
            int flags = 0;
            if (API.GetNthClosestVehicleNode(pedPosition.X, pedPosition.Y, pedPosition.Z, 1, ref nodePos, 0x4, 0, 0)
                && API.GetVehicleNodeProperties(nodePos.X, nodePos.Y, nodePos.Z, ref density, ref flags))
            {
                if ((flags & (int)PathNodeFlags.DeadEnd) > 0 && ThePed.CurrentVehicle.Speed < 3f)
                {
                    Blackboard.Set(bb => bb.ShouldDecamp, true);
                }
            }
        }

        private Task SetDrivingStyle(int oldValue, int newValue, bool replicated)
        {
            //_log.Debug($"Setting driving style of NetId:{ThePed.NetworkId} to {newValue}...");
            API.SetDriveTaskDrivingStyle(ThePed.Handle, newValue);
            return Task.FromResult(0);
        }

        private Task HandleStopForVehicleChange(bool oldValue, bool newValue, bool replicated)
        {
            if (!ThePed.HasNetworkControl())
            {
                return Task.FromResult(0);
            }

            //_log.Debug($"StopForVehicle changed to {newValue}");
            SetDrivingStyleFlag(DrivingStyleFlag.StopBeforeVehicle, newValue);
            return Task.FromResult(0);
        }

        private Task HandleRecklessChange(bool oldValue, bool newValue, bool replicated)
        {
            if (!ThePed.HasNetworkControl())
            {
                return Task.FromResult(0);
            }

            //_log.Debug($"Reckless changed to {newValue}");
            SetDrivingStyleFlag(DrivingStyleFlag.Reckless, newValue);
            return Task.FromResult(0);
        }

        private void SetDrivingStyleFlag(DrivingStyleFlag flag, bool value)
        {
            var drivingStyle = Blackboard.Get(bb => bb.DrivingStyle);
            var iFlag = (int)flag;
            var currentValue = (drivingStyle & iFlag) > 1;

            if (currentValue != value)
            {
                if (value)
                {
                    drivingStyle |= iFlag;
                }
                else
                {
                    drivingStyle &= ~iFlag;
                }
            }

            Blackboard.Set(bb => bb.DrivingStyle, drivingStyle);
        }

        private Task CheckCarCrash(CarCrashOccurredEvent @event)
        {
            bool pedInvolved = @event.AttackerDriverNetworkId == ThePed.Handle ||
                               @event.VictimDriverNetworkId == ThePed.Handle;
            if (pedInvolved && @event.Damage > 50f)
            {
                Blackboard.Set(bb => bb.ShouldDecamp, true);
            }

            return Task.FromResult(0);
        }

        private Task TestSiren(IReadOnlyList<Entity> involved, Entity source, IReadOnlyList<object> data)
        {
            if (source is Vehicle vehicleSource)
            {
                //_log.Debug($"I heard a siren from {vehicleSource?.Driver?.GetPlayer()?.Name}. Ahhhhhh!");
                return Task.FromResult(0);
            }
            else
            {
                throw new ArgumentException($"This ain't no popo car... this is a {source.GetType()}!");
            }
        }

        private bool CheckTpac()
        {
            if (ThePed.IsDead || !ThePed.IsInVehicle())
            {
                return false;
            }

            var dimensions = ThePed.CurrentVehicle.Model.GetDimensions();
            var checkDistance = dimensions.Y / 2 + TpacCheckDistance;

            if (DebugUtils.DebugEnabled)
            {
                World.DrawLine(ThePed.CurrentVehicle.Position,
                    ThePed.CurrentVehicle.Position + ThePed.CurrentVehicle.ForwardVector * checkDistance,
                    Color.FromArgb(255, 255, 0, 0));
                World.DrawLine(ThePed.CurrentVehicle.Position,
                    ThePed.CurrentVehicle.Position - ThePed.CurrentVehicle.ForwardVector * checkDistance,
                    Color.FromArgb(255, 255, 0, 0));
            }

            // Look forward
            var raycast = World.Raycast(ThePed.CurrentVehicle.Position, ThePed.CurrentVehicle.ForwardVector,
                checkDistance, IntersectOptions.MissionEntities, ThePed.CurrentVehicle);
            if (!raycast.DitHit)
            {
                return false;
            }

            // Look behind
            raycast = World.Raycast(ThePed.CurrentVehicle.Position, ThePed.CurrentVehicle.ForwardVector * -1,
                checkDistance, IntersectOptions.MissionEntities, ThePed.CurrentVehicle);
            if (!raycast.DitHit)
            {
                return false;
            }

            return true;
        }

        protected override void OnDrawDebug()
        {
            base.OnDrawDebug();

            World.DrawMarker(MarkerType.DebugSphere, _fleeFromDebugPos, Vector3.Zero, Vector3.Zero, Vector3.One,
                Color.FromArgb(255, 10, 100, 255));
            World.DrawLine(_fleeFromDebugPos, ThePed.Position, Color.FromArgb(255, 10, 100, 255));

            DrawDebugText($"Speed: {ThePed.CurrentVehicle?.Speed}");
            DrawDebugText(
                $"ActiveMission: {(VehicleMissionType)API.GetActiveVehicleMissionType(ThePed.CurrentVehicle?.Handle ?? 0)}");

            DrawDebugText($"Distance: {World.GetDistance(ThePed.Position, Game.PlayerPed.Position)}");

            World.DrawMarker(MarkerType.DebugSphere, _lastHitRaycast, Vector3.Zero, Vector3.Zero, Vector3.One,
                Color.FromArgb(255, 255, 0, 0));
        }
    }

}