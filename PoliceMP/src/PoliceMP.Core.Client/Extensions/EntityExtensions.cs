using CitizenFX.Core;
using CitizenFX.Core.Native;
using System;
using System.Threading.Tasks;
using PoliceMP.Shared.Constants.States;
using System.Diagnostics;

namespace PoliceMP.Core.Client.Extensions
{
    public static class EntityExtensions
    {
        public static string GetCurrentStreet(this Entity entity)
        {
            uint streetHashKey = 0;
            uint crossingRoad = 0;
            API.GetStreetNameAtCoord(entity.Position.X,
                entity.Position.Y,
                entity.Position.Z,
                ref streetHashKey,
                ref crossingRoad);
            string streetName = API.GetStreetNameFromHashKey(streetHashKey);
            return streetName;
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

        public static bool IsAtVehicleDriverDoor(this Entity entity, Vehicle vehicle)
        {
            return entity.IsAtVehicleDoor(vehicle, "door_dside_f");
        }
        
        public enum PedTransportMode
        {
            Any,
            OnFoot,
            InVehicle
        }

        public static bool IsEntityInAngledArea(this Entity entity, Vector3 origin, Vector3 extent, float width,
            bool highlightArea = false, bool do3dCheck = true, PedTransportMode pedTransportMode = PedTransportMode.Any)
        {
            return API.IsEntityInAngledArea(entity.Handle, origin.X, origin.Y, origin.Z, extent.X, extent.Y, extent.Z, width,
                highlightArea, do3dCheck, (int)pedTransportMode);
        }

        public static bool IsAtVehicleDoor(this Entity entity, Vehicle vehicle, string boneName, float distance = 1f)
        {
            var doorPos = API.GetWorldPositionOfEntityBone(vehicle.Handle, API.GetEntityBoneIndexByName(vehicle.Handle, boneName));
            var entityPos = API.GetEntityCoords(entity.Handle, true);

            var distanceBetweenCoords = API.GetDistanceBetweenCoords(entityPos.X, entityPos.Y, entityPos.Z, doorPos.X, doorPos.Y,
                doorPos.Z, true);

            return distanceBetweenCoords < distance;
        }

        public static VehicleDoorIndex? GetVehicleDoorIsLookingAt(this Entity entity, Vehicle vehicle, float distance = 1.8f)
        {
            if (entity.IsAtVehicleDoor(vehicle, "door_dside_f", distance))
                return VehicleDoorIndex.FrontLeftDoor;

            if (entity.IsAtVehicleDoor(vehicle, "door_dside_r", distance))
                return VehicleDoorIndex.BackLeftDoor;

            if (entity.IsAtVehicleDoor(vehicle, "door_pside_f", distance))
                return VehicleDoorIndex.FrontRightDoor;

            if (entity.IsAtVehicleDoor(vehicle, "door_pside_r", distance))
                return VehicleDoorIndex.BackRightDoor;
            
            if (entity.IsAtVehicleDoor(vehicle, "boot", distance))
                return VehicleDoorIndex.Trunk;
            
            if (entity.IsAtVehicleDoor(vehicle, "bonnet", 2.5f))
                return VehicleDoorIndex.Hood;

            return null;
        }

        public static int GetEntityIdInFront(this Entity entity, float distance = 3f, int flag = 12)
        {
            var inFront = API.GetOffsetFromEntityInWorldCoords(entity.Handle, 0f, distance, 0f);

            // Raycast stuff
            var rayHandle = API.CastRayPointToPoint(entity.Position.X, entity.Position.Y, entity.Position.Z, inFront.X, inFront.Y, inFront.Z, flag, Game.PlayerPed.Handle, 0);
            var hit = false;
            var endCoords = Vector3.Zero;
            var surfaceNormal = Vector3.Zero;
            var entityHit = -1;

            API.GetRaycastResult(rayHandle, ref hit, ref endCoords, ref surfaceNormal, ref entityHit);
            return entityHit;
        }

        public static Entity GetEntityInFront(this Entity entity, float distance = 3f, int flag = 12)
        {
            int entityId = entity.GetEntityIdInFront(distance, flag);
            return Entity.FromHandle(entityId);
        }

        public static Vehicle GetVehicleInFront(this Entity entity, float maxDistance = 40f)
        {
            Entity entityInFront = null;
            var distance = 2f;

            while (entityInFront == null)
            {
                entityInFront = entity.GetEntityInFront(distance, 2);
                distance += 2f;

                if (distance >= maxDistance && entityInFront == null) break;
            }

            if (entityInFront == null || !API.DoesEntityExist(entityInFront.Handle) || !API.IsEntityAVehicle(entityInFront.Handle))
                return null;

            if (entityInFront is Vehicle vehicle)
                return vehicle;

            return null;
        }

        public static Vehicle GetVehicleInFrontWithHeading(this Entity entity, float heading, float maxDistance = 40f)
        {
            var vehicle = entity.GetVehicleInFront(maxDistance);
            if (vehicle == null) return null;

            var playerHeading = Game.PlayerPed.Heading;

            if (Math.Abs(vehicle.Heading - playerHeading) <= heading)
                return vehicle;

            if (Math.Abs(vehicle.Heading - 180f - playerHeading) <= heading)
                return vehicle;

            return null;
        }

        public static bool IsCloseEnoughToPoint(this Entity entity, Vector3 point, float maxDistance)
        {
            var entityPos = entity.Position;

            var distance = API.GetDistanceBetweenCoords(entityPos.X, entityPos.Y, entityPos.Z, point.X, point.Y,
                point.Z, false);

            return maxDistance > distance;
        }

        public static bool IsCloseEnoughToEntity(this Entity entity, Entity toEntity, float maxDistance)
        {
            if (!API.DoesEntityExist(toEntity.Handle))
                return false;

            var toEntityPos = API.GetEntityCoords(toEntity.Handle, true);
            return IsCloseEnoughToPoint(entity, toEntityPos, maxDistance);
        }

        public static bool IsAnyPedInFront(this Entity entity)
        {
            int entityId = entity.GetEntityIdInFront(3f, 12);
            return (API.IsEntityAPed(entityId));
        }

        public static Ped GetPedInFront(this Entity entity)
        {
            var entityInFront = entity.GetEntityInFront(3f, 12);
            if (entityInFront == null)
            {
                return null;
            }

            var ped = (Ped)Entity.FromHandle(entityInFront.Handle);

            if (ped == null || !API.DoesEntityExist(ped.Handle) || !API.IsEntityAPed(ped.Handle))
                return null;

            return ped;
        }

        /// <summary>
        /// Static method of setting net migration
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="canMigrate">True - Can Migrate</param>
        public static void SetNetworkMigration(this Entity entity, bool canMigrate = false)
        {
            API.SetNetworkIdCanMigrate(entity.NetworkId, canMigrate);
        }
        

        /// <summary>
        /// Requesting the player control of a network ID
        /// </summary>
        /// <param name="networkId">NetworkID you want control of</param>
        /// <param name="resource">String of a resource name for debug</param>
        /// <returns>True = Control gained   False = Control cannot be gained or other issue.</returns>
        public static async Task<bool> TryRequestNetworkEntityControl(this Entity entity, bool canMigrate = true, int timeoutMs = 5000)
        {
            var stacktrace = new StackTrace();
            var method = stacktrace.GetFrame(3).GetMethod();
            var resource = $"{method.DeclaringType?.DeclaringType?.Name}.{method.DeclaringType?.Name}";
            if (entity == null)
                return false;

            CitizenFX.Core.Debug.WriteLine($"{resource} : Requested control of entity {entity.NetworkId}");

            if(!API.NetworkGetEntityIsNetworked(entity.Handle) || API.NetworkGetEntityIsLocal(entity.Handle))
            {
                return true;
            }

            var networkId = entity.NetworkId;
            if (!API.NetworkDoesEntityExistWithNetworkId(networkId))
            {
                CitizenFX.Core.Debug.WriteLine($"{resource} : NetworkID {networkId} does not exist.");
                return false;
            }

            var endTime = Game.GameTime + timeoutMs;
            while (!API.NetworkHasControlOfNetworkId(networkId) && Game.GameTime < endTime)
            {
                if (!API.NetworkDoesEntityExistWithNetworkId(networkId))
                {
                    CitizenFX.Core.Debug.WriteLine($"{resource} : NetworkID {networkId} no longer exists during control attempt.");
                    return false;
                }

                API.NetworkRequestControlOfNetworkId(networkId);
                await BaseScript.Delay(1);
            }

            if (!API.NetworkHasControlOfNetworkId(networkId))
            {
                CitizenFX.Core.Debug.WriteLine($"{resource} : NetworkID {networkId} control could not be gained in {timeoutMs}ms.");
                return false;
            }

            API.SetNetworkIdCanMigrate(entity.NetworkId, canMigrate);
            if (API.NetworkHasControlOfNetworkId(networkId)) return true;
            CitizenFX.Core.Debug.WriteLine($"{resource} : NetworkID {networkId} something broke when requesting.");
            return false;
        }

        public static bool HasNetowrkControl(this Entity entity)
        {
            return API.NetworkHasControlOfNetworkId(entity.NetworkId);
        }

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

        public static bool GetIsBeingInteractedWith(this Entity entity)
        {
            return entity != null && GetBoolDecor(entity, EntityDecors.IsBeingInteractedWith);
        }

        public static void SetIsBeingInteractedWith(this Entity entity, bool toggle)
        {
            if (entity == null) return;
            SetBoolDecor(entity, EntityDecors.IsBeingInteractedWith, toggle);
        }

        public static Entity GetEntityCurrentlyInteracting(this Entity entity)
        {
            var currentNetworkLock = GetIntDecor(entity, EntityDecors.InteractionLockNetworkId);
            if (currentNetworkLock == default)
                return null;

            return Entity.FromNetworkId(currentNetworkLock);
        }

        public static bool CanInteractWith(this Entity entity)
        {
            if (entity == null)
                return false;

            var currentNetworkLock = GetIntDecor(entity, EntityDecors.InteractionLockNetworkId);
            if (currentNetworkLock == 0)
                return true;

            var networkEntity = GetEntityCurrentlyInteracting(entity);
            if (networkEntity == null || networkEntity == Game.PlayerPed)
                return true;

            // Otherwise somebody else has a lock on it, and is still in the game
            return false;
        }

        [Obsolete("Further Investigation", true)]
        public static async Task<bool> TryObtainInteractionLock(this Entity entity, int timeoutMs = -1, bool tryGainNetworkControl = true, bool requireNetworkControl = true)
        {
            var endTime = timeoutMs < 0 ? int.MaxValue : Game.GameTime + timeoutMs;

            if (!tryGainNetworkControl
                && requireNetworkControl && API.NetworkHasControlOfEntity(entity.Handle))
            {
                return false;
            }

            while (Game.GameTime <= endTime)
            {
                if (!entity.CanInteractWith())
                {
                    await BaseScript.Delay(1);
                    continue;
                }

                if (tryGainNetworkControl)
                {
                    if (entity is Ped pedEntity && pedEntity.CurrentVehicle != null)
                    {
                        if (!await pedEntity.CurrentVehicle.TryRequestNetworkEntityControl(!requireNetworkControl) && requireNetworkControl)
                        {
                            return false;
                        }
                    }

                    if (!await entity.TryRequestNetworkEntityControl(!requireNetworkControl) && requireNetworkControl)
                    {
                        return false;
                    }
                }
                API.SetNetworkIdCanMigrate(entity.NetworkId, !requireNetworkControl);
                SetIntDecor(entity, EntityDecors.InteractionLockNetworkId, Game.PlayerPed.NetworkId);

                if (!entity.CanInteractWith())
                {
                    await BaseScript.Delay(1);
                    continue;
                }

                var currentEntity = GetEntityCurrentlyInteracting(entity);
                if (currentEntity != null && currentEntity != Game.PlayerPed)
                {
                    await BaseScript.Delay(1);
                    continue;
                }
                return true;
            }

            return false;
        }

        [Obsolete("Further Investigation", true)]
        public static void ReleaseInteractionLock(this Entity entity, bool releaseNetworkControl = true)
        {
            if (TryObtainInteractionLock(entity, 1, false, false).Result)
            {
                SetIntDecor(entity, EntityDecors.InteractionLockNetworkId, 0);
            }

            if(releaseNetworkControl && entity.HasNetworkControl())
            {
                API.SetNetworkIdCanMigrate(entity.NetworkId, true);
            }
        }

        public static bool HasNetworkControl(this Entity entity)
        {
            return API.NetworkHasControlOfNetworkId(entity.NetworkId);
        }

        public static void MarkAsNoLongerNeededKeepHandle(this Entity entity)
        {
            var handle = entity.Handle;
            API.SetEntityAsNoLongerNeeded(ref handle);
        }
    }
}