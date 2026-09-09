-- smallp13's addition to scba marker and drawtext location

Citizen.CreateThread(function()
    local coords = {
        vector3(1164.3984, -1493.9532, 34.8518), -- Whitechapel
        vector3(-349.019714, 6122.737305, 31.485289), -- Croydon
        vector3(1918.181152, 4567.957031, 38.858089), -- Romford
        vector3(325.735413, 3391.863770, 36.613930) -- East Ham
    }

    local displayText = "SCBA Recharge"

    while true do
        Citizen.Wait(0)  -- Prevent freezing

        local playerPed = PlayerPedId()
        local playerPos = GetEntityCoords(playerPed)

        for _, coord in ipairs(coords) do
            local distance = #(playerPos - coord) 

            if distance < 5.0 then
                DrawMarker(1, coord.x, coord.y, coord.z - 1.0, 0, 0, 0, 0, 0, 0, 1.0, 1.0, 2.0, 232, 232, 0, 50, false, true, 2, false, nil, nil, false)
                local onScreen, _x, _y = World3dToScreen2d(coord.x, coord.y, coord.z + 1.0)
                if onScreen then
                    SetTextFont(4)
                    SetTextProportional(1)
                    SetTextScale(0.5, 0.5)
                    SetTextColour(250, 250, 250, 255)
                    SetTextOutline()
                    SetTextCentre(true)
                    SetTextEntry("STRING")
                    AddTextComponentString(displayText)
                    DrawText(_x, _y)
                end
            end
        end
    end
end)
