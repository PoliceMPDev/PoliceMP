RegisterCommand('setdirt', function(source, args)
    if IsPlayerAceAllowed(source, "group.qa") then
        local playerPed = GetPlayerPed(source)
        local vehicle = GetVehiclePedIsIn(playerPed, false)

        if vehicle ~= 0 then
            local dirtLevel = tonumber(args[1])
            if dirtLevel and dirtLevel >= 0.0 and dirtLevel <= 15.0 then
                SetVehicleDirtLevel(vehicle, dirtLevel)
            end
        end
    end
end, false)