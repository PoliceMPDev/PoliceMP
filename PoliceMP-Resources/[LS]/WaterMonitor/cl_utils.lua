dlc = false

local monitors = {}
local monitorParticles = {}
local supplyLine = {}

RegisterNetEvent('Client:ActivateDLC')
AddEventHandler('Client:ActivateDLC', function(enabled)
    dlc = enabled
end)

TriggerEvent('chat:addSuggestion', '/'..main.commandName, translations.commandSuggestion, {
    { name=translations.setup.."/"..translations.remove, help=translations.commandHelp },
})

AddEventHandler('Client:receiveSupplyLineTable', function(table)
    supplyLine = table
end)

AddEventHandler('Client:updateSupplyLineTable', function(key, entry, remove)
    if remove then 
        supplyLine[key] = nil
        return 
    end
    supplyLine[key] = entry
end)

AddEventHandler('Client:updateMonitorsTable', function(key, entry, remove)
    if remove then 
        monitors[key] = nil
        return 
    end
    monitors[key] = entry
end)

AddEventHandler('Client:receiveMonitorsTable', function(table)
    monitors = table
end)

local has_permission = main.defaultHasPermission

RegisterNetEvent('Client:hasWaterMonitorPermission')
AddEventHandler('Client:hasWaterMonitorPermission', function()
    has_permission = true
end)

local function supplyLineNearby(ped, coords)
    if dlc then
        for k, v in pairs(supplyLine) do
            local distance = #(coords - v[3])
            if distance < 25.0 then
                return true
            end
        end
    else
        return true
    end
    return false
end

function showNotification(message)
    message = message.."."
    SetNotificationTextEntry("STRING")
	AddTextComponentString(message)
	DrawNotification(0,1)
end

Citizen.CreateThread(function()
    while true do
        if not has_permission then
            TriggerServerEvent("Server:checkWaterMonitorPermissions")
            Wait(20000)
        else
            local ped = PlayerPedId()
            local coords = GetEntityCoords(ped)
            for k, v in pairs(monitors) do
                if not v[5] then
                    local distance = #(coords - v[2])
                    if distance < main.usageDistance then
                        if distance < main.pitchDistance then
                            if v[4] then
                                DisableControlAction(main.upKey[1], main.upKey[2], true)
                                DisableControlAction(main.downKey[1], main.downKey[2], true)
                                local change = 0.0                         
                                if IsDisabledControlPressed(main.upKey[1], main.upKey[2]) then
                                    change = change + 15.0
                                end
                                if IsDisabledControlPressed(main.downKey[1], main.downKey[2]) then
                                    change = change - 15.0
                                end
                                if change ~= 0.0 then
                                    TriggerServerEvent("Server:adjustPitch", k, change)
                                    Wait(1000)
                                end
                                DisableControlAction(main.toggleKey[1],main.toggleKey[2], true)
                                if IsDisabledControlPressed(main.toggleKey[1], main.toggleKey[2]) then
                                    pressed = true
                                    TriggerServerEvent("Server:toggleWater", k)
                                    showNotification(translations.monitorToggled)
                                    Wait(main.cooldown * 1000)
                                end
                            else
                                if supplyLineNearby(ped, coords) then
                                    DisableControlAction(main.toggleKey[1],main.toggleKey[2], true)
                                    if IsDisabledControlPressed(main.toggleKey[1], main.toggleKey[2]) then
                                        TriggerServerEvent("Server:toggleWater", k)
                                        showNotification(translations.monitorToggled)
                                        Wait(main.cooldown * 1000)
                                    end
                                else
                                    showNotification(translations.noSupplyLineFound)
                                end
                            end
                        else
                            if supplyLineNearby(ped, coords) then
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
                                    TriggerServerEvent("Server:toggleWater", k)
                                    showNotification(translations.monitorToggled)
                                    Wait(main.cooldown * 1000)
                                end
                            else
                                showNotification(translations.noSupplyLineFound)        
                                if v[4] then
                                    DisableControlAction(main.toggleKey[1],main.toggleKey[2], true)
                                    if IsDisabledControlPressed(main.toggleKey[1], main.toggleKey[2]) then
                                        pressed = true
                                        TriggerServerEvent("Server:toggleWater", k)
                                        showNotification(translations.monitorToggled)
                                        Wait(main.cooldown * 1000)
                                    end
                                end
                            end
                        end
                    end
                end
            end
        end
        Wait(0)
    end
end)

function toFloat(integer)
    return integer + 0.0
end
