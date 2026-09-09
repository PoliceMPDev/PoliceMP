using CitizenFX.Core;
using CitizenFX.Core.UI;
using Color = System.Drawing.Color;
using Vector3 = CitizenFX.Core.Vector3;

namespace PoliceMP.Core.Client
{
    public static class DebugUtils
    {
        public static bool DebugEnabled { get; set; }
        public static void DrawDebugAngledArea(float x1, float y1, float z1, float x2, float y2, float z2,
            float width, Color color)
        {
            DrawDebugAngledArea(new Vector3(x1, y1, z2), new Vector3(x2, y2, z2), width, color,
                Color.FromArgb(255, 255, 255, 255));
        }

        public static void DrawDebugAngledArea(float x1, float y1, float z1, float x2, float y2, float z2,
            float width, Color color, Color lineColor)
        {
            DrawDebugAngledArea(new Vector3(x1, y1, z2), new Vector3(x2, y2, z2), width, color, lineColor);
        }

        public static void DrawDebugAngledArea(Vector3 origin, Vector3 extent, float width, Color color)
        {
            DrawDebugAngledArea(origin, extent, width, color, Color.FromArgb(255, 255, 255, 255));
        }

        public static void DrawDebugAngledArea(Vector3 origin, Vector3 extent, float width, Color color,
            Color lineColor)
        {
            float halfWidth = width / 2;

            var midPoint = (extent - origin) / 2;
            var height = midPoint.Z * 2;
            var dir = midPoint;
            dir.Normalize();

            // Calculate the right angle direction
            Vector3 forward = Vector3.Cross(dir, Vector3.Up);
            forward.Normalize();


            Vector3 offset = forward * halfWidth;
            World.DrawMarker(MarkerType.DebugSphere, origin + offset, Vector3.Zero, Vector3.Zero, Vector3.One * 0.5f,
                Color.FromArgb(255, 255, 0, 255));

            var heightVector = new Vector3(0f, 0f, height);
            Vector3[] vertices = new Vector3[8]
            {
                origin + offset,
                origin + offset + heightVector,
                extent + offset,
                extent + offset - heightVector,
                origin - offset,
                origin - offset + heightVector,
                extent - offset,
                extent - offset - heightVector,
            };

            int[,] edges = new int[,]
                {{0, 1}, {1, 2}, {2, 3}, {3, 0}, {4, 5}, {5, 6}, {6, 7}, {7, 4}, {0, 4}, {1, 5}, {2, 6}, {3, 7}};

            int[,] faces = new int[,]
            {
                {0, 1, 5}, {0, 5, 4},
                {1, 2, 6}, {1, 6, 5},
                {2, 3, 7}, {2, 7, 6},
                {3, 0, 4}, {3, 4, 7},
                {4, 5, 6}, {4, 6, 7},
                {0, 2, 1}, {0, 3, 2},
            };

            for (int i = 0; i < edges.GetLength(0); i++)
            {
                World.DrawLine(vertices[edges[i, 0]], vertices[edges[i, 1]], lineColor);
            }

            for (int i = 0; i < faces.GetLength(0); i++)
            {
                World.DrawPoly(vertices[faces[i, 0]], vertices[faces[i, 1]], vertices[faces[i, 2]], color);
            }


            World.DrawMarker(MarkerType.DebugSphere, origin, Vector3.Zero, Vector3.Zero, Vector3.One * 0.05f,
                Color.FromArgb(255, 100, 255, 100));
            World.DrawMarker(MarkerType.DebugSphere, extent, Vector3.Zero, Vector3.Zero, Vector3.One * 0.05f,
                Color.FromArgb(255, 255, 100, 100));

            var originPos = Screen.WorldToScreen(origin);
            if (originPos.X + originPos.Y != 0f)
            {
                new Text("origin", originPos, 0.2f).Draw();
            }

            var extentPos = Screen.WorldToScreen(extent);
            if (extentPos.X + extentPos.Y != 0f)
            {
                new Text("extent", Screen.WorldToScreen(extent), 0.2f).Draw();
            }
        }
    }
}