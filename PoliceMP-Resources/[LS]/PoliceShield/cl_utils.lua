local shield = {
    active = false,
    id = 0,
    prop = 0,
    inVehicle = false,
}

RegisterNetEvent("updateShield")
AddEventHandler("updateShield", function(shieldData)
    shield = shieldData
end)

RegisterNetEvent("shieldNotification")
AddEventHandler("shieldNotification", function(message)
    Notify(message)
end)

exports('isShieldActive', function ()
    return shield.active
end)

vRP = nil
ESX = nil

if Inventory.quasarInventory then
    function inventorySetupShield(item, meta)
        createShield(item.name)
    end
    
    exports("inventorySetupShield", inventorySetupShield)
end

if Notifications.Enabled and Notifications.Framework.ESX then
    ESX = exports["es_extended"]:getSharedObject()
end

if Notifications.Enabled and Notifications.Framework.vRP then
    vRP = Proxy.getInterface("vRP")
end

function Notify(text)

    if not Notifications.Enabled then
        return
    end

    if Notifications.Framework.ESX then
        if ESX ~= nil then
            ESX.ShowNotification(text)
        end
    elseif Notifications.Framework.QBCore then
        TriggerEvent('QBCore:Notify', text, 'info')
    elseif Notifications.Framework.QBX then
        exports.qbx_core:Notify(text, 'primary')
    elseif Notifications.Framework.vRP then
        vRP.notify(source, {text})
    elseif Notifications.Framework.okok then
        exports['okokNotify']:Alert("Police Tracker", text, 2000, 'info', true)
    else
        showBaseNotification(text)
    end
end

function showBaseNotification(message)
    SetNotificationTextEntry("STRING")
    AddTextComponentString(message)
    DrawNotification(0,1)
end


local shieldTypes = ""
Citizen.CreateThread(function()
    for k, v in pairs(shields) do
        if shieldTypes == "" then
            shieldTypes = v.name
        else
            shieldTypes = shieldTypes..", "..v.name
        end
    end
    TriggerEvent('chat:addSuggestion', "/"..main.commandName, main.commandChatSuggestion, {
        { name=main.parameterType, help=tostring(shieldTypes) },
    })
    while true do
        if shield.active then
            local ped = PlayerPedId()
            
            if IsPedInAnyVehicle(ped, true) then
                if not shield.inVehicle then
                    SetEntityCollision(shield.prop, false, true)
                    shield.inVehicle = true
                end
            else
                if shield.inVehicle then
                    SetEntityCollision(shield.prop, shields[shield.id].collision, true)
                    shield.inVehicle = false
                end
            end
            
            if not shields[shield.id].allowShooting then
                DisablePlayerFiring(ped, true)
                DisableControlAction(0,24) -- INPUT_ATTACK
                DisableControlAction(0,69) -- INPUT_VEH_ATTACK
                DisableControlAction(0,70) -- INPUT_VEH_ATTACK2
                DisableControlAction(0,92) -- INPUT_VEH_PASSENGER_ATTACK
                DisableControlAction(0,114) -- INPUT_VEH_FLY_ATTACK
                DisableControlAction(0,257) -- INPUT_ATTACK2
                DisableControlAction(0,331) -- INPUT_VEH_FLY_ATTACK2
            end

            if not shield.inVehicle then
                if shields[shield.id] ~= nil and shields[shield.id].animDict ~= nil and shields[shield.id].animName ~= nil then
                    if not IsEntityPlayingAnim(ped, shields[shield.id].animDict, shields[shield.id].animName, 3) then
                        TaskPlayAnim(ped, shields[shield.id].animDict, shields[shield.id].animName, 8.0, -8.0, -1, 50, 0.0, 0, 0, 0)
                    end
                end
                
            end
            Wait(0)
        else
            Wait(1000)
        end
    end
end)

