-- Existing TCAS alert event
RegisterServerEvent('tcas:sendAlert')
AddEventHandler('tcas:sendAlert', function(targetPlayer)
    if Config.Debug then
        print(string.format("TCAS alert sent to player %d", targetPlayer))
    end
    TriggerClientEvent('tcas:receiveAlert', targetPlayer)
end)

-- Existing Clear of Conflict event
RegisterServerEvent('tcas:clearConflict')
AddEventHandler('tcas:clearConflict', function(targetPlayer)
    if Config.Debug then
        print(string.format("Clear of conflict sent to player %d", targetPlayer))
    end
    TriggerClientEvent('tcas:receiveClearConflict', targetPlayer)
end)

-- New Postal Alert event
RegisterServerEvent('tcas:sendPostalAlert')
AddEventHandler('tcas:sendPostalAlert', function(targetPlayer, postal)
    if Config.Debug then
        print(string.format("Postal alert sent to player %d for postal %s", targetPlayer, postal))
    end
    TriggerClientEvent('tcas:receivePostalAlert', targetPlayer, postal)
end)
