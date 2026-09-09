RegisterNetEvent("dev:spawnVehicle", function(vehicleName)
    if not vehicleName then
        TriggerEvent("chat:addMessage", {
            args = { "^1[ERROR]^0 Please specify a vehicle name. Usage: /devsvtcp [vehicle_name]" }
        })
        return
    end

    vehicleName = string.lower(vehicleName)

    if not IsModelInCdimage(vehicleName) or not IsModelAVehicle(vehicleName) then
        TriggerEvent("chat:addMessage", {
            args = { "^1[ERROR]^0 Vehicle model does not exist or is not a valid vehicle: " .. vehicleName }
        })
        return
    end

    RequestModel(vehicleName)
    while not HasModelLoaded(vehicleName) do Wait(100) end

    local playerPed = PlayerPedId()
    local coords = GetEntityCoords(playerPed)
    local heading = GetEntityHeading(playerPed)

    local vehicle = CreateVehicle(vehicleName, coords.x, coords.y, coords.z, heading, true, false)
    TaskWarpPedIntoVehicle(playerPed, vehicle, -1)

    SetEntityAsNoLongerNeeded(vehicle)
    SetModelAsNoLongerNeeded(vehicleName)

    TriggerEvent("chat:addMessage", {
        args = { "^2[SUCCESS]^0 Vehicle spawned: " .. vehicleName }
    })
end)
