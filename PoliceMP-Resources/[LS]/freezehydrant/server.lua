-- Notify all clients to freeze specified hydrant objects
RegisterServerEvent('freezeHydrants')
AddEventHandler('freezeHydrants', function()
    TriggerClientEvent('client:freezeHydrants', -1) -- Sends to all clients
end)

-- Trigger freeze on resource start
AddEventHandler('onResourceStart', function(resourceName)
    if GetCurrentResourceName() == resourceName then
        TriggerEvent('freezeHydrants')
    end
end)
