local vehicleOffsets = {}

Citizen.CreateThread(function()
    local fileContent = LoadResourceFile(GetCurrentResourceName(), 'vehicle_offsets.lua')
    if fileContent then
        local func, err = load(fileContent, "vehicle_offsets")
        if func then
            vehicleOffsets = func()
            if type(vehicleOffsets) ~= "table" then
                print("[mwindowshoot] Error: Loaded file does not return a table.")
                vehicleOffsets = {}
            else
                print("[mwindowshoot] Vehicle offsets loaded successfully.")
            end
        else
            print("[mwindowshoot] Error loading vehicle offsets!" .. err)
            vehicleOffsets = {}
        end
    else
        print("[mwindowshoot] Error: Failed to load vehicle offsets!")
        vehicleOffsets = {}
    end
end)

local isAttached = false
local attachData = {}
local originalClothing = {}

local function isInFrontPassengerSeat()
    local ped = PlayerPedId()
    local vehicle = GetVehiclePedIsIn(ped, false)
    if vehicle ~= 0 then
        local seatIndex = GetPedInVehicleSeat(vehicle, 0)
        return seatIndex == ped
    end
    return false
end

RegisterCommand("+mount", function()
    if not isInFrontPassengerSeat() then
        
        return
    end

    -- Trigger the server-side event to check ACE permissions
    TriggerServerEvent('checkMountPermission')
end)

RegisterNetEvent('proceedWithMount')
AddEventHandler('proceedWithMount', function()
    -- This code runs only if the player has permission.
    local ped = PlayerPedId()
    local pedCoords = GetEntityCoords(ped)
    local closestVehicle = GetVehiclePedIsIn(ped, false)

    if closestVehicle == 0 then
        closestVehicle = GetClosestVehicle(pedCoords, 5.0, 0, 70)
    end

    if closestVehicle ~= 0 then
        local vehicleModel = GetDisplayNameFromVehicleModel(GetEntityModel(closestVehicle))
        local offsets = vehicleOffsets[vehicleModel]

        if not offsets then
            
            return
        end

        if IsPedInAnyVehicle(ped, false) then
            ExecuteCommand("rollfront")
            Wait(500)
            TaskLeaveVehicle(ped, closestVehicle, 16)
        end

        local vehicleNetId = NetworkGetNetworkIdFromEntity(closestVehicle)
        originalClothing = {
            pants = {
                drawable = GetPedDrawableVariation(ped, 4),
                texture = GetPedTextureVariation(ped, 4)
            },
            shoes = {
                drawable = GetPedDrawableVariation(ped, 6),
                texture = GetPedTextureVariation(ped, 6)
            },
            shirt = {
                drawable = GetPedDrawableVariation(ped, 8),
                texture = GetPedTextureVariation(ped, 8)
            }
        }

        SetPedComponentVariation(ped, 4, 11, 0, 2) -- Pants
        SetPedComponentVariation(ped, 6, 13, 0, 2) -- Shoes
        SetPedComponentVariation(ped, 8, 261, 0, 2) -- Shirt

        attachData = {
            vehicleNetId = vehicleNetId,
            offsetX = offsets.offsetX,
            offsetY = offsets.offsetY,
            offsetZ = offsets.offsetZ
        }

        isAttached = true
        TriggerServerEvent('syncDbSitting', vehicleNetId, offsets.offsetX, offsets.offsetY, offsets.offsetZ,
            originalClothing.pants.texture, originalClothing.shoes.texture, originalClothing.shirt.texture)

        CreateThread(function()
            Wait(500)
            SetPedMovementClipset(ped, "move_m@fire", 0.25)
        end)
    else
        TriggerEvent('chat:addMessage', {
            color = {255, 0, 0},
            multiline = true,
            args = {"System", "No vehicle nearby!"}
        })
    end
end)


