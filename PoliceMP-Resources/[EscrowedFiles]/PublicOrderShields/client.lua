ShieldEntity = nil

RegisterCommand(Config.CommandName, function(pSource, pArguments)
    if pArguments[1] == nil then 
        if ShieldEntity ~= nil then 
            ClearShieldActivity() 
        end
        return
    end

    local shieldType = pArguments[1]
    if shieldType == nil or Config.ShieldTypes[shieldType] == nil then return Debug('Shield (' .. shieldType .. ') does not exist in Config.ShieldTypes.') end

    PlayShieldAnimation(Config.ShieldTypes[shieldType])
end)

function PlayShieldAnimation(pShieldType)
    local PlayerPed = GetPlayerPed(-1)

    if ShieldEntity ~= nil then ClearShieldActivity() end

    LoadAnimation(pShieldType.AnimDict, true)

    TaskPlayAnim(PlayerPed, pShieldType.AnimDict, pShieldType.AnimName, 2.0, 2.0, -1, 51, 0, false, false, false)

    AttachShieldToPlayer(pShieldType.PropLocation)

    LoadAnimation(pShieldType.AnimDict, false)
end

AddEventHandler('onResourceStop', function(resourceName)
    if (GetCurrentResourceName() ~= resourceName) then return end
    if ShieldEntity ~= nil then ClearShieldActivity() end
end)
