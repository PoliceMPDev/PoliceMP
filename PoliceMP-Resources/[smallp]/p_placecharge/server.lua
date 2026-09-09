local playerBombs = {}

RegisterServerEvent("bomb:syncPlace")
AddEventHandler("bomb:syncPlace", function(pos, heading, entityHit)
    local src = source

    if playerBombs[src] then
        TriggerClientEvent("bomb:notify", src, "~r~ERROR: ~s~You already placed a plastic explosive!")
        return
    end

    playerBombs[src] = {
        pos = pos,
        heading = heading,
        entityHit = entityHit
    }

    -- Only tell this player to spawn their bomb
    TriggerClientEvent("bomb:clientPlace", src, pos, heading, entityHit)
end)

RegisterServerEvent("bomb:syncDetonate")
AddEventHandler("bomb:syncDetonate", function(sourceId)
    local src = sourceId or source
    if not playerBombs[src] then return end

    TriggerClientEvent("bomb:clientDetonate", src)
    playerBombs[src] = nil
end)

RegisterCommand("placecharge", function(src)
    if not IsPlayerAceAllowed(src, "Police.afoTrained") then
        TriggerClientEvent("bomb:notify", src, "~r~ERROR: ~s~Nice Try! Genius.")
        return
    end
    TriggerClientEvent("bomb:requestPlacement", src)
end, false)

RegisterCommand("triggercharge", function(src)
    if not IsPlayerAceAllowed(src, "Police.afoTrained") then
        TriggerClientEvent("bomb:notify", src, "~r~ERROR: ~s~Nice Try! Genius.")
        return
    end
    TriggerEvent("bomb:syncDetonate", src)
end, false)

