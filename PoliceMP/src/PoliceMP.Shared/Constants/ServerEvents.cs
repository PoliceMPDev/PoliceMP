namespace PoliceMP.Shared.Constants
{
    public static class ServerEvents
    {
        public static class Legacy
        {
            public const string CalloutBackupRequest = "PoliceMP:Callout:BackupRequest";
        }

        public const string RequestSpawnEntityVehicle = "PoliceMP:ServerSpawner:RequestVehicle";
        public const string RequestSpawnEntityPed = "PoliceMP:ServerSpawner:RequestPed";
        public const string RequestSpawnEntityObject = "PoliceMP:ServerSpawner:RequestObject";

        public const string ReplicateSayPedSpeech = "PoliceMP:Speech:ReplicateSayPedSpeech";
        public const string ReplicateDoPedSpeech = "PoliceMP:Speech:ReplicateDoPedSpeech";
        public const string FetchUserAces = "PoliceMP:AceSystem:FetchUserAces";
        public const string SendUserRole = "PoliceMP:PermissionSystem:SendUserRole";
        public const string FetchUserRoleByNetId = "PoliceMP:PermissionSystem:FetchRoleByNetId";
        public const string FetchHasModPerm = "PoliceMP:AdminSystem:HasModPermission";
        public const string SendMessageToMods = "PoliceMP:AdminSystem:SendMessageToMods";
        public const string SendBackupRequestToServer = "PoliceMP:BackupSystem:SendBackupRequest";
        public const string RequestRayPerms = "PoliceMP:Networking:AdminTools:RequestRayPerms";
        public const string RequestPlayerToFling = "PoliceMP:Networking:AdminTools:RequestPlayerToFling";
        public const string GetLockerPoints = "PoliceMP:Networking:ClothingLocker:GetLockerPoints";
        public const string GetLockerOutfits = "PoliceMP:Networking:ClothingLocker:GetLockerOutfits";
        public const string GetLockerClothes = "PoliceMP:Networking:ClothingLocker:GetLockerClothing";
        public const string DiscordRichPresenceGetOptions = "PoliceMP:DiscordRichPresence:GetOptions";
        public const string SendDogMeCommand = "PoliceMP:Dsu:SendDogMeCommand";
        public const string SendDogDoCommand = "PoliceMP:Dsu:SendDogDoCommand";
        public const string FetchDsuAceAllowed = "PoliceMP:Dsu:RequestDsuAceAllowed";
        public const string GetGaragePoints = "PoliceMP:Networking:Garage:GetGaragePoints";
        public const string GetGarageData = "PoliceMP:Networking:Garage:GetGarageData";
        public const string GetSirenType = "PoliceMP:Networking:Garage:GetVehicleSirenType";
        public const string OnGarageVehicleCreated = "PoliceMP:Networking:Garage:OnGarageVehicleCreated";
        public const string GetPedInfoByNetworkId = "PoliceMP:GetPedInfoByNetworkId";
        public const string GetPedInfoByName = "PoliceMP:GetPedInfoByName";
        public const string GetVehicleInfoByNetworkId = "PoliceMP:GetVehicleInfoByNetworkId";
        public const string UpdateVehicleInfoMarkers = "PoliceMP:UpdateVehicleInfoMarkers";
        public const string UpdatedVehicleExpiredMarkers = "PoliceMP:UpdatedVehicleExpiredMarkers";
        public const string GetAllQuestions = "PoliceMP:GetAllQuestions";
        public const string SendPropToServer = "PoliceMP:RoadManagement:SendPropToServer";
        public const string GetPropData = "PoliceMP:RoadManagement:GetPropData";
        public const string GetNearestPropFromServer = "PoliceMP:RoadManagement:GetNearestPropFromServer";
        public const string RemovePropFromServerList = "PoliceMP:RoadManagement:RemovePropFromServerList";
        public const string SendCloseRoadEventToServer = "PoliceMP:RoadManagement:SendCloseRoadEventToServer";
        public const string SendOpenRoadEventToServer = "PoliceMP:RoadManagement:SendOpenRoadEventToServer";
        public const string SendSpeedZoneToServer = "PoliceMP:RoadManagement:SendSpeedZoneToServer";
        public const string RequestAllRoadManagementPositions = "PoliceMP:RoadManagement:RequestAllSpeedZones";
        public const string RequestPlacedConesByUser = "PoliceMP:RoadManagement:RequestPlacedConesByUser";
        public const string SendCreateRoadPropToServer = "PoliceMP:RoadManagement:SendCreateRoadPropToServer";
        public const string FetchAcePerms = "PoliceMP:RoadManagement:FetchAcePerms";
        public const string SendTweetCommandToServer = "PoliceMp:Roleplay:SendTweetCommandToServer";
        public const string Send101CallToServer = "PoliceMp:Roleplay:Send101CallToServer";
        public const string Send999CallToServer = "PoliceMp:Roleplay:Send999CallToServer";
        public const string SendMeCommandToServer = "PoliceMp:Roleplay:SendMeCommandToServer";
        public const string SendDoCommandToServer = "PoliceMp:Roleplay:SendDoCommandToServer";
        public const string SendControlNotificationToServer = "PoliceMp:Roleplay:SendControlNotificationToServer";
        public const string ControlStatus = "PoliceMp:Control:ControlStatus";
        public const string GetControlStatus = "PoliceMp:Control:GetControlStatus";
        public const string CROStatus = "PoliceMp:CRO:CROStatus";
        public const string GetCROStatus = "PoliceMp:CRO:GetCROStatus";
        public const string GetOptions = "PoliceMP:GetOptions";
        public const string SpawnGetOptions = "PoliceMP:Spawn:GetOptions";
        public const string PlayerSpawned = "PoliceMP:Spawn:PlayerSpawned";
        public const string SendUserDivision = "PoliceMP:Spawn:SendUserDivision";
        public const string VehicleFaultGeneratorGetOptions = "PoliceMP:VehicleFaultGenerator:GetOptions";
        public const string SetVoiceChannelForPlayer = "PoliceMp:Voice:SetVoiceChannelForPlayer";
        public const string SendFlashBangEventToServer = "PoliceMP:Weapons:SendFlashBangEventToServer";
        public const string FetchAllNetworkIdsFromServer = "PoliceMP:PlayerService:FetchAllNetworkIdsFromServer";
        public const string FetchPlayerNameFromNetworkId = "PoliceMP:PlayerService:FetchPlayerNameFromNetworkId";
        public const string FetchAllPlayersFromServer = "PoliceMP:PlayerService:FetchPlayersFromServer";
        public const string SendAllPlayersToClient = "PoliceMP:PlayerService:SendAllPlayersToClient";
        public const string SendBackupPointToPlayer = "PoliceMP:BackupSystem:SendBackupPointToPlayer";
        public const string RequestPlayerInfo = "PoliceMP:PlayerInfo:RequestPlayerInfo";
        public const string SendDogSoundEventToServer = "PoliceMP:Dog:SendDogBarkEventToServer";
        public const string SendDogParticleFxEventToServer = "SendDogParticleFxEventToServer";
        public const string SendWhatThreeWordPosToClient = "PoliceMP:WhatThreeWord:SendWordDataToClient";
        public const string SendWhatThreeWordToClient = "PoliceMP:WhatThreeWord:SendWordDataToClient";
        public const string RoadsDeleteProp = "PoliceMP:RoadManagement:DeletePropServer";

        public const string SpawnNotifyNHS = "PoliceMP:Spawn:NotifyNHS";
        public const string SpawnNotifyPlayerToRevive = "PoliceMP:Spawn:NotifyPlayerToRevive";

        public const string TaserDetectSuspect = "PoliceMP:Taser:ServerDetectSuspect";
        public const string TaserSuspectDetected = "PoliceMP:Taser:ServerSuspectDetected";
        public const string TaserReactivateSuspect = "PoliceMP:Taser:ServerReactivateSuspect";
        public const string TaserUpdateLog = "PoliceMP:Taser:ServerUpdateLog";
        public const string TaserCheckLocation = "PoliceMP:Taser:ServerCheckLocation";
        public const string TaserBarbsInvalidated = "PoliceMP:Taser:ServerBarbsInvalidated";
        public const string TaserNotifyBarbsRemoved = "PoliceMP:Taser:ServerNotifyBarbsRemoved";
        public const string TaserDisplayNotification = "PoliceMP:Taser:ServerDisplayNotification";
        public const string TaserReload = "PoliceMP:Taser:ServerTaserReload";

        public const string SoundToClient = "PoliceMP:Sound:ServerToClient";
        public const string SoundToAll = "PoliceMP:Sound:ServerToAll";
        public const string SoundToRadius = "PoliceMP:Sound:ServerToRadius";
        public const string SoundToCoords = "PoliceMP:Sound:ServerToCoords";

        public const string AdminSummonPlayerToPlayer = "PoliceMP:AdminMenu:SummonPlayerToPlayer";
        public const string AdminSendPopupToServer = "PoliceMP:AdminMenu:SendPopupToServer";
        public const string AdminTeleportPlayerToPlayer = "PoliceMP:AdminMenu:TeleportPlayerToPlayer";
        public const string AdminKillPlayer = "PoliceMP:AdminMenu:KillPlayer";
        public const string ForceSetWeatherToServer = "PoliceMP:AdminMenu:ForceSetWeatherToServer";
        public const string ClearAreaOfPlayer = "PoliceMP:AdminMenu:ClearNearbyAreaOfPlayer";
        public const string AdminSendFreezeEventToServer = "PoliceMP:AdminMenu:AdminSendFreezeEventToServer";
        public const string AdminAttachPedToServer = "PoliceMP:AdminMenu:AdminAttachPedToServer";
        public const string ForcePlayerTime = "PoliceMP:Time:ForcePlayerTime";
        public const string AdminManageServerTime = "PoliceMP:Time:ManageServerTime";
        public const string FetchServerTime = "PoliceMP:Time:FetchServerTime";
        public const string AdminYeetEntity = "PoliceMP:Admin:YeetEntity";
        public const string AdminGetNetworkFirstOwner = "PoliceMP:Admin:GetNetworkFirstOwner";
        public const string AdminYeetEntityLog = "PoliceMP:Admin:YeetEntityLog";
        public const string AdminJoinBucket = "PoliceMP:Admin:JoinBucket";
        public const string AdminListBucket = "PoliceMP:Admin:ListBucket";
        public const string AdminMovePlayerToBucket = "PoliceMP:Admin:MovePlayerToBucket";
        public const string AdminMoveAllPlayersToBucket = "PoliceMP:Admin:MoveAllPlayersToBucket";
        public const string AdminGetPlayerBucket = "PoliceMP:Admin:AdminGetPlayerBucket";
        public const string SendModMessageToServer = "PoliceMP:Admin:SendModMessageToServer";

        public const string FetchWeatherFromServer = "PoliceMP:Weather:FetchFromServer";

        public const string RequestTrainNetID = "PoliceMP:Trains:RequestIDFromServer";

        public const string StartCharacterCustomisation = "PoliceMP:Characters:StartCustomisation";
        public const string OnFinishCharacterCustomisation = "PoliceMP:Characters:FinishCustomisation";
        public const string SetPlayerCharacterCustomisation = "PoliceMP:Characters:SetPlayerCharacterCustomisation";
        public const string OnCharacterAppearanceApplied = "PoliceMP:Characters:AppearanceApplied";

        public const string OnReceiveCallSign = "PoliceMP:Callsign:OnReceiveCallSign";

        public const string RequestVehicleSirenType = "PoliceMP:Vehicle:RequestVehicleSirenType";

        public const string SendLFBMDTSoundToPlayers = "PoliceMP:LFB:SendLFBMDTSoundToPlayers";

        public const string GrassBagGrass = "PoliceMP:GrassBag:Grass";
        public const string GrassBagEntityDamagedByPlayer = "PoliceMP:GrassBag:EntityDamagedByPlayer";
        public const string GrassBagVehicleDamagedByPlayer = "PoliceMP:GrassBag:VehicleDamaged";
        public const string GrassBagPedRunOverByPlayer = "PoliceMP:GrassBag:PedRunOverByPlayer";
        public const string GrassBagPedDamagedByPlayer = "PoliceMP:GrassBag:PedDamaged";
        public const string GrassBagDrivingWrongSide = "PoliceMP:GrassBag:DrivingWrongSide";

        // New Road Management
        public const string CreateRoadPropOnServer = "PoliceMP:RoadManagement:CreateRoadPropOnServer";
        public const string FetchNodePosition = "PoliceMP:RoadManagement:FetchNodePosition";
        public const string UpdateNodeAtPosition = "PoliceMP:RoadManagement:UpdateNodeAtPosition";
        public const string UpdateSpeedAtPosition = "PoliceMP:RoadManagement:UpdateSpeedAtPosition";
        public const string ClearRoadPropOnServer = "PoliceMP:RoadManagement:ClearRoadPropOnServer";

        //Player to player actions
        public const string PlayerTacklePlayer = "PoliceMP:TackleScript:PlayerTacklePlayer";
        public const string PlayerArrestPlayer = "PoliceMP:TackleScript:PlayerArrestPlayer";
        public const string PlayerCPRPlayer = "PoliceMP:CPRAction:PlayerCPRPlayer";
        public const string PlayerDefibPlayer = "PoliceMP:CPRAction:PlayerDefibPlayer";

        //Network owner instructions
        public const string NetOwnerClientRequestingCarToggleLock = "PoliceMP:NetworkOwner:UnlockCar";

        // Send Vehicle to Players Command / Script
        public const string SendVehicleSpawnToServer = "PoliceMP:VehicleSpawn:SendVehicleSpawnToServer";

        // Anpr Pings
        public const string AnprPingRequest = "PoliceMP:AnprPings:AnprPingRequest";
        public const string AnprPingSendCoords = "PoliceMP:AnprPings:AnprPingSendCoords";

        // Remote Hide Blips For Player
        public const string RemoteHideBlipsToServer = "PoliceMP:RemoteHideBlip:RemoteHideBlipsToServer";
        public const string RemoteShowBlipsToServer = "PoliceMP:RemoteHideBlip:RemoteShowBlipsToServer";

        // Logging player DISCORD ID to Console on Connect
        public const string LogDiscordIDToServer = "PoliceMP:LogDiscordID:LogDiscordIDToServer";

        // Force Info chat messages
        public const string SendForceInfoToServer = "PoliceMP:ForceInfo:SendForceInfoToServer";

        // AiCallouts
        public const string NewCallout = "PoliceMP:AiCallouts:NewCallout";
        public const string AttachedToCallout = "PoliceMP:AiCallouts:AttachedToCallout";
        public const string ExperiencePointsUpdater = "PoliceMP:AICallouts:ExperiencePointsUpdater";

        // Indicators
        public const string IndicatorsChanged = "PoliceMP:Indicators:IndicatorsChanged";

        // Pass Alarm LFB
        public const string FirefighterPass = "PoliceMP:Firefighter:FirefighterPass";
        
        // CreateScent for Civ-DSU
        public const string CreateScent = "PoliceMP:CivDSU:CreateScent";
    }
}