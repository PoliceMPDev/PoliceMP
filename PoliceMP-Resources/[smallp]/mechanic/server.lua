RegisterCommand('mech', function(source, args, rawCommand)
    local playerId = source
    if IsPlayerAceAllowed(playerId, "Highways.Trained") then
        TriggerClientEvent('mechanic:openMenu', playerId)
    else        
        TriggerClientEvent('chat:addMessage', playerId, { args = { '^1[Mechanic]', 'You do not have permission to access this menu.' } })
    end
end, false)

RegisterNetEvent('mechanic:repairEngine')
AddEventHandler('mechanic:repairEngine', function(vehicleNetId)
    local src = source
    TriggerClientEvent('mechanic:performRepair', src, vehicleNetId)
end)

RegisterNetEvent('mechanic:changeTires')
AddEventHandler('mechanic:changeTires', function(vehicleNetId)
    local src = source
    TriggerClientEvent('mechanic:performRepair', src, vehicleNetId)
end)

RegisterNetEvent('mechanic:uiClosed')
AddEventHandler('mechanic:uiClosed', function()
end)
