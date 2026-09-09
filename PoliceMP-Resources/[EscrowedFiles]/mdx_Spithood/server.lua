-- Event to add spit hood prop to the specified player
RegisterServerEvent("placeSpitHood")
AddEventHandler("placeSpitHood", function(targetPlayerId)
    local sourcePlayerId = source
    -- Trigger the event for the specified player
    TriggerClientEvent("addSpitHood", targetPlayerId)
end)

-- Event to remove Spit Hood prop from the specified player
RegisterServerEvent("removeSpitHood")
AddEventHandler("removeSpitHood", function(targetPlayerId)
    local sourcePlayerId = source
    TriggerClientEvent("removeSpitHood", targetPlayerId) -- Broadcast to all clients to remove spit hood
end)