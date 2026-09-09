RegisterServerEvent('checkParamedicPermission')
AddEventHandler('checkParamedicPermission', function(playerId, coords_spawn)
    if IsPlayerAceAllowed(playerId, "nhs.paramedic") or IsPlayerAceAllowed(playerId, "nhs.lasStudent") then
        TriggerClientEvent('spawnStretcher', playerId, coords_spawn)
    end
end)

RegisterCommand("+spstretcher", function(source, args, rawCommand)
    local src = source
    if not IsPlayerAceAllowed(src, "nhs.hems") then
        return
    end

    if src > 0 then
        local playerPed = GetPlayerPed(src)
        local playerCoords = GetEntityCoords(playerPed)
        TriggerClientEvent("spawnStretcher", src, playerCoords)
    end
end, false)

RegisterServerEvent("validateAndDeleteBodyBag")
AddEventHandler("validateAndDeleteBodyBag", function(propCoords)
    local src = source

    if IsPlayerAceAllowed(src, "nhs.paramedic") or IsPlayerAceAllowed(src, "nhs.lasStudent") then
        TriggerClientEvent("deleteBodyBag", src, propCoords)
        TriggerClientEvent("XNL_NET:AddPlayerXP", src, 15)
    else
        TriggerClientEvent("permissionDenied", src)
    end
end)
