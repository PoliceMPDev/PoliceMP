using System;
using System.Threading.Tasks;
using CitizenFX.Core;
using CitizenFX.Core.Native;

namespace PoliceMP.Core.Client.Abstraction
{
    public static class PathUtils
    {
        public class PulloverPositionQueryResult
        {
            public Vector3 NodePosition { get; }
            public float NodeHeading { get; }
            public Vector3 RoadsidePosition { get; }
            public float RoadsideHeading { get; }
            public PathNodeFlags NodeFlags { get; }

            public PulloverPositionQueryResult(Vector3 nodePosition, float nodeHeading, Vector3 roadsidePosition, float roadsideHeading, PathNodeFlags nodeFlags)
            {
                NodePosition = nodePosition;
                NodeHeading = nodeHeading;
                RoadsidePosition = roadsidePosition;
                RoadsideHeading = roadsideHeading;
                NodeFlags = nodeFlags;
            }
        }

        public static async Task<PulloverPositionQueryResult> TryGetPulloverPosition(Vehicle vehicle, float minTravelDistance,
            float maxTravelDistance, int depth = 100)
        {
            var p = vehicle.Position;
            var heading = vehicle.Heading;

            var offset = Vector3.Up * (vehicle.Velocity + 30);
            var worldCoords = vehicle.GetOffsetPosition(offset);
            API.DrawDebugLine(p.X, p.Y, p.Z, worldCoords.X, worldCoords.Y, worldCoords.Z, 255, 90, 90, 200);
            PathNodeFlags nodeFlags = 0;

            var current = 0;
            var currentDistance = float.MaxValue;

            var nodePosition = Vector3.Zero;
            var nodeHeading = 0f;
            var roadsidePosition = Vector3.Zero;
            var roadsideHeading = 0f;
            bool foundPoint = false;

            while (current < depth)
            {
                current++;
                Vector3 testNodePos = Vector3.Zero;
                float testNodeHeading = 0f;
                if (!API.GetNthClosestVehicleNodeFavourDirection(
                        p.X, p.Y, p.Z,
                        worldCoords.X, worldCoords.Y, worldCoords.Z,
                        current, ref testNodePos, ref testNodeHeading,
                        (int)(NodeSearchFlags.IgnoreDeadEnds | NodeSearchFlags.IgnoreSecondaryNodes), 0x40400000, 0))
                {
                    break;
                }
                //var testNodeId = API.GetNthClosestVehicleNodeIdWithHeading(p.X, p.Y, p.Z, current, ref testNodePos,
                //    heading, 9, 3.0f, 2.5f);

                if (!IsNodeSuitableForPullover(vehicle, testNodePos, testNodeHeading, out var testNodeRoadsidePos, out var testNodeRoadsideHeading, out var flags))
                {
                    await BaseScript.Delay(0);
                    continue;
                }
                 
                var distance = World.CalculateTravelDistance(vehicle.Position, testNodePos);

                if (distance < minTravelDistance || distance > maxTravelDistance)
                {
                    Debug.WriteLine("Too close or far away");
                    await BaseScript.Delay(0);
                    continue;
                }

                var directionToRoadside = roadsidePosition + Vector3.Up * 2 - nodePosition;
                directionToRoadside.Normalize();
                var raycastResult = World.Raycast(nodePosition + Vector3.Up * 2, directionToRoadside, World.GetDistance(nodePosition, roadsidePosition),
                    IntersectOptions.Objects | IntersectOptions.Map);

                if (raycastResult.DitHit)
                {
                    await BaseScript.Delay(0);
                    continue;
                }

                if (distance < currentDistance)
                {
                    nodePosition = testNodePos;
                    nodeHeading = testNodeHeading;
                    roadsidePosition = testNodeRoadsidePos;
                    roadsideHeading = testNodeRoadsideHeading;
                    foundPoint = true;
                    currentDistance = distance;
                    nodeFlags = flags;
                    Debug.WriteLine("Found a point!");
                }

                await BaseScript.Delay(0);
            }

            Debug.WriteLine($"We have a node! {nodePosition}; {nodeFlags}");
            return foundPoint ? new PulloverPositionQueryResult(nodePosition, nodeHeading, roadsidePosition, roadsideHeading, nodeFlags) : null;
        }

