-- Load the configuration
Config = Config or {} -- Ensure Config table exists

local tcasActive = false -- Tracks if TCAS alert is active
local notificationText = nil -- Text to display
local notificationEndTime = 0 -- Time when the notification should disappear

local lastPostal = nil -- Variable to track the last postal the player was in
local lastAlertTime = 0 -- Tracks the time when the last alert sound was played

-- Function to check if the vehicle is an aircraft (plane or helicopter)
function IsAircraft(vehicle)
    local model = GetEntityModel(vehicle)
    return IsThisModelAPlane(model) or IsThisModelAHeli(model)
end

-- Utility function to play sound without `isPlaying` or position
local function playSoundDirectly(soundUrl, soundId)
    if Config.Debug then
        print(string.format("Attempting to play sound: %s (ID: %s)", soundUrl, soundId))
    end

    local success, errorMsg = pcall(function()
        exports.xsound:PlayUrl(soundId, soundUrl, Config.Volume, Config.Loop) -- Removed positioning
    end)

    if success and Config.Debug then
        print("Sound is playing successfully.")
    elseif not success then
        print("Error while playing sound: " .. errorMsg)
    end
end

-- Utility function to stop sound without `isPlaying`
local function stopSound(soundId)
    local success, errorMsg = pcall(function()
        exports.xsound:Destroy(soundId)
        if Config.Debug then
            print(string.format("Destroyed sound: %s", soundId))
        end
    end)

    if not success then
        print("Error while stopping sound: " .. errorMsg)
    end
end

-- Function to show notification on the right side of the screen for a set duration
local function ShowRightSideNotification(text, duration)
    notificationText = text
    notificationEndTime = GetGameTimer() + duration -- Set the end time for the notification
end

-- Function to detect nearby aircraft and trigger TCAS alerts
function DetectNearbyAircraft()
    local playerPed = PlayerPedId()
    local playerVehicle = GetVehiclePedIsIn(playerPed, false)

    if IsAircraft(playerVehicle) then
        local playerCoords = GetEntityCoords(playerVehicle)
        local foundAircraft = false -- Tracks if another aircraft is nearby

        local allVehicles = GetGamePool('CVehicle')

        for _, nearbyVehicle in ipairs(allVehicles) do
            if IsAircraft(nearbyVehicle) and nearbyVehicle ~= playerVehicle then
                local nearbyCoords = GetEntityCoords(nearbyVehicle)
                local distance = #(playerCoords - nearbyCoords)

                if Config.Debug then
                    print(string.format("Nearby vehicle detected. Distance: %.2f meters, Vehicle ID: %s", distance, nearbyVehicle))
                end

                if distance <= Config.TCASAlertDistance then
                    foundAircraft = true
                    if not tcasActive then
                        playSoundDirectly(Config.TCASAlertSoundURL, "tcas_alert_" .. math.random(1000))
                        tcasActive = true
                        ShowRightSideNotification("TCAS Alert: Aircraft detected nearby!", 5000)
                        if Config.Debug then
                            print("TCAS Alert Triggered.")
                        end
                    end
                    break
                end
            else
                if Config.Debug then
                    print("Ignored own vehicle or non-aircraft.")
                end
            end
        end

        if not foundAircraft and tcasActive then
            if Config.Debug then
                print("No aircraft nearby, playing 'Clear of Conflict' sound.")
            end
            stopSound("tcas_alert")
            playSoundDirectly(Config.ClearOfConflictSoundURL, "clear_conflict_" .. math.random(1000))
            tcasActive = false
            ShowRightSideNotification("Clear of Conflict: No nearby aircraft.", 5000)
            if Config.Debug then
                print("Clear of Conflict Triggered.")
            end
        else
            if Config.Debug then
                print("No need to trigger 'Clear of Conflict', aircraft still detected or TCAS not active.")
            end
        end
    end
end

-- Function to check the player's current postal and play a sound if it's restricted
function CheckPostal()
    local playerPed = PlayerPedId()
    local playerVehicle = GetVehiclePedIsIn(playerPed, false)

    if IsAircraft(playerVehicle) then -- Check if the player is in an aircraft
        local postal = exports['nearest-postal']:getPostal()
        local currentTime = GetGameTimer()

        if Config.PostalList[tostring(postal)] then
            if lastPostal ~= postal then
                -- New postal detected, play sound immediately
                if Config.Debug then
                    print("Player entered restricted postal: " .. postal)
                end
                ShowRightSideNotification("Warning: You are in a restricted area (Postal: " .. postal .. ")", Config.PostalAlertDuration)
                playSoundDirectly(Config.PostalAlertSoundURL, "postal_alert_" .. math.random(1000))
                lastAlertTime = currentTime -- Update last alert time
            elseif currentTime - lastAlertTime >= 10000 then -- 10 seconds in milliseconds
                -- Play the sound again if 10 seconds have passed and player is still in the same postal
                if Config.Debug then
                    print("Player still in restricted postal: " .. postal .. " (10 seconds elapsed)")
                end
                ShowRightSideNotification("Warning: You are still in a restricted area (Postal: " .. postal .. ")", Config.PostalAlertDuration)
                playSoundDirectly(Config.PostalAlertSoundURL, "postal_alert_" .. math.random(1000))
                lastAlertTime = currentTime -- Update last alert time
            end
        end

        -- Update the lastPostal variable to the current postal
        lastPostal = postal
    else
        -- Reset lastPostal if the player is not in an aircraft
        lastPostal = nil
    end
end

-- Thread to periodically check nearby aircraft
Citizen.CreateThread(function()
    while true do
        Citizen.Wait(5000) -- Check every 1 second
        DetectNearbyAircraft()
        CheckPostal() -- Check postal each cycle
    end
end)

-- Thread to handle displaying notifications
Citizen.CreateThread(function()
    while true do
        Citizen.Wait(0) -- Keep checking for notifications

        if notificationText and GetGameTimer() < notificationEndTime then
            local x, y = 0.85, 0.5 -- X is near the right side, Y is centered vertically
            
            SetTextFont(4)
            SetTextProportional(1)
            SetTextScale(0.5, 0.5)
            SetTextColour(255, 255, 255, 255)
            SetTextOutline()
            SetTextEntry("STRING")
            AddTextComponentString(notificationText)
            DrawText(x, y)
        end
    end
end)
