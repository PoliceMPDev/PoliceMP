using System.Threading.Tasks;
using CitizenFX.Core;
using CitizenFX.Core.Native;
using PoliceMP.Core.Client.Interface;
using PoliceMP.Core.Client.Scripts;

namespace PoliceMP.Client.Utils.CameraUtils
{
    public abstract class BaseCamera
    {
        public Camera GameCamera { get; private set; }
        private bool _enabled = false;

        public Vector3 Position
        {
            get => GameCamera?.Position ?? _startingPosition;
            set
            {
                if (GameCamera != null)
                    GameCamera.Position = value;
                else
                    _startingPosition = value;
            }
        }

        public Vector3 Rotation
        {
            get => GameCamera?.Rotation ?? _startingRotation;
            set
            {
                if (GameCamera != null)
                    GameCamera.Rotation = value;
                else
                    _startingRotation = value;
            }
        }

        public float FieldOfView
        {
            get => GameCamera?.FieldOfView ?? _startingFov;
            set
            {
                if (GameCamera != null)
                    GameCamera.FieldOfView = value;
                else
                    _startingFov = value;
            }
        }

        private readonly ITickManager _tickManager;
        private Vector3 _startingPosition;
        private Vector3 _startingRotation;
        private float _startingFov;

        protected BaseCamera()
        {
            _startingPosition = API.GetGameplayCamCoord();
            _startingRotation = API.GetGameplayCamRot(0);
            _startingFov = API.GetGameplayCamFov();
        }

        protected BaseCamera(ITickManager tickManager, Vector3 position, Vector3 rotation, float? fieldOfView = null)
        {
            _tickManager = tickManager;
            _startingPosition = position;
            _startingRotation = rotation;
            _startingFov = fieldOfView ?? API.GetGameplayCamFov();
        }

        public virtual void Enable(bool ease = false, int easeTime = 300)
        {
            if (GameCamera == null)
                GameCamera = World.CreateCamera(_startingPosition, _startingRotation, _startingFov);

            _tickManager.On(DoTick);

            GameCamera.IsActive = true;
            API.RenderScriptCams(true, ease, easeTime, true, false);

            _enabled = true;
        }

        public async Task EnableAndWait(bool ease = false, int easeTime = 300)
        {
            Enable(ease, easeTime);
            while (GameCamera is null || !GameCamera.IsActive || World.RenderingCamera != GameCamera)
            {
                await Script.Delay(0);
            }
        }

        public virtual void Disable(bool ease = false, int easeTime = 300)
        {
            _enabled = false;

            if (World.RenderingCamera == GameCamera || World.RenderingCamera.Handle == -1)
            {
                API.RenderScriptCams(false, ease, easeTime, true, false);
            }

            _tickManager.Off(DoTick);

            API.ClearFocus();
        }

        public async Task DisableAndWait(bool ease = false, int easeTime = 300)
        {
            Disable(ease, easeTime);
            while (GameCamera is not null && GameCamera.IsActive && World.RenderingCamera == GameCamera)
            {
                await Script.Delay(0);
            }
        }

        public bool IsEnabled()
        {
            if (GameCamera == null 
                || !GameCamera.IsActive
                || World.RenderingCamera != GameCamera) 
                return false;

            return World.RenderingCamera == GameCamera;
        }

        public void Dispose()
        {
            Disable();
            GameCamera?.Delete();
        }

        public async Task InterpToAndWait(Camera to, int duration, int easePosition, int easeRotation)
        {
            GameCamera.InterpTo(to, duration, easePosition, easeRotation);

            while (GameCamera.IsActive || GameCamera.IsInterpolating)
            {
                await Task.Delay(0);
            }
        }

        // Wrapping so we get derived implementations
        private Task DoTick() => CameraTick();

        protected virtual Task CameraTick()
        {
            if (IsEnabled())
            {
                var pos = Position;
                API.SetFocusPosAndVel(pos.X, pos.Y, pos.Z, 0, 0, 0);
            }

            return Task.FromResult(0);
        }
    }
}
