RegisterServerEvent('drugalyser.server:doBacTest')
AddEventHandler('drugalyser.server:doBacTest', function(target)
    TriggerClientEvent('drugalyser.client:requestBac', target, source, target)
end)

RegisterServerEvent('drugalyser.server:returnBac')
AddEventHandler('drugalyser.server:returnBac', function(bac, leo)
    TriggerClientEvent('drugalyser.client:displayBac', leo, bac, '--color-black')
end)

RegisterServerEvent('drugalyser.server:refusedBac')
AddEventHandler('drugalyser.server:refusedBac', function(leo, target)
    TriggerClientEvent('drugalyser.client:bacRefused', leo, target)
end)

RegisterServerEvent('drugalyser.server:acceptedBac')
AddEventHandler('drugalyser.server:acceptedBac', function(leo, target)
    TriggerClientEvent('drugalyser.client:acceptedBac', leo, target)
end)