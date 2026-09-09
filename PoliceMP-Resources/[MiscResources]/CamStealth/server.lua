
local steeringAngles = {}
RegisterNetEvent("wheelSync:updateSteering", function(vehNetId, angle)
    steeringAngles[vehNetId] = angle
    TriggerClientEvent("wheelSync:applySteering", -1, vehNetId, angle)
end)

AddEventHandler("entityRemoved", function(entity)
    if DoesEntityExist(entity) and IsEntityAVehicle(entity) then
        local netId = NetworkGetNetworkIdFromEntity(entity)
        steeringAngles[netId] = nil
    end
end)

RegisterNetEvent("aimcontrol:checkPermission")
AddEventHandler("aimcontrol:checkPermission", function()
    local src = source
    local hasPerm = IsPlayerAceAllowed(src, "group.dev")
    TriggerClientEvent("aimcontrol:receivePermission", src, hasPerm)
end)
