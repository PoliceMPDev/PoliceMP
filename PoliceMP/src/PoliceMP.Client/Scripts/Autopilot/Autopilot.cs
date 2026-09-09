#pragma warning disable CS0618 // Type or member is obsolete
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CitizenFX.Core;
using CitizenFX.Core.Native;
using MenuAPI;
using PoliceMP.Client.Overlays.NewNotification;
using PoliceMP.Core.Client;
using PoliceMP.Core.Client.Abstraction;
using PoliceMP.Core.Client.Communications.Interfaces;
using PoliceMP.Core.Client.Extensions;
using PoliceMP.Core.Client.Interface;
using PoliceMP.Core.Client.Scripts;
using PoliceMP.Core.Shared;
using PoliceMP.Core.Shared.Models;
using PoliceMP.Shared.Constants;
using PoliceMP.Shared.Constants.Decors;
using Color = System.Drawing.Color;

namespace PoliceMP.Client.Scripts.Autopilot
{
    public enum AutopilotBehaviorState
    {
        Engaged,
        Disengaged,
        GracePeriod
    }

    // ReSharper disable once ClassNeverInstantiated.Global
    public class Autopilot : Script
    {
        private readonly ILogger<Autopilot> _logger;
        private readonly INewNotificationOverlay _newNotificationOverlay;
        private readonly ILegacyClientCommunicationsManager _comms;

        private float _speed = 50f;
        private bool _debugMode;
        private bool _brakesEngaged;
        private int _lastDrivingSpeedUpdate;
        private bool _currentElsState;

        private bool _isFixingVehicle;

        private int _drivingStyle;

        private bool _on;

        private const int NoRushDrivingStyle = 2_883_771;
        private const int RushDrivingStyle = (int)DrivingStyle.Rushed;
        private const float DestinationArrivalRadius = 10f;

        private const float PosX = 0.5f;
        private const float PosY = 0.5f;
        private const float Scale = 0.84f;
        private const int Font = 4;

        private bool _didBeginDriving;
        private bool _isFullSelfDriving;
        private double _initialDamage;
        private AutopilotBehaviorState _state;
        private PmpVector3 _destination;
        private double _gracePeriodStartTime;
        private const double GracePeriodDuration = 0.5; // in seconds

        private const float MphToKph = 2.23694f;

        // ReSharper disable once ConvertToPrimaryConstructor
        public Autopilot(ITickManager ticks,
            ILogger<Autopilot> logger,
            INewNotificationOverlay newNotificationOverlay,
            ILegacyClientCommunicationsManager comms)
        {
            _logger = logger;
            _newNotificationOverlay = newNotificationOverlay;
            _comms = comms;

            comms.On(ClientEvents.AutopilotState, async (bool state, bool fsd) =>
            {
                if (state)
                {
                    _isFullSelfDriving = fsd;
                    Initialize();
                }
                else
                    await Disengage();
            });

            comms.On(ClientEvents.IsFixingVehicle, (bool isFixing) => { _isFixingVehicle = isFixing; });

            comms.On(ClientEvents.SetAutopilotState, async (bool mode) =>
            {
                if (mode)
                {
                    if (!_on)
                        Initialize();
                }
                else
                {
                    if (_on)
                        await Disengage();
                }
            });

            ticks.On(Think);
        }

        private void ResetAllValues()
        {
            _speed = 50f;
            _debugMode = false;
            _brakesEngaged = false;
            _lastDrivingSpeedUpdate = 0;
            _currentElsState = false;
            _didBeginDriving = false;
            _initialDamage = 0;
            _destination = Vector3.Zero.ToPmpVector3();
            _on = true;
            _state = AutopilotBehaviorState.Engaged;
            _gracePeriodStartTime = 0;
            _comms.ToClient(ClientEvents.SetManualCarMode, false);
        }

        private async void Initialize()
        {
            ResetAllValues();

            if (null != Game.PlayerPed.CurrentVehicle)
                _currentElsState = Game.PlayerPed.CurrentVehicle.GetBoolDecor(ELSDecors.LIGHTS_MAIN);
            _drivingStyle = _currentElsState ? RushDrivingStyle : NoRushDrivingStyle;

            await Beep();
            API.ExecuteCommand("[LOG] Autopilot has been engaged!");
            _didBeginDriving = false;
            var vehicle = Game.PlayerPed.CurrentVehicle;
            if (vehicle != null) return;
            _newNotificationOverlay.SendNotification(new NewNotificationMessage("Autopilot", "error",
                "You must be in a vehicle to use this!", new NewNotificationMessageContent[0]));
            _state = AutopilotBehaviorState.Disengaged;
        }

