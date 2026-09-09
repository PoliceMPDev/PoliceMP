local SafeZonesState = {
    ["bishopgate"] = false,
    ["sandy"] = false,
    ["paleto"] = false
}

RegisterCommand("safezone", function(source, args)
    if not IsPlayerAceAllowed(source, "group.dev") then
        TriggerClientEvent("chat:addMessage", source, {
            args = { "[System]", "You do not have permission to use this command." }
        })
        return
    end

    local zone = args[1] and args[1]:lower()
    if not zone or SafeZonesState[zone] == nil then
        TriggerClientEvent("chat:addMessage", source, {
            args = { "[System]", "Invalid zone name. Use: bishopgate, sandy, paleto" }
        })
        return
    end

    SafeZonesState[zone] = not SafeZonesState[zone]
    TriggerClientEvent("zombies:updateSafeZoneState", -1, zone, SafeZonesState[zone])

    local status = SafeZonesState[zone] and "ENABLED" or "DISABLED"
    TriggerClientEvent("chat:addMessage", -1, {
        args = { "[System]", ("Safe Zone '%s' is now %s."):format(zone, status) }
    })
end)

RegisterCommand("zombies", function(source, args)
    if not IsPlayerAceAllowed(source, "Police.modAuth") then
        TriggerClientEvent("chat:addMessage", source, {
            args = { "[Ambient]", "You do not have permission to use this command." }
        })
        return
    end

    local count = tonumber(args[1]) or 5
    TriggerClientEvent("zombies:spawnAmbientPeds", source, count)
end, false)

RegisterCommand("zalert", function(source, args)
    if not IsPlayerAceAllowed(source, "group.dev") then
        TriggerClientEvent("chat:addMessage", source, {
            args = { "[System]", "You do not have permission to use this command." }
        })
        return
    end

    TriggerClientEvent("zombies:alertAll", -1)
end, false)
