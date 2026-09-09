RegisterServerEvent('checkCTMountPermission')
AddEventHandler('checkCTMountPermission', function()
    local src = source
    local player = src

    if IsPlayerAceAllowed(player, "Police.afoTrained") then
        TriggerClientEvent('proceedWithCTMount', player)        
    end
end)

RegisterServerEvent('syncDbCTSitting')
AddEventHandler('syncDbCTSitting', function(vehicleNetId, offsetX, offsetY, offsetZ)
    local src = source
    TriggerClientEvent('syncDbCTSittingClient', -1, src, vehicleNetId, offsetX, offsetY, offsetZ)
end)

RegisterServerEvent('syncDbCTDetach')
AddEventHandler('syncDbCTDetach', function()
    local src = source
    TriggerClientEvent('syncDbCTDetachClient', -1, src)
end)
