-- Command registered server side below:
RegisterCommand(main.commandName, function(source, args, rawCommand)
    if (source > 0) then
        if args[1] == nil or (tostring(args[1]) ~= translations.setup and tostring(args[1]) ~= translations.remove) then
            TriggerClientEvent("Client:decontentNotification", source, translations.commandError)
            return 
        end
        local setup = false
        if tostring(args[1]) == translations.setup then setup = true end
        if tostring(args[1]) == translations.remove then setup = false end

        -- Add your permission check here, send event if they have permission

        TriggerClientEvent("Client:toggleTent", source, setup)
    end
end, main.enableAcePermissions)

RegisterServerEvent('Server:checkTentsPermissions')
AddEventHandler('Server:checkTentsPermissions', function()
    local source = source

    -- Add your permission check here, send event if they have permission
    -- This enables them to toggle on/off the shower
    -- This event is triggered to CHECK if they have permission
    -- This only needs to be sent once, send it again to take their permission away
    -- You can easily add ace permissions here, by checking their ace

    TriggerClientEvent("Client:hasTentsPermission", source)
end)

local tents = {}

RegisterServerEvent('Server:updateTentsTable')
AddEventHandler('Server:updateTentsTable', function(key, entry, remove)
    if remove then 
        tents[key] = nil
        TriggerClientEvent("Client:updateTentsTable", -1, key, entry, remove)
        return 
    end
    tents[key] = entry
    TriggerClientEvent("Client:updateTentsTable", -1, key, entry, remove)
end)

RegisterServerEvent('Server:receiveTentsTable')
AddEventHandler('Server:receiveTentsTable', function()
    TriggerClientEvent("Client:receiveTentsTable", source, tents)
end)

RegisterServerEvent('Server:toggleWaterTents')
AddEventHandler('Server:toggleWaterTents', function(key)
    TriggerClientEvent("Client:toggleWaterTents", -1, key)
end)