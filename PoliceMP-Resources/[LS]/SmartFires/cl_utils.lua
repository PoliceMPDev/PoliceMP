RegisterNetEvent("Client:fireNotify")
AddEventHandler("Client:fireNotify", function(message)
    showNotification(message)
end)

function showNotification(message)
    if main.useMythicNotify then
        exports['mythic_notify']:DoHudText('inform', message)
    else
        SetNotificationTextEntry("STRING")
        AddTextComponentString(message)
        DrawNotification(0,1)
    end
end