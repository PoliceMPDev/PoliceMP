
-- REMOVE IDLE CAM
Citizen.CreateThread(function()
    while true do
        Citizen.Wait(20000) -- Run every 20 seconds to stop idle cam
        InvalidateIdleCam() -- Prevents the spinning camera effect
        InvalidateVehicleIdleCam() -- Prevents vehicle idle camera
    end
end)

local holdingE = false

-- Valid weapon group names (converted to hashes)
local validWeaponGroups = {
    [GetHashKey("GROUP_PISTOL")] = true,
    [GetHashKey("GROUP_SMG")] = true,
    [GetHashKey("GROUP_RIFLE")] = true,
    [GetHashKey("GROUP_MG")] = true,
    [GetHashKey("GROUP_SHOTGUN")] = true,
    [GetHashKey("GROUP_SNIPER")] = true,
    [GetHashKey("GROUP_STUNGUN")] = true,
}

Citizen.CreateThread(function()
    while true do
        Citizen.Wait(0)

        local playerPed = PlayerPedId()
        local weaponHash = GetSelectedPedWeapon(playerPed)
        local groupHash = GetWeapontypeGroup(weaponHash)

        if validWeaponGroups[groupHash] then
            if IsControlPressed(0, 177) then -- Right mouse button
                if not holdingE then
                    holdingE = true
                end
                SetPedStealthMovement(playerPed, true, "DEFAULT_ACTION")
            else
                if holdingE then
                    holdingE = false
                    SetPedStealthMovement(playerPed, false, "DEFAULT_ACTION")
                end
            end
        else
            if holdingE then
                holdingE = false
                SetPedStealthMovement(playerPed, false, "DEFAULT_ACTION")
            end
        end
    end
end)

-- Vehicle Wheels angle
local lastVehicle = nil
local applyingSteering = {}

Citizen.CreateThread(function()
    while true do
        Citizen.Wait(0)

        local ped = PlayerPedId()

        if IsPedInAnyVehicle(ped, false) then
            local veh = GetVehiclePedIsIn(ped, false)

            -- Detect player pressing F (Exit Vehicle)
            if IsControlJustPressed(0, 75) then
                local angle = GetVehicleSteeringAngle(veh)

                -- Freeze steering before exit
                SetVehicleSteeringAngle(veh, angle)
                SetVehicleSteeringScale(veh, 0.0)

                -- Send to server
                local vehNetId = NetworkGetNetworkIdFromEntity(veh)
                TriggerServerEvent("wheelSync:updateSteering", vehNetId, angle)

                -- Start apply for myself (local player)
                applyingSteering[veh] = {
                    angle = angle,
                    timer = GetGameTimer() + 2000 -- apply for 2 seconds after exit
                }
            end

            lastVehicle = veh
        else
            lastVehicle = nil
        end

        for veh, data in pairs(applyingSteering) do
            if veh ~= nil and DoesEntityExist(veh) then
                SetVehicleSteeringAngle(veh, data.angle)
                SetVehicleSteeringScale(veh, 0.0)
            else
                applyingSteering[veh] = nil -- clean up if invalid
            end

            if data.timer and GetGameTimer() > data.timer then
                applyingSteering[veh] = nil
            end
        end
    end
end)

RegisterNetEvent("wheelSync:applySteering", function(vehNetId, angle)
    local veh = NetworkGetEntityFromNetworkId(vehNetId)

    if veh ~= 0 and DoesEntityExist(veh) then
        applyingSteering[veh] = {
            angle = angle,
            timer = GetGameTimer() + 2000
        }
    else
        --print("[wheelSync] Invalid entity from net ID:", vehNetId, "Resolved entity:", veh)
    end
end)
