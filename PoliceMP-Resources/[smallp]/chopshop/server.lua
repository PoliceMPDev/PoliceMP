RegisterCommand('ch', function(source, args, rawCommand)
    if IsPlayerAceAllowed(source, "Civ.Trained") then
        TriggerClientEvent('chopShop:startChop', source)
    else
        TriggerClientEvent('chopShop:notify', source, 'You do not have permission to use this command.')
    end
end, false)

RegisterNetEvent('chopShop:applyDamage')
AddEventHandler('chopShop:applyDamage', function(vehicleNetId, part)
    
    TriggerClientEvent('chopShop:applyDamageToVehicle', -1, vehicleNetId, part)
end)
