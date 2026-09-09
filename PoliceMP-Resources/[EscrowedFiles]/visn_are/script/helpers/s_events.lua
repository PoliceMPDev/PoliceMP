--[[
-- Author: Tim Plate
-- Project: Advanced Roleplay Environment
-- Copyright (c) 2022 Tim Plate Solutions
--]]

QS = nil
if ServerConfig.m_customInventory.enabled then
    if ServerConfig.m_customInventory.inventory_type == "qs-inventory" then
        TriggerEvent('qs-core:getSharedObject', function(library) QS = library end)
    end
end

RegisterNetEvent(ENUM_EVENT_TYPES.EVENT_APPLY_TOURNIQUET, function(authToken, targetPlayer, bodyPart)
    if not ValidateAuthToken(source, authToken) then return false end
    if not ValidateDistanceCheck(source, targetPlayer) then return false end
    if not ValidateBodyPart(bodyPart) then return false end
    if not HasItem(source, 'tourniquet', 1) then return false end

    if ServerConfig.m_itemsNeeded and not ShouldIgnoreItems(source) then RemoveItem(source, 'tourniquet', 1) end
    TriggerClientEvent(ENUM_EVENT_TYPES.EVENT_APPLY_TOURNIQUET, targetPlayer, bodyPart, AuthToken)
end)

RegisterNetEvent(ENUM_EVENT_TYPES.EVENT_REMOVE_TOURNIQUET, function(authToken, targetPlayer, bodyPart)
    if not ValidateAuthToken(source, authToken) then return false end
    if not ValidateDistanceCheck(source, targetPlayer) then return false end
    if not ValidateBodyPart(bodyPart) then return false end

    if ServerConfig.m_itemsNeeded and not ShouldIgnoreItems(source) then AddItem(source, 'tourniquet', 1) end
    TriggerClientEvent(ENUM_EVENT_TYPES.EVENT_REMOVE_TOURNIQUET, targetPlayer, bodyPart, AuthToken)
end)


RegisterNetEvent(ENUM_EVENT_TYPES.EVENT_INJECT_MEDICATION, function(authToken, targetPlayer, bodyPart, medication)
    if not ValidateDistanceCheck(source, targetPlayer) then return false end
    if not ValidateBodyPart(bodyPart) then return false end
    if not MEDICATIONS[medication] then return false end
    if not HasItem(source, medication, 1) then return false end
    if ServerConfig.m_itemsNeeded and not ShouldIgnoreItems(source) then RemoveItem(source, medication, 1) end
    TriggerClientEvent(ENUM_EVENT_TYPES.EVENT_INJECT_MEDICATION, targetPlayer, bodyPart, medication, AuthToken)
end)


RegisterNetEvent(ENUM_EVENT_TYPES.EVENT_APPLY_BANDAGE, function(authToken, targetPlayer, bodyPart, bandageType)
    if not ValidateDistanceCheck(source, targetPlayer) then return false end
    if not ValidateBodyPart(bodyPart) then return false end
    if not BANDAGES[bandageType] then return end
    if not HasItem(source, bandageType, 1) then return false end

    if ServerConfig.m_itemsNeeded then RemoveItem(source, bandageType, 1) end
    TriggerClientEvent(ENUM_EVENT_TYPES.EVENT_APPLY_BANDAGE, targetPlayer, bodyPart, bandageType, AuthToken)
end)

RegisterNetEvent(ENUM_EVENT_TYPES.EVENT_APPLY_SURGICAL_KIT, function(authToken, targetPlayer, bodyPart)
    if not ValidateDistanceCheck(source, targetPlayer) then return false end
    if not ValidateBodyPart(bodyPart) then return false end
    if not HasItem(source, 'surgical_kit', 1) then return false end

    if ServerConfig.m_itemsNeeded and not ShouldIgnoreItems(source) then RemoveItem(source, 'surgical_kit', 1) end
    TriggerClientEvent(ENUM_EVENT_TYPES.EVENT_APPLY_SURGICAL_KIT, targetPlayer, bodyPart, AuthToken)
end)

