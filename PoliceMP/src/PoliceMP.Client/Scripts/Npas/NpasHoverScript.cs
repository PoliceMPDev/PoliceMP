using System;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using CitizenFX.Core;
using CitizenFX.Core.Native;
using CitizenFX.Core.NaturalMotion;
using CitizenFX.Core.UI;
using PoliceMP.Client.Overlays.NewNotification;
using PoliceMP.Client.Scripts.DebugScripts;
using PoliceMP.Client.Services.Interfaces;
using PoliceMP.Core.Client;
using PoliceMP.Core.Client.Abstraction;
using PoliceMP.Core.Client.Commands.Interfaces;
using PoliceMP.Core.Client.Extensions;
using PoliceMP.Core.Client.Interface;
using PoliceMP.Core.Client.Scripts;
using PoliceMP.Core.Shared;

namespace PoliceMP.Client.Scripts.Npas
{
    public class NpasHoverScript : Script
    {
        private readonly ILogger<NpasHoverScript> _log;
        private readonly IGameInputManager _input;
        private readonly ITickManager _ticks;
		private readonly INewNotificationOverlay _newNotificationOverlay;
		private Vector3? _holdPosition;
        private int _holdHeight;
        private float _holdHeading;
        private bool _forceUpdate;
        private const float HoldPositionChangeSpeed = 5f;
        private const float HoldHeadingChangeSpeed = 30f;
        private const float MetersToFeet = 3.28084f;
        private const float UpdatePosWhenSpeedAbove = HoldPositionChangeSpeed * 2f;
        private const float UpdateHoldPosWhenFasterThan = 1f;
        private const float UpdateHoldPosWhenFurtherThan = 5f;
        private int _heliFlags = 64;

        public NpasHoverScript(ILogger<NpasHoverScript> log, IGameInputManager input, ITickManager ticks, ICommandManager commands, INewNotificationOverlay newNotificationOverlay)
        {
            _log = log;
            _input = input;
            _ticks = ticks;
			_newNotificationOverlay = newNotificationOverlay;

			void Handler(string s)
            {
                if (int.TryParse(s, out _heliFlags))
                {
					_newNotificationOverlay.SendNotification(new NewNotificationMessage("NPAS Hover Mode", "success", $"Flags: {_heliFlags}, Flags: {_heliFlags}", new NewNotificationMessageContent[0]));
					_forceUpdate = true;
                }
                else
                {
					_newNotificationOverlay.SendNotification(new NewNotificationMessage("NPAS Hover Mode", "error", $"Could not parse {s} to int", new NewNotificationMessageContent[0]));
				}
            }

            commands.Register("hover").WithHandler(Handler);
        }

        protected override Task OnStartAsync()
        {
            _ticks.On(NpasHoverScriptTick);
            
            return Task.FromResult(0);
        }

