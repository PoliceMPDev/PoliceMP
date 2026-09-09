local tents = {}
local tentParticles = {}
local first_spawn = false
local has_permission = main.defaultHasPermission

TriggerEvent('chat:addSuggestion', '/'..main.commandName, translations.commandSuggestion, {
    { name=translations.setup.."/"..translations.remove, help=translations.commandHelp },
})

RegisterNetEvent('Client:decontentNotification')
AddEventHandler('Client:decontentNotification', function(message)
    showNotification(message)
end)

RegisterNetEvent('Client:updateTentsTable')
AddEventHandler('Client:updateTentsTable', function(key, entry, remove)
    if remove then 
        tents[key] = nil
        return 
    end
    tents[key] = entry
end)

RegisterNetEvent('Client:toggleWaterTents')
AddEventHandler('Client:toggleWaterTents', function(key)
    if tents[key] ~= nil then
        tents[key][8] = not tents[key][8]
    end
    if tentParticles[key] ~= nil and tentParticles[key].handle ~= nil and DoesParticleFxLoopedExist(tentParticles[key].handle) then
        StopParticleFxLooped(tentParticles[key].handle, false)
        StopParticleFxLooped(tentParticles[key].handle2, false)
        StopParticleFxLooped(tentParticles[key].handle3, false)
        StopParticleFxLooped(tentParticles[key].handle4, false)
        tentParticles[key] = nil
    end
    if tents[key] ~= nil and tents[key][8] then
        RequestNamedPtfxAsset("core")
        while not HasNamedPtfxAssetLoaded("core") do Wait(0) end
        UseParticleFxAssetNextCall("core")
        SetParticleFxShootoutBoat(1)
        tentParticles[key] = {}
        tentParticles[key].handle = StartParticleFxLoopedAtCoord("water_cannon_jet", tents[key][3].x, tents[key][3].y, tents[key][3].z, -90.0, 0.0, --[[Heading]]0.0, main.showerIntensity, false, false, false, false)
        UseParticleFxAssetNextCall("core")
        SetParticleFxShootoutBoat(1)
        tentParticles[key].handle2 = StartParticleFxLoopedAtCoord("water_cannon_jet", tents[key][4].x, tents[key][4].y, tents[key][4].z, -90.0, 0.0, --[[Heading]]0.0, main.showerIntensity, false, false, false, false)
        UseParticleFxAssetNextCall("core")
        SetParticleFxShootoutBoat(1)
        tentParticles[key].handle3 = StartParticleFxLoopedAtCoord("water_cannon_jet", tents[key][5].x, tents[key][5].y, tents[key][5].z, -90.0, 0.0, --[[Heading]]0.0, main.showerIntensity, false, false, false, false)
        UseParticleFxAssetNextCall("core")
        SetParticleFxShootoutBoat(1)
        tentParticles[key].handle4 = StartParticleFxLoopedAtCoord("water_cannon_jet", tents[key][6].x, tents[key][6].y, tents[key][6].z, -90.0, 0.0, --[[Heading]]0.0, main.showerIntensity, false, false, false, false)
    end
end)

RegisterNetEvent('Client:receiveTentsTable')
AddEventHandler('Client:receiveTentsTable', function(table)
    tents = table
end)

function showNotification(message)
    message = message.."."
    SetNotificationTextEntry("STRING")
    AddTextComponentString(message)
    DrawNotification(0,1)
end

RegisterNetEvent('Client:hasTentsPermission')
AddEventHandler('Client:hasTentsPermission', function()
    has_permission = true
end)

AddEventHandler('playerSpawned', function()
    if not first_spawn then
        TriggerServerEvent('Server:receiveTentsTable')
        first_spawn = true
    end
end)

function tableHas(table, key)
    for k in pairs(table) do
        if k == key then
            return true
        end
    end
    return false
end

function tableLength(t)
    local count = 0
    for _ in pairs(t) do count = count + 1 end
    return count
end

function net(id)
    SetNetworkIdExistsOnAllMachines(id, true)
    SetNetworkIdCanMigrate(id, false)
end

