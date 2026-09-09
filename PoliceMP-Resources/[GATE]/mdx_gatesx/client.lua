local spawnedGates = {}
local isPlayerNearGateKey = false
local currentGateData = nil

-- Notification Function using ox_lib
function Notify(title, description, type)
    -- Check if lib.notify is available before using it
    if lib and lib.notify then
        lib.notify({
            title = title,
            description = description,
            type = type
        })
    else
        -- Fallback: Default Notification System
        SetNotificationTextEntry("STRING")
        AddTextComponentString(description)
        DrawNotification(false, true)
    end
end

-- Load models and animations
function LoadModel(model)
    while not HasModelLoaded(model) do
        RequestModel(model)
        Citizen.Wait(0)
    end
end

function LoadAnimation(dict)
    while not HasAnimDictLoaded(dict) do
        RequestAnimDict(dict)
        Citizen.Wait(0)
    end
end

-- Spawn all gates from Config
Citizen.CreateThread(function()
    for stationName, gates in pairs(Config.Gates) do
        for _, gate in ipairs(gates) do
            LoadModel(gate.model)
            local gateEntity = CreateObjectNoOffset(gate.model, gate.GateCoords, false, true, true)
            SetEntityRotation(gateEntity, gate.GateRotation, 2, true)
            FreezeEntityPosition(gateEntity, true)
            table.insert(spawnedGates, { entity = gateEntity, data = gate })
        end
    end
end)

-- Gate interaction and notifications
Citizen.CreateThread(function()
    while true do
        Citizen.Wait(500)

        local playerPed = PlayerPedId()
        local playerCoords = GetEntityCoords(playerPed)

        local nearGate = false
        for _, gate in ipairs(spawnedGates) do
            local distanceOutside = #(playerCoords - gate.data.KeyCoordsOutside)
            local distanceInside = #(playerCoords - gate.data.KeyCoordsInside)
            local interactionDistance = gate.data.InteractionDistance or 5.0 -- Default to 5.0 if undefined

            if distanceOutside < interactionDistance or distanceInside < interactionDistance then
                nearGate = true
                if not isPlayerNearGateKey then
                    isPlayerNearGateKey = true
                    currentGateData = gate.data
                    -- Only notify if ox_lib is available, otherwise fallback to default
                    Notify('Gate Interaction', 'Press [E] to open the gate.', 'inform')
                end
                break
            end
        end

        if not nearGate and isPlayerNearGateKey then
            isPlayerNearGateKey = false
            currentGateData = nil
        end
    end
end)

-- Handle gate opening
Citizen.CreateThread(function()
    while true do
        Citizen.Wait(0)

        if isPlayerNearGateKey and currentGateData and IsControlJustReleased(0, 38) then -- Key 'E'
            local gateEntity = nil
            for _, gate in ipairs(spawnedGates) do
                if gate.data == currentGateData then
                    gateEntity = gate.entity
                    break
                end
            end

            if gateEntity then
                -- Play Animation on the Gate Entity
                LoadAnimation(currentGateData.animationDict)
                PlayEntityAnim(gateEntity, currentGateData.animationName, currentGateData.animationDict, 8.0, false, true, false, 0, 0)

                -- Open Gate by disabling collision
                FreezeEntityPosition(gateEntity, false)
                SetEntityCollision(gateEntity, false, false)
                
                -- Notify that the gate is opening
                Notify('Gate Interaction', 'Gate has opened.', 'success')

                Citizen.Wait(22000) -- Gate opening duration
                SetEntityCollision(gateEntity, true, true)
                FreezeEntityPosition(gateEntity, true)                
            end
        end
    end
end)

-- Cleanup on resource stop
AddEventHandler('onResourceStop', function(resourceName)
    if GetCurrentResourceName() ~= resourceName then return end
    for _, gate in ipairs(spawnedGates) do
        DeleteEntity(gate.entity)
    end
end)
