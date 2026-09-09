util = { }

-- @param: Print a error message in the console.
util.error = function(filePath, line, reason)
    print('^1SCRIPT ERROR: @' .. GetCurrentResourceName() .. filePath .. ':' .. line .. ': ' .. reason)
end

-- @param: Print a success message in the console.
util.success = function(filePath, line, reason)
    print('^2SCRIPT SUCCESS: @' .. GetCurrentResourceName() .. filePath .. ':' .. line .. ': ' .. reason)
end

-- @param: Add an entity to the table.
util.addToTable = function(entityid)
    if ( entityid <= 0 or entityid == nil ) then 
        return
    end
    table.insert(world.objects, entityid)
end

-- @param: Load and play an animation.
util.animation = function(dict, anim, wait, int)
    RequestAnimDict(dict)
    while not HasAnimDictLoaded(dict) do
       Wait(0)
    end
 
    TaskPlayAnim(GetPlayerPed(PlayerId()), dict, anim, 8.0, 8.0, int, 51, 0, false, false, false)
    Wait(wait)
    StopAnimTask(GetPlayerPed(PlayerId()), dict, anim, 1.5)
end

-- @param: Send a notification to the player.
-- @param: The argument 'type' will return one of two strings: 'error' or 'success.' This value can be used for customizing your notifications.
util.notification = function(string, type)
    if ( cfg.notificationSound ) then 
        PlaySoundFrontend(GetSoundId(), 'Text_Arrive_Tone', 'Phone_SoundSet_Default', true)
    end
    SetNotificationTextEntry('STRING')
    AddTextComponentString(string)
    SetNotificationBackgroundColor(140)
    -- SetNotificationMessage('CHAR_WE', 'logo', true, 4, 'zCustody-Alarm', 'By Zea Development')
    DrawNotification(false, true)
end

-- @param: Load a model.
util.load = function(hash)
    if ( not HasModelLoaded(hash) ) then
        RequestModel(hash)
        while not HasModelLoaded(hash) do
           Citizen.Wait(0)
        end
    end
    return ( true )
end

-- @param: Notify clients when an alarm is activated
util.notifyUponActivation = function(stationName, stationAddress)
    if ( not cfg.createCad ) then 
        if ( policeDuty ) then 
            PlaySoundFrontend(GetSoundId(), '10_SEC_WARNING', 'HUD_MINI_GAME_SOUNDSET', true)
            TriggerServerEvent('chat:addMessage', {
                color = { 255, 0, 0},
                multiline = true,
                args = {'Control Room', '\nNew Incident: Custody Alarm Activated \n Address: ' .. stationName .. ' : ' .. stationAddress}
            })
        end
    else
        TriggerServerEvent("zCustody-Alarm:createCad", { 
            type = 'Police - Custody Alarm Activation',
            description = 'Custody Alarm Activation at ' .. stationName,
            location = stationAddress
        })
    end
end