RegisterCommand("+umount", function()
    if isAttached then
        local ped = PlayerPedId()
        TriggerServerEvent('syncDbDetach')

        local vehicle = NetworkGetEntityFromNetworkId(attachData.vehicleNetId)

        if DoesEntityExist(vehicle) then
            DetachEntity(ped, true, true)
            ClearPedTasksImmediately(ped)
            SetPedCanRagdoll(ped, true)
            TaskEnterVehicle(ped, vehicle, 10000, 0, 1.0, 16, 0)
            Wait(500)
            ExecuteCommand("rollfront")

        else
            TriggerEvent('chat:addMessage', {
                color = {255, 0, 0},
                multiline = true,
                args = {"System", "Vehicle no longer exists!"}
            })
        end

        isAttached = false
        attachData = {}
     
        SetPedComponentVariation(ped, 4, originalClothing.pants.drawable, originalClothing.pants.texture, 2) -- Pants
        SetPedComponentVariation(ped, 6, originalClothing.shoes.drawable, originalClothing.shoes.texture, 2) -- Shoes
        SetPedComponentVariation(ped, 8, originalClothing.shirt.drawable, originalClothing.shirt.texture, 2) -- Shirt

        ResetPedMovementClipset(ped, 0.25)
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
        if IsControlJustReleased(1, 73) then 
            if isAttached then
                ExecuteCommand("+umount")
            else
                ExecuteCommand("+mount")
            end
        end
    end
end)

RegisterNetEvent('syncDbSittingClient')
AddEventHandler('syncDbSittingClient', function(serverId, vehicleNetId, offsetX, offsetY, offsetZ, pantsTexture, shoesTexture, shirtTexture)
    local ped = GetPlayerPed(GetPlayerFromServerId(serverId))
    local vehicle = NetworkGetEntityFromNetworkId(vehicleNetId)

    local boneIndex = 0xE0FD
    if DoesEntityExist(ped) and DoesEntityExist(vehicle) then
        AttachEntityToEntity(ped, vehicle, boneIndex, offsetX, offsetY, offsetZ, 0.0, 0.0, -90.0, false, false, false, false, 2, true)
        
        SetPedCanRagdoll(ped, false)
        SetPedCanRagdollFromPlayerImpact(ped, false)
        SetPedConfigFlag(ped, 32, false)
        SetPedConfigFlag(ped, 304, true)

        SetPedComponentVariation(ped, 4, GetPedDrawableVariation(ped, 4), pantsTexture, 2)
        SetPedComponentVariation(ped, 6, GetPedDrawableVariation(ped, 6), shoesTexture, 2)
        SetPedComponentVariation(ped, 8, GetPedDrawableVariation(ped, 8), shirtTexture, 2)
    else
        print("Error: Ped or Vehicle does not exist.")
    end

    CreateThread(function()
        while isAttached do
            Wait(100)
            
        end
    end)
end)

RegisterNetEvent('syncDbDetachClient')
AddEventHandler('syncDbDetachClient', function(serverId)
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
        Wait(500)
        if isAttached and attachData.vehicleNetId then
            local vehicle = NetworkGetEntityFromNetworkId(attachData.vehicleNetId)
            if DoesEntityExist(vehicle) then
                local ped = PlayerPedId()
                AttachEntityToEntity(ped, vehicle, 0xE0FD, attachData.offsetX, attachData.offsetY, attachData.offsetZ, 0.0, 0.0, -90.0, false, false, false, false, 2, true)
                
            else
                print("Error: Vehicle no longer exists.")
                                
                local ped = PlayerPedId()
                SetPedComponentVariation(ped, 4, originalClothing.pants.drawable, originalClothing.pants.texture, 2) -- Pants
                SetPedComponentVariation(ped, 6, originalClothing.shoes.drawable, originalClothing.shoes.texture, 2) -- Shoes
                SetPedComponentVariation(ped, 8, originalClothing.shirt.drawable, originalClothing.shirt.texture, 2) -- Shirt
                
                DetachEntity(ped, true, true)

                isAttached = false
                attachData = {}
            end
        end
    end
end)
