RegisterNetEvent('smallp_executeDJmenu', function(input_one, input_two, input_three)
    local source = source
    local tokenizer = Config.TokenizerToken
    -- Broadcast the play music event to all clients
    TriggerClientEvent('smallp_play_music', -1, input_one, input_two, input_three, tokenizer)
end)

RegisterNetEvent('smallp_destroyMusic', function(djName)
    if not djName then return end  -- Prevent errors if no DJ name is provided
    
    -- Broadcast only to players near the DJ panel
    for _, playerId in ipairs(GetPlayers()) do
        TriggerClientEvent('smallp_destoryMusic', playerId, djName)
    end
end)

RegisterNetEvent('smallp_checkPermission', function()
    local src = source
    local allowed = IsPlayerAceAllowed(src, "group.TierTwo")
    TriggerClientEvent('smallp_returnPermission', src, allowed)
end)

RegisterNetEvent('smallp_startVehicleMusic', function(url, volume, range, vehicleNetId)
    local tokenizer = Config.TokenizerToken
    TriggerClientEvent('smallp_playVehicleMusic', -1, url, volume, range, vehicleNetId, tokenizer)
end)
