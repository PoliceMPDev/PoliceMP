local activeRequests = {}
local cooldowns = {}
local COOLDOWN_TIME = 5 * 60 * 1000 -- 5 minutes in milliseconds

RegisterNetEvent("celltrace:initiateTrace", function(targetId)
    local src = source

    if not IsPlayerAceAllowed(src, "Police.cidTrained") then
        TriggerClientEvent("ox_lib:notify", src, {
            title = "[CELLPING]",
            description = "You do not have permission to use this.",
            type = "error",
            position = "bottom"
        })
        return
    end

    local now = GetGameTimer()
    if cooldowns[src] and now - cooldowns[src] < COOLDOWN_TIME then
        local remaining = math.ceil((COOLDOWN_TIME - (now - cooldowns[src])) / 1000)
        TriggerClientEvent("ox_lib:notify", src, {
            title = "[CELLPING]",
            description = ("You must wait %d more seconds before pinging again."):format(remaining),
            type = "error",
            position = "bottom",
            duration = 8000
        })
        return
    end

    cooldowns[src] = now

    targetId = tonumber(targetId)
    if not targetId or not GetPlayerName(targetId) then
        TriggerClientEvent("ox_lib:notify", src, {
            title = "[CELLPING]",
            description = "Invalid player selected.",
            type = "error",
            position = "bottom"
        })
        return
    end

    local requestId = ("req_%s_%s"):format(src, targetId)
    activeRequests[requestId] = {
        requester = src,
        target = targetId
    }

    TriggerClientEvent("celltrace:requestConfirmation", targetId)
    TriggerClientEvent("ox_lib:notify", src, {
        title = "[CELLPING]",
        description = "Cell ping request sent. Awaiting response...",
        type = "inform",
        position = "bottom",
        duration = 5000
    })
end)

RegisterNetEvent("celltrace:confirmationResult", function(accepted)
    local targetId = source
    for requestId, data in pairs(activeRequests) do
        if data.target == targetId then
            local requester = data.requester
            activeRequests[requestId] = nil

            if not accepted then
                TriggerClientEvent("ox_lib:notify", requester, {
                    title = "[CELLPING]",
                    description = "Trace Rejected - Phone Off / No Signal.",
                    type = "error",
                    position = "bottom",
                    duration = 8000
                })
                return
            end

            local ped = GetPlayerPed(targetId)
            local coords = GetEntityCoords(ped)
            local blipPositions = {}
            local usedAngles = {}

            for i = 1, 3 do
                local angle
                repeat
                    angle = math.random() * 2 * math.pi
                until not usedAngles[math.floor(angle * 10)]
                usedAngles[math.floor(angle * 10)] = true
                local distance = math.random(90, 110) -- ~100m ±10m
                local x = coords.x + math.cos(angle) * distance
                local y = coords.y + math.sin(angle) * distance
                local z = coords.z
                table.insert(blipPositions, vector3(x, y, z))
            end

            CreateThread(function()
                TriggerClientEvent("ox_lib:notify", requester, {
                    title = "[CELLPING]",
                    description = "📶 Pinging cell towers.....",
                    type = "inform",
                    position = "bottom",
                    duration = 8000
                })

                Wait(8000)

                TriggerClientEvent("ox_lib:notify", requester, {
                    title = "[CELLPING]",
                    description = "🛰️ Analysing possible locations.....",
                    type = "inform",
                    position = "bottom",
                    duration = 8000
                })

                Wait(8000)

                TriggerClientEvent("ox_lib:notify", requester, {
                    title = "📡 Cell Tower Ping",
                    description = "Triangulated Possible Locations - Review on Minimap",
                    type = "success",
                    position = "bottom",
                    duration = 8000
                })

                TriggerClientEvent("celltrace:showBlips", requester, blipPositions)
            end)

            return
        end
    end
end)

RegisterNetEvent("celltrace:requestPlayerList", function()
    local src = source
    local players = {}

    for _, playerId in ipairs(GetPlayers()) do
        local name = GetPlayerName(playerId)
        if name then
            table.insert(players, {
                label = string.format("%s (%s)", name, playerId),
                value = tonumber(playerId)
            })
        end
    end

    TriggerClientEvent("celltrace:showPlayerList", src, players)
end)

AddEventHandler("playerDropped", function()
    local src = source
    cooldowns[src] = nil
end)