RegisterNetEvent(ENUM_EVENT_TYPES.EVENT_CLEAR_INJURY, function(authToken, targetPlayer, bodyPart, injuries)
    if not ValidateDistanceCheck(source, targetPlayer) then return false end
    if not ValidateBodyPart(bodyPart) then return false end
    if not HasItem(source, 'surgical_kit', 1) then return false end

    if ServerConfig.m_itemsNeeded and not ShouldIgnoreItems(source) then RemoveItem(source, 'surgical_kit', 1) end
    TriggerClientEvent(ENUM_EVENT_TYPES.EVENT_CLEAR_INJURY, targetPlayer, bodyPart, AuthToken, injuries)
end)

RegisterNetEvent(ENUM_EVENT_TYPES.EVENT_APPLY_INFUSION, function(authToken, targetPlayer, bodyPart, infusion, volume)

    if not ValidateDistanceCheck(source, targetPlayer) then return false end
    if not ValidateBodyPart(bodyPart) then return false end
    if not HasItem(source, infusion .. '_' .. volume, 1) then return false end

    if ServerConfig.m_itemsNeeded and not ShouldIgnoreItems(source) then RemoveItem(source, infusion .. '_' .. volume, 1) end
    TriggerClientEvent(ENUM_EVENT_TYPES.EVENT_APPLY_INFUSION, targetPlayer, bodyPart, infusion, volume, AuthToken)
end)


RegisterNetEvent("visn_are:test123", function(authToken, targetPlayer, bodyPart, OXYGEN, volume)

    if not ValidateAuthToken(source, authToken) then return false end
    if not ValidateDistanceCheck(source, targetPlayer) then return false end
    if not ValidateBodyPart(bodyPart) then return false end
    if not HasItem(source, OXYGEN .. '_' .. volume, 1) then return false end

    if ServerConfig.m_itemsNeeded and not ShouldIgnoreItems(source) then RemoveItem(source, OXYGEN .. '_' .. volume, 1) end
    TriggerClientEvent("visn_are:test123", targetPlayer, bodyPart, OXYGEN, volume, AuthToken)
end)


RegisterNetEvent(ENUM_EVENT_TYPES.EVENT_APPLY_CPR, function(authToken, targetPlayer, bodyPart)
    if not ValidateDistanceCheck(source, targetPlayer) then return false end
    if not ValidateBodyPart(bodyPart) then return false end

    TriggerClientEvent(ENUM_EVENT_TYPES.EVENT_APPLY_CPR, targetPlayer, bodyPart, AuthToken)
end)

RegisterNetEvent(ENUM_EVENT_TYPES.EVENT_APPLY_DEFIBRILLATOR, function(authToken, targetPlayer, bodyPart)
    if not ValidateDistanceCheck(source, targetPlayer) then return false end
    if not ValidateBodyPart(bodyPart) then return false end
    if not HasItem(source, "defibrillator", 1) then return false end

    TriggerClientEvent(ENUM_EVENT_TYPES.EVENT_APPLY_DEFIBRILLATOR, targetPlayer, bodyPart, AuthToken)
end)

RegisterNetEvent(ENUM_EVENT_TYPES.EVENT_APPLY_EMERGENCY_REVIVE_KIT, function(authToken, targetPlayer, bodyPart)
    if not ValidateDistanceCheck(source, targetPlayer) then return false end
    if not ValidateBodyPart(bodyPart) then return false end
    if not HasItem(source, "emergency_revive_kit", 1) then return false end

    if ServerConfig.m_itemsNeeded and not ShouldIgnoreItems(source) then RemoveItem(source, 'emergency_revive_kit', 1) end
    TriggerClientEvent(ENUM_EVENT_TYPES.EVENT_APPLY_EMERGENCY_REVIVE_KIT, targetPlayer, bodyPart, AuthToken)
end)

RegisterNetEvent(ENUM_EVENT_TYPES.EVENT_PUT_IN_VEHICLE, function(authToken, targetPlayer)
    if not ValidateDistanceCheck(source, targetPlayer) then return false end

    TriggerClientEvent(ENUM_EVENT_TYPES.EVENT_PUT_IN_VEHICLE, targetPlayer, AuthToken)
end)

RegisterNetEvent(ENUM_EVENT_TYPES.EVENT_PUT_IN_BODYBAG, function(authToken, targetPlayer)
    if not ValidateDistanceCheck(source, targetPlayer) then return false end
    if not HasItem(source, 'bodybag', 1) then return false end
    if ServerConfig.m_itemsNeeded and not ShouldIgnoreItems(source) then RemoveItem(source, 'bodybag', 1) end

    TriggerClientEvent(ENUM_EVENT_TYPES.EVENT_PUT_IN_BODYBAG, targetPlayer, AuthToken)
end)

