function hasPermission(source, permission)
    return IsPlayerAceAllowed(source, permission)
end

RegisterNetEvent('weaponMenu:checkPermission')
AddEventHandler('weaponMenu:checkPermission', function()
    local _source = source
    if hasPermission(_source, 'group.TierTwo') or hasPermission(_source, 'group.devFunNight') then
        TriggerClientEvent('weaponMenu:showMenu', _source)
    else
        TriggerClientEvent('weaponMenu:noPermission', _source)
    end
end)
