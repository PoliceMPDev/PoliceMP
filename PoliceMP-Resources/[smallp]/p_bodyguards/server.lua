RegisterCommand("guards", function(source, args, rawCommand)
    if not IsPlayerAceAllowed(source, "group.dev") and not IsPlayerAceAllowed(source, "Civ.dir") then
        TriggerClientEvent("chat:addMessage", source, {
            args = { "^1You do not have permission to use this command." }
        })
        return
    end

    TriggerClientEvent("bodyguard:spawnGuards", source)
end, false)

RegisterCommand("clearguards", function(source)
    if not IsPlayerAceAllowed(source, "group.dev") and not IsPlayerAceAllowed(source, "Civ.dir") then
        TriggerClientEvent("chat:addMessage", source, {
            args = { "^1You do not have permission to use this command." }
        })
        return
    end

    TriggerClientEvent("bodyguard:clearGuards", source)
end, false)
