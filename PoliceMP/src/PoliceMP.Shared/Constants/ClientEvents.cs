using PoliceMP.Core.Shared.Communications;

namespace PoliceMP.Shared.Constants
{
    public static class ClientEvents
    {
        public const string ReplicateSayPedSpeech = "PoliceMP:Speech:ReplicateSayPedSpeech";
        public const string ReplicateDoPedSpeech = "PoliceMP:Speech:ReplicateDoPedSpeech";
        public const string PedCuffed = "PoliceMP:PedCuffed";
        public const string PedGrabbed = "PoliceMP:PedGrabbed";
        public const string ActionExecuteStart = "PoliceMP:ActionExecuteStart";
        public const string ActionExecuteEnd = "PoliceMP:ActionExecuteEnd";
        public const string ShowNotification = "PoliceMP:Notifications:ShowNotification";
        public const string LearnedName = "PoliceMP:Action:AskForId:LearnedName";
        public const string ReceiveModMessage = "PoliceMP:AdminSystem:ReceiveModMessage";
        public const string ReceiveBackupRequest = "PoliceMP:BackupSystem:ReceiveBackupRequest";
        public const string TellPlayerToFling = "PoliceMP:Networking:AdminTools:TellPlayerToFling";
        public const string FetchAfkTimeData = "PoliceMp:AFK:FetchTimeData";
        public const string SetSpawnClothes = "PoliceMP:Networking:ClothingLocker:SetSpawnClothes";
        public const string SendPedFleeArea = "PoliceMP:RoadManagement:SendPedFleeArea";
        public const string SendCloseRoadEventToClient = "PoliceMP:RoadManagement:SendCloseRoadEventToClient";
        public const string SendOpenRoadEventToClient = "PoliceMP:RoadManagement:SendOpenRoadEventToClient";
        public const string SendSpeedZoneToClient = "PoliceMP:RoadManagement:SendSpeedZoneToClient";
        public const string PlayerSpawned = "PoliceMP:Spawn:PlayerSpawned";
        public const string SetVoiceChannelCommand = "PoliceMp:Voice:SetChannelResourceCommand";
        public const string SendFlashBangEventToClient = "PoliceMP:Weapons:SendFlashBangEventToClient";
        public const string SendBackupLocationToClient = "PoliceMP:Backup:SendBackupLocationToClient";
        public const string SendDogSoundEventToClient = "PoliceMP:Dog:SendDogSoundEventToClient";
        public const string SendDogParticleFxEventToClient = "SendDogParticleFxEventToClient";
        public const string GoToWhatThreeWord = "PoliceMP:WhatThreeWord:SendWordToServer";

        public const string SpawnNhsNotification = "PoliceMP:Spawn:NHSNotify";
        public const string SpawnToldToRevive = "PoliceMP:Spawn:ToldToRevive";
        
        public const string TaserDetectSuspect = "PoiliceMP:Taser:ClientDetectSuspect";
        public const string TaserSuspectDetected = "PoliceMP:Taser:ClientSuspectDetected";
        public const string TaserReactivateSuspect = "PoliceMP:Taser:ClientReactivateSuspect";
        public const string TaserUpdateLog = "PoliceMP:Taser:ClientUpdateLog";
        public const string TaserCheckLocation = "PoliceMP:Taser:ClientCheckLocation";
        public const string TaserBarbsInvalidated = "PoliceMP:Taser:ClientBarbsInvalidated";
        public const string TaserNotifyBarbsRemoved = "PoliceMP:Taser:ClientNotifyBarbsRemoved";
        public const string TaserDisplayNotification = "PoliceMP:Taser:ClientDisplayNotification";
        public const string TaserReload = "PoliceMP:Taser:ClientTaserReload";

        
        public const string SoundToClient = "PoliceMP:Sound:ClientToClient";
        public const string SoundToAll = "PoliceMP:Sound:ClientToAll";
        public const string SoundToRadius = "PoliceMP:Sound:ClientToRadius";
        public const string SoundToCoords = "PoliceMP:Sound:ClientToCoords";

