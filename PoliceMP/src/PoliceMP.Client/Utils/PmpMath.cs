using System;
using CitizenFX.Core;

namespace PoliceMP.Client.Utils
{
    public static class PmpMath
    {
        public static Vector3 DirectionToRotationFixed(Vector3 direction, float roll)
        {
            direction = Vector3.Normalize(direction);
            Vector3 rotpos = Vector3.Normalize(new Vector3(direction.Z,  (float)Math.Sqrt(direction.X * direction.X + direction.Y * direction.Y), 0.0f));
            return new Vector3(
                MathUtil.RadiansToDegrees((float)Math.Atan2(rotpos.X, rotpos.Y)),
                roll,
                -MathUtil.RadiansToDegrees((float)Math.Atan2(direction.X, direction.Y))
            );
        }
    }
}
