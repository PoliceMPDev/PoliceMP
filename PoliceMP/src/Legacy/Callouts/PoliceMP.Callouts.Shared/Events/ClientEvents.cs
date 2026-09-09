namespace PoliceMP.Callouts.Shared.Events
{
    public class ClientEvents
    {
        public const string RECEIVE_CALLOUT_NOTIFICATION = "Callouts:ReceiveCalloutNotification";
        public const string SPAWN_CALLOUT = "Callouts:SpawnCallout";
        public const string JOIN_CALLOUT = "Callouts:JoinCallout";
        public const string LEAVE_CALLOUT = "Callouts:LeaveCallout";

        public const string SPAWN_PED = "Callouts:SpawnPed";
        public const string SPAWN_VEHICLE = "Callouts:SpawnVehicle";
        public const string SPAWN_RANDOM_PED = "Callouts:SpawnRandomPed";
        public const string SPAWN_RANDOM_VEHICLE = "Callouts:SpawnRandomVehicle";
        public const string SET_PED_ANIM = "Callouts:SetPedAnim";
        public const string DOES_ENTITY_EXIST = "Callouts:DoesEntityExist";
        public const string SET_PED_INTO_VEHICLE = "Callouts:SetPedIntoVehicle";

        public const string SEND_CHAT_MESSAGE = "Callouts:SendChatMessage";
        public const string SHOW_SUBTITLE = "Callouts:ShowSubtitle";

        public const string CALLOUT_ENDED = "Callouts:CalloutEnded";

        public const string REMOVE_ALL_BLIPS = "Callouts:RemoveAllBlips";
        public const string ADD_BLIP_FOR_COORD = "Callouts:AddBlipForCoord";
        public const string ADD_BLIP_FOR_ENTITY = "Callouts:AddBlipForEntity";
        public const string REMOVE_BLIP_FOR_ENTITY = "Callouts:RemoveBlipForEntity";
        public const string ADD_BLIP_FOR_RADIUS = "Callouts:AddBlipForRadius";
        public const string ADD_BLIP_FOR_HOUSERAID = "Callouts:AddHouseRaidBlip";

        public const string SET_PED_FLEE_FROM_PLAYER = "Callouts:SetPedFleeFromPlayer";
        public const string SET_PED_ATTACK_PLAYER = "Callouts:SetPedAttackPlayer";
        public const string SET_PED_TASK_WANDER = "Callouts:SetPedTaskWander";
        public const string SET_PED_TASK_DRIVE_WANDER = "Callouts:SetPedTaskDriveWander";
        public const string SET_PED_ATTACK_PED = "Callouts:SetPedAttackPed";
        public const string SET_VEHICLE_PHYSICAL_DAMAGE = "Callouts:SetVehiclePhysicalDamage";
        public const string SET_VEHICLE_ROTATION = "Callouts:SetVehicleRotation";
        public const string SET_PED_RANDOM_IDLE_ANIM = "Callouts:SetPedRandomIdleAnim";
        public const string SET_PED_DRIVE_TO_FAST = "Callouts:SetPedDriveToFast";
        public const string APPLY_PED_DAMAGE = "Callouts:ApplyPedDamage";
        public const string REMOVE_PED_ELEGANTLY = "Callouts:RemovePedElegantly";
        public const string RUN_GANG_WAR = "Callouts:RunGangWar";
        public const string RUN_BANK_HEIST = "Callouts:RunBankHeist";

        public const string GET_NEXT_POSITION_ON_STREET = "Callouts:GetNextPositionOnStreet";
        public const string GET_NEXT_POSITION_ON_SIDEWALK = "Callouts:GetNextPositionOnSidewalk";
    }
}
