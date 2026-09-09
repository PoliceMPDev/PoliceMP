local teleportPoints = {
    { name = "Level 1", location = vector3(-510.826263, -2903.03735, 5.42822742), heading = 315.87 },
    { name = "Level 2", location = vector3(-475.061981, -2817.82983, -29.1365318), heading = 315.87 },
    { name = "Level 3", location = vector3(-475.061981, -2817.82983, -19.4293385), heading = 315.87 },
    { name = "Level 4", location = vector3(-475.061981, -2817.82983, -39.4907379), heading = 315.87 },
    { name = "Level 5", location = vector3(-475.061981, -2817.82983, -48.7502327), heading = 315.87 }
}

local teleportRadius = 2.0
local isMenuOpen = false
local nearTeleportPoint = nil

-- Proximity Detection Loop
Citizen.CreateThread(function()
    local playerPed, playerCoords
    while true do
        playerPed = PlayerPedId()
        playerCoords = GetEntityCoords(playerPed)
        nearTeleportPoint = nil

        for _, point in ipairs(teleportPoints) do
            if #(playerCoords - point.location) < teleportRadius then
                nearTeleportPoint = point
                break
            end
        end

        Wait(250) -- Check every 250ms to reduce load
    end
end)

-- Interaction Logic
Citizen.CreateThread(function()
    while true do
        if nearTeleportPoint and not isMenuOpen then
            DrawText3D(nearTeleportPoint.location.x, nearTeleportPoint.location.y, nearTeleportPoint.location.z, "Press ~g~E~w~ to open teleport menu")

            if IsControlJustReleased(0, 38) then -- E key
                OpenTeleportMenu(PlayerPedId(), nearTeleportPoint)
            end
        end

        Wait(0) -- Minimal resource usage in this loop
    end
end)

-- Open Teleport Menu
function OpenTeleportMenu(playerPed, currentPoint)
    isMenuOpen = true
    local options = {}

    for _, point in ipairs(teleportPoints) do
        if point ~= currentPoint then
            table.insert(options, {
                label = point.name,
                coords = point.location,
                heading = point.heading
            })
        end
    end

    Citizen.CreateThread(function()
        local selectedOption = 1
        local maxOptions = #options

        while isMenuOpen do
            -- Close menu if the player moves too far
            if #(GetEntityCoords(playerPed) - currentPoint.location) > teleportRadius then
                isMenuOpen = false
                break
            end

            for i, option in ipairs(options) do
                local verticalSpacing = 0.5 - (i * 0.2)
                local text = i == selectedOption and ("> " .. option.label .. " <") or option.label
                local scale = i == selectedOption and 0.7 or 0.6
                DrawText3DWithScale(currentPoint.location.x, currentPoint.location.y, currentPoint.location.z + verticalSpacing, text, scale)
            end

            if IsControlJustReleased(0, 172) then -- Arrow Up
                selectedOption = (selectedOption - 2 + maxOptions) % maxOptions + 1
            elseif IsControlJustReleased(0, 173) then -- Arrow Down
                selectedOption = (selectedOption % maxOptions) + 1
            elseif IsControlJustReleased(0, 201) then -- Enter
                TeleportPlayer(playerPed, options[selectedOption].coords, options[selectedOption].heading)
                isMenuOpen = false
            elseif IsControlJustReleased(0, 202) then -- Backspace
                isMenuOpen = false
            end

            Wait(0) -- Low-cost loop for menu navigation
        end
    end)
end

-- Teleport Player
function TeleportPlayer(playerPed, coords, heading)
    local vehicle = GetVehiclePedIsIn(playerPed, false)
    if vehicle ~= 0 then
        SetEntityCoords(vehicle, coords.x, coords.y, coords.z, false, false, false, true)
        SetEntityHeading(vehicle, heading)
    else
        SetEntityCoords(playerPed, coords.x, coords.y, coords.z, false, false, false, true)
        SetEntityHeading(playerPed, heading)
    end
end

-- Draw Text Functions
function DrawText3DWithScale(x, y, z, text, scale)
    local onScreen, _x, _y = World3dToScreen2d(x, y, z)
    if onScreen then
        SetTextScale(scale, scale)
        SetTextFont(4)
        SetTextProportional(1)
        SetTextColour(255, 255, 255, 215)
        SetTextEntry("STRING")
        SetTextCentre(1)
        AddTextComponentString(text)
        DrawText(_x, _y)
    end
end

function DrawText3D(x, y, z, text)
    DrawText3DWithScale(x, y, z, text, 0.6)
end
