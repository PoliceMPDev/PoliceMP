local function getNearbyPlayers(coords, radius)
    local players = {}
    for _, id in ipairs(GetPlayers()) do
        local targetPed = GetPlayerPed(id)
        local targetCoords = GetEntityCoords(targetPed)
        if #(coords - targetCoords) <= radius then
            table.insert(players, id)
        end
    end
    return players
end

RegisterNetEvent("bullhorn:trigger", function(coords)
    local src = source
    local nearby = getNearbyPlayers(coords, 60.0)
    for _, id in ipairs(nearby) do
        TriggerClientEvent("bullhorn:play", id, coords, src)
    end
end)

RegisterNetEvent("bullhorn:stop", function()
    local src = source
    local coords = GetEntityCoords(GetPlayerPed(src))
    local nearby = getNearbyPlayers(coords, 60.0)
    for _, id in ipairs(nearby) do
        TriggerClientEvent("bullhorn:stop", id, src)
    end
end)

RegisterNetEvent("bullhorn:updatePos", function(coords)
    local src = source
    local nearby = getNearbyPlayers(coords, 60.0)
    for _, id in ipairs(nearby) do
        TriggerClientEvent("bullhorn:updatePos", id, coords, src)
    end
end)