        public static bool IsNodeSuitableForPullover(Vehicle vehicle, Vector3 nodePos, float heading, out Vector3 roadsidePos, out float roadsideHeading, out PathNodeFlags nodeFlags)
        {
            int iNodeFlags = 0;
            nodeFlags = 0;
            int density = 0;
            roadsidePos = Vector3.Zero;
            roadsideHeading = 0f;
            var directionTolerance = 0.3f;
            var trafficLightDistance = 10f;

            if (!API.GetVehicleNodeProperties(nodePos.X, nodePos.Y, nodePos.Z, ref density, ref iNodeFlags))
            {
                return false;
            }

            var direction = nodePos - vehicle.Position;
            direction.Normalize();
            var dot = Vector3.Dot(vehicle.ForwardVector, direction);
            if (dot < directionTolerance)
            {
                return false;
            }

            nodeFlags = (PathNodeFlags)iNodeFlags;

            if ((nodeFlags & PathNodeFlags.TrafficLightStop) != 0
                || (nodeFlags & PathNodeFlags.Stop) != 0
                || (nodeFlags & PathNodeFlags.Junction) != 0
                || (nodeFlags & PathNodeFlags.Tunnel) != 0
                || (nodeFlags & PathNodeFlags.WanderTarget) == 0)
            {
                return false;
            }

            if (!GetRoadsidePosition(nodePos, vehicle, false, out roadsidePos, out roadsideHeading))
            {
                return false;
            }

            var directionToRoadside = roadsidePos - vehicle.Position;
            directionToRoadside.Normalize();

            var headingToNode = GameMath.DirectionToHeading(direction);
            var headingToRoadside = GameMath.DirectionToHeading(directionToRoadside);

            if (headingToNode > headingToRoadside)
                return false;


            //if (!API.GetRoadSidePointWithHeading(nodePos.X, nodePos.Y, nodePos.Z, heading, ref roadsidePos))
            //{
            //    return false;
            //}

            for (int i = 0; i < 10; i++)
            {
                var p = Vector3.Zero;
                if (API.GetNthClosestVehicleNode(nodePos.X, nodePos.Y, nodePos.Z, i, ref p, 0,
                        0x40400000, 0))
                {
                    int trafficTestDensity = 0;
                    int iTrafficLightFlags = 0;
                    if (API.GetVehicleNodeProperties(p.X, p.Y, p.Z, ref trafficTestDensity, ref iTrafficLightFlags))
                    {
                        var trafficLightsFlags = (PathNodeFlags)iTrafficLightFlags;
                        if (((trafficLightsFlags & PathNodeFlags.TrafficLightStop) != 0 || (trafficLightsFlags & PathNodeFlags.Junction) != 0) && World.GetDistance(nodePos, p) < trafficLightDistance)
                        {
                            return false;
                        }
                    }
                }
            }

            return true;

            //var roadsideDirection = roadsidePos - vehicle.Position;
            //roadsideDirection.Normalize();

            //var headingToNode = (GameMath.DirectionToHeading(direction) - vehicle.Heading) % 360;
            //var headingToRoadside = (GameMath.DirectionToHeading(roadsideDirection) - vehicle.Heading) % 360;
            //if (headingToRoadside < headingToNode)
            //{
            //    return false;
            //}

            //roadsideHeading += 180;
            //roadsideHeading %= 360;
        }

