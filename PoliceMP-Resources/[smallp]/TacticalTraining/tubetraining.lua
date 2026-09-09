local placingTrain = false
local previewTrain = nil
local offset = vector3(0, 5, 0)
local currentHeading = 0.0
local lastPlacedTrain = nil

function ShowTubeNotification(msg)
    BeginTextCommandThefeedPost("STRING")
    AddTextComponentSubstringPlayerName(msg)
    EndTextCommandThefeedPostTicker(false, false)
end

function ShowTubeSubtitle(msg, duration)
    BeginTextCommandPrint("STRING")
    AddTextComponentSubstringPlayerName(msg)
    EndTextCommandPrint(duration or 1000, true)
end

RegisterNetEvent("tubetrain:startPlacementMode", function()
    if placingTrain then return end

    local model = `metrotrain`
    RequestModel(model)
    while not HasModelLoaded(model) do Wait(0) end

    local playerPed = PlayerPedId()
    local pos = GetEntityCoords(playerPed)
    local train = CreateVehicle(model, pos + offset, 0.0, true, false)

    -- Ghost preview setup
    SetEntityAlpha(train, 128, false)
    SetEntityCollision(train, false, false)
    FreezeEntityPosition(train, false)
    SetEntityHeading(train, currentHeading)

    for door = 0, 5 do
        SetVehicleDoorBroken(train, door, true)
    end

    previewTrain = train
    placingTrain = true

    ShowTubeNotification("🚇 Tube placement started. ESC to cancel.")
end)

CreateThread(function()
    while true do
        Wait(0)
        if placingTrain and previewTrain then
            local pos = GetEntityCoords(PlayerPedId())
            ShowTubeSubtitle(
            "~c~Q/E~s~ Up/Down  ~c~NUM 7/9~s~ Rotate" ..
            "~n~~c~NUM 8/5/4/6~s~ Move (F/B/L/R)" ..
            "~n~~c~ENTER~s~ Place Tube" ..
            "   ~c~ESC~s~ Cancel", 0)

            -- Movement (NUMPAD)
            if IsControlPressed(0, 111) then offset = offset + vector3(0.0, 0.1, 0.0) end -- NUMPAD 8
            if IsControlPressed(0, 112) then offset = offset - vector3(0.0, 0.1, 0.0) end -- NUMPAD 5
            if IsControlPressed(0, 108) then offset = offset - vector3(0.1, 0.0, 0.0) end -- NUMPAD 4
            if IsControlPressed(0, 109) then offset = offset + vector3(0.1, 0.0, 0.0) end -- NUMPAD 6
            if IsControlPressed(0, 44)  then offset = offset + vector3(0.0, 0.0, 0.1) end -- Q
            if IsControlPressed(0, 38)  then offset = offset - vector3(0.0, 0.0, 0.1) end -- E

            -- Rotation (NUMPAD 7/9)
            if IsControlPressed(0, 117) then currentHeading = currentHeading - 1.0 end -- NUMPAD 7
            if IsControlPressed(0, 118) then currentHeading = currentHeading + 1.0 end -- NUMPAD 9

            local newPos = pos + offset
            SetEntityCoordsNoOffset(previewTrain, newPos.x, newPos.y, newPos.z, true, true, true)
            SetEntityHeading(previewTrain, currentHeading)

            -- Confirm placement
            if IsControlJustPressed(0, 191) then -- ENTER
                SetEntityAlpha(previewTrain, 255, false)
                SetEntityCollision(previewTrain, true, true)
                PlaceObjectOnGroundProperly(previewTrain)
                Wait(100)
                FreezeEntityPosition(previewTrain, true)
                SetEntityInvincible(previewTrain, true)
                SetVehicleDoorsLocked(previewTrain, 1)

                for door = 0, 5 do
                    SetVehicleDoorBroken(previewTrain, door, true)
                end

                local netId = NetworkGetNetworkIdFromEntity(previewTrain)
                TriggerServerEvent("tubetrain:registerTrain", netId)
                TriggerServerEvent("tubetrain:breakTrainDoors", netId)

                lastPlacedTrain = previewTrain
                placingTrain = false
                previewTrain = nil

                ShowTubeNotification("✅ Train placed successfully.")
            end

            if IsControlJustPressed(0, 322) then -- ESC
                DeleteEntity(previewTrain)
                placingTrain = false
                previewTrain = nil
                ShowTubeNotification("❌ Train placement cancelled.")
            end
        end
    end
end)

RegisterNetEvent("tubetrain:breakTrainDoorsClient", function(netId)
    local entity = NetworkGetEntityFromNetworkId(netId)
    if entity and DoesEntityExist(entity) then
        for door = 0, 5 do
            SetVehicleDoorBroken(entity, door, true)
        end
    end
end)

RegisterNetEvent("tubetrain:forceDeleteTrain", function(netId)
    local ent = NetworkGetEntityFromNetworkId(netId)
    if ent and DoesEntityExist(ent) then
        DeleteEntity(ent)
    end
end)
