local attachedWheelProp = nil

RegisterNetEvent('stealwheels:startBreaking')
AddEventHandler('stealwheels:startBreaking', function()
    local playerPed = PlayerPedId()
    local playerPos = GetEntityCoords(playerPed)

    if IsPedInAnyVehicle(playerPed, false) then
        Notify('Dont be lazy! You cannot steal wheels while inside a vehicle!')
        return
    end

    local vehicle = GetClosestVehicle(playerPos, 3.5)
    if vehicle and vehicle ~= 0 then
        StartBreakingWheels(vehicle, playerPos)
    else
        Notify('No vehicle found nearby.')
    end
end, false)

function StartBreakingWheels(vehicle, playerPos)
    ExecuteCommand('e mechanic2')
    Citizen.Wait(8000)
    ClearPedTasks(PlayerPedId())

    ApplyDamageToNearestWheel(vehicle, playerPos)
end

function ApplyDamageToNearestWheel(vehicle, playerPos)
    local wheels = {
        {name = 'wheel_lf', index = 0},
        {name = 'wheel_rf', index = 1},
        {name = 'wheel_lr', index = 2},
        {name = 'wheel_rr', index = 3}
    }

    local closestWheel = nil
    local minDistance = math.huge

    for _, wheel in ipairs(wheels) do
        local boneIndex = GetEntityBoneIndexByName(vehicle, wheel.name)
        if boneIndex and boneIndex ~= -1 then
            local bonePos = GetWorldPositionOfEntityBone(vehicle, boneIndex)
            local distance = #(playerPos - bonePos)
            if distance < minDistance then
                minDistance = distance
                closestWheel = wheel
            end
        end
    end

    if closestWheel then
        -- Trigger the server event for breaking the wheel
        TriggerServerEvent('stealwheels:applyDamage', VehToNet(vehicle), closestWheel)
       
        CreateAndAttachWheelProp(PlayerPedId(), closestWheel.index)
    else
        Notify('No wheels nearby to steal!')
    end
end

function CreateAndAttachWheelProp(playerPed, wheelIndex)
    -- Load the animation dictionary
    RequestAnimDict("anim@heists@box_carry@")
    while not HasAnimDictLoaded("anim@heists@box_carry@") do
        Citizen.Wait(100) 
    end

    attachedWheelProp = CreateObject(GetHashKey('prop_wheel_01'), 0, 0, 0, true, true, true) 

    if not DoesEntityExist(attachedWheelProp) then
        Notify('Failed to create wheel prop.')
        return
    end

    local boneIndex = GetPedBoneIndex(playerPed, 28422) 
    AttachEntityToEntity(attachedWheelProp, playerPed, boneIndex, 0.0, 0.0, 0.1, 0.0, 0.0, 180.0, true, true, false, true, 1, true)

    Citizen.Wait(500)
    TaskPlayAnim(playerPed, "anim@heists@box_carry@", "idle", 8.0, -8.0, -1, 51, 0, false, false, false)


    Citizen.CreateThread(function()
        while attachedWheelProp do
            Citizen.Wait(0)  -- Run every frame
            if IsControlJustPressed(1, 73) then -- 73 is the control index for the X key
                DetachAndStopAnimation(playerPed)
                break
            end
        end
    end)
end

function DetachAndStopAnimation(playerPed)
    if attachedWheelProp and DoesEntityExist(attachedWheelProp) then
        DetachEntity(attachedWheelProp, true, true)
        attachedWheelProp = nil  -- Clear the reference
    end

    ClearPedTasksImmediately(playerPed)  -- Stop the animation
    Notify('You have dropped a Wheel.')
end

function GetClosestVehicle(position, radius)
    local vehicles = GetGamePool('CVehicle')
    local closestVehicle = nil
    local minDistance = radius

    for _, vehicle in ipairs(vehicles) do
        local vehiclePos = GetEntityCoords(vehicle)
        local distance = #(position - vehiclePos)

        if distance < minDistance then
            minDistance = distance
            closestVehicle = vehicle
        end
    end

    return closestVehicle
end

function Notify(message)
    SetNotificationTextEntry('STRING')
    AddTextComponentString(message)
    DrawNotification(false, true)
end

RegisterNetEvent('stealwheels:applyDamageToVehicle')
AddEventHandler('stealwheels:applyDamageToVehicle', function(vehicleNetId, part)
    local vehicle = NetToVeh(vehicleNetId)

    if DoesEntityExist(vehicle) then
        -- Request control first
        if not NetworkHasControlOfEntity(vehicle) then
            NetworkRequestControlOfEntity(vehicle)
            local timeout = GetGameTimer() + 2000
            while not NetworkHasControlOfEntity(vehicle) and GetGameTimer() < timeout do
                Citizen.Wait(10)
            end
        end

        -- Now break the wheel with control
        BreakOffVehicleWheel(vehicle, part.index, false, true, false, true)

        -- Apply force as extra effect
        ApplyForceToEntity(vehicle, 0, 0, 70.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, true, true, true, true, true, true)

        -- Cleanup broken wheel debris
        Citizen.CreateThread(function()
            local playerPos = GetEntityCoords(PlayerPedId())
            local radius = 0.6 -- 2 ft = about 0.6 meters in GTA units
        
            local objects = GetGamePool('CObject')
            for _, obj in ipairs(objects) do
                if DoesEntityExist(obj) then
                    local objPos = GetEntityCoords(obj)
                    local distance = #(playerPos - objPos)
        
                    if distance < radius then
                        -- 🚨 DO NOT DELETE if it's our carried wheel prop
                        if obj ~= attachedWheelProp then
        
                            -- Request control first
                            if not NetworkHasControlOfEntity(obj) then
                                NetworkRequestControlOfEntity(obj)
                                local timeout = GetGameTimer() + 2000
                                while not NetworkHasControlOfEntity(obj) and GetGameTimer() < timeout do
                                    Citizen.Wait(10)
                                end
                            end
        
                            -- Delete the object (broken wheel debris)
                            SetEntityAsMissionEntity(obj, true, true)
                            DeleteEntity(obj)
                        end
                    end
                end
            end
        end)
             
    end
end)
