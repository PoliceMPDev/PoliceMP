RegisterServerEvent('checkMountPermission')
AddEventHandler('checkMountPermission', function()
    local src = source
    local player = src

    if IsPlayerAceAllowed(player, "Police.afoTrained") then
        TriggerClientEvent('proceedWithMount', player)        
    end
end)


RegisterServerEvent('syncDbSitting')
AddEventHandler('syncDbSitting', function(vehicleNetId, offsetX, offsetY, offsetZ)
    local src = source
    TriggerClientEvent('syncDbSittingClient', -1, src, vehicleNetId, offsetX, offsetY, offsetZ)
end)

RegisterServerEvent('syncDbDetach')
AddEventHandler('syncDbDetach', function()
    local src = source
    TriggerClientEvent('syncDbDetachClient', -1, src)
end)
