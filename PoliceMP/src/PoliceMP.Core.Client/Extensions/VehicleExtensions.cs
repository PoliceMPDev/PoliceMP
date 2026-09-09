using System;
using System.Drawing;
using System.Threading;
using System.Threading.Tasks;
using CitizenFX.Core;
using CitizenFX.Core.Native;
using PoliceMP.Core.Client.Abstraction;
using PoliceMP.Shared.Constants.Decors;
using PoliceMP.Shared.Constants.States;
using Vector3 = CitizenFX.Core.Vector3;

namespace PoliceMP.Core.Client.Extensions
{
    public static class VehicleExtensions
    {
        public static int GetDriverPedNetworkId(this Vehicle vehicle)
        {
            var driverPed = vehicle.GetPedOnSeat(VehicleSeat.Driver);
            return driverPed == null ? -1 : driverPed.NetworkId;
        }

        public static int GetMph(this Vehicle entity) => (int)Math.Round(entity.Speed * 2.2369);

        public static string GetColourNameWithoutMetallic(this Vehicle vehicle)
        {
            string colour = vehicle.Mods.PrimaryColor.ToString();

            if (colour.ToUpper().Contains("METALLIC"))
                colour = colour.Substring(8);

            return colour;
        }

        public static bool HasGeneratedFaults(this Vehicle vehicle)
            => vehicle.GetBoolDecor(VehicleDecors.HasGeneratedFaults);

        public static bool HasBritishPlate(this Vehicle vehicle)
            => vehicle.GetBoolDecor(VehicleDecors.HasBritishPlate);

        public static void SetPlateText(this Vehicle vehicle, string text)
            => API.SetVehicleNumberPlateText(vehicle.Handle, text);

        public static string GetPlateText(this Vehicle vehicle)
            => API.GetVehicleNumberPlateText(vehicle.Handle);

        public static bool IsLegal(this Vehicle vehicle)
            => vehicle.ClassType == VehicleClass.Emergency ||
               vehicle.Model == VehicleHash.Taxi ||
               vehicle.Model == VehicleHash.Burrito2 ||
               vehicle.Model == VehicleHash.TowTruck ||
               vehicle.Model == VehicleHash.TowTruck2;

        public static bool HasDriver(this Vehicle vehicle)
            => vehicle.Driver != null && vehicle.Driver.Handle != 0;

        public static void SetEngineState(this Vehicle vehicle, bool turnOn, bool instantly = true, bool disableAutoStart = true)
        {
            API.SetVehicleEngineOn(vehicle.Handle, turnOn, instantly, disableAutoStart);
        }

        static bool CanPark(this Vehicle vehicle, Vector3 pos)
        {
            var direction = pos - vehicle.Position;
            direction.Normalize();

            var result = World.Raycast(vehicle.Position, direction,
                World.GetDistance(vehicle.Position + Vector3.ForwardLH, pos + Vector3.ForwardLH),
                IntersectOptions.Everything, vehicle);

            return !result.DitHit;
        }

