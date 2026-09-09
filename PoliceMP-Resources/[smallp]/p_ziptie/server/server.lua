RegisterServerEvent("ZIPTIE:SZIPBACK")
AddEventHandler("ZIPTIE:SZIPBACK", function(player)
    TriggerClientEvent('ZIPTIE:PLAYANIM', source)
    TriggerClientEvent("ZIPTIE:CZIPBACK", player)
    TriggerClientEvent('ZIPTIE:PLAYSOUNDD', source, 'zip', config.sound_vol)
    TriggerClientEvent('ZIPTIE:PLAYSOUNDD', player, 'zip', config.sound_vol)
end)

RegisterServerEvent("ZIPTIE:FREEDAUSER")
AddEventHandler("ZIPTIE:FREEDAUSER", function(player)
    TriggerClientEvent('ZIPTIE:PLAYANIM', source)
    TriggerClientEvent('ZIPTIE:PLAYSOUNDD', source, 'unzip', config.sound_vol)    
    TriggerClientEvent('ZIPTIE:PLAYSOUNDD', player, 'unzip', config.sound_vol)
    TriggerClientEvent("ZIPTIE:BEFREEWEIRDO", player)
end)

RegisterServerEvent("ZIPTIE:SZIPFRONT")
AddEventHandler("ZIPTIE:SZIPFRONT", function(player)
    TriggerClientEvent('ZIPTIE:PLAYANIM', source)
    TriggerClientEvent("ZIPTIE:CZIPFRONT", player)
    TriggerClientEvent('ZIPTIE:PLAYSOUNDD', source, 'zip', config.sound_vol)
    TriggerClientEvent('ZIPTIE:PLAYSOUNDD', player, 'zip', config.sound_vol)
end)

RegisterCommand("zip", function(source)
    if config.enable_perms and not IsPlayerAceAllowed(source, config.ace_perm) then
        TriggerClientEvent('ZIPTIE:NOTIFY', source, "~r~Access Denied")
    else
        TriggerClientEvent("ZIPTIE:BACKZIP", source)
    end
end)

RegisterCommand("unzip", function(source)
    if config.enable_perms and not IsPlayerAceAllowed(source, config.ace_perm) then
        TriggerClientEvent('ZIPTIE:NOTIFY', source, "~r~Access Denied")
    else
        TriggerClientEvent("ZIPTIE:UNZIPUSER", source)
    end
end)

RegisterCommand("+zfrontcuff", function(source)
    if config.enable_perms and not IsPlayerAceAllowed(source, config.ace_permpc) then
        TriggerClientEvent('ZIPTIE:NOTIFY', source, "~r~Access Denied")
    else
        TriggerClientEvent("ZIPTIE:FRONTZIP", source)
    end
end)

RegisterCommand("+zfrontuncuff", function(source)
    if config.enable_perms and not IsPlayerAceAllowed(source, config.ace_permpc) then
        TriggerClientEvent('ZIPTIE:NOTIFY', source, "~r~Access Denied")
    else
        TriggerClientEvent("ZIPTIE:UNZIPUSER", source)
    end
end)