        public const string AdminSendPopupClient = "PoliceMP:AdminMenu:AdminSendPopupClient";
        public const string AdminTeleportIntoVehicle = "PoliceMP:AdminMenu:TeleportIntoVehicle";
        public const string AdminSendKillEventToPlayer = "PoliceMP:AdminMenu:SendKillEventToPlayer";
        public const string AdminClearAreaAroundPosition = "PoliceMP:AdminMenu:ClearAreaAroundPosition";
        public const string AdminSendFreezeEventToPlayer = "PoliceMP:AdminMenu:SendFreezeEventToPlayer";
        public const string AdminSendAttachEventToPlayer = "PoliceMP:AdminMenu:SendAttachEventToPlayer";

        public const string SendWeatherToPlayers = "PoliceMP:Weather:SendToAllPlayers";

        public const string RequestTrainCreation = "PoliceMP:Trains:RequestTrainToBeCreated";
        
        public const string SendLfbBackupRequest = "PoliceMP:BackupSystem:SendLfbBackupRequest";
        public const string StartFire = "PoliceMP:Fire:StartFire";

        public const string CivBlend = "Blendffs";

        public const string TacklePed = "PoliceMP:TackleScript:TacklePed";
        public const string BeTackledBy = "PoliceMP:TackleScript:BeTackledBy";
        
        public const string ArrestPlayerPed = "PoliceMP:PlayerToPlayerActions:ArrestPed";
        public const string BeArrestedPlayerPed = "PoliceMP:PlayerToPlayerActions:BeArrestedPed";
        public const string CPRPlayerPed = "PoliceMP:PlayerToPlayerActions:CPRPed";
        public const string BeCPRByPlayerPed = "PoliceMP:PlayerToPlayerActions:BeCPRByPed";
        public const string DefibPlayerPed = "PoliceMP:PlayerToPlayerActions:DefibPed";
        public const string BeDefibByPlayerPed = "PoliceMP:PlayerToPlayerActions:BeDefibByPed";
        
        //Network owner events
        public const string SetCarLockState = "PoliceMP:NewotkOwner:TellClientToggleLock";

        public const string ToggleDebug = "PoliceMP:DebugUtils:ToggleDebug";

        // Spawn Vehicle To Player Command / Script
        public const string SendVehicleToClient = "PoliceMP:VehicleSpawn:SendVehicleToClient";

        // Anpr Pings
        public const string AnprPingResponse = "PoliceMP:AnprPings:AnprPingResponse";
        public const string CheckForPlateOnClient = "PoliceMP:AnprPings:CheckForPlateOnClient";


        // Remote Hide Blips For Player
        public const string RemoteHideBlipsToClient = "PoliceMP:RemoteHideBlip:RemoteHideBlipsToClient";
        public const string RemoteShowBlipsToClient = "PoliceMP:RemoteHideBlip:RemoteShowBlipsToServer";
        
        // Control Status
        public const string ControlStatus = "PoliceMP:Control:ControlStatus";

        // CRO Status
        public const string CROStatus = "PoliceMP:Control:CROStatus";

        // Autopilot
        public const string AutopilotState = "PoliceMP:Autopilot:AutopilotState";
        public const string IsFixingVehicle = "PoliceMP:Autopilot:IsFixingVehicle";
        public const string SetAutopilotState = "PoliceMP:Autopilot:SetAutopilotState";
        
        // Indicators
        public const string IndicatorsChanged = "PoliceMP:Indicators:IndicatorsChanged";
        
        // Pass Alarm LFB
        public const string FirefighterPass = "PoliceMP:Firefighter:FirefighterPass";
        
        // Manual Car Mode
        public const string SetManualCarMode = "PoliceMP:Manual:SetManualCarMode";
        
        // CIV - DSU Scent immersion script
        public const string SendNewScentToClient = "PoliceMP:DSUScent:SendNewScentToClient";
    }
}