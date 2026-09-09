local activeStrobes = {} 
local strobeColors = {}  

RegisterNetEvent("helmetstrobe:syncState")
AddEventHandler("helmetstrobe:syncState", function(playerId, isActive)
    if isActive then
        activeStrobes[playerId] = true
    else
        activeStrobes[playerId] = nil
    end
end)

RegisterNetEvent("helmetstrobe:syncColor")
AddEventHandler("helmetstrobe:syncColor", function(playerId, rgb)
    strobeColors[playerId] = rgb
end)

RegisterNetEvent("helmetstrobe:notify")
AddEventHandler("helmetstrobe:notify", function(msg)
    ShowNotification(msg)
end)

CreateThread(function()
    while true do
        local myPed = PlayerPedId()
        local myVeh = GetVehiclePedIsIn(myPed, false)
        local isInHeli = myVeh ~= 0 and (GetVehicleClass(myVeh) == 15 or GetVehicleClass(myVeh) == 16)

        if isInHeli then
            for playerId, _ in pairs(activeStrobes) do
                local tgt = GetPlayerFromServerId(playerId)
                if tgt then
                    local tgtPed = GetPlayerPed(tgt)
                    if tgtPed and tgtPed ~= -1 then
                        local bone = GetPedBoneIndex(tgtPed, 31086)
                        local pos = GetWorldPositionOfEntityBone(tgtPed, bone)
                        local rgb = strobeColors[playerId] or {255, 0, 0}
                        DrawLightWithRange(pos.x, pos.y, pos.z, rgb[1], rgb[2], rgb[3], 3.0, 15.0)
                    end
                end
            end
        end

        Wait(500)
    end
end)

function ShowNotification(msg)
    SetNotificationTextEntry("STRING")
    AddTextComponentSubstringPlayerName(msg)
    DrawNotification(false, false)
end
