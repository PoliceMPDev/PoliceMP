local trafficZones = {} -- Table to store all traffic zones
local actionCooldowns = {} -- Table to track cooldowns
local discordWebhookURL = "https://discord.com/api/webhooks/1299081539497361471/0s4dmc8TAqiJXdXiPmqM02wedIpbfCDgeil0yb2kJw2k7rshnFlZryUrlQI4qfh63f-0"

-- Utility: Send to Discord
function sendToDiscord(name, message, color)
    local timestamp = os.date("%Y-%m-%d %H:%M:%S")
    local embed = {
        {
            ["color"] = color,
            ["title"] = name,
            ["description"] = message,
            ["footer"] = {
                ["text"] = "Logged at: " .. timestamp,
            },
        }
    }
    PerformHttpRequest(discordWebhookURL, function(err, text, headers) end, 'POST', json.encode({username = "Traffic Logs", embeds = embed}), { ['Content-Type'] = 'application/json' })
end

-- Utility: Check Cooldown
function isOnCooldown(src, action)
    local playerCooldowns = actionCooldowns[src]
    if not playerCooldowns then return false end
    local lastActionTime = playerCooldowns[action]
    return lastActionTime and (os.time() - lastActionTime < 5) -- 5 seconds cooldown
end

-- Utility: Set Cooldown
function setCooldown(src, action)
    if not actionCooldowns[src] then actionCooldowns[src] = {} end
    actionCooldowns[src][action] = os.time()
end

RegisterNetEvent('traffic:slowLog')
AddEventHandler('traffic:slowLog', function(coords, postal)
    local src = source

    -- Prevent logging spam
    if isOnCooldown(src, "slowLog") then
        return
    end

    setCooldown(src, "slowLog") -- Use the same actionCooldowns table

    local identifiers = GetPlayerIdentifiers(src)
    local discordID = "Not Linked"
    local playerName = GetPlayerName(src)
    
    for _, id in ipairs(identifiers) do
        if string.find(id, "discord:") then
            discordID = id:gsub("discord:", "")
        end
    end

    local message = string.format("**Player:** %s\n**Discord ID:** %s\n**Postal:** %s\n**Action:** Traffic Slowed\n**Coords:** %s", playerName, discordID, postal, coords)
    sendToDiscord("Traffic Slow Log", message, 16753920) 
end)

RegisterNetEvent('traffic:stopLog')
AddEventHandler('traffic:stopLog', function(coords, postal)
    local src = source

    if isOnCooldown(src, "stopLog") then
        return
    end

    setCooldown(src, "stopLog") -- Use the same actionCooldowns table

    local identifiers = GetPlayerIdentifiers(src)
    local discordID = "Not Linked"
    local playerName = GetPlayerName(src)
    
    for _, id in ipairs(identifiers) do
        if string.find(id, "discord:") then
            discordID = id:gsub("discord:", "")
        end
    end

    local message = string.format("**Player:** %s\n**Discord ID:** %s\n**Postal:** %s\n**Action:** Traffic Stopped\n**Coords:** %s", playerName, discordID, postal, coords)
    sendToDiscord("Traffic Stop Log", message, 15158332) 
end)

RegisterNetEvent('traffic:resumeLog')
AddEventHandler('traffic:resumeLog', function(coords, postal)
    local src = source

    if isOnCooldown(src, "resumeLog") then
        return
    end

    setCooldown(src, "resumeLog") -- Use the same actionCooldowns table

    local identifiers = GetPlayerIdentifiers(src)
    local discordID = "Not Linked"
    local playerName = GetPlayerName(src)
    
    for _, id in ipairs(identifiers) do
        if string.find(id, "discord:") then
            discordID = id:gsub("discord:", "")
        end
    end

    local message = string.format("**Player:** %s\n**Discord ID:** %s\n**Postal:** %s\n**Action:** Traffic Resumed\n**Coords:** %s", playerName, discordID, postal, coords)
    sendToDiscord("Traffic Resume Log", message, 3066993)
end)

RegisterNetEvent('traffic:openMenu')
AddEventHandler('traffic:openMenu', function()
    local src = source 
    if IsPlayerAceAllowed(src, "Police.pc") or IsPlayerAceAllowed(src, "Highways.Trained") then
        TriggerClientEvent('traffic:menuOpened', src)
    end
end)

RegisterNetEvent('traffic:slow')
AddEventHandler('traffic:slow', function()
    local src = source
    local coords = GetEntityCoords(GetPlayerPed(src))

    if isOnCooldown(src, "slow") then
        TriggerClientEvent('traffic:notify', src, "Please wait before slowing traffic again.")
        return
    end

    setCooldown(src, "slow")
    trafficZones[src] = {action = "slow", coords = coords}
    TriggerClientEvent('traffic:update', -1, "resume", coords, src)
    TriggerClientEvent('traffic:update', -1, "slow", coords, src)
    TriggerClientEvent('traffic:notify', src, "Traffic Slowed")
end)

RegisterNetEvent('traffic:stop')
AddEventHandler('traffic:stop', function()
    local src = source
    local coords = GetEntityCoords(GetPlayerPed(src))

    if isOnCooldown(src, "stop") then
        TriggerClientEvent('traffic:notify', src, "Please wait before stopping traffic again.")
        return
    end

    setCooldown(src, "stop")
    trafficZones[src] = {action = "stop", coords = coords}
    TriggerClientEvent('traffic:update', -1, "resume", coords, src)
    TriggerClientEvent('traffic:update', -1, "stop", coords, src)
    TriggerClientEvent('traffic:notify', src, "Traffic Stopped")
end)

RegisterNetEvent('traffic:resume')
AddEventHandler('traffic:resume', function(targetPlayerId)
    local src = source
    local playerCoords = GetEntityCoords(GetPlayerPed(src))

    if targetPlayerId then
        local targetZone = trafficZones[targetPlayerId]
        if targetZone then
            TriggerClientEvent('traffic:update', -1, "resume", targetZone.coords, targetPlayerId)
            trafficZones[targetPlayerId] = nil
            TriggerClientEvent('traffic:notify', src, "Traffic resumed for the selected player's zone.")
        else
            TriggerClientEvent('traffic:notify', src, "Invalid player or no active zone to resume.")
        end
    else
        -- Find the nearest zone
        local nearestZone = nil
        local nearestDistance = math.huge

        local detectionRadius = 5.0 -- Radius in meters

        for zoneOwner, zoneData in pairs(trafficZones) do
            local distance = #(playerCoords - zoneData.coords)
            if distance < nearestDistance and distance <= detectionRadius then
                nearestDistance = distance
                nearestZone = {owner = zoneOwner, data = zoneData}
            end
        end

        if nearestZone then
            TriggerClientEvent('traffic:update', -1, "resume", nearestZone.data.coords, nearestZone.owner)
            trafficZones[nearestZone.owner] = nil
            TriggerClientEvent('traffic:notify', src, "Traffic resumed for the nearest zone.")
        else
            TriggerClientEvent('traffic:notify', src, "No active zones within range.")
        end

    end
end)

AddEventHandler('playerDropped', function(reason)
    local src = source
    if trafficZones[src] then
        TriggerClientEvent('traffic:removeZone', -1, src)
        trafficZones[src] = nil
    end
end)
