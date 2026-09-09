AddTextEntry("bombrobot", "Bomb Robot")
TriggerEvent('chat:addSuggestion', '/eod', 'Setup or remove an EOD robot.')

if main == nil then
    main = {
        model = `bombrobot`,
        tabletModel = `prop_cs_tablet`,
        pedModel = `s_m_m_bouncer_01`,
    }
    keys = {
        forward = {0, 172},
        backward = {0, 173},
        left = {0, 174},
        right = {0, 175},
        camera = {0, 18},
        nightVision = {0, 212},   
        thermalImaging = {0, 214},
        explosion = {0, 208},
        cancelExplosion = {0, 207},
    }
end

eod = {
    active = false,
    vehicleHandle = 0,
    driverHandle = 0,
    cameraEnabled = false,
    cameraHandle = 0,
    nightVisionEnabled = false,
    hoseEnabled = false,
    tablet = 0,
}

-- TODO:
-- Going straight back doesn't work

RegisterNetEvent('toggleEOD')
AddEventHandler('toggleEOD', function()
    if eod.active then
        TriggerEvent("deleteEOD")
    else
        TriggerEvent("newEOD")
    end
end)

Citizen.CreateThread(function()
    while true do
        if eod.active then
            local ped = PlayerPedId()
            local coords = GetEntityCoords(ped)
            local eodCoords = GetEntityCoords(eod.vehicleHandle)
            local distance = #(coords - eodCoords)
            if distance <= 100.0 then
                if distance > 500.0 then
                    TriggerEvent("requestControlEOD")
                end
            else
                TaskVehicleTempAction(eod.driverHandle, eod.vehicleHandle, 6, 2500)
            end
            DisableControlAction(keys.forward[1], keys.forward[2], true)
            DisableControlAction(keys.backward[1], keys.backward[2], true)
            DisableControlAction(keys.left[1], keys.left[2], true)
            DisableControlAction(keys.right[1], keys.right[2], true)
            DisableControlAction(keys.camera[1], keys.camera[2], true)
            DisableControlAction(keys.nightVision[1], keys.nightVision[2], true)
            DisableControlAction(keys.thermalImaging[1], keys.thermalImaging[2], true)
            DisableControlAction(keys.explosion[1], keys.explosion[2], true)
            DisableControlAction(keys.cancelExplosion[1], keys.cancelExplosion[2], true)
            DisableControlAction(keys.hose[1], keys.hose[2], true)

            if IsDisabledControlPressed(keys.forward[1], keys.forward[2]) and not IsDisabledControlPressed(keys.backward[1], keys.backward[2]) then
                TaskVehicleTempAction(eod.driverHandle, eod.vehicleHandle, 9, 1)
            end
            
            if IsDisabledControlJustReleased(keys.forward[1], keys.forward[2]) or IsDisabledControlJustReleased(keys.backward[1], keys.backward[2]) then
                TaskVehicleTempAction(eod.driverHandle, eod.vehicleHandle, 6, 2500)
            end
    
            if IsControlPressed(keys.backward[1], keys.backward[2]) and not IsDisabledControlPressed(keys.forward[1], keys.forward[2]) then
                TaskVehicleTempAction(eod.driverHandle, eod.vehicleHandle, 22, 1)
            end
    
            if IsDisabledControlPressed(keys.left[1], keys.left[2]) and IsDisabledControlPressed(keys.backward[1], keys.backward[2]) then
                TaskVehicleTempAction(eod.driverHandle, eod.vehicleHandle, 13, 1)
            end
    
            if IsDisabledControlPressed(keys.right[1], keys.right[2]) and IsDisabledControlPressed(keys.backward[1], keys.backward[2]) then
                TaskVehicleTempAction(eod.driverHandle, eod.vehicleHandle, 14, 1)
            end
    
            if IsDisabledControlPressed(keys.forward[1], keys.forward[2]) and IsDisabledControlPressed(keys.backward[1], keys.backward[2]) then
                TaskVehicleTempAction(eod.driverHandle, eod.vehicleHandle, 30, 100)
            end
    
            if IsDisabledControlPressed(keys.left[1], keys.left[2]) and IsDisabledControlPressed(keys.forward[1], keys.forward[2]) then
                TaskVehicleTempAction(eod.driverHandle, eod.vehicleHandle, 7, 1)
            end
    
            if IsDisabledControlPressed(keys.right[1], keys.right[2]) and IsDisabledControlPressed(keys.forward[1], keys.forward[2]) then
                TaskVehicleTempAction(eod.driverHandle, eod.vehicleHandle, 8, 1)
            end
    
            if IsDisabledControlPressed(keys.left[1], keys.left[2]) and not IsDisabledControlPressed(keys.forward[1], keys.forward[2]) and not IsDisabledControlPressed(keys.backward[1], keys.backward[2]) then
                TaskVehicleTempAction(eod.driverHandle, eod.vehicleHandle, 4, 1)
            end
    
            if IsDisabledControlPressed(keys.right[1], keys.right[2]) and not IsDisabledControlPressed(keys.forward[1], keys.forward[2]) and not IsDisabledControlPressed(keys.backward[1], keys.backward[2]) then
                TaskVehicleTempAction(eod.driverHandle, eod.vehicleHandle, 5, 1)
            end

            -- Toggle Camera
            if IsDisabledControlJustPressed(keys.camera[1], keys.camera[2]) then
                TriggerEvent("toggleCameraEod")
            end

            -- Thermal Imaging
            if IsDisabledControlJustPressed(keys.thermalImaging[1], keys.thermalImaging[2]) then
                if (eod.cameraEnabled) then
                    if (eod.thermalEnabled) then
                        SetSeethrough(false)
                        eod.thermalEnabled = false
                    else
                        SetSeethrough(true)
                        eod.thermalEnabled = true
                    end
                end
            end

            -- Night Vision
            if IsDisabledControlJustPressed(keys.nightVision[1], keys.nightVision[2]) then
                if (eod.cameraEnabled) then
                    if (eod.nightVisionEnabled) then
                        SetNightvision(false)
                        eod.nightVisionEnabled = false
                    else
                        SetNightvision(true)
                        eod.nightVisionEnabled = true
                    end
                end
            end

            -- Controlled Explosion
            if IsDisabledControlJustPressed(keys.explosion[1], keys.explosion[2]) then
                TriggerEvent("controlledExplosion")
            end

            -- Cancel Explosion
            if (IsDisabledControlJustPressed(keys.cancelExplosion[1], keys.cancelExplosion[2])) then
                TriggerEvent("cancelExplosion")
            end

            -- Hose Handler
            if not eod.hoseEnabled then
                if IsDisabledControlPressed(keys.hose[1], keys.hose[2]) then
                    eod.hoseEnabled = true
                    TriggerServerEvent("activateHoseServer", NetworkGetNetworkIdFromEntity(eod.vehicleHandle))
                    Citizen.CreateThread(function()
                        while not IsDisabledControlJustReleased(keys.hose[1], keys.hose[2]) or (not eod.hoseEnabled) do
                            Wait(0)
                        end
                        if eod.hoseEnabled then
                            TriggerServerEvent("deactivateHoseServer", NetworkGetNetworkIdFromEntity(eod.vehicleHandle))
                            eod.hoseEnabled = false
                        end
                    end)
                end
            end
        end
        Wait(0)
    end
end)

