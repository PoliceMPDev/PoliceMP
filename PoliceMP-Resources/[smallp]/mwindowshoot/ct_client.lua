local isAttached = false
local attachData = {}
local originalSeatIndex = nil -- Variable to store the original seat index

local function isInBackSeatHeli()
    local ped = PlayerPedId()
    local vehicle = GetVehiclePedIsIn(ped, false)
    if vehicle ~= 0 then
        -- Check if the player is in the back seat (indices 1 and 2)
        local seatIndex1 = GetPedInVehicleSeat(vehicle, 1) -- First back seat
        local seatIndex2 = GetPedInVehicleSeat(vehicle, 2) -- Second back seat
        return seatIndex1 == ped or seatIndex2 == ped
    end
    return false
end

RegisterCommand("+ctmount", function()
    if not isInBackSeatHeli() then
        return
    end

    TriggerServerEvent('checkCTMountPermission')
end)

RegisterNetEvent('proceedWithCTMount')
AddEventHandler('proceedWithCTMount', function()
    local ped = PlayerPedId()
    local closestVehicle = GetVehiclePedIsIn(ped, false)

    if closestVehicle == 0 then
        closestVehicle = GetClosestVehicle(GetEntityCoords(ped), 5.0, 0, 70)
    end

    if closestVehicle ~= 0 then
        local vehicleModel = GetEntityModel(closestVehicle)
        local vehicleName = GetDisplayNameFromVehicleModel(vehicleModel)

        if vehicleName ~= "dauphin" then
            
            return 
        end

        local vehicleNetId = NetworkGetNetworkIdFromEntity(closestVehicle)

        local offsets = {
            [1] = { offsetX = -0.850, offsetY = 1.750, offsetZ = 0.7600 }, -- First passenger seat
            [2] = { offsetX = 0.850, offsetY = 1.750, offsetZ = 0.7600 }  -- Second passenger seat
        }

        local seatIndex1 = GetPedInVehicleSeat(closestVehicle, 1) -- Check seat index 1 (first back seat)
        local seatIndex2 = GetPedInVehicleSeat(closestVehicle, 2) -- Check seat index 2 (second back seat)

        if seatIndex1 == ped or seatIndex2 == ped then
            if IsPedInAnyVehicle(ped, false) then
                TaskLeaveVehicle(ped, closestVehicle, 16)
            end

            originalSeatIndex = seatIndex1 == ped and 1 or 2

            attachData = {
                vehicleNetId = vehicleNetId,
                offsetX = offsets[originalSeatIndex].offsetX,
                offsetY = offsets[originalSeatIndex].offsetY,
                offsetZ = offsets[originalSeatIndex].offsetZ
            }

            isAttached = true
            TriggerServerEvent('syncDbCTSitting', vehicleNetId, offsets[originalSeatIndex].offsetX, offsets[originalSeatIndex].offsetY, offsets[originalSeatIndex].offsetZ)
        end
    else
        TriggerEvent('chat:addMessage', {
            color = {255, 0, 0},
            multiline = true,
            args = {"System", "No vehicle nearby!"}
        })
    end
end)

RegisterCommand("+ctunmount", function()
    if isAttached then
        local ped = PlayerPedId()
        TriggerServerEvent('syncDbCTDetach')

        local vehicle = NetworkGetEntityFromNetworkId(attachData.vehicleNetId)

        if DoesEntityExist(vehicle) then
            DetachEntity(ped, true, true)
            ClearPedTasksImmediately(ped)
            SetPedCanRagdoll(ped, true)

            TaskEnterVehicle(ped, vehicle, -1, originalSeatIndex, 1.0, 16, 0)
        else
            TriggerEvent('chat:addMessage', {
                color = {255, 0, 0},
                multiline = true,
                args = {"System", "Vehicle no longer exists!"}
            })
        end

        isAttached = false
        attachData = {}
        originalSeatIndex = nil -- Reset original seat index
    else
        TriggerEvent('chat:addMessage', {
            color = {255, 0, 0},
            multiline = true,
            args = {"System", "You're not attached to any vehicle!"}
        })
    end
end)

CreateThread(function()
    while true do
        Wait(0)
        if IsControlJustReleased(1, 20) then 
            if isAttached then
                ExecuteCommand("+ctunmount")
            else
                ExecuteCommand("+ctmount")
            end
        end
    end
end)

RegisterNetEvent('syncDbCTSittingClient')
AddEventHandler('syncDbCTSittingClient', function(serverId, vehicleNetId, offsetX, offsetY, offsetZ)
    local ped = GetPlayerPed(GetPlayerFromServerId(serverId))
    local vehicle = NetworkGetEntityFromNetworkId(vehicleNetId)

    local boneIndex = 0xE0FD 
    if DoesEntityExist(ped) and DoesEntityExist(vehicle) then
        AttachEntityToEntity(ped, vehicle, boneIndex, offsetX, offsetY, offsetZ, 0.0, 0.0, -90.0, false, false, false, false, 2, true)
        
        SetPedCanRagdoll(ped, false)
        SetPedCanRagdollFromPlayerImpact(ped, false)
        SetPedConfigFlag(ped, 32, false)
        SetPedConfigFlag(ped, 304, true)
    else
        print("Error: Ped or Vehicle does not exist.")
    end

    CreateThread(function()
        while isAttached do
            Wait(100)
        end
    end)
end)

RegisterNetEvent('syncDbCTDetachClient')
AddEventHandler('syncDbCTDetachClient', function(serverId)
    local ped = GetPlayerPed(GetPlayerFromServerId(serverId))
    if DoesEntityExist(ped) then
        DetachEntity(ped, true, true)
        ClearPedTasks(ped)
        SetPedCanRagdoll(ped, true)
        SetPedCanRagdollFromPlayerImpact(ped, true)
        SetPedConfigFlag(ped, 32, true)
        SetPedConfigFlag(ped, 304, false)
        SetEntityRotation(ped, 0.0, 0.0, 0.0, 0, false)
    else
        print("Error: Ped does not exist.")
    end
end)

CreateThread(function()
    while true do
        Wait(0)
        if isAttached and attachData.vehicleNetId then
            local vehicle = NetworkGetEntityFromNetworkId(attachData.vehicleNetId)
            if DoesEntityExist(vehicle) then
                local ped = PlayerPedId()
                local boneIndex = 0xE0FD -- Use the back driver-side door bone for both seats
                local rotationOffset = (originalSeatIndex == 1) and 90.0 or -90.0 -- Adjust rotation to face outside the helicopter
                AttachEntityToEntity(ped, vehicle, boneIndex, attachData.offsetX, attachData.offsetY, attachData.offsetZ, 0.0, 0.0, rotationOffset, false, false, false, false, 2, true)
            else
                print("Error: Vehicle no longer exists.")

                local ped = PlayerPedId()
                DetachEntity(ped, true, true)

                isAttached = false
                attachData = {}
            end
        end
    end
end)
