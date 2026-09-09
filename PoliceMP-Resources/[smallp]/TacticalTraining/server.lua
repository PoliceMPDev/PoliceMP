RegisterCommand('trainingh3', function(source)
    local player = source
    if player == 0 then return end

    if not IsPlayerAceAllowed(player, "group.TierTwo") then
        TriggerClientEvent('gunTraining:notify', player, '~r~Developers Only!')
        return
    end

    TriggerClientEvent('gunTraining:spawn', source)
end, false)

RegisterCommand('trainingkh', function(source)
    local player = source
    if player == 0 then return end

    if not IsPlayerAceAllowed(player, "group.TierTwo") then
        TriggerClientEvent('gunTraining:notify', player, '~r~Developers Only!')
        return
    end

    TriggerClientEvent('gunTraining:spawn2', source)
end, false)

RegisterCommand('trainingkh2', function(source)
    local player = source
    if player == 0 then return end

    if not IsPlayerAceAllowed(player, "group.TierTwo") then
        TriggerClientEvent('gunTraining:notify', player, '~r~Developers Only!')
        return
    end

    TriggerClientEvent('gunTraining:spawn3', source)
end, false)

RegisterCommand('trscene', function(source)
    local player = source
    if player == 0 then return end

    if not IsPlayerAceAllowed(player, "group.TierTwo") then
        TriggerClientEvent('gunTraining:notify', player, '~r~Developers Only!')
        return
    end

    TriggerClientEvent('gunTraining:spawn4', source)
end, false)

RegisterCommand('clearguns', function(source)
    local player = source
    if player == 0 then return end

    if not IsPlayerAceAllowed(player, "group.TierTwo") then
        TriggerClientEvent('gunTraining:notify', player, '~r~Developers Only!')
        return
    end

    TriggerClientEvent('gunTraining:clear', -1)
end, false)

RegisterCommand("tubetraining", function(source)
    if IsPlayerAceAllowed(source, "Police.afoTrained") and IsPlayerAceAllowed(source, "Police.sergeant") then
        TriggerClientEvent("tubetrain:startPlacementMode", source)
    else
        TriggerClientEvent("tubetrain:notify", source, "🚫 You are not authorized to use this command.")
    end
end, false)

local playerTrains = {} -- Stores per-player train NetIDs

RegisterCommand("tubedelete", function(source)
    if IsPlayerAceAllowed(source, "Police.afoTrained") and IsPlayerAceAllowed(source, "Police.sergeant") then
        local trains = playerTrains[source]
        if trains and #trains > 0 then
            for _, netId in ipairs(trains) do
                TriggerClientEvent("tubetrain:forceDeleteTrain", -1, netId)
            end
            playerTrains[source] = {}
            TriggerClientEvent("tubetrain:notify", source, "🗑️ All your placed trains have been deleted.")
        else
            TriggerClientEvent("tubetrain:notify", source, "⚠️ You haven’t placed any trains.")
        end
    else
        TriggerClientEvent("tubetrain:notify", source, "🚫 You are not authorized to use this command.")
    end
end, false)


RegisterNetEvent("tubetrain:registerTrain", function(netId)
    local src = source
    playerTrains[src] = playerTrains[src] or {}
    table.insert(playerTrains[src], netId)
    print(("[TubeTrain] Player ID: %s placed train NetID: %s"):format(src, netId))
end)

RegisterNetEvent("tubetrain:breakTrainDoors", function(netId)
    TriggerClientEvent("tubetrain:breakTrainDoorsClient", -1, netId)
end)
