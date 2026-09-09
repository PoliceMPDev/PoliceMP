local plantedBomb = nil

RegisterNetEvent("bomb:clientPlace")
AddEventHandler("bomb:clientPlace", function(pos, heading, entityHit)
    local model = `prop_c4_final`
    RequestModel(model)
    while not HasModelLoaded(model) do Wait(50) end

    local bomb = CreateObject(model, pos.x, pos.y, pos.z, true, true, false)
    SetEntityHeading(bomb, heading)
    FreezeEntityPosition(bomb, true)

    if entityHit and entityHit ~= 0 then
        AttachEntityToEntity(bomb, entityHit, 0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0,
                             false, false, false, false, 2, true)
    end

    plantedBomb = bomb
end)

RegisterNetEvent("bomb:clientDetonate")
AddEventHandler("bomb:clientDetonate", function()
    if not plantedBomb or not DoesEntityExist(plantedBomb) then return end

    local coords = GetEntityCoords(plantedBomb)

    -- Particle FX
    local ptfxDict = "des_gas_station"
    local ptfxName = "ent_ray_paleto_gas_explosion"
    RequestNamedPtfxAsset(ptfxDict)
    while not HasNamedPtfxAssetLoaded(ptfxDict) do Wait(50) end
    UseParticleFxAssetNextCall(ptfxDict)
    StartParticleFxNonLoopedAtCoord(ptfxName, coords.x, coords.y, coords.z, 0.0, 0.0, 0.0, 0.1, false, false, false)

    -- Explosion
    AddExplosion(coords.x, coords.y, coords.z, 2, 0.15, true, false, 0.4)

    DeleteEntity(plantedBomb)
    plantedBomb = nil
end)

RegisterNetEvent("bomb:requestPlacement")
AddEventHandler("bomb:requestPlacement", function()
    local ped = PlayerPedId()
    local startCoords = GetEntityCoords(ped)
    local forwardVector = GetEntityForwardVector(ped)
    local endCoords = startCoords + forwardVector * 1.5

    local rayHandle = StartShapeTestRay(startCoords.x, startCoords.y, startCoords.z,
                                        endCoords.x, endCoords.y, endCoords.z, 287, ped, 7)

    local _, hit, hitCoords, _, entityHit = GetShapeTestResult(rayHandle)

    if hit ~= 1 then
        SetNotificationTextEntry("STRING")
        AddTextComponentString("~r~ERROR: ~s~Place the plastic explosive on a surface!")
        DrawNotification(false, true)
        return
    end

    -- Animation
    local animDict = "anim@scripted@heist@ig6_explosive_plant@male@"
    RequestAnimDict(animDict)
    while not HasAnimDictLoaded(animDict) do Wait(100) end

    TaskPlayAnim(ped, animDict, "plant_male", 8.0, -8.0, 2000, 49, 0, false, false, false)
    Wait(2000)
    ClearPedTasks(ped)

    -- Send placement to server
    TriggerServerEvent("bomb:syncPlace", hitCoords, GetEntityHeading(ped), entityHit or 0)
end)

RegisterNetEvent("bomb:notify")
AddEventHandler("bomb:notify", function(message)
    SetNotificationTextEntry("STRING")
    AddTextComponentString(message)
    DrawNotification(false, true)
end)
