RegisterCommand("cloneself", function(source, args)
    if not IsPlayerAceAllowed(source, "Digital.Team") then
        TriggerClientEvent("chat:addMessage", source, {
            color = {255, 0, 0},
            args = {"System", "You don't have permission to use this command."}
        })
        return
    end

    local animKey = args[1] and args[1]:lower() or nil
    TriggerClientEvent("clone:requestData", source, animKey)
end, false)

RegisterNetEvent("clone:createNetworkedPed")
AddEventHandler("clone:createNetworkedPed", function(model, coords, heading)
    local src = source
    TriggerClientEvent("clone:createNetworkedPedClient", src, model, coords, heading)
end)


RegisterNetEvent("clone:syncPlacedClone")
AddEventHandler("clone:syncPlacedClone", function(netId, coords, heading, appearance, headBlend, hairColor, hairHighlight, eyeColor, overlays, props)
    TriggerClientEvent("clone:placeClone", -1, netId, coords, heading, appearance, headBlend, hairColor, hairHighlight, eyeColor, overlays, props)
end)

RegisterCommand("cloneclear", function(source)
    if not IsPlayerAceAllowed(source, "Digital.Team") then
        TriggerClientEvent("chat:addMessage", source, {
            color = {255, 0, 0},
            args = {"System", "You don't have permission to use this command."}
        })
        return
    end

    TriggerClientEvent("clone:clearAll", -1)
end, false)

RegisterCommand("clone", function(source, args)
    if not IsPlayerAceAllowed(source, "group.dev") then
        TriggerClientEvent("chat:addMessage", source, {
            color = {255, 0, 0},
            args = {"System", "You don't have permission to use this command."}
        })
        return
    end

    local targetName = args[1]
    local emoteKey = args[2] and args[2]:lower() or nil

    if not targetName then
        TriggerClientEvent("chat:addMessage", source, {
            color = {255, 255, 0},
            args = {"Usage", "/clone <playername> <emotename>"}
        })
        return
    end

    for _, playerId in ipairs(GetPlayers()) do
        local name = GetPlayerName(playerId)
        if name and string.lower(name) == string.lower(targetName) then
            TriggerClientEvent("clone:requestData", tonumber(playerId), emoteKey, source) -- target sends their data to the requester
            return
        end
    end

    TriggerClientEvent("chat:addMessage", source, {
        color = {255, 0, 0},
        args = {"System", "Player not found."}
    })
end, false)

RegisterNetEvent("clone:createNetworkedPedFor")
AddEventHandler("clone:createNetworkedPedFor", function(target, model, coords, heading, data, gunClone)
    TriggerClientEvent("clone:createNetworkedPedClientFor", target, model, coords, heading, data, gunClone)
end)

RegisterCommand("clonegun", function(source, args)
    if not IsPlayerAceAllowed(source, "group.dev") then
        TriggerClientEvent("chat:addMessage", source, {
            color = {255, 0, 0},
            args = {"System", "You don't have permission to use this command."}
        })
        return
    end

    local targetName = args[1]
    if not targetName then
        TriggerClientEvent("chat:addMessage", source, {
            color = {255, 255, 0},
            args = {"Usage", "/clonegun <playername>"}
        })
        return
    end

    for _, playerId in ipairs(GetPlayers()) do
        local name = GetPlayerName(playerId)
        if name and string.lower(name) == string.lower(targetName) then
            -- Pass gun mode flag
            TriggerClientEvent("clone:requestData", tonumber(playerId), nil, source, true)
            return
        end
    end

    TriggerClientEvent("chat:addMessage", source, {
        color = {255, 0, 0},
        args = {"System", "Player not found."}
    })
end, false)
