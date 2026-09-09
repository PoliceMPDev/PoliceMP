--[[
-- Author: Tim Plate
-- Project: Advanced Roleplay Environment
-- Copyright (c) 2022 Tim Plate Solutions
--]]

--- Client extension module for Advanced Roleplay Environment.

local progressBarPool = {}

-- Notification

---Shows a notification
---@param text any
function ShowNotification(text)
    LogDebug(true, "ShowNotification: ", text)
	BeginTextCommandThefeedPost("STRING")
	AddTextComponentString(text)
	EndTextCommandThefeedPostTicker(false, false)
end

-- ClosestPlayer
---Gets all players
---@return table
function GetPlayers()
	local players = {}

	for _, player in ipairs(GetActivePlayers()) do
		local ped = GetPlayerPed(player)

		if DoesEntityExist(ped) then
			table.insert(players, player)
		end
	end

	return players
end

---Gets the closest player
---@return integer
---@return number
function GetClosestPlayer()
    local players         = GetPlayers()
    local closestDistance = -1
    local closestPlayer   = -1
    local playerId        = PlayerId()

    for i=1, #players, 1 do
        local target = GetPlayerPed(players[i])

        if players[i] ~= playerId then
            local targetCoords = GetEntityCoords(target)
            local distance     = GetDistanceBetweenCoords(targetCoords, ClientData.coords, true)

            if closestDistance == -1 or closestDistance > distance then
                closestPlayer   = players[i]
                closestDistance = distance
            end
        end
    end

    return closestPlayer, closestDistance
end

-- Animations

-- Source: https://github.com/zaphosting/esx/blob/master/es_extended/client/modules/streaming.lua#L59
---Requests an animation dictionary
---@param animDict any
---@param cb any
function RequestAnimDictCustom(animDict, cb)
	if not HasAnimDictLoaded(animDict) then
		RequestAnimDict(animDict)

		while not HasAnimDictLoaded(animDict) do
			Citizen.Wait(1)
		end
	end

	if cb ~= nil then
		cb()
	end
end

---Runs an animation
---@param animation any
---@param duration any
function RunAnimation(animation, duration)
    if animation == nil then
        return
    end

    RequestAnimDictCustom(animation.lib, function()
        TaskPlayAnim(ClientData.ped, animation.lib, animation.name, 8.0, -8.0, -1, 9, 0, false, false, false)
        
        local gameTimer = GetGameTimer()
        while true do
            if GetGameTimer() - gameTimer < duration then
                Citizen.Wait(0)
            else
                BroadcastStop3DSound()
                ClearPedTasks(ClientData.ped)
                return
            end

            Citizen.Wait(0)
        end
    end)
end

-- ProgressBar