function requestControl(milliseconds, entity)
    local timeout = false
    Citizen.SetTimeout(milliseconds, function()
        timeout = true
    end)
    if not NetworkHasControlOfEntity(entity) then
        NetworkRequestControlOfEntity(entity)
        while not timeout do
            if NetworkHasControlOfEntity(entity) then 
                timeout = true
            end
            Wait(0)
        end
    end
end

RegisterNetEvent('Client:toggleTent')
AddEventHandler('Client:toggleTent', function(setup)
    local ped = PlayerPedId()
    local vehicle = GetVehiclePedIsIn(ped, false)
    if vehicle ~= 0 then
        showNotification(translations.outsideVehicle)
        return
    end
    local coords = GetEntityCoords(ped)
    if setup then
        RequestModel(main.tentModel)
        while not HasModelLoaded(main.tentModel) do Wait(0) end
        local offSet = GetOffsetFromEntityInWorldCoords(ped, main.spawnOffset[1], main.spawnOffset[2], main.spawnOffset[3])
        local prop = CreateObject(main.tentModel, offSet.x, offSet.y, offSet.z, true, true, true)
        while not DoesEntityExist(prop) do Wait(0) end
        local propNet = NetworkGetNetworkIdFromEntity(prop)
        PlaceObjectOnGroundProperly(prop)
        FreezeEntityPosition(prop, true)
        SetEntityHeading(prop, GetEntityHeading(ped))
        net(propNet)
        local offSet1 = GetOffsetFromEntityInWorldCoords(prop, -1.75, 0.0, 2.9)
        local offSet2 = GetOffsetFromEntityInWorldCoords(prop, -0.63, 0.0, 2.9)
        local offSet3 = GetOffsetFromEntityInWorldCoords(prop, 0.63, 0.0, 2.9)
        local offSet4 = GetOffsetFromEntityInWorldCoords(prop, 1.75, 0.0, 2.9)
        tents[propNet] = {propNet, offSet, offSet1, offSet2, offSet3, offSet4, GetEntityHeading(prop), false}
        TriggerServerEvent('Server:updateTentsTable', propNet, tents[propNet], false)
        SetModelAsNoLongerNeeded(main.tentModel)
        showNotification(translations.tentSetup)
    else
        local found = false
        local foundKey = 0
        for k, v in pairs(tents) do
            local distance = #(coords - v[2])
            if distance < 15.0 then
                foundKey = k
                found = true
                break
            end
        end
        if found then
            if tents[foundKey][8] then
                showNotification(translations.tentActive)
            else
                local prop = NetworkGetEntityFromNetworkId(tents[foundKey][1])
                TriggerServerEvent('Server:updateTentsTable', foundKey, tents[foundKey], true)
                if DoesEntityExist(prop) then
                    requestControl(2000, prop)
                    DeleteEntity(prop)
                end
                tents[foundKey] = nil
                showNotification(translations.tentRemoved)
            end
        else
            showNotification(translations.noTentFound)
        end
    end
end)

function displayHelpText(message)
    SetTextComponentFormat('STRING')
    AddTextComponentString(message)
    DisplayHelpTextFromStringLabel(0, 0, 1, -1)
end

Citizen.CreateThread(function()
    while true do
        if not has_permission then
            TriggerServerEvent("Server:checkTentsPermissions")
            Wait(20000)
        else
            local ped = PlayerPedId()
            local coords = GetEntityCoords(ped)
            for k, v in pairs(tents) do
                local distance = #(coords - v[2])
                if distance < main.usageDistance then
                    displayHelpText(translations.press.." ~"..translations.keyHelp.."~ "..translations.toUse)
                    local timeout = false
                    local pressed = false
                    while not timeout do
                        DisableControlAction(main.toggleKey[1],main.toggleKey[2], true)
                        if IsDisabledControlPressed(main.toggleKey[1], main.toggleKey[2]) then
                            pressed = true
                            timeout = true
                        end
                        Wait(0)
                    end
                    if pressed then
                        TriggerServerEvent("Server:toggleWaterTents", k)
                        showNotification(translations.showerToggled)
                        Wait(main.cooldown * 1000)
                    end
                end
            end
        end
        Wait(0)
    end
end)