local rerouteCounter = 0
local rerouteMap = {} -- [id] = {src = player, coords = vector3, heading = float}

RegisterNetEvent("reroute:placeVehicle", function(coords, heading)
    local src = source
    rerouteCounter = rerouteCounter + 1
    local id = "reroute_" .. rerouteCounter

    rerouteMap[id] = {
        src = src,
        coords = coords,
        heading = heading
    }

    TriggerClientEvent("reroute:createPlacedVehicle", src, coords, heading, id)
    TriggerClientEvent("reroute:createRerouteBlip", -1, coords, id)
end)

RegisterNetEvent("reroute:removeVehicleById", function(id)
    rerouteMap[id] = nil
    TriggerClientEvent("reroute:removeRerouteBlip", -1, id)
end)

AddEventHandler("playerDropped", function()
    local src = source
    for id, data in pairs(rerouteMap) do
        if data.src == src then
            rerouteMap[id] = nil
            TriggerClientEvent("reroute:removeRerouteBlip", -1, id)
            TriggerClientEvent("reroute:deleteRerouteVehicle", -1, id)
        end
    end
end)

RegisterCommand("+zreroute", function(src)
    if not IsPlayerAceAllowed(src, "Police.pc") then
        TriggerClientEvent("reroute:notify", src, "~r~You do not have permission to use this command.")
        return
    end

    TriggerClientEvent("reroute:startPreview", src)
    TriggerClientEvent("reroute:notify", src, "~g~Started reroute placement.")
end, false)

RegisterCommand("+zunroute", function(src)
    if not IsPlayerAceAllowed(src, "Police.pc") then
        TriggerClientEvent("reroute:notify", src, "~r~You do not have permission to use this command.")
        return
    end

    TriggerClientEvent("reroute:attemptDeleteClosest", src)
end, false)
