local initiatorForTarget = {} 

RegisterServerEvent("lantern:showNUI")
AddEventHandler("lantern:showNUI", function(targetPlayerId)
    local initiator = source
    initiatorForTarget[targetPlayerId] = initiator 
    TriggerClientEvent("lantern:openUI", targetPlayerId) 
    print(string.format("[Lantern] Tracking Initiator %d for Target %d", initiator, targetPlayerId))
end)

RegisterServerEvent("lantern:submitDetails")
AddEventHandler("lantern:submitDetails", function(data)
    local responder = source
    local initiator = initiatorForTarget[responder] 

    if initiator then
        TriggerClientEvent("chat:addMessage", initiator, {
            args = {
                "Lantern", 
                string.format("First Name: ~r~%s\n~w~Last Name: ~r~%s\n~w~DOB: ~r~%s", data.firstName, data.lastName, data.dob)
            }
        })

        initiatorForTarget[responder] = nil
    else
        -- print(string.format("[Lantern] Error: No Initiator found for Responder %d", responder)) -- Debugging
    end
end)
