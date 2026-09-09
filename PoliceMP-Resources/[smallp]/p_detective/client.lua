local isInvestigating = false
local uiOpen = false

RegisterCommand("+xinvestigate", function()
    if not isInvestigating then
        isInvestigating = true
        SendNUIMessage({ action = "openUI", question = "What was the call about?" })
        SetNuiFocus(true, true)
        uiOpen = true
    end
end, false)

RegisterNUICallback("submitQuestion", function(data, cb)
    local question = data.text
    if question and question ~= "" then
        TriggerServerEvent("detective:submitResponse", question)
    end
    cb("ok")
end)

RegisterNetEvent("detective:receiveResponse")
AddEventHandler("detective:receiveResponse", function(response)
    SendNUIMessage({ action = "receiveResponse", response = response })
end)

RegisterNUICallback("closeUI", function(data, cb)
    SetNuiFocus(false, false)
    isInvestigating = false
    uiOpen = false
    cb("ok")
end)

