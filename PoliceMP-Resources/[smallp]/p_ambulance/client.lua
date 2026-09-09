local npcAmbulance = nil
local npcDriver = nil
local trackingPlayer = nil
local despawnTimer = nil
local ambulanceBlip = nil
local lastCallTime = 0
local cooldownTime = 60

RegisterCommand("+xambulance", function(source, args, rawCommand)
    local currentTime = GetGameTimer() / 1000
    if currentTime - lastCallTime < cooldownTime then
        local remainingTime = math.ceil(cooldownTime - (currentTime - lastCallTime))
        drawNotification("~r~You must wait " .. remainingTime .. " seconds before calling another ambulance.")
        return
    end
    
    ExecuteCommand("e radioear")
    Wait(3000)
    ExecuteCommand("emotecancel")

    lastCallTime = currentTime
    local playerPed = PlayerPedId()
    local playerCoords = GetEntityCoords(playerPed)

    -- Get closest road position
    local found, roadCoord = GetClosestVehicleNode(playerCoords.x, playerCoords.y + 50.0, playerCoords.z, 1)

    if not found then
        drawNotification("~r~No suitable road found for ambulance spawn.")
        return
    end

    -- Load models
    RequestModel("sprinter22")
    RequestModel("s_m_m_paramedic_01")
    while not HasModelLoaded("sprinter22") or not HasModelLoaded("s_m_m_paramedic_01") do
        Wait(100)
    end
    
    -- Spawn vehicle on road
    npcAmbulance = CreateVehicle("sprinter22", roadCoord.x, roadCoord.y, roadCoord.z, GetEntityHeading(playerPed) + 180, true, true)
    SetVehicleOnGroundProperly(npcAmbulance)
    
    -- Create blip
    ambulanceBlip = AddBlipForEntity(npcAmbulance)
    SetBlipSprite(ambulanceBlip, 1) -- Round circle blip
    SetBlipColour(ambulanceBlip, 5) -- Yellow color
    SetBlipAsShortRange(ambulanceBlip, true)
    BeginTextCommandSetBlipName("STRING")
    AddTextComponentString("Ambulance")
    EndTextCommandSetBlipName(ambulanceBlip)
    
    -- Spawn NPC driver
    npcDriver = CreatePedInsideVehicle(npcAmbulance, 4, GetHashKey("s_m_m_paramedic_01"), -1, true, false)
    SetDriverAbility(npcDriver, 1.0)
    SetDriverAggressiveness(npcDriver, 0.0)
    
    -- Drive to player
    TaskVehicleDriveToCoordLongrange(npcDriver, npcAmbulance, playerCoords.x, playerCoords.y, playerCoords.z, 20.0, 786475, 3.0)
    
    -- Monitor when player enters passenger seat
    Citizen.CreateThread(function()
        while npcAmbulance do
            Wait(1000)
            if IsPedInVehicle(playerPed, npcAmbulance, false) then
                trackingPlayer = playerPed
                FollowWaypoint()
                break
            end
        end
    end)
end, false)

