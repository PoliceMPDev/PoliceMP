-- Server-side event to sync animation
RegisterNetEvent("syncMarkerAnimation")
AddEventHandler("syncMarkerAnimation", function()
    local src = source
    TriggerClientEvent("playMarkerAnimation", src)
end)