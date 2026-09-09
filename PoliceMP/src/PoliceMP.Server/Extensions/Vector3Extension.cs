using CitizenFX.Core;
using System;

namespace PoliceMP.Server.Extensions
{
    public static class Vector3Extension
    {
        public static float Distance(this Vector3 position, Vector3 targetPosition, bool ignoreZ = false)
        {
            var diffX = position.X - targetPosition.X;
            var diffY = position.Y - targetPosition.Y;
            if (!ignoreZ)
            {
                var diffZ = position.Z - targetPosition.Z;
                var sum = diffX * diffX + diffY * diffY + diffZ * diffZ;
                return (float)Math.Sqrt(sum);
            }
            
            var sum3D = diffX * diffX + diffY * diffY;
            return (float)Math.Sqrt(sum3D);
        }
    }
}