RegisterNetEvent(ENUM_EVENT_TYPES.EVENT_START_ECG, function(authToken, targetPlayer)
    if not ValidateDistanceCheck(source, targetPlayer) then return false end
    if not HasItem(source, 'ecg_monitor', 1) then return false end

    TriggerClientEvent(ENUM_EVENT_TYPES.EVENT_START_ECG, targetPlayer, AuthToken)
end)

RegisterNetEvent(ENUM_EVENT_TYPES.EVENT_STOP_ECG, function(authToken, targetPlayer)
    if not ValidateDistanceCheck(source, targetPlayer) then return false end

    TriggerClientEvent(ENUM_EVENT_TYPES.EVENT_STOP_ECG, targetPlayer, AuthToken)
end)

RegisterNetEvent(ENUM_EVENT_TYPES.EVENT_PULL_OUT_VEHICLE, function(authToken, targetPlayer)
    if not ValidateDistanceCheck(source, targetPlayer) then return false end

    TriggerClientEvent(ENUM_EVENT_TYPES.EVENT_PULL_OUT_VEHICLE, targetPlayer, AuthToken)
end)

-- TODO: Try only getting .logs and setting it instead of whole health buffer
RegisterNetEvent(ENUM_EVENT_TYPES.EVENT_SAVE_LOG, function(authToken, targetPlayer, logMessage)

    local healthBuffer = Player(targetPlayer).state.healthBuffer
    if not healthBuffer or healthBuffer == nil then return end
    if not healthBuffer.logs then healthBuffer.logs = {} end

    table.insert(healthBuffer.logs, {
        message = logMessage,
        timestamp = os.time()
    })

    Player(targetPlayer).state:set('healthBuffer', healthBuffer, true)
    TriggerClientEvent(ENUM_EVENT_TYPES.EVENT_SAVE_LOG, targetPlayer, healthBuffer.logs, AuthToken)
end)

RegisterNetEvent(ENUM_EVENT_TYPES.EVENT_LOAD_PLAYER, function(authToken)
    if not ValidateSource(source) then return end
    if not ValidateAuthToken(source, authToken) then return end
    LoadPlayer(source)
end)

RegisterNetEvent(ENUM_EVENT_TYPES.EVENT_LOAD_PLAYER_STANDALONE, function(authToken)
    if not ServerConfig.m_stateSaving.enabled then return end
    if not ValidateSource(source) then return end
    if FRAMEWORK ~= ENUM_DEFINED_FRAMEWORKS.STANDALONE then return end

    LoadPlayer(source)
end)

RegisterNetEvent(ENUM_EVENT_TYPES.EVENT_START_CARRY, function(authToken, targetPlayer)
    if not ValidateDistanceCheck(source, targetPlayer) then return false end

    local localAnimationData = {
        lib = "missfinale_c2mcs_1",
        name = "fin_c2_mcs_1_camman",
        length = -1,
        flag = 49
    }

    local targetAnimationData = {
        lib = "nm",
        name = "firemans_carry",
        length = -1,
        xDistance = 0.15,
        yDistance = 0.27,
        zDistance = 0.63,
        zRotation = 0.0,
        flag = 33
    }

    TriggerClientEvent(ENUM_EVENT_TYPES.EVENT_SYNC_LOCAL, source, AuthToken, localAnimationData)
    TriggerClientEvent(ENUM_EVENT_TYPES.EVENT_SYNC_TARGET, targetPlayer, AuthToken, source, targetAnimationData)
end)

RegisterNetEvent(ENUM_EVENT_TYPES.EVENT_STOP_CARRY, function(authToken, targetPlayer)
    TriggerClientEvent(ENUM_EVENT_TYPES.EVENT_STOP_CARRY, source, AuthToken)
    TriggerClientEvent(ENUM_EVENT_TYPES.EVENT_STOP_CARRY, targetPlayer, AuthToken)
end)

