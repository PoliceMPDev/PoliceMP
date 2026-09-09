-- Command registered server side below:
RegisterCommand(main.commandName, function(source, args, rawCommand)
    if (source > 0) then
        if args[1] == nil or (tostring(args[1]) ~= translations.setup and tostring(args[1]) ~= translations.remove) then
            TriggerClientEvent("Client:monitorNotification", source, translations.commandError)
            return 
        end
        local setup = false
        if tostring(args[1]) == translations.setup then setup = true end
        if tostring(args[1]) == translations.remove then setup = false end

        -- Add your permission check here, send event if they have permission
        local permission = userHasPermission(source, jobCheck)
        if permission then
            TriggerClientEvent("Client:toggleMonitor", source, setup)
        end

    end
end, main.enableAcePermissions)

if jobCheck.QBCore.enabled then
    QBCore = exports["qb-core"]:GetCoreObject()
end

if jobCheck.vRP.enabled then
    Proxy = module("vrp", "lib/Proxy")
    vRP = Proxy.getInterface("vRP")
end

if jobCheck.ESX.enabled then
    ESX = nil
    ESX = exports["es_extended"]:getSharedObject()
end

usedWatermonitor = {}
if inventory.itemEnabled then
    if inventory.oxInventory then
        exports('useWaterMonitor', function(event, item, inventory, slot, data)
            if event == 'usingItem' and userHasPermission(inventory.player.source, jobCheck) then
                TriggerClientEvent("Client:toggleMonitor", inventory.player.source, not usedWatermonitor[inventory.player.source])
                usedWatermonitor[inventory.player.source] = not usedWatermonitor[inventory.player.source]
                return false
            end
        end)
    elseif inventory.qbInventory or (inventory.coreInventory and jobCheck.QBCore.enabled) or (inventory.quasarInventory and jobCheck.QBCore.enabled) then
        QBCore.Functions.CreateUseableItem("watermonitor", function(source, item)
            if userHasPermission(source, jobCheck) then
                TriggerClientEvent("Client:toggleMonitor", source, not usedWatermonitor[source])
                usedWatermonitor[source] = not usedWatermonitor[source]
            end
        end)
    elseif inventory.esxInventory or (inventory.coreInventory and jobCheck.ESX.enabled) or (inventory.quasarInventory and jobCheck.ESX.enabled) then
        ESX.RegisterUsableItem("watermonitor", function(source)
            if userHasPermission(source, jobCheck) then
                TriggerClientEvent("Client:toggleMonitor", source, not usedWatermonitor[source])
                usedWatermonitor[source] = not usedWatermonitor[source]
            end
        end)
    elseif inventory.vRPInventory then
        RegisterNetEvent("Server:useWaterMonitor", function(source)
            if userHasPermission(source, jobCheck) then
                TriggerClientEvent("Client:toggleMonitor", source, not usedWatermonitor[source])
                usedWatermonitor[source] = not usedWatermonitor[source]
            end
        end)
    end 
end


function userHasPermission(source, location)
    local permission = false
    local usingPermissions = false
    -- ESX Permissions
    if location.ESX.enabled then
        local xPlayer = ESX.GetPlayerFromId(source)
        if location.ESX.checkJob.enabled then
            usingPermissions = true
            for k, v in pairs(location.ESX.checkJob.jobs) do
                if xPlayer.job.name == v then
                    permission = true
                end
            end
        end
    end

    -- vRP Permission
    if location.vRP.enabled then
        if location.vRP.checkPermission.enabled then
            usingPermissions = true
            for k, v in pairs(location.vRP.checkPermission.permissions) do
                if vRP.hasPermission({vRP.getUserId({source}),v}) then
                    permission = true
                end
            end
        end

        if location.vRP.checkGroup.enabled then
            usingPermissions = true
            for k, v in pairs(location.vRP.checkGroup.groups) do
                if vRP.hasGroup({vRP.getUserId({source}),v}) then
                    permission = true
                end
            end
        end
    end

    -- QBCore Permission
    if location.QBCore.enabled then
        local player = QBCore.Functions.GetPlayer(source)
        if location.QBCore.checkJob.enabled then
            usingPermissions = true
            for k, v in pairs(location.QBCore.checkJob.jobs) do
                if player.PlayerData.job.name == v then
                    permission = true
                end
            end
        end
        if location.QBCore.checkPermission.enabled then
            usingPermissions = true
            for k, v in pairs(location.QBCore.checkPermission.permissions) do
                if QBCore.Functions.HasPermission(source, v) then
                    permission = true
                end
            end
        end
    end

    if not usingPermissions then
        permission = true
    end
    return permission
end

-- More permissions here:

RegisterServerEvent('Server:checkWaterMonitorPermissions')
AddEventHandler('Server:checkWaterMonitorPermissions', function()
    local source = source

    -- Add your permission check here, send event if they have permission
    -- This enables them to toggle on/off and rotate the water monitor
    -- This event is triggered to CHECK if they have permission
    -- This only needs to be sent once, send it again to take their permission away
    -- You can easily add ace permissions here, by checking their ace

    TriggerClientEvent("Client:hasWaterMonitorPermission", source)
end)

-- We do not recommend editing below this point
local monitors = {}

RegisterServerEvent('Server:updateMonitorsTable')
AddEventHandler('Server:updateMonitorsTable', function(key, entry, remove)
    if remove then 
        monitors[key] = nil
        TriggerClientEvent("Client:updateMonitorsTable", -1, key, entry, remove)
        return 
    end
    monitors[key] = entry
    TriggerClientEvent("Client:updateMonitorsTable", -1, key, entry, remove)
end)

RegisterServerEvent('Server:receiveMonitorsTable')
AddEventHandler('Server:receiveMonitorsTable', function()
    TriggerClientEvent("Client:receiveMonitorsTable", source, monitors)
end)

RegisterServerEvent('Server:toggleWater')
AddEventHandler('Server:toggleWater', function(key)
    TriggerClientEvent("Client:toggleWater", -1, key)
end)

RegisterServerEvent('Server:adjustPitch')
AddEventHandler('Server:adjustPitch', function(key, change)
    TriggerClientEvent('Client:adjustPitch', -1, key, change)
end)
