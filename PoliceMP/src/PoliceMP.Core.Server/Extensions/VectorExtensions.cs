using CitizenFX.Core;

namespace PoliceMP.Core.Server.Extensions
{
    public static class VectorExtensions
    {
        public static Shared.Models.PmpVector3 ToNetworkVector(this Vector3 vector)
        {
            return new Shared.Models.PmpVector3(vector.X, vector.Y, vector.Z);
        }

        public static Shared.Models.PmpVector2 ToNetworkVector(this Vector2 vector)
        {
            return new Shared.Models.PmpVector2(vector.X, vector.Y);
        }
    }
}