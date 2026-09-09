local SmokeEnabled, SmokeR, SmokeG, SmokeB, SmokeSize = false, 255, 0, 0, 1.0 -- Default to white smoke
local PlayerSmokeSettings = {}
local ActiveFx = {}
local HasPermission = false

local function RequestPermission()
    TriggerServerEvent("pmp:CheckPlaneSmokePermission")
end

RegisterNetEvent("pmp:ReceivePlaneSmokePermission")
AddEventHandler("pmp:ReceivePlaneSmokePermission", function(permission)
    HasPermission = permission
end)

Citizen.CreateThread(function()
    RequestPermission()
end)

local function UpdatePlaneSmokeSettings(sEnabled, sR, sG, sB, sSize)
    if not HasPermission then return end
    SmokeEnabled, SmokeR, SmokeG, SmokeB, SmokeSize = sEnabled, sR, sG, sB, sSize
    TriggerServerEvent("pmp:PlaneSmokeSettingsUpdate", {SmokeEnabled, SmokeR, SmokeG, SmokeB, SmokeSize})
end

RegisterNetEvent('pmp:PlaneSmokeSettingsUpdate')
AddEventHandler('pmp:PlaneSmokeSettingsUpdate', function(Data)
    PlayerSmokeSettings = Data
end)

Citizen.CreateThread(function()
    DecorRegister("smoke_trail_r", 3)
    DecorRegister("smoke_trail_g", 3)
    DecorRegister("smoke_trail_b", 3)

    while true do
        Citizen.Wait(0)
        local ped = PlayerPedId()
        local veh = GetVehiclePedIsUsing(ped)

        if IsControlJustPressed(0, 73) and IsPedInAnyPlane(ped) then
            DecorSetInt(veh, "smoke_trail_r", SmokeR)
            DecorSetInt(veh, "smoke_trail_g", SmokeG)
            DecorSetInt(veh, "smoke_trail_b", SmokeB)
            UpdatePlaneSmokeSettings(not SmokeEnabled, SmokeR, SmokeG, SmokeB, SmokeSize)
        end
    end
end)

Citizen.CreateThread(function()
    local particleDictionary = "scr_ar_planes"
    local particleName = "scr_ar_trail_smoke"
    RequestNamedPtfxAsset(particleDictionary)

    while not HasNamedPtfxAssetLoaded(particleDictionary) do
        Citizen.Wait(0)
    end

    while true do
        Citizen.Wait(0)

        for _, player in ipairs(GetActivePlayers()) do
            local ped = GetPlayerPed(player)
            local veh = GetVehiclePedIsUsing(ped)
            local Data = PlayerSmokeSettings[GetPlayerServerId(player)]

            if Data ~= nil then
                local SmokeEnabled, SmokeR, SmokeG, SmokeB, SmokeSize = table.unpack(Data)
                if (IsPedInAnyPlane(ped) and not IsEntityDead(veh)) and SmokeEnabled then
                    if not ActiveFx[veh] then
                        UseParticleFxAssetNextCall(particleDictionary)
                        local ox, oy, oz = 0.0, 0.0, 0.0
                        ActiveFx[veh] = StartNetworkedParticleFxLoopedOnEntityBone(particleName, veh, ox, oy, oz, 0.0, 0.0, 0.0, -1, SmokeSize + 0.0, ox, oy, oz)
                    elseif ActiveFx[veh] and not IsEntityDead(veh) then
                        SetParticleFxLoopedScale(ActiveFx[veh], SmokeSize + 0.0)
                        SetParticleFxLoopedRange(ActiveFx[veh], 10000.0)
                        SetParticleFxLoopedColour(ActiveFx[veh], SmokeR + 0.0, SmokeG + 0.0, SmokeB + 0.0)
                    end
                else
                    if ActiveFx[veh] or IsEntityDead(veh) or not veh then
                        StopParticleFxLooped(ActiveFx[veh], 0)
                        ActiveFx[veh] = nil
                    end
                end
            end
        end
    end
end)

RegisterCommand("scolor", function(source, args, raw)
    if not HasPermission then return end
    local ped = PlayerPedId()
    local veh = GetVehiclePedIsUsing(ped)
    
    if IsPedInAnyPlane(ped) then
        if #args < 3 then
            
            return
        end
        
        SmokeR = tonumber(args[1]) or SmokeR
        SmokeG = tonumber(args[2]) or SmokeG
        SmokeB = tonumber(args[3]) or SmokeB

        if SmokeR < 0 then SmokeR = 0 end
        if SmokeG < 0 then SmokeG = 0 end
        if SmokeB < 0 then SmokeB = 0 end
        if SmokeR > 255 then SmokeR = 255 end
        if SmokeG > 255 then SmokeG = 255 end
        if SmokeB > 255 then SmokeB = 255 end

        DecorSetInt(veh, "smoke_trail_r", SmokeR)
        DecorSetInt(veh, "smoke_trail_g", SmokeG)
        DecorSetInt(veh, "smoke_trail_b", SmokeB)
    end

    UpdatePlaneSmokeSettings(SmokeEnabled, SmokeR, SmokeG, SmokeB, SmokeSize)
end)

RegisterCommand("ssize", function(source, args, raw)
    if not HasPermission then return end
    local newSize = tonumber(args[1]) + 0.0
    if newSize > 5.0 then newSize = 5.0 elseif newSize <= 0.0 then newSize = 0.1 end
    SmokeSize = newSize
    UpdatePlaneSmokeSettings(SmokeEnabled, SmokeR, SmokeG, SmokeB, SmokeSize)
end)

UpdatePlaneSmokeSettings(SmokeEnabled, SmokeR, SmokeG, SmokeB, SmokeSize)