function FollowWaypoint()
    Citizen.CreateThread(function()
        local stuckCounter = 0

        while npcAmbulance and trackingPlayer do
            Wait(1000) -- Check every second
            local waypoint = GetFirstBlipInfoId(8)

            if DoesBlipExist(waypoint) then
                local coords = GetBlipCoords(waypoint)
                TaskVehicleDriveToCoordLongrange(npcDriver, npcAmbulance, coords.x, coords.y, coords.z, 20.0, 786469, 3.0) -- Smooth driving mode
            end

            -- Check if player exits the vehicle
            if not IsPedInVehicle(trackingPlayer, npcAmbulance, false) then
                TaskVehicleDriveWander(npcDriver, npcAmbulance, 0.0, 786468) -- Stop movement but keep NPC in the car
                SetVehicleHandbrake(npcAmbulance, true) -- Apply handbrake
                StartDespawnTimer()
            else
                if despawnTimer then
                    despawnTimer = nil
                end

                -- Stuck detection: Check if the vehicle speed is too low
                local speed = GetEntitySpeed(npcAmbulance)
                if speed < 0.5 then
                    stuckCounter = stuckCounter + 1
                else
                    stuckCounter = 0
                end

                if stuckCounter >= 5 then -- If vehicle is stuck for 5 seconds
                    local heading = GetEntityHeading(npcAmbulance)
                    local forwardVector = vector3(math.cos(math.rad(heading)), math.sin(math.rad(heading)), 0.0)
                    local rightVector = vector3(-forwardVector.y, forwardVector.x, 0.0) -- Perpendicular for left/right movement
                    local pushAmount = 2.0 -- Adjust push strength

                    -- Randomly select a direction
                    local direction = math.random(1, 4) -- 1=Forward, 2=Backward, 3=Left, 4=Right
                    local moveOffset = vector3(0.0, 0.0, 0.0)

                    if direction == 1 then
                        moveOffset = forwardVector * pushAmount -- Forward
                    elseif direction == 2 then
                        moveOffset = forwardVector * -pushAmount -- Backward
                    elseif direction == 3 then
                        moveOffset = rightVector * -pushAmount -- Left
                    elseif direction == 4 then
                        moveOffset = rightVector * pushAmount -- Right
                    end

                    -- Apply the nudge
                    SetEntityCoords(npcAmbulance, GetEntityCoords(npcAmbulance) + moveOffset, true, true, true, false)
                    stuckCounter = 0 -- Reset counter
                end
            end
        end
    end)
end

function StartDespawnTimer()
    if despawnTimer then return end
    despawnTimer = GetGameTimer() + (2 * 60 * 1000) -- 5 minutes
    
    Citizen.CreateThread(function()
        while npcAmbulance and GetGameTimer() < despawnTimer do
            Wait(5000)
            if IsPedInAnyVehicle(trackingPlayer, false) then
                return
            end
        end
        if npcAmbulance then
            local despawnOffset = GetOffsetFromEntityInWorldCoords(npcAmbulance, 0.0, 50.0, 0.0)
            TaskVehicleDriveToCoordLongrange(npcDriver, npcAmbulance, despawnOffset.x, despawnOffset.y, despawnOffset.z, 20.0, 786468, 5.0)
            Wait(10000)
            DeleteEntity(npcAmbulance)
            DeleteEntity(npcDriver)
            RemoveBlip(ambulanceBlip)
            npcAmbulance = nil
            npcDriver = nil
            trackingPlayer = nil
            ambulanceBlip = nil
        end
    end)
end

-- Teleport ambulance near player on F7 key press
local tpcooldown = 30 -- Cooldown time in seconds
local lastF7PressTime = 0 -- Store the last press time

Citizen.CreateThread(function()
    while true do
        Wait(0)
        if IsControlJustReleased(0, 168) then -- F7 key
            local currentTime = GetGameTimer() / 1000
            if currentTime - lastF7PressTime < tpcooldown then
                local remainingTime = math.ceil(tpcooldown - (currentTime - lastF7PressTime))
                drawNotification("~r~You must wait " .. remainingTime .. " seconds before using F7 again.")
                return
            end

            lastF7PressTime = currentTime

            if npcAmbulance then
                local playerPed = PlayerPedId()
                local teleportOffset = GetOffsetFromEntityInWorldCoords(playerPed, 3.0, 0.0, 0.0) -- 3 ft away
                SetEntityCoords(npcAmbulance, teleportOffset.x, teleportOffset.y, teleportOffset.z, false, false, false, true)
                SetVehicleOnGroundProperly(npcAmbulance)
            end
        end
    end
end)

function drawNotification(text)
    SetNotificationTextEntry("STRING")
    AddTextComponentString(text)
    DrawNotification(false, false)
end