function topNotification(text)
    SetTextComponentFormat('STRING')
    AddTextComponentString(text)
    DisplayHelpTextFromStringLabel(0, 0, 1, -1)
end

function ButtonMessage(text)
    BeginTextCommandScaleformString("STRING")
    AddTextComponentScaleform(text)
    EndTextCommandScaleformString()
end

function Button(ControlButton)
    N_0xe83a3e3557a56640(ControlButton)
end

function setupScaleform(scaleform)
    local scaleform = RequestScaleformMovie(scaleform)
    while not HasScaleformMovieLoaded(scaleform) do
        Citizen.Wait(0)
    end
    PushScaleformMovieFunction(scaleform, "CLEAR_ALL")
    PopScaleformMovieFunctionVoid()
    
    PushScaleformMovieFunction(scaleform, "SET_CLEAR_SPACE")
    PushScaleformMovieFunctionParameterInt(200)
    PopScaleformMovieFunctionVoid()

    PushScaleformMovieFunction(scaleform, "SET_DATA_SLOT")
    PushScaleformMovieFunctionParameterInt(0)
    Button(GetControlInstructionalButton(0, keys.cancelExplosion[2], true))
    ButtonMessage("Cancel Explosion")
    PopScaleformMovieFunctionVoid()

    PushScaleformMovieFunction(scaleform, "SET_DATA_SLOT")
    PushScaleformMovieFunctionParameterInt(1)
    Button(GetControlInstructionalButton(0, keys.explosion[2], true))
    ButtonMessage("Controlled Explosion")
    PopScaleformMovieFunctionVoid()

    PushScaleformMovieFunction(scaleform, "SET_DATA_SLOT")
    PushScaleformMovieFunctionParameterInt(2)
    Button(GetControlInstructionalButton(0, keys.nightVision[2], true))
    ButtonMessage("Night Vision")
    PopScaleformMovieFunctionVoid()

    PushScaleformMovieFunction(scaleform, "SET_DATA_SLOT")
    PushScaleformMovieFunctionParameterInt(3)
    Button(GetControlInstructionalButton(0, keys.camera[2], true))
    ButtonMessage("Camera Stream")
    PopScaleformMovieFunctionVoid()

    PushScaleformMovieFunction(scaleform, "SET_DATA_SLOT")
    PushScaleformMovieFunctionParameterInt(4)
    Button(GetControlInstructionalButton(0, keys.right[2], true))
    ButtonMessage("Right")
    PopScaleformMovieFunctionVoid()

    PushScaleformMovieFunction(scaleform, "SET_DATA_SLOT")
    PushScaleformMovieFunctionParameterInt(5)
    Button(GetControlInstructionalButton(0, keys.left[2], true)) -- The button to display
    ButtonMessage("Left") -- the message to display next to it
    PopScaleformMovieFunctionVoid()

    PushScaleformMovieFunction(scaleform, "SET_DATA_SLOT")
    PushScaleformMovieFunctionParameterInt(6)
    Button(GetControlInstructionalButton(0, keys.backward[2], true))
    ButtonMessage("Backwards")
    PopScaleformMovieFunctionVoid()

    PushScaleformMovieFunction(scaleform, "SET_DATA_SLOT")
    PushScaleformMovieFunctionParameterInt(7)
    Button(GetControlInstructionalButton(0, keys.forward[2], true))
    ButtonMessage("Forward")
    PopScaleformMovieFunctionVoid()

    PushScaleformMovieFunction(scaleform, "SET_DATA_SLOT")
    PushScaleformMovieFunctionParameterInt(8)
    Button(GetControlInstructionalButton(1, keys.hose[2], true))
    ButtonMessage("Hose")
    PopScaleformMovieFunctionVoid()

    PushScaleformMovieFunction(scaleform, "DRAW_INSTRUCTIONAL_BUTTONS")
    PopScaleformMovieFunctionVoid()

    PushScaleformMovieFunction(scaleform, "SET_BACKGROUND_COLOUR")
    PushScaleformMovieFunctionParameterInt(0)
    PushScaleformMovieFunctionParameterInt(0)
    PushScaleformMovieFunctionParameterInt(0)
    PushScaleformMovieFunctionParameterInt(80)
    PopScaleformMovieFunctionVoid()

    return scaleform
end

-- Scaleform Handler
Citizen.CreateThread(function()
    form = setupScaleform("instructional_buttons")
    while true do
        if (eod.active) then
            DrawScaleformMovieFullscreen(form, 255, 255, 255, 255, 0)
        end
        Wait(0)
    end
end)

function toFloat(integer)
    return integer + 0.0
end