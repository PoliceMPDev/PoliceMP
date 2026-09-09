--[[
-- Author: Tim Plate
-- Project: Advanced Roleplay Environment
-- Copyright (c) 2022 Tim Plate Solutions
--]]

ENUM_EVENT_TYPES = {
    NETWORK_ENTITY_DAMAGE = "CEventNetworkEntityDamage", -- string: NetworkEntityDamage event (low-level) from gta
    RECEIVE_AUTH_TOKEN = "CEventReceiveAuthToken", -- string: Receive Auth token event
    PROCEDURE_GET_MENU_INFO = "CEventGetMenuInfo", -- string: Procedure allowed to open menu event
    PROCEDURE_REQUEST_HEALTH_BUFFER = "CProcedureRequestHealthBuffer", -- string: Callback for request health buffer,
    PROCEDURE_OPEN_PLAYER_MENU = "SProcedureOpenPlayerMenu", -- string: Opens the player menu for the specified client.
    PROCEDURE_HAS_ITEM = "SProcedureHasItem", -- string: Callback for has item.
    PROCEDURE_PERFORM_ACTION = "SProcedurePerformAction", -- string: Callback for perform action.
    PROCEDURE_GET_UNCONSCIOUS_TIME = "SProcedureGetUnconsciousTime", -- string: Callback for get unconscious time.
    PROCEDURE_UPDATE_TRIAGE = "SProcedureUpdateTriage", -- string: Callback for update triage.
    EVENT_APPLY_TOURNIQUET = "SEventApplyTourniquet", -- string: Applies a tourniquet at a specified bodypart.
    EVENT_REMOVE_TOURNIQUET = "SEventRemoveTourniquet", -- string: Removes a tourniquet at a specified bodypart.
    EVENT_BREAK_BONE = "SEventEventBreakBone", -- string: Removes a tourniquet at a specified bodypart.

    EVENT_INJECT_MEDICATION = "SEventInjectMedication", -- string: Injects a medication at a specified bodypart.
    EVENT_APPLY_BANDAGE = "SEventApplyBandage", -- string: Applies a bandage at a specified bodypart.
    EVENT_APPLY_SURGICAL_KIT = "SEventApplySurgicalKit", -- string: Applies a surgical kit at a specified bodypart.
    EVENT_APPLY_INFUSION = "SEventApplyInfusion", -- string: Applies an infusion at a specified bodypart.
    EVENT_APPLY_INFUSION_OXYGEN = "SEventApplyInfusion", -- string: Applies an infusion at a specified bodypart.
    EVENT_APPLY_PORTABLE_SUCTION = "SEventApplyPortableSuction",
    EVENT_CLEAR_INJURY = "SEventClearInjury",

    EVENT_APPLY_CPR = "SEventApplyCPR", -- string: Applies CPR at a specified bodypart.
    EVENT_APPLY_DEFIBRILLATOR = "SEventApplyDefibrillator", -- string: Applies a defibrillator at a specified bodypart.
    EVENT_APPLY_EMERGENCY_REVIVE_KIT = "SEventApplyEmergencyReviveKit", -- string: Applies an emergency revive kit at a specified bodypart.
    EVENT_SAVE_LOG = "SEventSaveLog", -- string: Saves the log from a player.
    EVENT_PUT_IN_BODYBAG = "SEventPutInBodyBag", -- string: Puts a player in a bodybag.
    EVENT_START_ECG = "SEventStartECG", -- string: Starts a ecg on a player.
    EVENT_STOP_ECG = "SEventStopECG", -- string: Stops a ecg on a player.
    EVENT_PUT_IN_VEHICLE = "SEventPutInVehicle", -- string: Puts a player in a vehicle.
    EVENT_PULL_OUT_VEHICLE = "SEventPullOutVehicle", -- string: Pulls a player out of a vehicle.
    EVENT_RESPAWN_PLAYER = "SEventRespawnPlayerEvent", -- string: Respawns the player.
    EVENT_STOP_CARRY = "SEventStopCarry", -- string: Stops carrying a player.
    EVENT_START_CARRY = "SEventStartCarry", -- string: Starts carrying a player.
    EVENT_SYNC_LOCAL = "SEventSyncLocal", -- string: Syncs the local player.
    EVENT_SYNC_TARGET = "SEventSyncTarget", -- string: Syncs the target player.
    EVENT_LOAD_PLAYER = "SEventLoadPlayer", -- string: Loads a player.
    EVENT_LOAD_PLAYER_STANDALONE = "SEventLoadPlayerStandalone", -- string: Loads a player standalone.
    EVENT_SET_HEALTH_BUFFER = "CEventSetHealthBuffer", -- string: Sets the health buffer for a player.
    EVENT_LOG_PLAYER_KILL = "SEventLogPlayerKill", -- string: Logs a player kill.
    PLAY_3D_SOUND = "CEventPlay3DSound", -- string: Plays a 3D sound.
    STOP_3D_SOUND = "CEventStop3DSound", -- string: Stops a 3D sound.
}

ENUM_HOOKABLE_EVENTS = {
    UNCONSCIOUS_STATE_CHANGED = "OnUnconsciousStateChanged", -- string: Unconscious state changed event.
    PLAYER_RESPAWNED = "OnPlayerRespawned", -- string: Player respawned event.
    DAMAGE_RECEIVED = "OnDamageReceived", -- string: Damage received event.
}

ENUM_BONE_AREAS = {
    HEAD = {39317, 31086, 47495, 20178, 17188}, -- Head
    LEFT_ARM = {18905, 61163, 45509, 60309}, -- Left arm
    RIGHT_ARM = {57005, 28252, 40269, 28422}, -- Right arm
    TORSO = {57597, 64729, 24816, 24817, 24818, 11816, 10706}, -- Torso
    LEFT_LEG = {63931, 58271, 14201}, -- Left leg
    RIGHT_LEG = {36864, 51826, 52301}, -- Right leg
    UNKNOWN = {0} -- Unknown
}

ENUM_DEFINED_FRAMEWORKS = {
    QB_CORE = "QB_CORE", 
    ES_EXTENDED = "ES_EXTENDED", 
    STANDALONE = "STANDALONE"
}

ENUM_AVAILABLE_AREAS = {}
local finalTable = {}
for k, v in pairs(ENUM_BONE_AREAS) do
    if k ~= "UNKNOWN" then table.insert(ENUM_AVAILABLE_AREAS, k) end
    if type(v) == "table" then for l, j in pairs(v) do finalTable[j] = k end end
end

ENUM_BONE_AREAS = {}
ENUM_BONE_AREAS = finalTable

ENUM_BLOOD_VOLUME_CLASSES = {
    CLASS_1_HERMORRHAGE = 6000, -- Lost less than 15% blood class i hemorrhage
    CLASS_2_HERMORRHAGE = 5100, -- Lost more than 15% blood class ii hemorrhage
    CLASS_3_HERMORRHAGE = 4200, -- Lost more than 30% blood class iii hemorrhage  
    CLASS_4_HERMORRHAGE = 3600, -- Lost more than 40% blood class iv hermorrhage
    FATAL = 3000, -- Lost more than 50% blood, unrecovarable
}