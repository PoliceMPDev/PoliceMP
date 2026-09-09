namespace PoliceMP.RoadManagement.Shared
{
    public class ServerEvents
    {
        public const string ADD_SPAWNED_OBJECT = "AddSpawnedObject";
        public const string REMOVE_SPAWNED_OBJECT = "RemoveSpawnedObject";

        public const string SYNC_SPEED_ZONES = "SyncSpeedZones";
        public const string ADD_SPEED_ZONE = "AddSpeedZone";
        public const string REMOVE_ALL_SPEED_ZONES = "RemoveAllSpeedZones";
        public const string REMOVE_SPEED_ZONE = "RemoveSpeedZone";

        public const string BURST_TYRES_ON_ALL_CLIENTS = "RoadManagement:EveryClientBurstTyres";
    }
}
