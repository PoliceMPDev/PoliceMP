local isInvestigating = false
local uiOpen = false

RegisterCommand("poltablet", function()
    if not isInvestigating then
        isInvestigating = true
        SendNUIMessage({ action = "openUI", question = "What was the original call?" }) 
        SetNuiFocus(true, true)
        uiOpen = true

        ExecuteCommand("e tablet2")

    end
end, false)

RegisterNUICallback("submitQuestion", function(data, cb)
    local question = data.text
    if question and question ~= "" then
        TriggerServerEvent("poltablet:submitResponse", question)
    end
    cb("ok")
end)

RegisterNetEvent("poltablet:receiveResponse")
AddEventHandler("poltablet:receiveResponse", function(response)
    SendNUIMessage({ action = "receiveResponse", response = response })
end)

RegisterNUICallback("closeUI", function(data, cb)
    SetNuiFocus(false, false)
    isInvestigating = false
    uiOpen = false
    cb("ok")

    ExecuteCommand("emotecancel")
end)

RegisterNUICallback("resetInvestigation", function(data, cb)
    TriggerServerEvent("poltablet:resetInvestigation")
    cb("ok")
end)
