RegisterCommand('snpc', function(source)
    local player = source
    if player == 0 then
        return
    end

    if not IsPlayerAceAllowed(player, "group.TierTwo") then
        TriggerClientEvent('networked_npcs:notify', player, '~r~Developers Only!')
        return
    end

    local playerPed = GetPlayerPed(player)
    local playerCoords = GetEntityCoords(playerPed)

    TriggerClientEvent('networked_npcs:spawn', source, playerCoords)
end, false)

RegisterCommand('cnpc', function(source)
    local player = source
    if player == 0 then
        return
    end

    if not IsPlayerAceAllowed(player, "group.TierTwo") then
        TriggerClientEvent('networked_npcs:notify', player, '~r~Developers Only!')
        return
    end

    TriggerClientEvent('networked_npcs:clear', -1)
end, false)

RegisterCommand('dnpc', function(source)
    local player = source
    if player == 0 then
        return
    end

    if not IsPlayerAceAllowed(player, "group.TierTwo") then
        TriggerClientEvent('networked_npcs:notify', player, '~r~Developers Only!')
        return
    end

    local playerPed = GetPlayerPed(player)
    local playerCoords = GetEntityCoords(playerPed)

    TriggerClientEvent('networked_npcs:spawnPartying', source, playerCoords)
end, false)

RegisterCommand('pnpc', function(source)
    local player = source
    if player == 0 then
        return
    end

    if not IsPlayerAceAllowed(player, "group.TierTwo") then
        TriggerClientEvent('networked_npcs:notify', player, '~r~Developers Only!')
        return
    end

    local playerPed = GetPlayerPed(player)
    local playerCoords = GetEntityCoords(playerPed)

    TriggerClientEvent('networked_npcs:spawnAndEmote', source, playerCoords, "protest")
end, false)

RegisterCommand('sitnpc', function(source)
    local player = source
    if player == 0 then
        return
    end

    if not IsPlayerAceAllowed(player, "group.TierTwo") then
        TriggerClientEvent('networked_npcs:notify', player, '~r~Developers Only!')
        return
    end

    local playerPed = GetPlayerPed(player)
    local playerCoords = GetEntityCoords(playerPed)

    TriggerClientEvent('networked_npcs:spawnAndSitAnim', source, playerCoords)
end, false)

RegisterCommand('courtnpc', function(source)
    local player = source
    if player == 0 then
        return
    end

    if not IsPlayerAceAllowed(player, "group.TierTwo") then
        TriggerClientEvent('networked_npcs:notify', player, '~r~Developers Only!')
        return
    end

    local playerPed = GetPlayerPed(player)
    local playerCoords = GetEntityCoords(playerPed)

    TriggerClientEvent('networked_npcs:courtspawn', source, playerCoords)
end, false)