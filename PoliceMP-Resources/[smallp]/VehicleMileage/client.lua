local lastVehicle = nil
local lastPosition = nil
local fetchedMileage = 0.0
local sessionMileage = 0.0
local isUIVisible = false
local isTrackingVisible = true -- Controls whether the mileage UI is shown

function getVehicleHash(vehicle)
    return GetEntityModel(vehicle)
end

function updateMileageUI(show, totalMileage)
    if isTrackingVisible then
        local message = {
            action = "updateMileage",
            show = show,
            mileage = totalMileage
        }
        SendNUIMessage(message)
        isUIVisible = show
    else
        if isUIVisible then
            -- Hide UI if it was previously visible
            local message = { action = "updateMileage", show = false }
            SendNUIMessage(message)
            isUIVisible = false
        end
    end
end

CreateThread(function()
    while true do
        Wait(1000)
        local ped = PlayerPedId()
        if IsPedInAnyVehicle(ped, false) then
            local vehicle = GetVehiclePedIsIn(ped, false)
            local vehicleHash = getVehicleHash(vehicle)

            if vehicle ~= lastVehicle then
                TriggerServerEvent("mileage:fetch", GetPlayerServerId(PlayerId()), vehicleHash)
                lastVehicle = vehicle
                lastPosition = GetEntityCoords(vehicle)
                sessionMileage = 0.0
            else
                local currentPosition = GetEntityCoords(vehicle)
                local distance = #(currentPosition - lastPosition)
                if distance > 0.1 then
                    sessionMileage = sessionMileage + distance
                    lastPosition = currentPosition
                    updateMileageUI(true, fetchedMileage + sessionMileage)
                end
            end
        else
            if lastVehicle then
                TriggerServerEvent("mileage:save", GetPlayerServerId(PlayerId()), getVehicleHash(lastVehicle), sessionMileage)
                fetchedMileage = fetchedMileage + sessionMileage
                sessionMileage = 0.0
                lastVehicle = nil
            end
            updateMileageUI(false, 0)
        end
    end
end)

RegisterNetEvent("mileage:show")
AddEventHandler("mileage:show", function(vehicleHash, mileage)
    fetchedMileage = mileage
    sessionMileage = 0.0
    updateMileageUI(true, fetchedMileage)
end)

-- Command to toggle mileage UI
RegisterCommand("togglemileage", function()
    isTrackingVisible = not isTrackingVisible
    if not isTrackingVisible then
        updateMileageUI(false, 0) -- Hide UI when toggling off
    else
        updateMileageUI(true, fetchedMileage + sessionMileage) -- Show UI when toggling on
    end
    print("Mileage UI toggled:", isTrackingVisible and "On" or "Off")
end, false)
