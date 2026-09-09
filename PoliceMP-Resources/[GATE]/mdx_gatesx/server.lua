RegisterServerEvent('mdx_Station_Gates:PlayAnimation')
AddEventHandler('mdx_Station_Gates:PlayAnimation', function(stationName, gate)
    -- Send the event to all clients to play the animation for the specified gate
    TriggerClientEvent('mdx_Station_Gates:PlayAnimation', -1, stationName, gate)
end)