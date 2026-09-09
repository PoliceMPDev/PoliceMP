RegisterNetEvent("playerJoining")
AddEventHandler("playerJoining", function()
    local src = source
    TriggerClientEvent("hat:forceStart", src)
end)

RegisterNetEvent("hat:pickup")
AddEventHandler("hat:pickup", function(sourceId, drawable, texture)
    local src = source

    -- Send to picker only → equip hat
    TriggerClientEvent("hat:pickupEquip", src, drawable, texture)

    -- Send to everyone (including picker → normal for viewing animation) → play pickup anim only
    --TriggerClientEvent("hat:pickupAnim", -1, src)
end)
