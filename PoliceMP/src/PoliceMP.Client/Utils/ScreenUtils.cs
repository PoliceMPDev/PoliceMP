using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CitizenFX.Core;
using CitizenFX.Core.Native;
using CitizenFX.Core.NaturalMotion;
using CitizenFX.Core.UI;

namespace PoliceMP.Client.Utils
{
    public static class ScreenUtils
    {
        public static Vector3 ScreenRelToWorld(Camera camera, Vector2 coord, out Vector3 forwardDirection)
        {
            return ScreenRelToWorld(camera.Position, camera.Rotation, coord, out forwardDirection);
        }
        
        public static Vector3 ScreenRelToWorld(Vector3 pos, Vector3 rot, Vector2 coord, out Vector3 forwardDirection)
        {
            var camForward = RotationToDirection(rot);
            var rotUp = rot + new Vector3(1, 0, 0);
            var rotDown = rot + new Vector3(-1, 0, 0);
            var rotLeft = rot + new Vector3(0, 0, -1);
            var rotRight = rot + new Vector3(0, 0, 1);

            var camRight = RotationToDirection(rotRight) - RotationToDirection(rotLeft);
            var camUp = RotationToDirection(rotUp) - RotationToDirection(rotDown);

            var rollRad = -DegToRad(rot.Y);

            var camRightRoll = camRight * (float)Math.Cos(rollRad) - camUp * (float)Math.Sin(rollRad);
            var camUpRoll = camRight * (float)Math.Sin(rollRad) + camUp * (float)Math.Cos(rollRad);

            var point3D = pos + camForward * 1.0f + camRightRoll + camUpRoll;

            float p2dx = 0f, p2dy = 0f;
            if (!API.World3dToScreen2d(point3D.X, point3D.Y, point3D.Z, ref p2dx, ref p2dy))
            {
                forwardDirection = camForward;
                return pos + camForward * 1.0f;
            }
            Vector2 point2D = new Vector2(p2dx, p2dy);

            var point3DZero = pos + camForward * 1.0f;
            if (!API.World3dToScreen2d(point3DZero.X, point3DZero.Y, point3DZero.Z, ref p2dx, ref p2dy))
            {
                forwardDirection = camForward;
                return pos + camForward * 1.0f;
            }
            Vector2 point2DZero = new Vector2(p2dx, p2dy);

            const double eps = 0.001;
            if (Math.Abs(point2D.X - point2DZero.X) < eps || Math.Abs(point2D.Y - point2DZero.Y) < eps)
            {
                forwardDirection = camForward;
                return pos + camForward * 1.0f;
            }
            var scaleX = (coord.X - point2DZero.X) / (point2D.X - point2DZero.X);
            var scaleY = (coord.Y - point2DZero.Y) / (point2D.Y - point2DZero.Y);
            var point3Dret = pos + camForward * 1.0f + camRightRoll * scaleX + camUpRoll * scaleY;
            forwardDirection = camForward + camRightRoll * scaleX + camUpRoll * scaleY;
            return point3Dret;
        }
        
        

        public static float DegToRad(float _deg)
        {
            double Radian = (Math.PI / 180) * _deg;
            return (float)Radian;
        }

        public static Vector3 RotationToDirection(Vector3 rotation)
        {
            var z = DegToRad(rotation.Z);
            var x = DegToRad(rotation.X);
            var num = Math.Abs(Math.Cos(x));
            return new Vector3
            {
                X = (float)(-Math.Sin(z) * num),
                Y = (float)(Math.Cos(z) * num),
                Z = (float)Math.Sin(x)
            };
        }
    }
}
