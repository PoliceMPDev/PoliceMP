local host = nil
local serversideFreight = nil
local serverMetro = nil
local serverMetro2 = nil
local discordWebhook = "https://discord.com/api/webhooks/1319610840869113917/INhlaWEBVYwbgYC47y-GVl2Gd1sK2x7CjqwWL9xzy5jjJBZpfkeju2kpLpSVIMhtnEWS"

local trainStates = {
    freightPos = nil,
    metroPos = nil,
    metro2Pos = nil
}

RegisterCommand("spawntrains", function(source, args, rawCommand)
    if IsPlayerAceAllowed(source, "SeniorCiv.Trained") or IsPlayerAceAllowed(source, "Police.modAuth") then
        if host ~= nil then
            TriggerClientEvent("ShowNotification", source, "A host already exists.")
            return
        end
        
        local title = "Spawn Trains Command Executed"
        local playerName = GetPlayerName(source)
        local discordId = GetDiscordIdentifier(source)
        local reason = "Player has spawned trains and is now the host!"

        sendToDiscord(title, playerName, discordId, reason)

        host = source
        TriggerClientEvent("k_trains:SpawnHostTrains", host)
        TriggerClientEvent("ShowNotification", source, "You are now the train host.")
    else
        TriggerClientEvent("ShowNotification", source, "Not Allowed!")
    end
end, false)


RegisterCommand("stoptrains", function(source, args, rawCommand)
    if IsPlayerAceAllowed(source, "Civ.Trained") or IsPlayerAceAllowed(source, "Police.modAuth") then
        if host == nil then
            return
        end

        local title = "Trains has stopped moving"
        local playerName = GetPlayerName(source)
        local discordId = GetDiscordIdentifier(source)
        local reason = "Player has stopped trains!!"

        sendToDiscord(title, playerName, discordId, reason)

        TriggerClientEvent("k_trains:stopTrains", host)
        TriggerClientEvent("ShowNotification", source, "Trains have been stopped.")
    else
        TriggerClientEvent("ShowNotification", source, "Not Allowed!")
    end
end, false)

RegisterCommand("starttrains", function(source, args, rawCommand)
    if IsPlayerAceAllowed(source, "Civ.Trained") or IsPlayerAceAllowed(source, "Police.modAuth") then
        if host == nil then
            return
        end

        local title = "Trains has started moving"
        local playerName = GetPlayerName(source)
        local discordId = GetDiscordIdentifier(source)
        local reason = "Player has started the trains!"

        sendToDiscord(title, playerName, discordId, reason)

        TriggerClientEvent("k_trains:startTrains", host)
        TriggerClientEvent("ShowNotification", source, "Trains have been started.")
    else
        TriggerClientEvent("ShowNotification", source, "Not Allowed")
    end
end, false)

RegisterCommand("cleartrains", function(source, args, rawCommand)
    if IsPlayerAceAllowed(source, "SeniorCiv.Trained") or IsPlayerAceAllowed(source, "Police.modAuth") then
        local trainModels = {
            GetHashKey("metrotrain"),  -- Metro train
            GetHashKey("freight"),    -- Freight train engine
            GetHashKey("freightcar"), -- Freight train car
            GetHashKey("freightcont1"), -- Freight container car
            GetHashKey("freightcont2"), -- Freight container car
            GetHashKey("freightgrain"), -- Freight grain car
            GetHashKey("tankercar"), -- Freight tanker car
        }

        local allVehicles = GetAllVehicles()
        local deletedCount = 0

        for _, vehicle in ipairs(allVehicles) do
            if DoesEntityExist(vehicle) then
                local model = GetEntityModel(vehicle)
                for _, trainModel in ipairs(trainModels) do
                    if model == trainModel then
                        DeleteEntity(vehicle)
                        deletedCount = deletedCount + 1
                        break
                    end
                end
            end
        end
        
        local title = "Clear Trains Command Executed"
        local playerName = GetPlayerName(source)
        local discordId = GetDiscordIdentifier(source)
        local reason = "Player has initited a clear trains command!"

        sendToDiscord(title, playerName, discordId, reason)

        host = nil
        TriggerEvent("train:clearBlips")
        TriggerClientEvent("ShowNotification", source, string.format("All trains have been stopped and deleted (%d trains removed)", deletedCount))
    else
        TriggerClientEvent("ShowNotification", source, "You do not have permission to use this command.")
    end
end, false)

RegisterNetEvent("train:clearBlips")
AddEventHandler("train:clearBlips", function()
    -- Notify all clients to clear train blips
    TriggerClientEvent("train:removeBlips", -1)
end)


