HookEventHandler(ENUM_HOOKABLE_EVENTS.PLAYER_RESPAWNED, function()
    -- Put here your stuff like cd_playerhud:ResetStatus
    TriggerServerEvent("cd_playerhud:ResetStatus")
end)

HookEventHandler(ENUM_HOOKABLE_EVENTS.UNCONSCIOUS_STATE_CHANGED, function(newState)
    if newState then -- Player is unconscious now
        ShowNotification("You are unconscious!")
    else -- Player is conscious now
        ShowNotification("LHS has brought you around.")
        ExecuteCommand("e passout4")

    end

    ResetThirstHungerStatus()
end)

function dump(o)
    if type(o) == 'table' then
       local s = '{ '
       for k,v in pairs(o) do
          if type(k) ~= 'number' then k = '"'..k..'"' end
          s = s .. '['..k..'] = ' .. dump(v) .. ','
       end
       return s .. '} '
    else
       return tostring(o)
    end
 end

-- HookEventHandler(ENUM_HOOKABLE_EVENTS.DAMAGE_RECEIVED, function(damageType, damageAmount, bodyPart)
--     ShowNotification(dump(damageType))
--     ShowNotification(damageAmount)
--     ShowNotification(bodyPart)
-- end)





-- HookEventHandler(ENUM_EVENT_TYPES.EVENT_APPLY_DEFIBRILLATOR, function()

--     ExecuteCommand ("e medic")
--     Citizen.Wait(5000)
--     -- wait(5000)
--     ExecuteCommand("e defibrillator")



-- end)

---Resets thirst hunger status for esx and qbcore
function ResetThirstHungerStatus()
    TriggerEvent("cd_playerhud:status:set", "hunger", 10)
    TriggerEvent("cd_playerhud:status:set", "thirst", 10)

    TriggerEvent('esx_status:add', 'hunger', 100000)
	TriggerEvent('esx_status:add', 'thirst', 100000)

    TriggerServerEvent("QBCore:Server:SetMetaData", "hunger", 10)
    TriggerServerEvent("QBCore:Server:SetMetaData", "thirst", 10)

    TriggerServerEvent("esx_ambulancejob:setDeathStatus", false)
end
