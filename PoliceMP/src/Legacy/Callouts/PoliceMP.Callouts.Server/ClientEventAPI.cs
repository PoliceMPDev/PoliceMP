using CitizenFX.Core;
using PoliceMP.Callouts.Shared.Events;

namespace PoliceMP.Callouts.Server
{
    public class ClientEventAPI : BaseScript
    {
        /**
         * Callouts
         */
        public static void LeaveCallout(Player player, int calloutId)
            => player.TriggerEvent(ClientEvents.LEAVE_CALLOUT, calloutId);

        public static void JoinCallout(Player player, int calloutId, string title, string description, Vector3 location, bool isBackup)
            => player.TriggerEvent(ClientEvents.JOIN_CALLOUT, calloutId, title, description, location, isBackup);

        public static void ReceiveCalloutNotification(Player player, int calloutId, string title, string description, int grade, Vector3 location, bool isBackup)
            => player.TriggerEvent(ClientEvents.RECEIVE_CALLOUT_NOTIFICATION, calloutId, title, description, grade, location, isBackup);

        public static void CalloutEnded(Player player, int calloutId)
            => player.TriggerEvent(ClientEvents.CALLOUT_ENDED, calloutId);

        /**
         * Blips
         */
        public static void RemoveAllBlips(Player player)
            => player.TriggerEvent(ClientEvents.REMOVE_ALL_BLIPS);

        public static void AddBlipForCoord(Player player, int blipSprite, Vector3 location, int blipColor, bool showRoute, bool isShortRange, string name)
            => player.TriggerEvent(ClientEvents.ADD_BLIP_FOR_COORD, blipSprite, location, blipColor, showRoute, isShortRange, name);

        public static void AddBlipForEntity(Player player, int networkId, int blipSprite, int blipColor, bool showRoute, bool isShortRange, string name, float scale = 1f)
            => player.TriggerEvent(ClientEvents.ADD_BLIP_FOR_ENTITY, networkId, blipSprite, blipColor, showRoute, isShortRange, name, scale);

        public static void RemoveBlipForEntity(Player player, int networkId)
            => player.TriggerEvent(ClientEvents.REMOVE_BLIP_FOR_ENTITY, networkId);

        public static void AddBlipForRadius(Player player, int blipSprite, Vector3 location, float radius, int blipColor, bool showRoute, bool isShortRange, string name)
            => player.TriggerEvent(ClientEvents.ADD_BLIP_FOR_RADIUS, blipSprite, location, radius, blipColor, showRoute, isShortRange, name);

        /**
         * Entities
         */
        public static void SpawnPed(Player player, int calloutId, string pedKey, uint pedHash, Vector3 location, uint weaponHash)
            => player.TriggerEvent(ClientEvents.SPAWN_PED, calloutId, pedKey, pedHash, location, weaponHash);

        public static void SpawnVehicle(Player player, int calloutId, string vehicleKey, string plate, uint vehicleHash, Vector3 location,
            int colour,
            float bodyHealth,
            float engineHealth,
            float fuelTankHealth)
            => player.TriggerEvent(ClientEvents.SPAWN_VEHICLE, calloutId, vehicleKey, plate, vehicleHash, location, colour, bodyHealth, engineHealth, fuelTankHealth);

        public static void SpawnRandomPed(Player player, int calloutId, Vector3 location)
            => player.TriggerEvent(ClientEvents.SPAWN_RANDOM_PED, calloutId, location);

        public static void SpawnRandomVehicle(Player player, int calloutId, int vehicleId, Vector3 location)
            => player.TriggerEvent(ClientEvents.SPAWN_RANDOM_VEHICLE, calloutId, vehicleId, location);

        public static void SetPedAnim(Player player, int networkId, string animDict, string animName)
            => player.TriggerEvent(ClientEvents.SET_PED_ANIM, networkId, animDict, animName);

        public static void SetPedIntoVehicle(Player player, int pedNetworkId, int vehicleNetworkId, int seat)
            => player.TriggerEvent(ClientEvents.SET_PED_INTO_VEHICLE, pedNetworkId, vehicleNetworkId, seat);

        public static void SetPedFleeFromPlayer(Player player, int pedNetworkId)
            => player.TriggerEvent(ClientEvents.SET_PED_FLEE_FROM_PLAYER, pedNetworkId);

        public static void SetPedAttackPlayer(Player player, int pedNetworkId)
            => player.TriggerEvent(ClientEvents.SET_PED_ATTACK_PLAYER, pedNetworkId);

        internal static void RunTheGangWar(Player player, string pgroupString, string ggroupString)
            => player.TriggerEvent(ClientEvents.RUN_GANG_WAR, pgroupString, ggroupString);

        internal static void RunBankHeist(Player player, string badPedString, string civPedString)
            => player.TriggerEvent(ClientEvents.RUN_BANK_HEIST, badPedString, civPedString);

        public static void SetPedTaskWander(Player player, int pedNetworkId)
            => player.TriggerEvent(ClientEvents.SET_PED_TASK_WANDER, pedNetworkId);

        public static void SetPedTaskDriveWander(Player player, int pedNetworkId, int vehicleNetworkId)
            => player.TriggerEvent(ClientEvents.SET_PED_TASK_DRIVE_WANDER, pedNetworkId, vehicleNetworkId);

        internal static void RemovePedElegantly(Player player, int pedNetworkId) =>
            player.TriggerEvent(ClientEvents.REMOVE_PED_ELEGANTLY, pedNetworkId);

        public static void SetPedAttackPed(Player player, int pedNetworkId1, int pedNetworkId2)
            => player.TriggerEvent(ClientEvents.SET_PED_ATTACK_PED, pedNetworkId1, pedNetworkId2);

        public static void SetVehiclePhysicalDamage(Player player, int networkId)
            => player.TriggerEvent(ClientEvents.SET_VEHICLE_PHYSICAL_DAMAGE, networkId);

        public static void SetVehicleRotation(Player player, int networkId, float rotation)
            => player.TriggerEvent(ClientEvents.SET_VEHICLE_ROTATION, networkId, rotation);

        public static void SetPedRandomIdleAnim(Player player, int networkId)
            => player.TriggerEvent(ClientEvents.SET_PED_RANDOM_IDLE_ANIM, networkId);

        public static void ApplyPedDamage(Player player, int networkId, int damage)
            => player.TriggerEvent(ClientEvents.APPLY_PED_DAMAGE, networkId, damage);

        public static void SetPedDriveToFast(Player player, int pedNetworkId, int vehicleNetworkId, Vector3 location)
            => player.TriggerEvent(ClientEvents.SET_PED_DRIVE_TO_FAST, pedNetworkId, vehicleNetworkId, location);

        /**
         * Messages
         */
        public static void SendChatMessage(Player player, string message, bool error = false)
            => player.TriggerEvent(ClientEvents.SEND_CHAT_MESSAGE, message, error);

        public static void ShowSubtitle(Player player, string message, int duration = 2500)
            => player.TriggerEvent(ClientEvents.SHOW_SUBTITLE, message, duration);



        /**
         * Requests
         */

    }
}
