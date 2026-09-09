using CitizenFX.Core;
using PoliceMP.Core.Client.Interface;

namespace PoliceMP.Client.Utils.CameraUtils
{
    public class BasicCamera : BaseCamera
    {
        public BasicCamera(ITickManager tickManager, Vector3 position, Vector3 rotation, float? fieldOfView = null)
            : base(tickManager, position, rotation, fieldOfView)
        {
        }
    }
}