        public static async Task PullOver(this Vehicle vehicle, CancellationToken ct = default)
        {
            API.TaskVehicleDriveWander(vehicle.Driver.Handle, vehicle.Handle, 15f, 427);

            var result = await PathUtils.TryGetPulloverPosition(vehicle, 30f + (vehicle.Speed * 3), 3000f);
            if (result == null)
            {
                await PullOver(vehicle, ct);
                return;
            }

            if (result != null)
            {
                var setupRadius = (result.NodeFlags & PathNodeFlags.Highway) != 0 ? 60f : 5f;
                var drivingStyle = 427;
                var timeOut = DateTime.Now;
                var roadsidePos = result.RoadsidePosition;
                var roadsideHeading = result.RoadsideHeading;
                var parkTolerance = 0f;
                var stoppingDistance = 1f;
                var timeoutSeconds = 240;
                var blip = World.CreateBlip(roadsidePos);
                var checkpoint = World.CreateCheckpoint(CheckpointIcon.Cyclinder3, roadsidePos, GameMath.HeadingToDirection(roadsideHeading), 2f, System.Drawing.Color.FromArgb(200, 255, 255, 128));
                blip.Color = BlipColor.Yellow;
                blip.IsFlashing = true;
                blip.ShowRoute = true;
    
                var direction = roadsidePos - vehicle.Position;
                direction.Normalize();
                
                //API.TaskVehicleDriveToCoord(
                //    vehicle.Driver.Handle, 
                //    vehicle.Handle, 
                //    result.RoadsidePosition.X,
                //    result.RoadsidePosition.Y,
                //    result.RoadsidePosition.Z,
                //    15f,
                //    0,
                //    0, 
                //    drivingStyle,
                //    2f,
                //    2f);

                API.TaskVehiclePark(
                        vehicle.Driver.Handle,
                        vehicle.Handle,
                        result.RoadsidePosition.X,
                        result.RoadsidePosition.Y,
                        result.RoadsidePosition.Z,
                        result.RoadsideHeading,
                        3,
                        30f,
                        true
                    );

                vehicle.Driver.Task.DriveTo(vehicle, result.RoadsidePosition, stoppingDistance, 15f, drivingStyle);
                //    0, vehicle.Handle,
                vehicle.Driver.BlockPermanentEvents = false;
                vehicle.Driver.AlwaysKeepTask = true;

                while (API.GetDistanceBetweenCoords(vehicle.Position.X, vehicle.Position.Y, vehicle.Position.Z,
                           roadsidePos.X, roadsidePos.Y, roadsidePos.Z, false) > setupRadius
                       && Vector3.Dot(vehicle.ForwardVector, direction) >= parkTolerance
                       && (DateTime.Now - timeOut).TotalSeconds <= timeoutSeconds)
                {
                    //API.DrawLine(vehicle.Position.X, vehicle.Position.Y, vehicle.Position.Z, roadsidePos.X,
                    //    roadsidePos.Y, roadsidePos.Z, 90, 255, 90, 200);

                    //World.DrawMarker(MarkerType.DebugSphere, roadsidePos, Vector3.Zero, Vector3.Zero, Vector3.One * setupRadius, Color.FromArgb(100, 255, 255, 255));

                    if (ct.IsCancellationRequested || Vector3.Dot(vehicle.ForwardVector, direction) < parkTolerance)
                    {
                        //API.DrawLine(vehicle.Position.X, vehicle.Position.Y, vehicle.Position.Z, roadsidePos.X,
                        //    roadsidePos.Y, roadsidePos.Z, 255, 90, 90, 200);

                        blip.Delete();
                        checkpoint.Delete();

                        if (!ct.IsCancellationRequested)
                            await PullOver(vehicle, ct);
                        return;
                    }

                    direction = roadsidePos - vehicle.Position;
                    direction.Normalize();
                    await BaseScript.Delay(0);
                }

                if (Vector3.Dot(vehicle.ForwardVector, direction) < parkTolerance)
                {
                    blip.Delete();
                    checkpoint.Delete();
                    await PullOver(vehicle, ct);
                }

                vehicle.Driver.DrivingSpeed = 10f;
                vehicle.Driver.MaxDrivingSpeed = 10f;
                //if (GameMath.DirectionToHeading(direction) < vehicle.Heading)
                //{
                //    API.DrawLine(vehicle.Position.X, vehicle.Position.Y, vehicle.Position.Z, roadsidePos.X,
                //        roadsidePos.Y, roadsidePos.Z, 255, 90, 90, 200);

                //    await PullOver(vehicle);
                //    return;
                //}
                //vehicle.Driver.Task.Driv(vehicle, roadsidePos, stoppingDistance, 10, 541327679);
                //API.TaskVehicleGotoNavmesh(vehicle.Driver.Handle, vehicle.Handle,
                //    roadsidePos.X, roadsidePos.Y, roadsidePos.Z,
                //    10f, 2359467, stoppingDistance);

                //vehicle.Driver.Task.ParkVehicle(vehicle, roadsidePos, roadsideHeading, 2f);
                //vehicle.Driver.DrivingStyle = (DrivingStyle)drivingStyle;
                vehicle.Driver.AlwaysKeepTask = true;
                //API.TaskVehiclePark(vehicle.Driver.Handle, vehicle.Handle, roadsidePos.X, roadsidePos.Y, roadsidePos.Z, );

                while (API.GetDistanceBetweenCoords(vehicle.Position.X, vehicle.Position.Y, vehicle.Position.Z,
                           roadsidePos.X, roadsidePos.Y, roadsidePos.Z, false) > 2f
                       && Vector3.Dot(vehicle.ForwardVector, direction) >= 0
                       && (DateTime.Now - timeOut).TotalSeconds <= timeoutSeconds)
                {
                    if (ct.IsCancellationRequested)
                    {
                        blip.Delete();
                        checkpoint.Delete();
                        return;
                    }

                    //var rayResult = World.RaycastCapsule(vehicle.Position, direction,
                    //    World.GetDistance(vehicle.Position, roadsidePos), 2f, IntersectOptions.MissionEntities, vehicle);

                    //if (rayResult.DitHitEntity)
                    //{
                    //    blip.Delete();
                    //    await PullOver(vehicle);
                    //    return;
                    //}

                    //API.DrawLine(vehicle.Position.X, vehicle.Position.Y, vehicle.Position.Z, roadsidePos.X,
                    //    roadsidePos.Y, roadsidePos.Z, 90, 90, 255, 200);


                    await BaseScript.Delay(0);
                }

                //if (Vector3.Dot(vehicle.ForwardVector, GameMath.HeadingToDirection(result.NodeHeading)) < parkTolerance)
                //{
                //    API.DrawLine(vehicle.Position.X, vehicle.Position.Y, vehicle.Position.Z, roadsidePos.X,
                //        roadsidePos.Y, roadsidePos.Z, 255, 90, 90, 200);

                //    await PullOver(vehicle);
                //    return;
                //}

                //var parkPos = roadsidePos + GameMath.HeadingToDirection(roadsideHeading) * 3;
                //API.TaskVehicleGotoNavmesh(vehicle.Driver.Handle, vehicle.Handle,
                //    parkPos.X, parkPos.Y, parkPos.Z,
                //    10f, 541327679, 2f);

                //vehicle.Driver.DrivingStyle = (DrivingStyle)786603;
                API.BringVehicleToHalt(vehicle.Handle, 2f, 300, true);
                vehicle.SetEngineState(false);

                await BaseScript.Delay(300);
                API.StopBringVehicleToHalt(vehicle.Handle);
                blip.Delete();
                checkpoint.Delete();
            }
        }

        public static bool GetIsBeingInteractedWith(this Vehicle vehicle)
        {
            return vehicle != null && vehicle.State.Get<bool>(VehicleStates.IsBeingInteractedWith);
        }

        public static void SetIsBeingInteractedWith(this Vehicle vehicle, bool toggle)
        {
            if (vehicle == null) return;
            vehicle.State.Set<bool>(VehicleStates.IsBeingInteractedWith, toggle, true);
        }

        public static bool HasFreeSlotForPrisoner(this Vehicle vehicle)
        {
            return vehicle.IsSeatFree(VehicleSeat.LeftRear) || vehicle.IsSeatFree(VehicleSeat.RightRear);
        }

        /// <summary>
        /// Returns true if any of the vehicle tyres have burst
        /// </summary>
        /// <param name="vehicle"></param>
        /// <returns></returns>
        public static bool HasAnyBurstTire(this Vehicle vehicle)
        {
            for (int i = 0; i < 6; i++)
            {
                if (API.DoesVehicleTyreExist(vehicle.Handle, i) 
                    && API.IsVehicleTyreBurst(vehicle.Handle, i, true))
                {
                    return true;
                }
            }

            return false;
        }
    }
}