RegisterNetEvent('chopShop:startChop')
AddEventHandler('chopShop:startChop', function()
    local playerPed = PlayerPedId()
    local playerPos = GetEntityCoords(playerPed)

    if IsPedInAnyVehicle(playerPed, false) then
        Notify('You cannot start chopping while inside a vehicle.')
        return
    end

    local vehicle = GetClosestVehicle(playerPos, 3.5)
    if vehicle and vehicle ~= 0 then
        StartChopping(vehicle, playerPos)
    else
        Notify('No vehicle found nearby.')
    end
end)

function StartChopping(vehicle, playerPos)

    TaskStartScenarioInPlace(PlayerPedId(), 'world_human_welding', 0, true)
    Citizen.Wait(8000)
    ClearPedTasksImmediately(PlayerPedId())
    
    ApplyDamageToNearestPart(vehicle, playerPos)
end

function ApplyDamageToNearestPart(vehicle, playerPos)
    local vehiclePos = GetEntityCoords(vehicle)
    local parts = {
        {name = 'wheel_lf', index = 0},
        {name = 'wheel_rf', index = 1},
        {name = 'wheel_lr', index = 2},
        {name = 'wheel_rr', index = 3},
        {name = 'bonnet', index = 4},
        {name = 'boot', index = 5},
        {name = 'door_dside_f', index = 0},
        {name = 'door_pside_f', index = 1},
        {name = 'door_dside_r', index = 2},
        {name = 'door_pside_r', index = 3}
    }

    local closestPart = nil
    local minDistance = math.huge

    for _, part in ipairs(parts) do
        local boneIndex = GetEntityBoneIndexByName(vehicle, part.name)
        if boneIndex and boneIndex ~= -1 then
            local bonePos = GetWorldPositionOfEntityBone(vehicle, boneIndex)
            local distance = #(playerPos - bonePos)
            if distance < minDistance then
                minDistance = distance
                closestPart = part
            end
        end
    end

    if closestPart then        
        TriggerServerEvent('chopShop:applyDamage', VehToNet(vehicle), closestPart)
    else
        Notify('No detachable part found on the vehicle.')
    end
end

RegisterNetEvent('chopShop:applyDamageToVehicle')
AddEventHandler('chopShop:applyDamageToVehicle', function(vehicleNetId, part)
    local vehicle = NetToVeh(vehicleNetId)

    if DoesEntityExist(vehicle) then
        if part.name:match('wheel_') then
            BreakOffVehicleWheel(vehicle, part.index)
        elseif part.name:match('bonnet') or part.name:match('boot') then
            SetVehicleDoorCanBreak(vehicle, part.index, true)
            SetVehicleDoorBroken(vehicle, part.index, false)
        elseif part.name:match('door_') then
            SetVehicleDoorCanBreak(vehicle, part.index, true)
            SetVehicleDoorBroken(vehicle, part.index, false)
        end
    end
end)

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