if main.enableDeveloperCommand then
    RegisterCommand("shieldposition", function(source, args)
        local ped = PlayerPedId()
        local vehicle = GetVehiclePedIsIn(ped, false)
        if vehicle ~= 0 and vehicle ~= nil then
            showNotification(translations.outsideVehicle)
            return
        end
        if args[1] == nil then
            showNotification(translations.invalidShieldModel)
            return 
        end
        local model = GetHashKey(args[1])
        if not IsModelInCdimage(model) then
            showNotification(translations.invalidShieldModel)
            return
        end
        RequestModel(model)
        while not HasModelLoaded(model) do Wait(0) end
        local coords = GetEntityCoords(ped)
        local shield = CreateObject(model, coords, main.networkedWhenDeveloping, true, true)
        while not DoesEntityExist(shield) do Wait(0) end
        SetModelAsNoLongerNeeded(model)
        local offSet = {0.0, 0.0, 0.0}
        local rotation = {0.0, 0.0, 0.0}
        local completed = false
        RequestAnimDict(main.developerAnimDict)
        while not HasAnimDictLoaded(main.developerAnimDict) do Wait(0) end
        TaskPlayAnim(ped, main.developerAnimDict, main.developerAnimName, 8.0, -8.0, -1, 50, 0.0, 0, 0, 0)
        AttachEntityToEntity(shield, ped, main.developerBoneIndex, offSet[1], offSet[2], offSet[3], rotation[1], rotation[2], rotation[3], true, true, main.developerCollision, false, 1, true)
        while not completed do
            local changed = false
            DisableControlAction(0, 191, true)
            if IsDisabledControlJustPressed(0, 191) then 
                completed = true 
            end

            DisableControlAction(0, 210, true)
            local adjustingRotation = IsDisabledControlPressed(0, 210) -- this enables rotational movement

            if IsControlPressed(0, 174) then -- Arrow left (X offSet) or rotation
                if adjustingRotation then 
                    rotation[1] = rotation[1] - main.rotationMovement 
                else
                    offSet[1] = offSet[1] - main.offSetMovement 
                end
                changed = true
            end

            if IsControlPressed(0, 175) then -- Arrow right (X offSet) or rotation
                if adjustingRotation then 
                    rotation[1] = rotation[1] + main.rotationMovement 
                else
                    offSet[1] = offSet[1] + main.offSetMovement 
                end
                changed = true
            end

            if IsControlPressed(0, 172) then -- Arrow up (Y offSet) or rotation
                if adjustingRotation then 
                    rotation[2] = rotation[2] + main.rotationMovement 
                else
                    offSet[2] = offSet[2] + main.offSetMovement 
                end
                changed = true
            end

            if IsControlPressed(0, 173) then -- Arrow down (Y offSet) or rotation
                if adjustingRotation then 
                    rotation[2] = rotation[2] - main.rotationMovement 
                else
                    offSet[2] = offSet[2] - main.offSetMovement 
                end
                changed = true
            end

            if IsControlPressed(0, 208) then -- Page up (Z offSet) or rotation
                if adjustingRotation then 
                    rotation[3] = rotation[3] + main.rotationMovement 
                else
                    offSet[3] = offSet[3] + main.offSetMovement 
                end
                changed = true
            end

            if IsControlPressed(0, 207) then -- Page down (Z offSet) or rotation
                if adjustingRotation then 
                    rotation[3] = rotation[3] - main.rotationMovement 
                else
                    offSet[3] = offSet[3] - main.offSetMovement 
                end
                changed = true
            end

            if changed then
                DetachEntity(shield)
                AttachEntityToEntity(shield, ped, main.developerBoneIndex, offSet[1], offSet[2], offSet[3], rotation[1], rotation[2], rotation[3], true, true, main.developerCollision, false, 1, true)
            end
            Wait(0)
        end
        Wait(1000)
        showNotification("Offset: {"..offSet[1]..", "..offSet[2]..", "..offSet[3].."}")
        print("Offset: {"..offSet[1]..", "..offSet[2]..", "..offSet[3].."}")
        showNotification("Rotation: {"..rotation[1]..", "..rotation[2]..", "..rotation[3].."}")
        print("Rotation: {"..rotation[1]..", "..rotation[2]..", "..rotation[3].."}")
        ClearPedTasksImmediately(ped)
        if DoesEntityExist(shield) then
            DetachEntity(shield)
            DeleteEntity(shield)
        end
    end, false)
end