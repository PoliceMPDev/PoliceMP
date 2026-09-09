RegisterNetEvent('moe:checkAcePermission', function()
    local src = source
    local allowed = IsPlayerAceAllowed(src, 'group.TierTwo')
    TriggerClientEvent('moe:returnAcePermission', src, allowed)
end)
