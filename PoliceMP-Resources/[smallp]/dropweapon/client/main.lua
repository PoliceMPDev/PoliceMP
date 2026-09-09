local isDropping = false
local droppedProps = {} 

function dropCurrentWeapon()
    if isDropping then return end
    isDropping = true

    local ped = PlayerPedId()
    local weaponHash = GetSelectedPedWeapon(ped)

    if weaponHash and weaponHash ~= WEAPON_UNARMED then

        local propName = Config.WeaponProps[weaponHash]
        
        if propName then

            local pedCoords = GetEntityCoords(ped)
            local pedHeading = GetEntityHeading(ped)

            local forwardVector = vector3(
                -math.sin(math.rad(pedHeading)),
                math.cos(math.rad(pedHeading)),
                0
            )
            local dropPosition = pedCoords + (forwardVector * 0.75) -- Adjusted distance

            TriggerServerEvent('dropWeapon:playAnimation', pedCoords, weaponHash, propName)
            RemoveWeaponFromPed(ped, weaponHash)
            SetCurrentPedWeapon(ped, WEAPON_UNARMED, true)

            TriggerServerEvent('dropWeapon:syncDrop', weaponHash, dropPosition, propName)
        else
            print("No matching prop found for this weapon.")
        end
    else
        print("No weapon equipped.")
    end

    isDropping = false
end

function deleteNearbyProps(pedCoords, radius)
    for i = #droppedProps, 1, -1 do
        local prop = droppedProps[i]
        if DoesEntityExist(prop) then
            local propCoords = GetEntityCoords(prop)
            local distance = Vdist(pedCoords, propCoords)
            if distance <= radius then
                DeleteEntity(prop)
                Citizen.Wait(100)
                table.remove(droppedProps, i)
            end
        end
    end
end

RegisterNetEvent('dropWeapon:checkAndDeleteNearbyProps')
AddEventHandler('dropWeapon:checkAndDeleteNearbyProps', function(pedCoords, radius)
    deleteNearbyProps(pedCoords, radius)
end)

RegisterNetEvent('dropWeapon:syncDropToClients')
AddEventHandler('dropWeapon:syncDropToClients', function(weaponHash, propCoords, propName)
    Wait(1000)
    local propHash = GetHashKey(propName)
    RequestModel(propHash)
    while not HasModelLoaded(propHash) do
        Citizen.Wait(10)
    end

    local prop = CreateObject(propHash, propCoords.x, propCoords.y, propCoords.z, true, true, true)
    SetEntityAsMissionEntity(prop, true, true)

    PlaceObjectOnGroundProperly(prop)
    FreezeEntityPosition(prop, true) 
    SetEntityCollision(prop, true, true)

    Citizen.Wait(50) 
    SetEntityRotation(prop, 90.0, 0.0, 0.0, 2, true)
    table.insert(droppedProps, prop)
end)

RegisterNetEvent('dropWeapon:playAnimationOnClients')
AddEventHandler('dropWeapon:playAnimationOnClients', function(pedCoords, weaponHash, propName)
    local ped = PlayerPedId()
    RequestAnimDict("random@mugging3")
    while not HasAnimDictLoaded("random@mugging3") do
        Citizen.Wait(10)
    end
    TaskPlayAnim(ped, "random@mugging3", "pickup_low", 8.0, -8.0, -1, 49, 0, false, false, false)
    Citizen.Wait(1000)
    ClearPedTasks(ped)
end)

RegisterCommand("drop", function()
    dropCurrentWeapon()
end)

RegisterCommand("take", function(source, args, rawCommand)
    local radius = tonumber(args[1]) or 10.0 
    TriggerServerEvent('dropWeapon:requestDeleteNearbyProps', radius)
end, false)
