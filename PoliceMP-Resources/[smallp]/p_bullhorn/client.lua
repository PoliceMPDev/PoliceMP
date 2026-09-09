local isHornPlaying = false
local myHornId = nil

RegisterNetEvent("bullhorn:play")
AddEventHandler("bullhorn:play", function(coords, src)
    local hornId = "bullhorn_" .. src
    exports['xsound']:PlayUrlPos(
        hornId,
        "nui://p_bullhorn/html/bullhorn.ogg",
        1.0,
        coords,
        true
    )
    exports['xsound']:Distance(hornId, 60)
end)

RegisterNetEvent("bullhorn:stop")
AddEventHandler("bullhorn:stop", function(src)
    local hornId = "bullhorn_" .. src
    exports['xsound']:Destroy(hornId)
end)

RegisterNetEvent("bullhorn:updatePos")
AddEventHandler("bullhorn:updatePos", function(coords, src)
    local hornId = "bullhorn_" .. src
    exports['xsound']:Position(hornId, coords)
end)

Citizen.CreateThread(function()
    while true do
        Wait(0)
        local ped = PlayerPedId()
        local veh = GetVehiclePedIsIn(ped, false)

        if veh ~= 0 and GetPedInVehicleSeat(veh, -1) == ped and GetVehicleClass(veh) == 18 then
            if IsControlPressed(0, 305) then -- B key
                if not isHornPlaying then
                    isHornPlaying = true
                    myHornId = GetPlayerServerId(PlayerId())
                    local coords = GetEntityCoords(veh)
                    TriggerServerEvent("bullhorn:trigger", coords)
                end
            else
                if isHornPlaying then
                    isHornPlaying = false
                    TriggerServerEvent("bullhorn:stop")
                end
            end
        elseif isHornPlaying then
            isHornPlaying = false
            TriggerServerEvent("bullhorn:stop")
        end
    end
end)

Citizen.CreateThread(function()
    local lastUpdate = 0
    while true do
        Wait(0)
        if isHornPlaying then
            local now = GetGameTimer()
            if now - lastUpdate > 100 then -- update every 100ms
                local veh = GetVehiclePedIsIn(PlayerPedId(), false)
                if veh ~= 0 then
                    local coords = GetEntityCoords(veh)
                    TriggerServerEvent("bullhorn:updatePos", coords)
                    lastUpdate = now
                end
            end
        end
    end
end)
