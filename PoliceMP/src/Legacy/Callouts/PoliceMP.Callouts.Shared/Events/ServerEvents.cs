namespace PoliceMP.Callouts.Shared.Events
{
    public static class ServerEvents
    {
        public const string REQUEST_JOIN_CALLOUT = "Callouts:RequestJoinCallout";
        public const string REQUEST_LEAVE_CALLOUT = "Callouts:RequestLeaveCallout";

        public const string CALLOUT_ENDED = "Callouts:CalloutEnded";
        public const string CLIENT_SPAWNED_PED = "Callouts:ClientSpawnedPed";
        public const string CLIENT_SPAWNED_VEHICLE = "Callouts:ClientSpawnedVehicle";
        public const string ARRIVED_AT_CALLOUT = "Callouts:ArrivedAtCallout";
        public const string RETURN_DOES_ENTITY_EXIST = "Callouts:ReturnDoesEntityExist";

        public const string SAVE_POSITION = "Callouts:SavePosition";

        public const string CALLOUT_BACKUP_REQUEST = "PoliceMP:Callout:BackupRequest";

        public const string WITHIN_RANGE_OF_CALLOUT = "Callouts:WithinRangeOfCallout";

        public const string RECEIVE_REQUEST_RESULT = "Callouts:ReceiveRequestResult";
        public const string REQUEST_CALLOUT_DATA = "Callouts:RequestCalloutData";
    }
}
