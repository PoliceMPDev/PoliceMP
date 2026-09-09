local playerStrobeState = {}
local playerStrobeColor = {}

RegisterCommand("+xstrobe", function(source, args)
    if not IsPlayerAceAllowed(source, "Police.afoTrained") then
        TriggerClientEvent("helmetstrobe:notify", source, "~r~You are not trained to use helmet strobes.")
        return
    end

    local colorArg = args[1] and string.lower(args[1]) or nil
    local colors = {
        red = {255, 0, 0},
        blue = {0, 0, 255},
        green = {0, 255, 0},
        purple = {128, 0, 128}
    }

    if not colorArg and playerStrobeState[source] then
        -- Toggle OFF
        playerStrobeState[source] = false
        TriggerClientEvent("helmetstrobe:syncState", -1, source, false)
        TriggerClientEvent("helmetstrobe:notify", source, "~r~Helmet strobe deactivated.")
        return
    end

    if colorArg and colors[colorArg] then
        playerStrobeColor[source] = colors[colorArg]
        TriggerClientEvent("helmetstrobe:syncColor", -1, source, colors[colorArg])

        if not playerStrobeState[source] then
            playerStrobeState[source] = true
            TriggerClientEvent("helmetstrobe:syncState", -1, source, true)
            TriggerClientEvent("helmetstrobe:notify", source, "~g~Helmet strobe activated (~s~" .. colorArg .. "~g~).")
        else
            TriggerClientEvent("helmetstrobe:notify", source, "~b~Strobe color changed to ~s~" .. colorArg)
        end
    else
        TriggerClientEvent("helmetstrobe:notify", source, "~r~Usage: /strobe red | blue | green | purple")
    end
end)

RegisterServerEvent("helmetstrobe:setState")
AddEventHandler("helmetstrobe:setState", function(state)
    local src = source
    playerStrobeState[src] = state
    TriggerClientEvent("helmetstrobe:syncState", -1, src, state)

    if state then
        local color = playerStrobeColor[src] or {255, 0, 0}
        TriggerClientEvent("helmetstrobe:syncColor", -1, src, color)
    end
end)

RegisterServerEvent("helmetstrobe:setColor")
AddEventHandler("helmetstrobe:setColor", function(colorName)
    local src = source
    local colors = {
        red = {255, 0, 0},
        blue = {0, 0, 255},
        green = {0, 255, 0},
        purple = {128, 0, 128}
    }

    local rgb = colors[colorName] or {255, 0, 0}
    playerStrobeColor[src] = rgb
    TriggerClientEvent("helmetstrobe:syncColor", -1, src, rgb)
end)

AddEventHandler("playerDropped", function()
    local src = source
    playerStrobeState[src] = nil
    playerStrobeColor[src] = nil
    TriggerClientEvent("helmetstrobe:syncState", -1, src, false)
end)

exports('GetPlayerStrobeState', function(playerId)
    return playerStrobeState[playerId]
end)