        public static bool GetRoadsidePosition(Vector3 pos, Vehicle vehicle, bool onlyMajorRoads, out Vector3 roadsidePos, out float heading)
        {
            roadsidePos = Vector3.Zero;
            Vector3 nodePos = Vector3.Zero;
            Vector3 src = Vector3.Zero;
            Vector3 dest = Vector3.Zero;
            heading = 0f;
            int lanesIn = 0;
            int lanesOut = 0;
            float width = 0f;
            int iFlags = 0;
            int density = 0;
            int totalLanes = 0;
            int nodeType = 1;
            float roadsideOffset = 0f;

            if (!API.GetNthClosestVehicleNodeWithHeading(pos.X, pos.Y, pos.Z, 1, ref nodePos, ref heading,
                    ref totalLanes, nodeType, 3.0f, 0f))
            {
                return false;
            }

            var nodeId = API.GetNthClosestVehicleNodeId(nodePos.X, nodePos.Y, nodePos.Z, 1, nodeType, 3.0f, 1.0f);
            if (!API.IsVehicleNodeIdValid(nodeId))
            {
                return false;
            }

            if (API.GetClosestRoad(pos.X, pos.Y, pos.Z, 0.0f, 2, ref src, ref dest, ref lanesIn, ref lanesOut,
                    ref width, onlyMajorRoads) == 0)
            {
                return false;
            }

            if (World.GetDistance(src, pos) > 0f &&
                World.GetDistance(dest, pos) > 0f)
            {
                return false;
            }

            var nodeHeadingIsNorth = heading < 90f || heading >= 270f;

            bool oneWay = false;
            if (nodeHeadingIsNorth && totalLanes == lanesIn)
            {
                oneWay = true;
            }
            else if (totalLanes == lanesOut)
            {
                oneWay = true;
            }

            if (width >= 0f)
            {
                var lanes = nodeHeadingIsNorth ? lanesIn : lanesOut;

                if (oneWay)
                {
                    roadsideOffset = 5.5f * lanes * 0.5f;

                    if (lanesIn > 2)
                        roadsideOffset += lanes - 2;
                }
                else
                {
                    roadsideOffset = 5.5f * lanes;

                    if (lanesIn > 1)
                        roadsideOffset += lanes - 1;
                }
            }

            if (!API.GetVehicleNodeProperties(nodePos.X, nodePos.Y, nodePos.Z, ref density, ref iFlags))
            {
                return false;
            }

            var flags = (PathNodeFlags)iFlags;
            if ((flags & PathNodeFlags.Highway) != 0)
                roadsideOffset += 0.9f * width;

            if ((flags & PathNodeFlags.NoBigVehicles) != 0)
                roadsideOffset += -0.7f;

            roadsidePos = API.GetObjectOffsetFromCoords(nodePos.X, nodePos.Y, nodePos.Z, heading, roadsideOffset, 0f, 0f);

            if (Math.Abs(World.GetGroundHeight(nodePos) - World.GetGroundHeight(roadsidePos)) > 5f)
            {
                return false;
            }

            Debug.WriteLine($"Width {width}, ({API.GetVehicleNodeIsSwitchedOff(nodeId)}, {!API.GetVehicleNodeIsGpsAllowed(nodeId)}) Offset {roadsideOffset}, Heading = {heading}, IsNorth = {nodeHeadingIsNorth}, LanesIn = {lanesIn}, LanesOut = {lanesOut}");
            return !API.IsPointObscuredByAMissionEntity(
                roadsidePos.X,
                roadsidePos.Y,
                roadsidePos.Z,
                3f,
                3f,
                3f,
                vehicle.Handle);
        }

        public static Vector3 RotateBy(Vector3 v, float degrees)
        {
            var radians = MathUtil.DegreesToRadians(degrees);
            var c = (float)System.Math.Cos(radians);
            var s = (float)System.Math.Sin(radians);

            return new Vector3(
                v.X * c - v.Y * s,
                v.X * s + v.Y * c,
                v.Z);
        }
    }
}
