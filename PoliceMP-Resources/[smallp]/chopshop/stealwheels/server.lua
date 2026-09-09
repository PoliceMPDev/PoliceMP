RegisterCommand('sw', function(source, args, rawCommand)
    -- Check if the player has the necessary ACE permission
    if not IsPlayerAceAllowed(source, "Civ.Trained") then
        TriggerClientEvent('Notify', source, 'You do not have permission to use this command.')
        return
    end

    TriggerClientEvent('stealwheels:startBreaking', source)
end, false)

RegisterNetEvent('stealwheels:applyDamage')
AddEventHandler('stealwheels:applyDamage', function(vehicleNetId, part)

    TriggerClientEvent('stealwheels:applyDamageToVehicle', -1, vehicleNetId, part)
end)