        private static Task Beep()
        {
            API.PlaySoundFrontend(-1, "Mission_Pass_Notify", "DLC_HEISTS_GENERAL_FRONTEND_SOUNDS",
                false);
            return Task.FromResult(0);
        }

        private Task ChangeDrivingSpeed(float speed)
        {
            _speed = speed;
            Game.PlayerPed.Task.ClearAll();
            DriveToDestination();
            _lastDrivingSpeedUpdate = Game.GameTime;
            return Task.FromResult(0);
        }

        private void DriveToDestination()
        {
            if (!_isFullSelfDriving) return;
            var vehicle = Game.PlayerPed.CurrentVehicle;

            Game.PlayerPed.Task.DriveTo(vehicle, _destination.ToCitizenVector3(),
                DestinationArrivalRadius, (float)Math.Round(_speed) / MphToKph,
                _drivingStyle);
        }

        private async Task CheckCarCrash()
        {
            if (!(_initialDamage - 10 > Game.PlayerPed.CurrentVehicle.BodyHealth))
                return;
            {
                await Disengage("Crash detected, autopilot disengaged automatically.");
                _state = AutopilotBehaviorState.Disengaged;
                EngageBrakesFor(1000, 15f);
            }
        }

        private async Task Disengage(string reason = null)
        {
            await Beep();
            API.ExecuteCommand("[LOG] Autopilot has been disengaged!");
            _logger.Debug($"Disengaging autopilot for ped {Game.PlayerPed.NetworkId}... {reason}");
            _state = AutopilotBehaviorState.Disengaged;
            if (reason != null)
                _newNotificationOverlay.SendNotification(new NewNotificationMessage("Autopilot", "info", reason,
                    new NewNotificationMessageContent[0]));
            Game.PlayerPed.Task.ClearAll();
            _on = false;
        }

        private void EngageBrakesFor(int ms, float reduction = 35.0f)
        {
            new Thread(async () =>
            {
                try
                {
                    _brakesEngaged = true;

                    var vehicle = Game.PlayerPed.CurrentVehicle;
                    if (vehicle == null || !vehicle.Exists()) return;
                    _logger.Debug($"Engaging brakes at {reduction} reduction for {ms}ms...");

                    vehicle.SoundHorn(ms * 3);

                    var startTime = Game.GameTime;
                    var endTime = startTime + ms;

                    while (Game.GameTime < endTime)
                    {
                        API.SetVehicleBrake(vehicle.Handle, true); // Full brake pedal

                        // Disable acceleration input
                        Game.DisableControlThisFrame(0, Control.VehicleAccelerate);

                        float currentSpeed = vehicle.Speed;
                        if (currentSpeed > 0f)
                        {
                            float newSpeed = Math.Max(currentSpeed - reduction * Game.LastFrameTime, 0f);
                            API.SetVehicleForwardSpeed(vehicle.Handle, newSpeed);
                        }

                        // Visual feedback
                        vehicle.AreBrakeLightsOn = true;

                        await BaseScript.Delay(0); // Process every frame
                    }

                    // Reset to normal state
                    API.SetVehicleBrake(vehicle.Handle, false);
                    vehicle.EnginePowerMultiplier = 1.0f; // Reset to default
                    vehicle.AreBrakeLightsOn = false;
                    _logger.Debug("Brakes disengaged.");

                    _brakesEngaged = false;
                }
                catch
                {
                    _logger.Error("Error engaging brakes.");
                }
            }).Start();
        }

