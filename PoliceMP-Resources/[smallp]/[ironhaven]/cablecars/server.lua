RegisterServerEvent("pmp:cablecar:host:sync")
AddEventHandler("pmp:cablecar:host:sync", function(index, state)
    if source == tonumber(GetPlayers()[1]) then
        TriggerClientEvent("pmp:cablecar:forceState", -1, index, state)
    end
end)
