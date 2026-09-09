function GetDiscordID(source)
    local discordIdentifier = GetPlayerIdentifierByType(source, "discord")
    if discordIdentifier then
        return string.sub(discordIdentifier, 9)  -- Remove "discord:" prefix
    end
    return nil
end

local MILEAGE_KEY = "mileage_data"

RegisterNetEvent("mileage:save")
AddEventHandler("mileage:save", function(playerId, vehicleHash, mileage)
    local _source = source -- Get the real source
    local discordId = GetDiscordID(_source) -- Use _source, not playerId
    if discordId then
        local data = json.decode(GetResourceKvpString(MILEAGE_KEY) or "{}")

        if not data[discordId] then
            data[discordId] = {}
        end

        local playerData = data[discordId]
        playerData[tostring(vehicleHash)] = (playerData[tostring(vehicleHash)] or 0.0) + mileage

        SetResourceKvp(MILEAGE_KEY, json.encode(data))
    end
end)

RegisterNetEvent("mileage:fetch")
AddEventHandler("mileage:fetch", function(playerId, vehicleHash)
    local _source = source -- Get the real source
    local discordId = GetDiscordID(_source) -- Use _source, not playerId
    if discordId then
        local data = json.decode(GetResourceKvpString(MILEAGE_KEY) or "{}")

        local mileage = 0.0
        if data[discordId] and data[discordId][tostring(vehicleHash)] then
            mileage = data[discordId][tostring(vehicleHash)]
        end

        TriggerClientEvent("mileage:show", _source, vehicleHash, mileage)
    end
end)

RegisterCommand("highestmileage", function(source, args, rawCommand)
    if not IsPlayerAceAllowed(source, "group.TierTwo") then
        TriggerClientEvent("chat:addMessage", source, { args = { "Mileage", "^1You do not have permission to use this command." } })
        return
    end

    local rawData = GetResourceKvpString("mileage_data")
    if not rawData then
        TriggerClientEvent("chat:addMessage", source, { args = { "Mileage", "No mileage data found." } })
        return
    end

    local data = json.decode(rawData)
    if not data then
        TriggerClientEvent("chat:addMessage", source, { args = { "Mileage", "Mileage data is empty or corrupted." } })
        return
    end

    local highestMileage = 0
    local highestDiscordId = nil

    -- Find the highest mileage
    for discordId, playerData in pairs(data) do
        for _, mileage in pairs(playerData) do
            if mileage > highestMileage then
                highestMileage = mileage
                highestDiscordId = discordId
            end
        end
    end

    if highestDiscordId then
        local playerName = GetPlayerNameByDiscordID(highestDiscordId)
        local mileageInMiles = highestMileage * 0.000621371 -- Convert meters to miles
        TriggerClientEvent("chat:addMessage", -1, {
            args = { "🏎️ Mileage Leaderboard", 
                ("🥇 ^3%s^7 (Discord ID: `%s`) has the highest mileage of ^2%.2f miles^7! 🏆")
                :format(playerName, highestDiscordId, mileageInMiles) }
        })
    else
        TriggerClientEvent("chat:addMessage", source, { args = { "Mileage", "No mileage records found." } })
    end
end, false)

-- Function to get player name by Discord ID (if they are online)
function GetPlayerNameByDiscordID(discordId)
    for _, playerId in ipairs(GetPlayers()) do
        local identifier = GetPlayerIdentifierByType(playerId, "discord")
        if identifier and string.sub(identifier, 9) == discordId then
            return GetPlayerName(playerId) -- Return in-game name
        end
    end
    return "N/A" -- If not found, show "Unknown Player"
end

