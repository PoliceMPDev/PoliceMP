RegisterCommand("devsv", function(source, args, rawCommand)
    if not IsPlayerAceAllowed(source, "group.TierTwo") then
        TriggerClientEvent("chat:addMessage", source, {
            args = { "^1[ERROR]^0 You do not have permission to use this command." }
        })
        return
    end

    TriggerClientEvent("dev:spawnVehicle", source, args[1])
end, false)
