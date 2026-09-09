RegisterNetEvent('actions:syncRam')
AddEventHandler('actions:syncRam', function(coords)
    local sourcePlayer = source 
    local players = GetPlayers() 

    for _, playerId in ipairs(players) do
        if playerId ~= sourcePlayer then
            local targetPed = GetPlayerPed(playerId)
            local targetCoords = GetEntityCoords(targetPed)

            local distance = calculateDistance(targetCoords.x, targetCoords.y, targetCoords.z, coords.x, coords.y, coords.z)
            
            if distance < 15.0 then  
                TriggerClientEvent('actions:syncRamClient', playerId, coords)
            end
        end
    end
end)

RegisterNetEvent('actions:syncDkick')
AddEventHandler('actions:syncDkick', function(coords)
    local sourcePlayer = source  
    local players = GetPlayers() 

    for _, playerId in ipairs(players) do
        if playerId ~= sourcePlayer then
            local targetPed = GetPlayerPed(playerId)
            local targetCoords = GetEntityCoords(targetPed)

            local distance = calculateDistance(targetCoords.x, targetCoords.y, targetCoords.z, coords.x, coords.y, coords.z)
            
            if distance < 15.0 then  
                TriggerClientEvent('actions:syncDkickClient', playerId, coords)
            end
        end
    end
end)

function calculateDistance(x1, y1, z1, x2, y2, z2)
    local dx = x2 - x1
    local dy = y2 - y1
    local dz = z2 - z1
    return math.sqrt(dx * dx + dy * dy + dz * dz)
end
