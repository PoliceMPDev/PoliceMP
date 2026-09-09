using CitizenFX.Core;
using PoliceMP.Callouts.Shared.Events;

namespace PoliceMP.Callouts.Client
{
    public class ServerEventAPI : BaseScript
    {
        /**
         * Callouts
         */
        public static void RequestJoinCallout(int calloutId)
            => TriggerServerEvent(ServerEvents.REQUEST_JOIN_CALLOUT, calloutId);

        public static void RequestLeaveCallout()
            => TriggerServerEvent(ServerEvents.REQUEST_LEAVE_CALLOUT);

        public static void ArrivedAtCallout(int calloutId)
            => TriggerServerEvent(ServerEvents.ARRIVED_AT_CALLOUT, calloutId);

        public static void WithinRangeOfCallout(int calloutId)
            => TriggerServerEvent(ServerEvents.WITHIN_RANGE_OF_CALLOUT, calloutId);

        /**
         * Entities
         */
        public static void ClientSpawnedPed(int calloutId, string pedKey, int networkId)
            => TriggerServerEvent(ServerEvents.CLIENT_SPAWNED_PED, calloutId, pedKey, networkId);

        public static void ClientSpawnedVehicle(int calloutId, string vehicleKey, int networkId, string plate)
            => TriggerServerEvent(ServerEvents.CLIENT_SPAWNED_VEHICLE, calloutId, vehicleKey, networkId, plate);

        public static void ReturnDoesEntityExist(int networkId, bool exists)
            => TriggerServerEvent(ServerEvents.RETURN_DOES_ENTITY_EXIST, networkId, exists);

        /**
         * Misc
         */
        public static void SavePosition(string name, Vector3 position, float heading)
            => TriggerServerEvent(ServerEvents.SAVE_POSITION, name, position, heading);
    }
}