        private async Task Think()
        {
            if (!_on) return;
            var vehicle = Game.PlayerPed.CurrentVehicle;

            if (_isFixingVehicle)
            {
                await Disengage("You are fixing a vehicle, autopilot disengaged automatically.");
                return;
            }

            if (null == vehicle)
            {
                await Disengage();
                return;
            }

            if (Game.IsControlJustPressed(0, Control.VehicleHeadlight) && !MenuController.IsAnyMenuOpen())
            {
                _debugMode = !_debugMode;
                Game.DisableControlThisFrame(0, Control.VehicleHeadlight);
                _logger.Debug($"Debug mode toggled to {_debugMode}");
            }

            var previousElsState = _currentElsState;
            _currentElsState = vehicle.GetBoolDecor(ELSDecors.LIGHTS_MAIN);
            if (previousElsState != _currentElsState)
            {
                _drivingStyle = _currentElsState ? RushDrivingStyle : NoRushDrivingStyle;
                if (_isFullSelfDriving)
                    DriveToDestination();
            }

            if (!_didBeginDriving)
            {
                if (_isFullSelfDriving)
                {
                    var waypoint = World.GetWaypointBlip();
                    if (waypoint == null)
                    {
                        _newNotificationOverlay.SendNotification(new NewNotificationMessage("No Waypoint", "error",
                            "You must have a waypoint set to use this.", new NewNotificationMessageContent[0]));
                        _state = AutopilotBehaviorState.Disengaged;
                        _didBeginDriving = true;
                        return;
                    }

                    _destination = waypoint.Position.ToPmpVector3();
                    _initialDamage = vehicle.BodyHealth;

                    Game.PlayerPed.Task.ClearAll();
                    DriveToDestination();
                }
                else
                    Game.PlayerPed.Task.ClearAll();

                _didBeginDriving = true;
                return;
            }

            try
            {
                Game.PlayerPed.Exists();
                vehicle.Exists();
            }
            catch (NullReferenceException)
            {
                await Disengage("No ped or vehicle found. Autopilot disengaged automatically.");
                return;
            }

            if (!Game.PlayerPed.Exists() || !vehicle.Exists())
            {
                await Disengage("No ped or vehicle found. Autopilot disengaged automatically.");
                return;
            }

            await CheckCarCrash();

            if (_state == AutopilotBehaviorState.Disengaged)
            {
                await Disengage();
                return;
            }

            if (_isFullSelfDriving)
            {
                var waypoint = World.GetWaypointBlip();
                if (waypoint == null)
                {
                    if (_state != AutopilotBehaviorState.GracePeriod)
                    {
                        _state = AutopilotBehaviorState.GracePeriod;
                        _gracePeriodStartTime = ServerInfo.GetServerTime().TotalSeconds;
                    }
                    else
                    {
                        double currentTime = ServerInfo.GetServerTime().TotalSeconds;
                        if (currentTime - _gracePeriodStartTime >= GracePeriodDuration)
                        {
                            await Disengage(
                                "You have arrived at your destination, autopilot disengaged automatically.");
                            EngageBrakesFor(1000, 15f);
                            return;
                        }
                    }
                }
                else
                {
                    if (_state == AutopilotBehaviorState.GracePeriod)
                    {
                        _state = AutopilotBehaviorState.Engaged;
                        _logger.Debug("Recovered from grace period to new destination.");
                    }

                    if (waypoint.Position.ToPmpVector3() != _destination)
                    {
                        _destination = waypoint.Position.ToPmpVector3();
                        DriveToDestination();
                    }
                }
            }

            if (_isFullSelfDriving && Game.PlayerPed.IsDead)
                await Disengage("You have died, autopilot disengaged automatically.");

            if (ShouldFix(out int ttc))
            {
                float ttcSeconds = ttc / 1000f; // Convert ttc from ms to seconds
                float reduction = (1f / ttcSeconds) * 25f; // Reduction based on seconds
                int duration = ttc / 2; // Duration in ms, ttc / 2 as intended

                // Safeguards to prevent extreme values
                reduction = Math.Min(reduction, 50f); // Cap reduction at 50
                duration = Math.Min(duration, 1000); // Cap duration at 1 second

                EngageBrakesFor(duration, reduction);
            }

            if ((Game.IsControlPressed(0, Control.VehicleAccelerate) ||
                 Game.IsControlPressed(0, Control.VehicleBrake) ||
                 Game.IsControlPressed(0, Control.VehicleHandbrake)) && _isFullSelfDriving)
                await Disengage();

            if (Game.IsControlPressed(0, Control.PrevWeapon) && _isFullSelfDriving)
            {
                Game.DisableControlThisFrame(0, Control.PrevWeapon);
                Game.DisableControlThisFrame(0, Control.VehicleSelectNextWeapon);
                // previous weapon = increase speed (SCROLL UP)
                if (!MenuController.IsAnyMenuOpen())
                    await ChangeDrivingSpeed(_speed + 1f);
            }
            else if (Game.IsControlPressed(0, Control.NextWeapon) && _isFullSelfDriving)
            {
                Game.DisableControlThisFrame(0, Control.NextWeapon);
                Game.DisableControlThisFrame(0, Control.VehicleNextRadio);
                // next weapon = decrease speed (SCROLL DOWN)
                if (!MenuController.IsAnyMenuOpen())
                    await ChangeDrivingSpeed(_speed - 1f);
            }

            if (_lastDrivingSpeedUpdate + 500 >= Game.GameTime)
            {
                var text = (int)Math.Round(_speed) + " MPH";

                API.SetTextScale(Scale, Scale);
                API.SetTextFont(Font);
                API.SetTextCentre(true);
                API.SetTextOutline();
                API.BeginTextCommandDisplayText("STRING");
                API.AddTextComponentSubstringPlayerName($"{text}");
                API.EndTextCommandDisplayText(PosX, PosY);
            }
        }

