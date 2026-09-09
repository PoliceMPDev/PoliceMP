SpawnedObjects = {}

RegisterCommand("smark", function(source, args, rawCommand)
    -- Validate Input
    markerNum = tonumber(args[1])

    if not type(markerNum) == "number" or markerNum > 20 then
        return showHelpNotification("That is not a marker number.")
    end

    TriggerServerEvent("syncMarkerAnimation")
    Citizen.Wait(1000) -- Wait for animation to play
    spawnMarker(markerNum)
end)

RegisterCommand("dmark", function(source, args, rawCommand)
    TriggerServerEvent("syncMarkerAnimation")
    Citizen.Wait(1000) -- Wait for animation to play
    
    -- Delete Marker
    closestDistance = 2.0
    closestModel = nil

    playerPed = PlayerPedId()
    playerCoords = GetEntityCoords(playerPed)

    for i = 1, 20 do
        markerSpawnName = 'mdxo_evi_' .. i
        markerSpawnHash = GetHashKey(markerSpawnName)

        local closestCheck = GetClosestObjectOfType(playerCoords, 2.0, markerSpawnHash, 1, 1, 1)

        if closestCheck ~= 0 then
            local distance = #(GetEntityCoords(closestCheck) - playerCoords)

            if distance <= closestDistance then
                closestDistance = distance
                closestModel = closestCheck
            end
        end
    end

    if closestModel ~= nil then
        local has_control = false

        RequestNetworkControl(closestModel, function(cb)
            has_control = cb
        end)

        if has_control then
            DeleteEntity(closestModel)
        end
    else
        return showHelpNotification("You are not near a marker")
    end
end)

RegisterNetEvent("playMarkerAnimation")
AddEventHandler("playMarkerAnimation", function()
    local playerPed = PlayerPedId()
    RequestAnimDict("random@domestic")
    while not HasAnimDictLoaded("random@domestic") do
        Citizen.Wait(10)
    end
    TaskPlayAnim(playerPed, "random@domestic", "pickup_low", 8.0, -8.0, 2000, 49, 0, false, false, false)
end)

AddEventHandler('onResourceStop', function(resourceName)
    if (resourceName == GetCurrentResourceName()) then
        for _, object in ipairs(SpawnedObjects) do
            local has_control = false

            RequestNetworkControl(object, function(cb)
                has_control = cb
            end)

            if has_control then
                DeleteEntity(object)
            end
        end
    end
end)

function spawnMarker(num)
    playerPed = PlayerPedId()
    playerCoords = GetEntityCoords(playerPed)
    markerSpawnName = 'mdxo_evi_' .. num
    markerSpawnHash = GetHashKey(markerSpawnName)

    while not HasModelLoaded(markerSpawnHash) do
        RequestModel(markerSpawnHash)
        Wait(10)
    end

    local marker = CreateObject(markerSpawnHash, playerCoords.x, playerCoords.y, playerCoords.z, 1, 1, 0)
    SetEntityHeading(marker, GetEntityHeading(playerPed))
    PlaceObjectOnGroundProperly(marker)
    FreezeEntityPosition(marker, true)

    NetworkRegisterEntityAsNetworked(marker)

    while not NetworkGetEntityIsNetworked(marker) do
        NetworkRegisterEntityAsNetworked(marker)
        Citizen.Wait(1)
    end

    table.insert(SpawnedObjects, marker)
end

function showHelpNotification(msg)
    BeginTextCommandDisplayHelp("STRING")
    AddTextComponentSubstringPlayerName(msg)
    EndTextCommandDisplayHelp(0, 0, 1, -1)
end

function RequestNetworkControl(entity, callback)
    local netId = NetworkGetNetworkIdFromEntity(entity)
    local timer = 0

    NetworkRequestControlOfNetworkId(netId)

    while not NetworkHasControlOfNetworkId(netId) do
        Citizen.Wait(1)
        NetworkRequestControlOfNetworkId(netId)
        timer = timer + 1

        if timer == 5000 then
            if DoesEntityExist(entity) then
                SetEntityAsMissionEntity(entity, true, true)
                DeleteEntity(entity)
            end

            Citizen.Trace("Control failed")
            callback(false)
            break
        end
    end

    callback(true)
end