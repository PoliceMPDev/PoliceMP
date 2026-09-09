using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CitizenFX.Core;
using CitizenFX.Core.Native;
using PoliceMP.Client.Services.Interfaces;
using PoliceMP.Client.Utils.CameraUtils;
using PoliceMP.Core.Client;
using PoliceMP.Core.Client.Interface;
using PoliceMP.Core.Client.Scripts;
using PoliceMP.Core.Shared;

namespace PoliceMP.Client.Scripts.CameraScripts
{
    public class HelicopterHoldCameraScript : Script
    {
        private readonly ILogger<HelicopterHoldCameraScript> _log;
        private readonly INotificationService _notifications;
        private readonly ITickManager _ticks;
        private readonly IGameInputManager _input;
        private Camera _camera;

        private bool _enabled = false;

        private float _holdHeading = 0f;
        private float _holdPitch = 0f;

        private float turnSpeed = 100f;

        public HelicopterHoldCameraScript(ILogger<HelicopterHoldCameraScript> log, INotificationService notifications, ITickManager ticks, IGameInputManager input)
        {
            _log = log;
            _notifications = notifications;
            _ticks = ticks;
            _input = input;
        }

        protected override Task OnStartAsync()
        {
            //_log.Debug("Helicopter hold camera script enabled");

            _ticks.On(HelicopterHoldCameraTick);

            return Task.FromResult(0);
        }

        Task HelicopterHoldCameraTick()
        {
            var curVeh = Game.PlayerPed.CurrentVehicle;
            if (curVeh is { ClassType: VehicleClass.Helicopters } || curVeh is { ClassType: VehicleClass.Planes })
            {
                if (_input.IsJustBeingHeld(Control.NextCamera))
                {
                    _enabled = !_enabled;
                    var message = _enabled ? "Static Camera Enabled" : "Static Camera Disabled";
                    _notifications.Info("Camera", message);

                    var gpRot = API.GetGameplayCamRot(0);
                    _holdHeading = gpRot.Z;
                    _holdPitch = gpRot.Y;
                }

                if (_enabled)
                {
                    if (curVeh.Handle == -1)
                    {
                        _enabled = false;
                    }

                    _camera ??= World.CreateCamera(Vector3.Zero, Vector3.Zero, API.GetGameplayCamFov());

                    var x = _input.GetControlNormal(Control.ScaledLookLeftRight) * -1;
                    var y = _input.GetControlNormal(Control.ScaledLookUpDown) * -1;

                    _holdPitch = MathUtil.Clamp(_holdPitch + y * turnSpeed * Game.LastFrameTime, -60f, 60f);
                    _holdHeading = (_holdHeading + x * turnSpeed * Game.LastFrameTime) % 360;

                    var vehicleRot = curVeh.Rotation;
                    //API.SetGameplayCamRelativeRotation(0f, _holdHeading - Game.PlayerPed.CurrentVehicle.Heading, _holdPitch - vehicleRot.X);
                    API.SetGameplayCamRelativePitch(_holdPitch - vehicleRot.X, 0.2f);
                    API.SetGameplayCamRelativeHeading(_holdHeading - curVeh.Heading);

                    _camera.Position = API.GetGameplayCamCoord();
                    _camera.PointAt(curVeh, Vector3.Zero);
                    //var gpRot = API.GetGameplayCamRot(0);
                    //_camera.Rotation = new Vector3(
                    //    gpRot.X,
                    //    0.0f,
                    //    gpRot.Z
                    //);
                    //var dir = Game.PlayerPed.Position - _camera.Position + Vector3.ForwardLH * 4f;
                    //dir.Normalize();
                    //var rot = PmpMath.DirectionToRotationFixed(dir, 0f);
                    //_camera.Rotation = rot;

                    if (API.GetFollowVehicleCamZoomLevel() == 4)
                    {
                        if (World.RenderingCamera == _camera)
                            World.RenderingCamera = null;
                    }
                    else if (World.RenderingCamera != _camera)
                    {
                        World.RenderingCamera = _camera;
                    }
                }
            }
            else if (_enabled)
            {
                _enabled = false;
            }

            if (!_enabled && _camera?.IsActive == true)
            {
                World.RenderingCamera = null;
                _camera.Delete();
                _camera = null;
            }

            return Task.FromResult(0);
        }
    }
}
