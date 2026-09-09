RegisterCommand("rk", function(source, args, rawCommand)
    local playerPed = PlayerPedId()
    local weaponHash = GetSelectedPedWeapon(playerPed)

    if weaponHash == GetHashKey("weapon_golfclub") then
        local animDict = "melee@large_wpn@streamed_variations"
        local animName = "car_side_attack_b"

        RequestAnimDict(animDict)
        while not HasAnimDictLoaded(animDict) do
            Citizen.Wait(10)
        end
        
        TaskPlayAnim(playerPed, "melee@large_wpn@streamed_variations", "car_side_attack_b", 8.0, -8.0, -1, 48, 0, false, false, false)
        Citizen.Wait(800)

        local playerCoords = GetEntityCoords(playerPed)

        PlaySoundFromCoord(-1, "CRASH", playerCoords.x, playerCoords.y, playerCoords.z, "PAPARAZZO_03A", false, 0, false)

        TriggerServerEvent('actions:syncRam', playerCoords)
    
    end
end, false)

RegisterCommand("dk", function(source, args, rawCommand)
    local playerPed = PlayerPedId()

    TaskPlayAnim(playerPed, "melee@unarmed@streamed_core", "kick_close_a", 8.0, -8.0, -1, 0, 0, false, false, false)

    Citizen.Wait(800)

    local playerCoords = GetEntityCoords(playerPed)

    PlaySoundFromCoord(-1, "CRASH", playerCoords.x, playerCoords.y, playerCoords.z, "PAPARAZZO_03A", false, 0, false)

    TriggerServerEvent('actions:syncDkick', playerCoords)
end, false)

RegisterNetEvent('actions:syncRamClient')
AddEventHandler('actions:syncRamClient', function(coords)
    local playerPed = PlayerPedId()
    local playerCoords = GetEntityCoords(playerPed)

    local distance = calculateDistance(playerCoords.x, playerCoords.y, playerCoords.z, coords.x, coords.y, coords.z)

    if distance < 15.0 then
        PlaySoundFromCoord(-1, "CRASH", coords.x, coords.y, coords.z, "PAPARAZZO_03A", false, 0, false)
        
    end
end)

RegisterNetEvent('actions:syncDkickClient')
AddEventHandler('actions:syncDkickClient', function(coords)
    local playerPed = PlayerPedId()
    local playerCoords = GetEntityCoords(playerPed)

    local distance = calculateDistance(playerCoords.x, playerCoords.y, playerCoords.z, coords.x, coords.y, coords.z)

    if distance < 15.0 then
        PlaySoundFromCoord(-1, "CRASH", coords.x, coords.y, coords.z, "PAPARAZZO_03A", false, 0, false)

    end
end)

function calculateDistance(x1, y1, z1, x2, y2, z2)
    local dx = x2 - x1
    local dy = y2 - y1
    local dz = z2 - z1
    return math.sqrt(dx * dx + dy * dy + dz * dz)
end
