using CitizenFX.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PoliceMP.Client.Extensions
{
    public static class QuaternionTools
    {
        public static Quaternion AngleAxis(float angle, Vector3 axis)
        {
            axis.Normalize();
            float rad = Deg2Rad(angle) * 0.5f;
            float sin = (float)Math.Sin(rad);
            float cos = (float)Math.Cos(rad);
            return new Quaternion(axis.X * sin, axis.Y * sin, axis.Z * sin, cos);
        }

        public static Quaternion AngleAxis(this Quaternion quaternion, float angle, Vector3 axis)
        {
            return AngleAxis(angle, axis);
        }
        
        public static float GetAngleFromAxis(this Quaternion q, Vector3 axis)
        {
            q.Normalize();
            // Normalize the input axis
            axis.Normalize();

            // Create a reference quaternion
            Quaternion reference = AngleAxis(0, axis);

            // Calculate the dot product of the input quaternion and the reference quaternion
            float dot = Quaternion.Dot(q, reference);

            // Calculate the angle (in radians)
            float angleRad = 2.0f * (float)Math.Acos(dot);

            // Convert the angle to degrees
            float angleDeg = angleRad * (180.0f / (float)Math.PI);

            return angleDeg;
        }

        private static float Deg2Rad(float degrees)
        {
            return degrees * (float)Math.PI / 180.0f;
        }

        public static Vector3 ToEulerAngles(this Quaternion q)
        {
            // Convert the quaternion to Euler angles (in radians)
            double ysqr = q.Y * q.Y;

            // Roll (x-axis rotation)
            double t0 = 2.0f * (q.W * q.X + q.Y * q.Z);
            double t1 = 1.0f - 2.0f * (q.X * q.X + ysqr);
            double roll = Math.Atan2(t0, t1);

            // Pitch (y-axis rotation)
            double t2 = 2.0f * (q.W * q.Y - q.Z * q.X);
            t2 = t2 > 1.0f ? 1.0f : t2;
            t2 = t2 < -1.0f ? -1.0f : t2;
            double pitch = Math.Asin(t2);

            // Yaw (z-axis rotation)
            double t3 = 2.0f * (q.W * q.Z + q.X * q.Y);
            double t4 = 1.0f - 2.0f * (ysqr + q.Z * q.Z);
            double yaw = Math.Atan2(t3, t4);

            return new Vector3((float)(roll * (180.0 / Math.PI)), (float)(pitch * (180.0 / Math.PI)), (float)(yaw * (180.0 / Math.PI)));
        }

        public static Quaternion FromEuler(Vector3 euler)
        {
            // Convert degrees to radians
            float pitch = MathUtil.DegreesToRadians(euler.X);
            float yaw = MathUtil.DegreesToRadians(euler.Y);
            float roll = MathUtil.DegreesToRadians(euler.Z);

            float sinPitch = (float)Math.Sin(pitch * 0.5f);
            float cosPitch = (float)Math.Cos(pitch * 0.5f);
            float sinYaw = (float)Math.Sin(yaw * 0.5f);
            float cosYaw = (float)Math.Cos(yaw * 0.5f);
            float sinRoll = (float)Math.Sin(roll * 0.5f);
            float cosRoll = (float)Math.Cos(roll * 0.5f);

            Quaternion q;
            q.X = sinRoll * cosPitch * cosYaw - cosRoll * sinPitch * sinYaw;
            q.Y = cosRoll * sinPitch * cosYaw + sinRoll * cosPitch * sinYaw;
            q.Z = cosRoll * cosPitch * sinYaw - sinRoll * sinPitch * cosYaw;
            q.W = cosRoll * cosPitch * cosYaw + sinRoll * sinPitch * sinYaw;

            return q;
        }
    }
}