local lastLogCooldown = {}
RegisterNetEvent(ENUM_EVENT_TYPES.EVENT_LOG_PLAYER_KILL, function(authToken, targetPlayer)
    if lastLogCooldown[source] or 0 > os.time() then return end
    lastLogCooldown[source] = os.time() + 5

    local playerHealthBuffer = Player(source).state.healthBuffer

    if targetPlayer == 0 then
        SendDiscordLog(TranslateText("NUI_PLAYER_KILLED_SELF", GetPlayerName(source), source), ServerConfig.m_discordLogging.healthBuffer and json.encode(playerHealthBuffer) or "")
        return
    end

    local targetName = GetPlayerName(targetPlayer)
    SendDiscordLog(TranslateText("NUI_PLAYER_KILLED_PLAYER", targetName, targetPlayer, GetPlayerName(source), source), ServerConfig.m_discordLogging.healthBuffer and json.encode(playerHealthBuffer) or "")
end)


RegisterServerCallback {
    eventName = ENUM_EVENT_TYPES.PROCEDURE_UPDATE_TRIAGE,
    eventCallback = function(source, authToken, targetPlayer, triageSelection)
        if not ValidateAuthToken(source, authToken) then return false end
        if not ValidateDistanceCheck(source, targetPlayer) then return false end
        if not IsAllowedToUseTriageSystem(source) then return false end

        TriggerClientEvent(ENUM_EVENT_TYPES.PROCEDURE_UPDATE_TRIAGE, targetPlayer, AuthToken, triageSelection)
        return true
    end
}

RegisterNetEvent(ENUM_EVENT_TYPES.EVENT_RESPAWN_PLAYER, function(coords, authToken)
    if FRAMEWORK == ENUM_DEFINED_FRAMEWORKS.ES_EXTENDED then
        local xPlayer = FRAMEWORK_DATA.GetPlayerFromId(source)
        if not xPlayer then return false end

        local respawnConfiguration = ServerConfig.m_respawnConfiguration

        if ServerConfig.m_customInventory.enabled then
            if ServerConfig.m_customInventory.inventory_type == "ox_inventory" then
                if respawnConfiguration.m_removeItemsOnDeath then
                    exports.ox_inventory:ClearInventory(source)
                end
            elseif ServerConfig.m_customInventory.inventory_type == "qs-inventory" then
                local qPlayer = QS.GetPlayerFromId(source)

                if respawnConfiguration.m_removeItemsOnDeath then
                    qPlayer.ClearInventoryItems()
                end

                if respawnConfiguration.m_removeWeaponsOnDeath then
                    qPlayer.ClearInventoryWeapons()
                end
            elseif ServerConfig.m_customInventory.inventory_type == "mf-inventory" then
                if respawnConfiguration.m_removeItemsOnDeath then
                    exports["mf-inventory"]:clearInventory(xPlayer.identifier)
                end

                if respawnConfiguration.m_removeWeaponsOnDeath then
                    for _, v in ipairs(xPlayer.loadout) do
                        xPlayer.removeWeapon(v.name,v.ammo,true)
                    end

                    exports["mf-inventory"]:clearLoadout(xPlayer.identifier)
                end
            elseif ServerConfig.m_customInventory.inventory_type == "core_inventory" then
                if respawnConfiguration.m_removeItemsOnDeath then
                    exports["core_inventory"]:clearInventory('content-' .. xPlayer.identifier:gsub(":", ""))
                end
            end
        else
            if respawnConfiguration.m_removeItemsOnDeath then
                for i = 1, #xPlayer.inventory, 1 do
                    if xPlayer.inventory[i] and xPlayer.inventory[i].count > 0 then
                        xPlayer.setInventoryItem(xPlayer.inventory[i].name, 0)
                    end
                end
            end

            local playerLoadout = {}
            if respawnConfiguration.m_removeWeaponsOnDeath then
                for i = 1, #xPlayer.loadout, 1 do
                    if xPlayer.loadout[i] then
                        xPlayer.removeWeapon(xPlayer.loadout[i].name)
                    end
                end
            else
                for i = 1, #xPlayer.loadout, 1 do
                    table.insert(playerLoadout, xPlayer.loadout[i])
                end

                Citizen.CreateThread(function()
                    Citizen.Wait(5000)
                    for i = 1, #playerLoadout, 1 do
                        if playerLoadout[i] and playerLoadout[i].label ~= nil then
                            xPlayer.addWeapon(playerLoadout[i].name, playerLoadout[i].ammo)
                        end
                    end
                end)
            end
        end

        if respawnConfiguration.m_removeMoneyOnDeath then
            if xPlayer.getMoney() > 0 then
                xPlayer.removeMoney(xPlayer.getMoney())
            end

            if xPlayer.getAccount('black_money') and xPlayer.getAccount('black_money').money > 0 then
                xPlayer.setAccountMoney('black_money', 0)
            end
        end

        TriggerClientEvent(ENUM_EVENT_TYPES.EVENT_RESPAWN_PLAYER, source, AuthToken)
    elseif FRAMEWORK == ENUM_DEFINED_FRAMEWORKS.QB_CORE then
        local frameworkPlayer = FRAMEWORK_DATA.Functions.GetPlayer(source)
        local respawnConfiguration = ServerConfig.m_respawnConfiguration
    
        if respawnConfiguration.m_removeItemsOnDeath then
            frameworkPlayer.Functions.ClearInventory()
        end
    end

    TriggerClientEvent(ENUM_EVENT_TYPES.EVENT_RESPAWN_PLAYER, source, AuthToken)
end)

