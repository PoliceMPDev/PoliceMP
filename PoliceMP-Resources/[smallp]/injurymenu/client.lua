local recentInjury = {
    bodyPart = "HEAD",
    severity = "minor",
    injuryType = "heart_attack"
}

RegisterCommand("+cxinjury", function()
    SetNuiFocus(true, true)  
    SendNUIMessage({
        action = "open",
        recentInjury = recentInjury
    })
end)

RegisterNUICallback("submitInjury", function(data, cb)
    local injuryType = data.injuryType
    local bodyPart = data.bodyPart
    local severity = data.severity

    if injuryType and bodyPart and severity then
        recentInjury = { bodyPart = bodyPart, severity = severity, injuryType = injuryType }

        ExecuteCommand(string.format("addinjury %s %s %s", injuryType, bodyPart, severity))
    end

    SendNUIMessage({
        action = "updateRecentInjury",
        recentInjury = recentInjury
    })
    cb("ok")
end)

RegisterNUICallback("exitNUI", function(_, cb)
    SetNuiFocus(false, false)  
    SendNUIMessage({
        action = "close"
    })
    cb("ok")
end)

RegisterCommand("+xlescape", function()
    SetNuiFocus(false, false)  
    SendNUIMessage({
        action = "close"
    })
end, false)

RegisterNUICallback("closeNUI", function(data, cb)
    SetNuiFocus(false, false)  
    cb("ok")
end)

AddEventHandler('onResourceStop', function(resourceName)
    if resourceName == GetCurrentResourceName() then
        SetNuiFocus(false, false)  
    end
end)

RegisterNUICallback("rtcsevere", function(_, cb)
    SetNuiFocus(false, false)
    SendNUIMessage({ action = "close" })

    Citizen.Wait(300) -- Small delay before execution
    TriggerEvent("rtcsevere") -- Calls the new optimized event

    cb("ok")
end)

RegisterNUICallback("rtcminor", function(_, cb)
    SetNuiFocus(false, false)
    SendNUIMessage({ action = "close" })

    Citizen.Wait(300) -- Small delay before execution
    TriggerEvent("rtcminor") -- Calls the new optimized event

    cb("ok")
end)

RegisterNUICallback("gswsevere", function(_, cb)
    SetNuiFocus(false, false)
    SendNUIMessage({ action = "close" })

    Citizen.Wait(300) -- Small delay before execution
    TriggerEvent("gswsevere") -- Calls the new optimized event

    cb("ok")
end)

RegisterNUICallback("gswminor", function(_, cb)
    SetNuiFocus(false, false)
    SendNUIMessage({ action = "close" })

    Citizen.Wait(300) -- Small delay before execution
    TriggerEvent("gswminor") -- Calls the new optimized event

    cb("ok")
end)

RegisterNUICallback("firesevere", function(_, cb)
    SetNuiFocus(false, false)
    SendNUIMessage({ action = "close" })

    Citizen.Wait(300) -- Small delay before execution
    TriggerEvent("firesevere") -- Calls the new optimized event

    cb("ok")
end)

RegisterNUICallback("fireminor", function(_, cb)
    SetNuiFocus(false, false)
    SendNUIMessage({ action = "close" })

    Citizen.Wait(300) -- Small delay before execution
    TriggerEvent("fireminor") -- Calls the new optimized event

    cb("ok")
end)

RegisterNUICallback("fallsevere", function(_, cb)
    SetNuiFocus(false, false)
    SendNUIMessage({ action = "close" })

    Citizen.Wait(300) -- Small delay before execution
    TriggerEvent("fallsevere") -- Calls the new optimized event

    cb("ok")
end)

RegisterNUICallback("fallminor", function(_, cb)
    SetNuiFocus(false, false)
    SendNUIMessage({ action = "close" })

    Citizen.Wait(300) -- Small delay before execution
    TriggerEvent("fallminor") -- Calls the new optimized event

    cb("ok")
end)

RegisterNUICallback("iwf", function(_, cb)
    SetNuiFocus(false, false)
    SendNUIMessage({ action = "close" })

    Citizen.Wait(300)

    local parts = { "TORSO", "HEAD" }
    local randomPart = parts[math.random(#parts)]
    local severity = "large"
    local injuryType = "iwf"

    ExecuteCommand(string.format("addinjury %s %s %s", injuryType, randomPart, severity))

    cb("ok")
end)
