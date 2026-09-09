local engineStalled = false
local lastVehicleHealth = 0
local dizzyEffectActive = false
local dizzyEffectTimer = 0

function ClearScreenEffects()
    ClearTimecycleModifier()
    StopGameplayCamShaking(true)
end

RegisterNetEvent("carEngineStalling:checkStalling")
AddEventHandler("carEngineStalling:checkStalling", function(vehicle)
    if DoesEntityExist(vehicle) and IsEntityAVehicle(vehicle) then
        local currentHealth = GetVehicleBodyHealth(vehicle)
        local healthDifference = lastVehicleHealth - currentHealth

        
        if healthDifference > Config.MinHealthDifference then
            TriggerEvent("carEngineStalling:stallEngineAndDizzy", vehicle)
        end

        lastVehicleHealth = currentHealth
    end
end)

RegisterNetEvent("carEngineStalling:stallEngineAndDizzy")
AddEventHandler("carEngineStalling:stallEngineAndDizzy", function(vehicle)
    if DoesEntityExist(vehicle) and IsEntityAVehicle(vehicle) then
        SetVehicleEngineOn(vehicle, false, false, true)

        TriggerEvent("carEngineStalling:dizzyEffect")
        TriggerEvent("rtcminor")
        
        SetTimeout(Config.StallingDuration * 1000, function()
            SetVehicleEngineOn(vehicle, true, false, true)

            engineStalled = false
        end)
    end
end)

RegisterNetEvent("carEngineStalling:dizzyEffect")
AddEventHandler("carEngineStalling:dizzyEffect", function()
    if not dizzyEffectActive then
        dizzyEffectActive = true

        -- Add your dizzy effect logic here
        local playerPed = PlayerPedId()

        if playerPed then
            SetTimecycleModifier("DRUNK", true)
            ShakeGameplayCam("DRUNK_SHAKE", Config.DizzyEffectIntensity)  -- Adjust intensity as needed

            dizzyEffectTimer = GetGameTimer() + Config.DizzyEffectDuration * 1000
        end
    end
end)

Citizen.CreateThread(function()
    while true do
        Citizen.Wait(0)

        local playerPed = PlayerPedId()
        if playerPed and IsPedInAnyVehicle(playerPed, false) then
            local vehicle = GetVehiclePedIsIn(playerPed, false)
            
            if DoesEntityExist(vehicle) and IsEntityAVehicle(vehicle) then
                local currentHealth = GetVehicleBodyHealth(vehicle)

                if lastVehicleHealth == 0 then
                    lastVehicleHealth = currentHealth
                end
            end
        end
    end
end)

Citizen.CreateThread(function()
    while true do
        Citizen.Wait(0)

        local playerPed = PlayerPedId()
        if playerPed and IsPedInAnyVehicle(playerPed, false) then
            local vehicle = GetVehiclePedIsIn(playerPed, false)
            
            if DoesEntityExist(vehicle) and IsEntityAVehicle(vehicle) then
                local currentHealth = GetVehicleBodyHealth(vehicle)
                local healthDifference = lastVehicleHealth - currentHealth
                local speed = GetEntitySpeed(vehicle) * 2.236936

                if healthDifference > Config.MinHealthDifference and speed > Config.MinimumSpeedForStalling  then
                    TriggerEvent("carEngineStalling:checkStalling", vehicle)
                end

                lastVehicleHealth = currentHealth
            end
        end
    end
end)

Citizen.CreateThread(function()
    while true do
        Citizen.Wait(1000)  -- Check every second

        if dizzyEffectActive and GetGameTimer() > dizzyEffectTimer then
            ClearScreenEffects()

            dizzyEffectActive = false
            dizzyEffectTimer = 0
        end
    end
end)
