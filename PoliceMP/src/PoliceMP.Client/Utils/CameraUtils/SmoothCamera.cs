using System;
using System.Threading.Tasks;
using CitizenFX.Core;
using CitizenFX.Core.Native;
using PoliceMP.Core.Client.Interface;

namespace PoliceMP.Client.Utils.CameraUtils
{
    public class SmoothCamera : BaseCamera
    {
        private readonly ITickManager _tickManager;
        public float SmoothLookModifier { get; set; }
        public float SmoothMoveModifier { get; set; }

        // updates TargetDirection every frame
        private Entity _lookEntity;

        // Override the base position and rotation to smooth the movement and rotation
        public Vector3 TargetPosition { get; private set; }
        public Vector3 TargetDirection { get; private set; }

        private Entity _movementEntity;

        public SmoothCamera(ITickManager tickManager, Vector3 position, Vector3 rotation, float? fov = null, float smoothLookModifier = 1f, float smoothMoveModifier = 1f)
            : base(tickManager, position, rotation, fov)
        {
            _tickManager = tickManager;
            SmoothLookModifier = smoothLookModifier;
            SmoothMoveModifier = smoothMoveModifier;
        }

        protected override Task CameraTick()
        {
            base.CameraTick();

            if (_lookEntity != null)
                TargetDirection = PmpMath.DirectionToRotationFixed(_lookEntity.Position - base.Position, 0f);

            LookAtTick();
            MoveTick();

            return Task.FromResult(0);
        }

        private void LookAtTick()
        {
            var cameraDirection = GameMath.RotationToDirection(Rotation);
            var targetDirection = TargetDirection;

            var newDir = SmoothLookModifier > 0f ?
                Vector3.Lerp(cameraDirection, targetDirection, MathUtil.Clamp(SmoothLookModifier * Game.LastFrameTime, 0f, 1f)) :
                TargetDirection;

            Rotation = PmpMath.DirectionToRotationFixed(newDir, 0f);

            //Debug.WriteLine($"({oldRot}).Lerp(({TargetDirection})) == ({newRot})");
        }

        private void MoveTick()
        {
            Position = SmoothMoveModifier > 0
                ? Vector3.Lerp(Position, TargetPosition, MathUtil.Clamp(SmoothMoveModifier * Game.LastFrameTime, 0f, 1f))
                : TargetPosition;
        }

        public override void Disable(bool ease = false, int easeTime = 500)
        {
            base.Disable(ease, easeTime);
            ClearLookAt();
        }

        public void LookAt(Entity entity, bool instant = false)
        {
            _lookEntity = entity;

            if (instant)
            {
                var dir = entity.Position - Position;
                dir.Normalize();
                Rotation = PmpMath.DirectionToRotationFixed(dir, 0f);
            }
        }

        public void LookAt(Vector3 position, bool instant = false)
        {
            var dir = position - base.Position;
            dir.Normalize();
            TargetDirection = dir;
            _lookEntity = null;

            if (instant)
            {
                Rotation = PmpMath.DirectionToRotationFixed(TargetDirection, 0f);

            }
        }

        public void ClearLookAt()
        {
            _lookEntity = null;
        }

        public void AttachTo(Entity entity, bool instant = false)
        {
            _movementEntity = entity;

            if (instant)
                Position = entity.Position;
        }

        public void MoveTo(Vector3 position, bool instant = false)
        {
            _movementEntity = null;
            TargetPosition = position;

            if (instant)
                Position = position;
        }

        public void SetRotation(Vector3 rotation, bool instant = false)
        {
            TargetDirection = GameMath.RotationToDirection(rotation);

            if (instant)
                Rotation = rotation;
        }
    }
}
