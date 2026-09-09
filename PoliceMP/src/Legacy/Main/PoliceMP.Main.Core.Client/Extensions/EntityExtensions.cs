using CitizenFX.Core;
using CitizenFX.Core.Native;
using System;

namespace PoliceMP.Main.Core.Client.Extensions
{
    public static class EntityExtensions
    {
        public static bool HasDecor(this Entity entity, string decor)
        {
            return API.DecorExistOn(entity.Handle, decor);
        }

        public static bool GetBoolDecor(this Entity entity, string decor)
        {
            return HasDecor(entity, decor) && API.DecorGetBool(entity.Handle, decor);
        }

        public static int GetIntDecor(this Entity entity, string decor)
        {
            if (!HasDecor(entity, decor))
                return -1;

            return API.DecorGetInt(entity.Handle, decor);
        }

        public static void SetBoolDecor(this Entity entity, string decor, bool value)
        {
            API.DecorSetBool(entity.Handle, decor, value);
        }

        public static void SetIntDecor(this Entity entity, string decor, int value)
        {
            API.DecorSetInt(entity.Handle, decor, value);
        }

        public static float GetHeadingToEntity(this Entity entity, Entity toEntity)
        {
            var p1 = entity.Position;
            var p2 = toEntity.Position;

            var dx = p2.X - p1.X;
            var dy = p2.Y - p1.Y;

            float heading = API.GetHeadingFromVector_2d(dx, dy);
            return heading;
        }

        /// <summary>
        /// Checks if the entity is at the vehicle's driver door.
        /// </summary>
        /// <param name="entityId">The entity ID.</param>
        /// <param name="vehicleId">The vehicle ID.</param>
        /// <returns>Whether the entity is at the driver's door.</returns>
        public static bool IsAtVehicleDriverDoor(this Entity entity, Vehicle vehicle)
        {
            return entity.IsAtVehicleDoor(vehicle, "door_dside_f");
        }

        public static bool IsAtVehicleDoor(this Entity entity, Vehicle vehicle, string boneName)
        {
            var doorPos = API.GetWorldPositionOfEntityBone(vehicle.Handle, API.GetEntityBoneIndexByName(vehicle.Handle, boneName));
            var entityPos = API.GetEntityCoords(entity.Handle, true);

            var distance = API.GetDistanceBetweenCoords(entityPos.X, entityPos.Y, entityPos.Z, doorPos.X, doorPos.Y,
                doorPos.Z, true);

            return distance < 1f;
        }

        public static VehicleDoorIndex GetVehicleDoorIsLookingAt(this Entity entity, Vehicle vehicle)
        {
            if (entity.IsAtVehicleDoor(vehicle, "door_dside_f"))
                return VehicleDoorIndex.FrontLeftDoor;

            if (entity.IsAtVehicleDoor(vehicle, "door_dside_r"))
                return VehicleDoorIndex.BackLeftDoor;

            if (entity.IsAtVehicleDoor(vehicle, "door_pside_f"))
                return VehicleDoorIndex.FrontRightDoor;

            if (entity.IsAtVehicleDoor(vehicle, "door_pside_r"))
                return VehicleDoorIndex.BackRightDoor;

            return VehicleDoorIndex.Trunk;
        }

        /// <summary>
        /// Gets the entity in front of the position
        /// </summary>
        /// <param name="position">The position</param>
        /// <param name="distance">The distance</param>
        /// <param name="flag">The raycast flag</param>
        /// <returns>The entity ID in front or -1 if none</returns>
        public static Entity GetEntityInFront(this Entity entity, float distance, int flag)
        {
            var inFront = API.GetOffsetFromEntityInWorldCoords(entity.Handle, 0f, distance, 0f);

            // Raycast stuff
            var rayHandle = API.CastRayPointToPoint(entity.Position.X, entity.Position.Y, entity.Position.Z, inFront.X, inFront.Y, inFront.Z, flag, Game.PlayerPed.Handle, 0);
            var hit = false;
            var endCoords = Vector3.Zero;
            var surfaceNormal = Vector3.Zero;
            var entityHit = -1;

            API.GetRaycastResult(rayHandle, ref hit, ref endCoords, ref surfaceNormal, ref entityHit);
            return Entity.FromHandle(entityHit);
        }

        /// <summary>
        /// Gets the vehicle in front of the player.
        /// </summary>
        /// <returns>The vehicle entity ID or null if none found.</returns>
        public static Vehicle GetVehicleInFront(this Entity entity)
        {
            Entity entityInFront = null;
            var distance = 2f;

            while (entityInFront == null)
            {
                entityInFront = entity.GetEntityInFront(distance, 2);
                distance += 2f;

                if (distance >= 40f && entityInFront == null) break;
            }

            if (entityInFront == null || !API.DoesEntityExist(entityInFront.Handle) || !API.IsEntityAVehicle(entityInFront.Handle))
                return null;

            if (entityInFront is Vehicle vehicle)
                return vehicle;

            return null;
        }

        /// <summary>
        /// Gets the vehicle in front of the player within a specified
        /// heading.
        /// </summary>
        /// <param name="heading">The heading.</param>
        /// <returns>The vehicle entity id.</returns>
        public static Vehicle GetVehicleInFront(this Entity entity, float heading)
        {
            var vehicle = entity.GetVehicleInFront();
            if (vehicle == null) return null;

            var playerHeading = Game.PlayerPed.Heading;

            if (Math.Abs(vehicle.Heading - playerHeading) <= heading)
                return vehicle;

            if (Math.Abs(vehicle.Heading - 180f - playerHeading) <= heading)
                return vehicle;

            return null;
        }

        /// <summary>
        /// Checks if the player is close enough to the specified entity.
        /// </summary>
        /// <param name="entityId">The entity ID.</param>
        /// <param name="maxDistance">The maximum distance.</param>
        /// <returns>Whether the player is close enough.</returns>
        public static bool IsCloseEnoughToEntity(this Entity entity, Entity toEntity, float maxDistance)
        {
            if (!API.DoesEntityExist(toEntity.Handle))
                return false;

            var entityPos = entity.Position;
            var toEntityPos = API.GetEntityCoords(toEntity.Handle, true);
            var distance = API.GetDistanceBetweenCoords(entityPos.X, entityPos.Y, entityPos.Z, toEntityPos.X, toEntityPos.Y,
                toEntityPos.Z, false);

            return maxDistance > distance;
        }

        /// <summary>
        /// Gets the ped that is in front of the player.
        /// </summary>
        /// <returns>The entity ID of the ped in front of the player.</returns>
        public static Ped GetInteractablePedInFront(this Entity entity)
        {
            var entityInFront = entity.GetEntityInFront(3f, 12);
            if (entityInFront == null)
            {
                return null;
            }

            var ped = (Ped)Entity.FromHandle(entityInFront.Handle);

            if (ped == null || !API.DoesEntityExist(ped.Handle) || !API.IsEntityAPed(ped.Handle) || !ped.IsInteractable())
                return null;

            return ped;
        }
    }
}
