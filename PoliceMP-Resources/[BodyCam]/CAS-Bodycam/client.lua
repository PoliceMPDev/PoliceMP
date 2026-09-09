RegisterNetEvent("cas-bodycam:action", function(k, v)
    if not k or not v then return end

    if k == "bodycam" then
        SendNUIMessage({
            action = "bodycam",
            name = v.name,
            grade = "", -- grade removed
            desc = CAS.recordDesc,
            header = CAS.recordName,
            webhook = CAS.webhook
        })
    elseif k == "records" then
        SendNUIMessage({
            action = "records",
            infos = v,
            header = CAS.Header,
            footer = CAS.Footer,
        })
        SetNuiFocus(true, true)
    end
end)

RegisterNUICallback("getVideoURL", function(data, cb)
    TriggerServerEvent("sendFileData", data.videoURL, data.videoName, data.videoDesc)
    cb("ok")
end)

RegisterNUICallback("escapeFromNUI", function(_, cb)
    SetNuiFocus(false, false)
    cb("ok")
end)

RegisterNetEvent("cas-bodycam:notify", function(msg)
    SetNotificationTextEntry("STRING")
    AddTextComponentString(msg)
    DrawNotification(false, false)
end)