---Creates a progress bar
---@param text any
---@param duration any
---@param xFactorParam any
function CreateProgressBar(text, duration, xFactorParam)
    if #progressBarPool > 0 then LogWarning("Can't create a progressbar. A progressbar is already running.") return end
    local gameTimer = GetGameTimer()
    table.insert(progressBarPool, {
        text = TranslateText("ACTION_" .. text),
        duration = duration,
        xFactorParam = xFactorParam,
        creationTimestamp = gameTimer,
        endTimestamp = gameTimer + duration
    })

    progressBarPool[#progressBarPool].position = #progressBarPool
end

---Destroys a progress bar
---@param data any
function DestroyProgressBar(data)
    table.remove(progressBarPool, data.position)
end

---Returns true if a progress bar is active
---@return boolean
function IsProgressBarActive()
    return #progressBarPool > 0
end

Citizen.CreateThread(function()
    local safeZoneSize = GetSafeZoneSize()

    if not HasStreamedTextureDictLoaded('timerbars') then
        RequestStreamedTextureDict('timerbars', true)
        while not HasStreamedTextureDictLoaded('timerbars') do Citizen.Wait(1) end
    end

    while true do
        Citizen.Wait(0)
        local gameTimer = GetGameTimer()

        for _, v in ipairs(progressBarPool) do
            local width, xValue, yValue

            if v.endTimestamp > gameTimer then
                local timeDifference = gameTimer - v.creationTimestamp
                if timeDifference < v.duration then width = timeDifference * (0.085 / v.duration) end
                local correctionValue = ((1.0 - RoundValue(safeZoneSize, 2)) * 100) * 0.005
                xValue, yValue = 0.9350 - correctionValue, 0.92 - correctionValue -- CHANGES BY SMALLP TO ADJUST 

                SetScriptGfxDrawOrder(0)
                DrawSprite('timerbars', 'all_black_bg', xValue, yValue, 0.15, 0.0325, 0.0, 255, 255, 255, 180)

                SetScriptGfxDrawOrder(1)
                DrawRect(xValue + 0.0275, yValue, 0.085, 0.0095, 143, 95, 13, 180)

                SetScriptGfxDrawOrder(2)
                DrawRect(xValue - 0.015 + (width / 2), yValue, width, 0.0095, 255, 162, 0, 180)

                SetTextColour(255, 255, 255, 180)
                SetTextFont(0)
                SetTextScale(0.3, 0.3)
                SetTextCentre(true)
                SetTextEntry('STRING')
                AddTextComponentString(v.text)
                SetScriptGfxDrawOrder(3)
                DrawText(xValue - v.xFactorParam, yValue - 0.012)
            else
                DestroyProgressBar(v)
            end
        end
    end
end)

-- Carry player

local carrying = false
local carryingData = {}
---Carrys a player
---@param targetData any
---@param targetHealthBuffer any
function CarryPlayer(targetData, targetHealthBuffer)
    if carrying then StopCarry() return end
    -- if not targetHealthBuffer.anesthesia then return end

    if targetData.ped == -1 or not DoesEntityExist(targetData.ped) or targetData.distance > 3.0 then return end
    carrying = true
    carryingData = {
        ped = targetData.ped,
        source = targetData.source,
        distance = targetData.distance
    }

    TriggerServerEvent(ENUM_EVENT_TYPES.EVENT_START_CARRY, TempAuthToken, TargetData.source)
    ClosePlayerMenu(false)
end

---Stop active carry
function StopCarry()
    if not carrying then return end

    local distance = GetDistanceBetweenCoords(ClientData.coords, GetEntityCoords(carryingData.ped), true)
    if carryingData.ped == -1 or distance > 3.0 then return end 

    TriggerServerEvent(ENUM_EVENT_TYPES.EVENT_STOP_CARRY, TempAuthToken, carryingData.source)
    carrying = false
    ClearPedTasks(ClientData.ped)
    DetachEntity(ClientData.ped, true, true)
end

---Plays the local animation loop
function PlayLocalAnimationLoop()
    while not IsEntityPlayingAnim(ClientData.ped, "mini@cpr@char_b@cpr_def", "cpr_pumpchest_idle", 3) and (ClientHealthBuffer.unconscious or ClientHealthBuffer.anesthesia) and CarryAnimationData == nil and not IsPedInAnyVehicle(ClientData.ped, true) do
        if UnconsciousAnimationAfter or 0 < GetGameTimer() then
            TaskPlayAnim(ClientData.ped, "mini@cpr@char_b@cpr_def", "cpr_pumpchest_idle", 8.0, -8.0, 100000, 1, 0, false, false, false)
        end
        Citizen.Wait(0)
    end

    if ClientData.inVehicle and IsEntityPlayingAnim(ClientData.ped, "mini@cpr@char_b@cpr_def", "cpr_pumpchest_idle", 3) then
        ClearPedTasks(ClientData.ped)
    end

    while CarryAnimationData ~= nil and not IsEntityPlayingAnim(ClientData.ped, CarryAnimationData.lib, CarryAnimationData.name, 3) do
        TaskPlayAnim(ClientData.ped, CarryAnimationData.lib, CarryAnimationData.name, 8.0, 8.0, -1, CarryAnimationData.flag, 0, false, false, false)
        Citizen.Wait(0)
    end
end

-- Player menu
---Callbacks if the player is allowed to use the menu and gets active items from player
function GetMenuInfo()
    local p = promise.new()

    TriggerServerCallback {
        eventName = ENUM_EVENT_TYPES.PROCEDURE_GET_MENU_INFO,
        args = { TempAuthToken },
        callback = function(result)
            if not result then p:reject("No value returned")end
            p:resolve(result)
        end
    }

    return Citizen.Await(p)
end

-- Framework functions
---Respawns the player
---@param coords any
---@param authToken any
function RespawnPlayer(coords, authToken)
    _TriggerEvent(ENUM_HOOKABLE_EVENTS.PLAYER_RESPAWNED, nil)
    TriggerServerEvent(ENUM_EVENT_TYPES.EVENT_RESPAWN_PLAYER, json.encode(coords), authToken)
end

-- https://github.com/zaphosting/esx_12/blob/master/esx_ambulancejob/client/main.lua#L331
---Respawns the ped
---@param ped any
---@param coords any
---@param heading any
function RespawnPed(ped, coords, heading)
    coords = { x = coords.x, y = coords.y, z = coords.z }
    local inWater, waterHeight = GetWaterHeight(coords.x, coords.y, coords.z, 0)
    if inWater then
        coords.z = waterHeight + 0.6
    end

    --SetEntityCoords(ped, coords.x, coords.y, coords.z - 0.8, false, false, false, false)
    --SetEntityHeading(ped, heading)
	NetworkResurrectLocalPlayer(coords.x, coords.y, coords.z - 0.8, heading, true, false)
	SetEntityHealth(ped, 200)
    SetPlayerInvincible(ped, false)
	ClearPedBloodDamage(ped)

    -- Causes bad spawning?
	-- TriggerEvent('playerSpawned')
end
RegisterCommand("panic", function()
    TriggerPanicCommand()
end, false)

---Emergency dispatch to configured receiver
---@param playerPed any
---@param coords any
function EmergencyDispatch(playerPed, coords)
    if not ClientHealthBuffer.unconscious then return end

    local emergencyDispatch = ClientConfig.m_emergencyDispatch
    if not emergencyDispatch.enabled then return end

    if emergencyDispatch.phoneConfiguration == "woodyspanic" then
                local command = "panic"
                TriggerServerEvent("chatMessage", "SYSTEM", {255, 0, 0}, command) -- Sends the /panic command to the server
            end
        
            -- Now you can call TriggerPanicCommand whenever you want to execute /panic
            -- For example, you can call it when a specific condition is met or from a menu
            -- Example usage:
            -- TriggerPanicCommand()
        end

---Loads the player
function LoadPlayer()
    while not NetworkIsPlayerActive(PlayerId()) do
        Citizen.Wait(0)
    end

    while not TempAuthToken or TempAuthToken == "" do
        Citizen.Wait(0)
    end

    TriggerServerEvent(ENUM_EVENT_TYPES.EVENT_LOAD_PLAYER, TempAuthToken)
end

---Spawns a game object
---@param name any
---@param x any
---@param y any
---@param z any
---@return any
function SpawnGameObject(name, x, y, z)
    if not ClientConfig.m_spawnGameObjects.enabled then return 0 end
    local model = GetHashKey(name)
    RequestModel(model)
    local timeout = 0
    while not HasModelLoaded(model) and timeout < 500 do timeout = timeout + 1 Citizen.Wait(0) end
    if timeout >= 500 then return end

    if #SpawnedGameObjects > (ClientConfig.m_spawnGameObjects.limit or 50) then DeleteObject(SpawnedGameObjects[1].object) table.remove(SpawnedGameObjects, 1) end

    local object = CreateObject(model, x, y, z, true, true, false)
    SetModelAsNoLongerNeeded(model)
    table.insert(SpawnedGameObjects, {
        object = object,
        timestamp = GetGameTimer()
    })

    return object
end

---Cleans up spawned game objects
function CleanUpGameObjects()
    for i, v in ipairs(SpawnedGameObjects) do
        if (GetGameTimer() - v.timestamp) > ClientConfig.m_spawnGameObjects.lifetime then
            DeleteObject(v.object)
            table.remove(SpawnedGameObjects, i)
        end
    end
end

-- Command functions
---Function for the emergency dispatch command
function CallEmergencyDispatch()
    if not ClientHealthBuffer then return end
    if not ClientHealthBuffer.unconscious then return end
    if not ClientConfig.m_emergencyDispatch.enabled then return end
    if (GetGameTimer() - LastDispatch) < ClientConfig.m_emergencyDispatch.cooldown * 1000 then return end
    if IsPauseMenuActive() then return end

    EmergencyDispatch(ClientData.ped, ClientData.coords)
    LastDispatch = GetGameTimer()
    SendNUIMessage({
        payload = "toggleDispatchInfo",
        payloadData = {
            value = false
        }
    })

    LastDispatchHidden = true
end

---Function for the self menu command
function OpenSelfMenu()
    -- if not ClientHealthBuffer then return end
    -- if ClientHealthBuffer.unconscious then return end
    -- if IsPauseMenuActive() then return end

    ShowPlayerMenu()
    
end

---Function for the interaction menu command
function OpenInteractionMenu()
    if not ClientHealthBuffer then return end
    if ClientHealthBuffer.unconscious then return end
    if IsPauseMenuActive() then return end

    local closestPlayer, closestDistance = GetClosestPlayer()
    if closestPlayer ~= -1 and closestDistance <= 3.0 then ShowPlayerMenu(GetPlayerServerId(closestPlayer)) end
end

---Function for the manual respawn command
function ManualRespawn()
    if not ClientHealthBuffer then return end
    if not ClientHealthBuffer.unconscious and not ClientHealthBuffer.bodybag then return end
    if not ClientConfig.m_allowManualRespawnWhileBeingUnconscious and not AllowManualRespawn then return end
    if not AllowManualRespawn and GetRemainingUnconsciousSeconds(ClientHealthBuffer) > 0 then return end
    if IsPauseMenuActive() then return end

    if AllowManualRespawn then AllowManualRespawn = false end
    RespawnPlayer(ClientData.coords, TempAuthToken)
end

---Function for the cancel interaction command
function CancelInteraction()
    if not ClientHealthBuffer then return end
    if CarryAnimationData ~= nil then StopCarry() end
    if ClientHealthBuffer.unconscious then return end
    if not IsProgressBarActive() then return end
    if IsPauseMenuActive() then return end

    BroadcastStop3DSound()
    DestroyProgressBar({ position = 1 })
    ClearPedTasks(ClientData.ped)
    TargetData.canceled = true
end

-- Non-Input-Mapper Key function handler
---Handles the key interactions
---@param keyConfig any
function HandleKeyInteractions(keyConfig)
    if IsControlJustPressed(0, keyConfig.keys.CANCEL_INTERACTION.control_id) then CancelInteraction() end
    if IsControlJustPressed(0, keyConfig.keys.OPEN_SELF_MENU.control_id) then OpenSelfMenu() end
    if IsControlJustPressed(0, keyConfig.keys.OPEN_OTHER_MENU.control_id) then OpenInteractionMenu() end
    if IsControlJustPressed(0, keyConfig.keys.MANUAL_RESPAWN.control_id) then ManualRespawn() end
    if IsControlJustPressed(0, keyConfig.keys.EMERGENCY_DISPATCH.control_id) then CallEmergencyDispatch() end
end

-- Deathscreen function
---Handles the deathscreen
---@param bloodVolume any
function HandleUnconsciousScreen(bloodVolume)
    local remainingSeconds = GetRemainingUnconsciousSeconds(ClientHealthBuffer)

    SendNUIMessage({
        payload = "updateUnconsciousTime",
        payloadData = {
            seconds = remainingSeconds,
            max_seconds = ClientHealthBuffer.unconsciousTimeUntilDeath
        }
    })

    if GetGameTimer() - LastDispatch > ClientConfig.m_emergencyDispatch.cooldown * 1000 and LastDispatchHidden then
        SendNUIMessage({
            payload = "toggleDispatchInfo",
            payloadData = {
                value = (ClientConfig.m_emergencyDispatch.enabled and ClientConfig.m_configurableKeys.keys.EMERGENCY_DISPATCH.enabled) and not ClientHealthBuffer.bodybag,
                key = ClientConfig.m_configurableKeys.m_useInputMapper
                and GetControlStringFromKeyMapping("emergency_dispatch")
                or GetControlStringFromKeyMappingNoInputMapper(ClientConfig.m_configurableKeys.keys.EMERGENCY_DISPATCH.control_id)
            }
        })

        LastDispatchHidden = false
    end

    if not CheckForCriticalVitals(bloodVolume) and math.random(0, 100) > 75 and
        (
        ClientHealthBuffer.heartRate > 50 and
            ClientHealthBuffer.bloodVolume > ENUM_BLOOD_VOLUME_CLASSES.CLASS_4_HERMORRHAGE) then
        SetUnconsciousState(false)
    end

    if ClientHealthBuffer.unconscious and remainingSeconds == 0 then
        if ClientConfig.m_allowManualRespawnWhileBeingUnconscious then
            SendNUIMessage({
                payload = "showManualRespawnText",
                payloadData = {
                    value = ClientConfig.m_configurableKeys.keys.MANUAL_RESPAWN.enabled,
                    key = ClientConfig.m_configurableKeys.m_useInputMapper
                    and GetControlStringFromKeyMapping("manual_respawn")
                    or GetControlStringFromKeyMappingNoInputMapper(ClientConfig.m_configurableKeys.keys.MANUAL_RESPAWN.control_id)
                }
            })
        else
            RespawnPlayer(ClientData.coords, TempAuthToken)
        end
    end
end

local anesthesiaActive = false

function ShowAnesthesiaScreen(state)
    anesthesiaActive = state
    SendNUIMessage({
        payload = "toggleAnesthesiaScreen",
        payloadData = {
            value = state
        }
    })

    if state then
        CreateThread(function()
            while anesthesiaActive do
                EnableControlAction(0, 1, true) -- enable mouse look updown
                EnableControlAction(0, 2, true) -- enable mouse look leftright
                EnableControlAction(0, 245, true) -- enable T for text chat
                Wait(0)
            end
        end)
    end
end

-- Enumerator
local entityEnumerator = {
    __gc = function(enum)
        if enum.destructor and enum.handle then
        enum.destructor(enum.handle)
        end
        enum.destructor = nil
        enum.handle = nil
    end
}

local function EnumerateEntities(initFunc, moveFunc, disposeFunc)
    return coroutine.wrap(function()
        local iter, id = initFunc()
        if not id or id == 0 then
        disposeFunc(iter)
        return
        end
        
        local enum = {handle = iter, destructor = disposeFunc}
        setmetatable(enum, entityEnumerator)
        
        local next = true
        repeat
        coroutine.yield(id)
        next, id = moveFunc(iter)
        until not next
        
        enum.destructor, enum.handle = nil, nil
        disposeFunc(iter)
    end)
end

function EnumerateObjects()
    return EnumerateEntities(FindFirstObject, FindNextObject, EndFindObject)
end

function EnumeratePeds()
    return EnumerateEntities(FindFirstPed, FindNextPed, EndFindPed)
end

function EnumerateVehicles()
    return EnumerateEntities(FindFirstVehicle, FindNextVehicle, EndFindVehicle)
end

function EnumeratePickups()
    return EnumerateEntities(FindFirstPickup, FindNextPickup, EndFindPickup)
end

function GetCustomClosestVehicle(coords)    
    local oldDistance = 99999
    local vehicle = nil

    for v in EnumerateVehicles() do
        local checkDistance = GetDistanceBetweenCoords(coords, GetEntityCoords(v), true)
        if checkDistance <= oldDistance and v ~= vehicle then
            vehicle = v
            oldDistance = checkDistance
        end
    end

    return vehicle, oldDistance
end

-- Function

---Returns if the player damage is disabled
---@return boolean
function IsPlayerDamageDisabled()
    if GetResourceState("ws_ffa") == "started" and exports["ws_ffa"]:isInZone() then return true end
    return PlayerDamageDisabled
end

-- ox_target
if ClientConfig.m_ox_target_support then
    Citizen.CreateThread(function()
        Citizen.Wait(1000)
        local ox_menu_options = {
            {
                name = 'visn_are:openMenu',
                icon = 'fa-solid fa-diagnoses',
                label = TranslateText("OPEN_PLAYER_MENU"),
                distance = 1.5,
                onSelect = function(data)
                    ShowPlayerMenu(GetPlayerServerId(NetworkGetPlayerIndexFromPed(data.entity)))
                end
            }
        }

        exports["ox_target"]:addGlobalPlayer(ox_menu_options)
    end)
end

-- State saving
Citizen.CreateThread(function()
    while not NetworkIsPlayerActive(PlayerId()) do
        Citizen.Wait(0)
    end

    while not TempAuthToken or TempAuthToken == "" do
        Citizen.Wait(0)
    end

    Citizen.Wait(2500)
    TriggerServerEvent(ENUM_EVENT_TYPES.EVENT_LOAD_PLAYER_STANDALONE, TempAuthToken)
end)

RegisterNetEvent('esx:playerLoaded', LoadPlayer)
RegisterNetEvent('QBCore:Client:OnPlayerLoaded', LoadPlayer)