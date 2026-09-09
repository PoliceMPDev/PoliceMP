RegisterNetEvent("forcePassengerExit")
AddEventHandler("forcePassengerExit", function(vehicleNetId)
    local vehicle = NetworkGetEntityFromNetworkId(vehicleNetId)

    if vehicle and DoesEntityExist(vehicle) then
        local frontPassenger = GetPedInVehicleSeat(vehicle, 0) -- Front passenger seat (index 0)

        if frontPassenger and frontPassenger ~= 0 then
            TaskLeaveVehicle(frontPassenger, vehicle, 16)

           
        end
    end
end)
