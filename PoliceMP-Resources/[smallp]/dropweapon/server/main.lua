RegisterServerEvent('dropWeapon:syncDrop')
AddEventHandler('dropWeapon:syncDrop', function(weaponHash, dropPosition, propName)
    local src = source
    TriggerClientEvent('dropWeapon:syncDropToClients', src, weaponHash, dropPosition, propName)
end)

RegisterServerEvent('dropWeapon:playAnimation')
AddEventHandler('dropWeapon:playAnimation', function(pedCoords, weaponHash, propName)
    local src = source
    TriggerClientEvent('dropWeapon:playAnimationOnClients', src, pedCoords, weaponHash, propName)
end)

RegisterServerEvent('dropWeapon:requestDeleteNearbyProps')
AddEventHandler('dropWeapon:requestDeleteNearbyProps', function(radius)
    local src = source
    local pedCoords = GetEntityCoords(GetPlayerPed(src))
    TriggerClientEvent('dropWeapon:checkAndDeleteNearbyProps', -1, pedCoords, radius)
end)
