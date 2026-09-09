RegisterServerEvent("helmetvision:checkPermission")
AddEventHandler("helmetvision:checkPermission", function()
    local src = source
    if IsPlayerAceAllowed(src, "group.dev") then
        TriggerClientEvent("helmetvision:setAuthorized", src, true)
    else
        TriggerClientEvent("helmetvision:setAuthorized", src, false)
    end
end)