        private bool IsPointInCapsule(Vector3 point, Vector3 start, Vector3 end, float radius)
        {
            Vector3 AB = end - start;
            Vector3 AP = point - start;
            float projection = Vector3.Dot(AP, AB) / Vector3.Dot(AB, AB);
            projection = MathUtil.Clamp(projection, 0f, 1f);
            Vector3 closest = start + projection * AB;
            float distance = Vector3.Distance(closest, point);
            return distance < radius;
        }

        private bool ShouldFix(out int ttc)
        {
            ttc = 0;
            var vehicle = Game.PlayerPed.CurrentVehicle;
            if (vehicle == null || !vehicle.Exists()) return false;

            // Define a dynamic capsule shape for collision detection
            Vector3 start = vehicle.Position;
            Vector3 direction = vehicle.Speed > 0.1f ? Vector3.Normalize(vehicle.Velocity) : vehicle.ForwardVector;
            float length = vehicle.Speed > 0.1f ? vehicle.Speed * 1.5f : 5.0f; // Reduced length scaling
            Vector3 end = start + direction * length;
            const float radius = 1.5f; // Reduced radius to match vehicle width more closely

            // Get all vehicles and filter those within the capsule
            var vehicles = World.GetAllVehicles();
            var vehiclesInCapsule = vehicles
                .Where(v => v != vehicle && IsPointInCapsule(v.Position, start, end, radius))
                .ToList();

            // Render the capsule in debug mode
            if (_debugMode)
            {
                var color = _brakesEngaged ? Color.FromArgb(50, 255, 100, 0) : Color.FromArgb(50, 0, 255, 0);
                World.DrawMarker(MarkerType.DebugSphere, start, Vector3.Zero, Vector3.Zero, Vector3.One * radius * 2,
                    color);
                World.DrawMarker(MarkerType.DebugSphere, end, Vector3.Zero, Vector3.Zero, Vector3.One * radius * 2,
                    color);
                World.DrawLine(start, end, color);
            }
            foreach (var v in vehiclesInCapsule)
            {
                World.DrawMarker(MarkerType.DebugSphere, v.Position, Vector3.Zero, Vector3.Zero, Vector3.One * 2f,
                    Color.FromArgb(50, 255, 0, 0));
            }

            // Calculate time to collision for vehicles in the capsule
            float minTtc = float.MaxValue;
            foreach (var v in vehiclesInCapsule)
            {
                Vector3 pos1 = vehicle.Position;
                Vector3 pos2 = v.Position;
                Vector3 vel1 = vehicle.Velocity;
                Vector3 vel2 = v.Velocity;
                Vector3 relativePosition = pos2 - pos1;
                Vector3 relativeVelocity = vel1 - vel2;

                // Check if the vehicle is ahead and in the travel path
                float directionDot = Vector3.Dot(Vector3.Normalize(relativePosition), direction);
                if (directionDot < 0.7f) continue; // Skip if not sufficiently ahead (cosine of ~45 degrees)

                float distance = relativePosition.Length();
                if (distance > 0.001f) // Avoid division by zero
                {
                    Vector3 directionNorm = relativePosition / distance;
                    float closingSpeed = Vector3.Dot(relativeVelocity, directionNorm);
                    if (closingSpeed > 0) // Only consider vehicles approaching
                    {
                        float ttcSeconds = distance / closingSpeed;
                        if (ttcSeconds < minTtc)
                            minTtc = ttcSeconds;
                    }
                }
            }

            // Only brake if TTC is imminent (less than 1 second)
            if (minTtc < 1f)
            {
                ttc = (int)(minTtc * 1000); // Convert to milliseconds
                if (ttc < 100) ttc = 100; // Minimum ttc to avoid extreme braking values
                return true;
            }

            return false;
        }
    }
}
#pragma warning restore CS0618 // Type or member is obsolete