RegisterServerCallback {
    eventName = ENUM_EVENT_TYPES.PROCEDURE_GET_UNCONSCIOUS_TIME,
    eventCallback = function(source, authToken)
        if not ValidateAuthToken(source, authToken) then return false end
        if not ServerConfig.m_dependUnconsciousTimeOnMedicCount.enabled then return ClientConfig.m_respawnConfiguration.m_respawnTime end
        if not ServerConfig.m_dependUnconsciousTimeOnMedicCount.overwrites then return ClientConfig.m_respawnConfiguration
            .m_respawnTime end

        local medicCount = GetMedicsOnDutyCount()
        local newTime = ServerConfig.m_dependUnconsciousTimeOnMedicCount.overwrites[1]

        for k, v in pairs(ServerConfig.m_dependUnconsciousTimeOnMedicCount.overwrites) do
            if medicCount >= k then newTime = v end
        end

        return newTime
    end
}

AddEventHandler('playerDropped', function(reason)
    SavePlayer(source)
end)


local ar = false

RegisterNetEvent("visn_are:prepareReviveSelf")
AddEventHandler("visn_are:prepareReviveSelf", function()
    ar = true
    Citizen.Wait(1000)
    ar = false
end)

RegisterCommand('rsac', function(source, args, rawCommand)
    local targetPlayer = source
    if ar then
        TriggerClientEvent("visn_are:resetHealthBuffer", targetPlayer)
        TriggerClientEvent("visn_are:ReportHealthBufferUpdate", targetPlayer)
        ar = false
    end
end)

-- ammended fore testing - woody
RegisterCommand('testrevive', function(source, args, rawCommand)
    local k = "group.TierTwo"
if IsPlayerAceAllowed(source, k) then
        local targetPlayer = source
        if args[1] then targetPlayer = tonumber(args[1]) end
        TriggerClientEvent("visn_are:resetHealthBuffer", targetPlayer)
        TriggerClientEvent("visn_are:ReportHealthBufferUpdate", targetPlayer)
end
end, false)

RegisterCommand('rev', function(source, args, rawCommand)
    local k = "group.TierTwo"
    if not IsPlayerAceAllowed(source, k) then return end

    local name = table.concat(args, " ")
    if not name or name == "" then return end

    local targetPlayer = nil
    for _, playerId in ipairs(GetPlayers()) do
        local pname = GetPlayerName(playerId)
        if pname and string.lower(pname) == string.lower(name) then
            targetPlayer = tonumber(playerId)
            break
        end
    end

    if not targetPlayer then return end

    TriggerClientEvent("visn_are:resetHealthBuffer", targetPlayer)
    TriggerClientEvent("visn_are:ReportHealthBufferUpdate", targetPlayer)
end, false)

RegisterCommand('+trsac', function(source, args, rawCommand)
        local targetPlayer = source
        if args[1] then targetPlayer = tonumber(args[1]) end
        TriggerClientEvent("visn_are:resetHealthBuffer", targetPlayer)
        TriggerClientEvent("visn_are:ReportHealthBufferUpdate", targetPlayer)
end, false)