RegisterNetEvent("train:updateBlips")
AddEventHandler("train:updateBlips", function(freightPos, metroPos, metro2Pos)
    -- Update the train state on the server
    trainStates.freightPos = freightPos
    trainStates.metroPos = metroPos
    trainStates.metro2Pos = metro2Pos

    -- Broadcast the positions to all clients
    TriggerClientEvent("train:syncBlips", -1, freightPos, metroPos, metro2Pos)
end)



RegisterNetEvent("k_trains:SetPlayerPos")
AddEventHandler("k_trains:SetPlayerPos", function(Pos, sec)
    if (sec == "Dg^6n@G7fM7KjR#Dizzd17BIJof") then
        SetEntityCoords(GetPlayerPed(source), Pos.x, Pos.y,
            Pos.z + 1.0, false, false, false, false)
    else
        DropPlayer(source, "Do not try to cheat ;D")
    end
end)


function indexOf(array, value)
    for i, v in ipairs(array) do
        if v == value then
            return i
        end
    end
    return nil
end

AddEventHandler('playerDropped', function(reason)
    if source == host then
        host = nil
        local trainModels = {
            GetHashKey("metrotrain"),  -- Metro train
            GetHashKey("freight"),    -- Freight train engine
            GetHashKey("freightcar"), -- Freight train car
            GetHashKey("freightcont1"), -- Freight container car
            GetHashKey("freightcont2"), -- Freight container car
            GetHashKey("freightgrain"), -- Freight grain car
            GetHashKey("tankercar"), -- Freight tanker car
        }

        local allVehicles = GetAllVehicles()
        local deletedCount = 0

        for _, vehicle in ipairs(allVehicles) do
            if DoesEntityExist(vehicle) then
                local model = GetEntityModel(vehicle)
                for _, trainModel in ipairs(trainModels) do
                    if model == trainModel then
                        DeleteEntity(vehicle)
                        deletedCount = deletedCount + 1
                        break
                    end
                end
            end
        end

        local title = "Train host and player dropped!!"
        local playerName = GetPlayerName(source)
        local discordId = GetDiscordIdentifier(source)
        local reason = "Player has disconnected and host is now available"

        sendToDiscord(title, playerName, discordId, reason)

        TriggerClientEvent("k_trains:checkAndDeleteTrains", -1) -- Deletes all trains for all clients
        print("Host disconnected. Trains deleted.")
    end
end)

RegisterNetEvent('Train:SyncDoors')
AddEventHandler('Train:SyncDoors', function(trainNetId, isOpening)
    TriggerClientEvent('Train:SyncDoors', -1, trainNetId, isOpening)
end)

Citizen.CreateThread(function()
    while true do
        Citizen.Wait(10000) -- Check every 10 seconds (adjust as needed)

        if host ~= nil then
        else
            TriggerClientEvent("k_trains:checkAndDeleteTrains", -1)
        end
    end
end)

-- Function to get the player's Discord identifier
function GetDiscordIdentifier(player)
    for _, identifier in ipairs(GetPlayerIdentifiers(player)) do
        if string.find(identifier, "discord:") then
            return identifier:gsub("discord:", "")
        end
    end
    return "N/A"
end

-- Function to send a log to Discord
function sendToDiscord(title, playerName, discordId, reason)
    local embed = {
        {
            ["color"] = 16711680, -- Red color
            ["title"] = title,
            ["fields"] = {
                {["name"] = "Player Name", ["value"] = playerName, ["inline"] = true},
                {["name"] = "Discord ID", ["value"] = discordId, ["inline"] = true},
                {["name"] = "Reason", ["value"] = reason, ["inline"] = false}
            },
            ["footer"] = {
                ["text"] = os.date("%Y-%m-%d %H:%M:%S")
            }
        }
    }

    PerformHttpRequest(discordWebhook, function(err, text, headers) end, "POST", json.encode({embeds = embed}), {["Content-Type"] = "application/json"})
end

local trainBlipPositions = {
    freight = nil,
    metro = nil,
    metro2 = nil
}

RegisterNetEvent("train:updateBlipPosition")
AddEventHandler("train:updateBlipPosition", function(trainType, position)
    trainBlipPositions[trainType] = position

    -- Broadcast to all players
    TriggerClientEvent("train:syncBlips", -1, trainBlipPositions)
end)

AddEventHandler("playerConnecting", function()
    local src = source
    TriggerClientEvent("train:syncBlips", src, trainBlipPositions)
end)
