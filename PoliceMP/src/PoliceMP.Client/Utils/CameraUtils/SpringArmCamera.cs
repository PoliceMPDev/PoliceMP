using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CitizenFX.Core;
using CitizenFX.Core.Native;
using CitizenFX.Core.UI;
using PoliceMP.Core.Client.Interface;

namespace PoliceMP.Client.Utils.CameraUtils
{
    public class SpringArmCamera : IDisposable
    {
        public Vector3 Position { get; set; }
        public Vector3 Rotation { get; set; }

        public float Length
        {
            get => _length;
            set => _length = Math.Max(0, value);
        }

        
        public bool IgnorePlayerPed { get; set; }
        public bool SyncGameplayCameraDirection { get; set; }
        public bool RaycastCheck { get; set; }
        public SmoothCamera Camera { get; private set; }

        private readonly ITickManager _tickManager;
        private readonly float _size;
        private readonly float _smoothMoveModifier;
        private Entity _attachedEntity = null;

        private IntersectOptions intersectOptions =
            IntersectOptions.Map |
            IntersectOptions.MissionEntities |
            IntersectOptions.Objects;

        private float _length;

        public bool IsEnabled() => Camera.IsEnabled();
        public void Dispose() => Camera.Dispose();

        public SpringArmCamera(
            ITickManager tickManager, 
            Vector3 position, 
            Vector3 rotation, 
            float length, 
            float size, 
            float? fov = null, 
            float smoothLookModifier = -1f,
            float smoothMoveModifier = 1f, 
            bool ignorePlayerPed = true,
            bool syncGameplayCameraDirection = true,
            bool raycastCheck = true)
        {
            Position = position;
            Rotation = rotation;
            Length = length;
            IgnorePlayerPed = ignorePlayerPed;
            SyncGameplayCameraDirection = syncGameplayCameraDirection;
            RaycastCheck = raycastCheck;
            _tickManager = tickManager;
            _size = size;
            _smoothMoveModifier = smoothMoveModifier;
            Camera = new SmoothCamera(tickManager, GetCameraPosition(), GetCameraRotation(), fov, smoothLookModifier,
                smoothMoveModifier);
        }

        private Task SpringArmCameraTick()
        {
            if (!IsEnabled())
                return Task.FromResult(0);

            if (_attachedEntity != null && _attachedEntity.Handle != 0)
                Position = _attachedEntity.Position;

            var pos = GetCameraPosition();
            var rot = GetCameraRotation();
            Camera.MoveTo(pos);
            Camera.SetRotation(rot);

            if (SyncGameplayCameraDirection)
            {
                var heading = GameMath.DirectionToHeading(rot);
                API.SetGameplayCamRelativeHeading(heading - Game.PlayerPed.Heading + 90);
            }

            return Task.FromResult(0);
        }

        public void Enable(bool ease = false, int easeTime = 300)
        {
            _tickManager.On(SpringArmCameraTick);
            Camera.Enable();
        }

        public async Task EnableAndWait(bool ease, int easeTime)
        {
            _tickManager.On(SpringArmCameraTick);
            await Camera.EnableAndWait(ease, easeTime);
        }

        public void Disable(bool ease = false, int easeTime = 300)
        {
            _tickManager.Off(SpringArmCameraTick);
            Camera.Disable();
        }

        public async Task DisableAndWait(bool ease, int easeTime)
        {
            _tickManager.Off(SpringArmCameraTick);
            await Camera.DisableAndWait(ease, easeTime);
        }

        Vector3 GetCameraTargetPosition()
        {
            var dir = -1 * GameMath.RotationToDirection(Rotation);
            return Position + dir * Length;
        }

        Vector3 GetCameraPosition()
        {
            var targetPos = GetCameraTargetPosition();
            if (Camera != null && RaycastCheck)
            {
                var dir = Camera.TargetPosition - Position;
                dir.Normalize();

                Entity ignoreEntity = null;

                if (IgnorePlayerPed)
                {
                    ignoreEntity = Game.PlayerPed.CurrentVehicle != null ? Game.PlayerPed.CurrentVehicle : Game.PlayerPed;
                }

                var raycastResult = World.Raycast(Position, dir, Length, intersectOptions, ignoreEntity);
                return raycastResult.DitHit ? raycastResult.HitPosition + -1 * dir * _size : Position + dir * Length;
            }

            return targetPos;
        }

        Vector3 GetCameraRotation()
        {
            var dir = Position - GetCameraTargetPosition();
            dir.Normalize();

            return PmpMath.DirectionToRotationFixed(dir, 0f);
        }

        public void AttachTo(Entity entity)
        {
            _attachedEntity = entity;
        }

        public void ResetCamera()
        {
            Camera.MoveTo(GetCameraTargetPosition(), true);
        }

        // Moves relative to camera rotation
        public void LocalMove(Vector3 movement)
        {
            var direction = GameMath.RotationToDirection(Rotation);
            Position += movement * direction;
        }
    }
}
