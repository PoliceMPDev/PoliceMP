table.insert(Cfg.voiceModes, {35.0, 'Megaphone'}) -- Here you can change the distance of megaphone

exports('setMegaphone', function(bool, value)
    if bool then
        mode = #Cfg.voiceModes
        setProximityState(Cfg.voiceModes[#Cfg.voiceModes][1], true)
        TriggerEvent('pma-voice:setTalkingMode', #Cfg.voiceModes)
    else
        mode = value
        setProximityState(Cfg.voiceModes[value][1], false)
        TriggerEvent('pma-voice:setTalkingMode', value)
    end
end)

exports('getMegaphone', function()
    return mode
end)

RegisterCommand('cycleproximity', function()
    -- Proximity is either disabled, or manually overwritten.
    if GetConvarInt('voice_enableProximityCycle', 1) ~= 1 or disableProximityCycle then return end
    local newMode = mode + 1

    -- If we're within the range of our voice modes, allow the increase, otherwise reset to the first state
    if newMode <= #Cfg.voiceModes and newMode ~= #Cfg.voiceModes-1 then
        mode = newMode
    else
        mode = 1
    end

    setProximityState(Cfg.voiceModes[mode][1], false)
    TriggerEvent('pma-voice:setTalkingMode', mode)
end, false)
if gameVersion == 'fivem' then
    RegisterKeyMapping('cycleproximity', 'Cycle Proximity', 'keyboard', GetConvar('voice_defaultCycle', 'F11'))
end