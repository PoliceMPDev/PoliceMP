RegisterServerEvent('helicopter:toggleDoors')
AddEventHandler('helicopter:toggleDoors', function(vehicleId, state)
    -- Trigger the client event to sync door states
    TriggerClientEvent('helicopter:syncToggleDoors', -1, vehicleId, state)
end)
