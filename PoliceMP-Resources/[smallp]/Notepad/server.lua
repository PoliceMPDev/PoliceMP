RegisterServerEvent('deleteProps')
AddEventHandler('deleteProps', function(entityId, propHash)
    TriggerClientEvent('syncDeleteProps', -1, entityId, propHash)
end)
