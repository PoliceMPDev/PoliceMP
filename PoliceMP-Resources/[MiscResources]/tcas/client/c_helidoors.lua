-- helicopter_doors.lua

-- Table of allowed helicopter models
local allowedHelicopters = {
    "dauphin",
    "addpolmh65",
    -- Add more models here if needed
}

-- Function to check if vehicle is an allowed helicopter
local function isAllowedHelicopter(vehicle)
    for _, modelName in ipairs(allowedHelicopters) do
        if IsVehicleModel(vehicle, GetHashKey(modelName)) then
            return true
        end
    end
    return false
end

-- Command to toggle helicopter back doors
RegisterCommand('helidoors', function(source, args, rawCommand)
    local playerPed = GetPlayerPed(-1) -- Get the player's character
    local vehicle = GetVehiclePedIsIn(playerPed, false) -- Get the vehicle the player is in

    if not DoesEntityExist(vehicle) then return end

    -- Check if the player is in a valid helicopter
    if isAllowedHelicopter(vehicle) then
        local seatIndex = GetSeatIndex(playerPed, vehicle) -- Custom function to get seat index

        -- Check if the player is in one of the back seats (1 or 2)
        if seatIndex == 1 or seatIndex == 2 then
            -- Check current state of the back doors
            local leftDoorState = GetVehicleDoorAngleRatio(vehicle, 2) -- Left back door
            local rightDoorState = GetVehicleDoorAngleRatio(vehicle, 3) -- Right back door

            -- Debugging prints to check the door states
            print("Left Door State: " .. leftDoorState)
            print("Right Door State: " .. rightDoorState)

            -- Checks if player is in "addpolmh65", if so, only open the rear right door.
            local model = GetEntityModel(vehicle)

            if model == GetHashKey("addpolmh65") then
                -- Only toggle right rear door
                if rightDoorState == 0 then
                    SetVehicleDoorOpen(vehicle, 3, false, false) -- Open right back door
                else
                    SetVehicleDoorShut(vehicle, 3, false) -- Close right back door
                end
                return -- Exit to skip the default toggle
            end

            -- Default behavior for other helicopters
            if leftDoorState == 0 and rightDoorState == 0 then
                -- If doors are closed, open them
                TriggerServerEvent('helicopter:toggleDoors', NetworkGetNetworkIdFromEntity(vehicle), true)
            else
                -- If doors are open, close them
                TriggerServerEvent('helicopter:toggleDoors', NetworkGetNetworkIdFromEntity(vehicle), false)
            end
        else
            TriggerEvent('chat:addMessage', {
                args = {"~r~You must be a back seat passenger to open or close the doors!"}
            })
        end
    end
end, false)

-- Function to get seat index
function GetSeatIndex(playerPed, vehicle)
    for i = -1, 2 do -- Check all possible seat indices
        if GetPedInVehicleSeat(vehicle, i) == playerPed then
            return i
        end
    end
    return nil -- Player is not in the vehicle
end

-- Event to synchronize door opening and closing across clients
RegisterNetEvent('helicopter:syncToggleDoors')
AddEventHandler('helicopter:syncToggleDoors', function(vehicle, state)
    local vehicleEntity = NetToVeh(vehicle)

    if DoesEntityExist(vehicleEntity) then
        if state then
            -- Open back doors
            SetVehicleDoorOpen(vehicleEntity, 2, false, false) -- Left back door
            SetVehicleDoorOpen(vehicleEntity, 3, false, false) -- Right back door
        else
            -- Close back doors
            SetVehicleDoorShut(vehicleEntity, 2, false)
            SetVehicleDoorShut(vehicleEntity, 3, false)
        end
    end
end)