        private Task NpasHoverScriptTick()
        {
            if (!Game.PlayerPed.IsInHeli && _holdPosition.HasValue)
            {
                _holdPosition = null;
                Game.PlayerPed.Task.ClearAll();
                return Task.FromResult(0);
            }
            
            if (Game.PlayerPed.IsInHeli
                && Game.PlayerPed.SeatIndex == VehicleSeat.LeftFront)
            {
                var isTaskActive = Game.PlayerPed.IsTaskActive(TaskType.CTaskControlVehicle);

                if (_input.IsJustBeingHeld(Control.VehicleSelectNextWeapon))
                {
                    if (isTaskActive)
                    {
                        //_log.Debug("Hover Disabled.");
                        _holdPosition = null;
                        Game.PlayerPed.Task.ClearAll();
						_newNotificationOverlay.SendNotification(new NewNotificationMessage("NPAS Hover Mode", "warning", "Hover mode disabled. You have control!", new NewNotificationMessageContent[0]));
					}
                    else if (Game.PlayerPed.CurrentVehicle.IsInAir)
                    {
                        //_log.Debug("Hover Enabled.");
                        _holdPosition = Game.PlayerPed.CurrentVehicle.Position;
                        _holdHeight = (int) Game.PlayerPed.CurrentVehicle.HeightAboveGround;
                        _holdHeading = Game.PlayerPed.Heading;
                        _forceUpdate = true;
						_newNotificationOverlay.SendNotification(new NewNotificationMessage("NPAS Hover Mode", "info", "Hover mode enabled!", new NewNotificationMessageContent[0]));
					}
                }
                
                if (isTaskActive)
                {
                    var hor = _input.GetControlNormal(Control.VehicleFlyRollLeftRight);
                    var ver = _input.GetControlNormal(Control.VehicleFlyPitchUpDown);
                    var up = _input.GetControlNormal(Control.VehicleFlyThrottleUp);
                    var down = _input.GetControlNormal(Control.VehicleFlyThrottleDown);
                    var yawLeft = _input.IsPressed(Control.VehicleFlyYawLeft);
                    var yawRight = _input.IsPressed(Control.VehicleFlyYawRight);
                    
                    if (Math.Abs(hor) > 0.2f 
                        || Math.Abs(ver) > 0.2f
                        || Math.Abs(up) > 0.2f
                        || Math.Abs(down) > 0.2f
                        || yawLeft
                        || yawRight)
                    {
                        var right = Vector3.Cross(Game.PlayerPed.CurrentVehicle.ForwardVector, Vector3.Up);
                        var forward = Vector3.Cross(right, Vector3.Up);

                        float yawOffset = 0f;
                        if (yawLeft)
                            yawOffset += HoldHeadingChangeSpeed;

                        if (yawRight)
                            yawOffset -= HoldHeadingChangeSpeed;

                        _holdHeading = (_holdHeading + yawOffset * Game.LastFrameTime);
                        if (_holdHeading > 360f) _holdHeading -= 360f;
                        if (_holdHeading < 0f) _holdHeading += 360f;

                        _holdPosition += right * hor * Game.LastFrameTime * HoldPositionChangeSpeed;
                        _holdPosition += forward * ver * Game.LastFrameTime * HoldPositionChangeSpeed;
                        _holdPosition += up * Vector3.Up * Game.LastFrameTime * HoldPositionChangeSpeed;
                        _holdPosition += down * Vector3.Up * -1 * Game.LastFrameTime * HoldPositionChangeSpeed;
                        _forceUpdate = true;
                    }

                    if (DebugUtils.DebugEnabled)
                    {
                        World.DrawMarker(MarkerType.DebugSphere, _holdPosition ?? Vector3.Zero, Vector3.Zero, Vector3.Zero, Vector3.One, Color.FromArgb(128, 255, 255, 255));
                    }
                    
                    Screen.ShowSubtitle($"~r~[HOVER MODE]", 0);
                }

                if (!_holdPosition.HasValue)
                {
                    _forceUpdate = false;
                    return Task.FromResult(0);
                }
                
                if (Game.PlayerPed.CurrentVehicle.Speed > UpdatePosWhenSpeedAbove)
                {
                    var curPos = Game.PlayerPed.CurrentVehicle.Position - (Game.PlayerPed.CurrentVehicle.Velocity * 0.1f);
                    var newPos = _holdPosition.Value;
                    newPos.X = curPos.X;
                    newPos.Y = curPos.Y;
                    _holdPosition = newPos;
                }
                else if (!_forceUpdate 
                         && (Game.PlayerPed.CurrentVehicle.Speed > UpdateHoldPosWhenFasterThan 
                             || World.GetDistance(Game.PlayerPed.CurrentVehicle.Position, _holdPosition.Value) > UpdateHoldPosWhenFurtherThan))
                {
                    _holdPosition = Game.PlayerPed.CurrentVehicle.Position - Game.PlayerPed.CurrentVehicle.Velocity;
                }

                if (_forceUpdate || World.GetDistance(Game.PlayerPed.CurrentVehicle.Position, _holdPosition.Value) > 5f)
                {
                    Function.Call(Hash.TASK_HELI_MISSION,
                        Game.PlayerPed.Handle, // iPedID
                        Game.PlayerPed.CurrentVehicle.Handle, // iVehicleID
                        0, // iTargetVehicleID
                        0, // iTargetPedID,
                        _holdPosition.Value.X, // scrVecCoors
                        _holdPosition.Value.Y,
                        _holdPosition.Value.Z,
                        (int) VehicleMissionType.GoTo, // iMission
                        HoldHeadingChangeSpeed * 2f, // fCruiseSpeed
                        -1f, // fTargetReached
                        _holdHeading, // fHeliOrientation
                        _holdHeight, // iFlightHeight
                        _holdHeight, // iMinHeightAboveTerrain
                        -1f, //fSlowDownDistance
                        _heliFlags // iHeliFlags
                    );

                    _forceUpdate = false;
                }
            }
            return Task.FromResult(0);
        }
    }
}