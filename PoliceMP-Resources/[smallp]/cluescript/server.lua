local json = require("json") 

local clues = {} 

local discordWebhookUrl = "https://discord.com/api/webhooks/1291400505658048603/f4dT_FnhI9G2L-_9TKDtB2HaxMaBHG2h4t3wFaqQtuN0_BeUKB5-13Hk7DDhEJprqPCF"

local function sendToDiscord(clueText, playerName, playerID, discordID, actionType)
    local actionMessage = actionType == "deleted" and "Clue Deleted" or "New Clue Submitted" -- Set the action message
    local embedColor = actionType == "deleted" and 16711680 or 5814783 -- Set color to red for deleted clues
    
    local data = {
        embeds = {
            {
                title = actionMessage,
                color = embedColor, 
                fields = {
                    {
                        name = "Clue",
                        value = clueText, 
                        inline = false
                    },
                    {
                        name = "Player Name",
                        value = playerName,
                        inline = true
                    },
                    {
                        name = "Player ID",
                        value = tostring(playerID),
                        inline = true
                    },
                    {
                        name = "Discord ID", 
                        value = discordID or "Not Available", 
                        inline = true
                    }
                },
                footer = {
                    text = "Clue Logger",
                    icon_url = "https://live.staticflickr.com/65535/54040149994_0ab65927d0.jpg" 
                },
                timestamp = os.date("!%Y-%m-%dT%H:%M:%SZ") 
            }
        }
    }

    PerformHttpRequest(discordWebhookUrl, function(err, text, headers) end, 'POST', json.encode(data), { ['Content-Type'] = 'application/json' })
end


local function clearDatabase()
    local file = io.open("resources/[PoliceMP-Resources]/[smallp]/cluescript/clues.json", "w") 
    if file then
        file:write(json.encode({})) 
        file:close()
        print("Database cleared successfully.") 
    else
        print("Failed to open database file.") 
    end
end

AddEventHandler("onResourceStart", function(resourceName)
    if resourceName == GetCurrentResourceName() then
        clearDatabase() 
        loadCluesFromFile() 
    end
end)

function saveCluesToFile()
    local file = io.open("resources/[PoliceMP-Resources]/[MiscResources]/cluescript/clues.json", "w") 
    if file then
        local jsonData = json.encode(clues)
        local formattedJson = jsonData:gsub("{", "{\n"):gsub("}", "\n}"):gsub(",", ",\n")
        file:write(formattedJson)
        file:close()
    end
end

function loadCluesFromFile()
    local file = io.open("resources/[PoliceMP-Resources]/[MiscResources]/cluescript/clues.json", "r") 
    if file then
        local contents = file:read("*a")
        clues = json.decode(contents) or {} 
        file:close()
        TriggerClientEvent("clue:loadClues", -1, clues) 
    end
end

function notifyClientsAboutClues()
    TriggerClientEvent("clue:loadClues", -1, clues) 
end

RegisterNetEvent("clue:add")
AddEventHandler("clue:add", function(clue)
    local id = #clues + 1
    clues[id] = clue -- Add the new clue to the table
    saveCluesToFile() -- Save updated clues
    notifyClientsAboutClues() -- Refresh all clients with the updated list

    local playerID = source
    local playerName = GetPlayerName(playerID)
    local discordID = GetPlayerIdentifierByType(playerID, "discord")
    sendToDiscord(clue, playerName, playerID, discordID, "added")
end)

RegisterNetEvent("clue:request")
AddEventHandler("clue:request", function()
    local _source = source
    if IsPlayerAceAllowed(_source, "Civ.Trained") then
        TriggerClientEvent("clue:openNUI", _source) -- Allow UI to open for the user
    end
end)

Citizen.CreateThread(function()
    loadCluesFromFile() -- Load existing clues from file
end)

RegisterNetEvent("clue:delete")
AddEventHandler("clue:delete", function(clueID)
    local playerID = source 
    local playerName = GetPlayerName(playerID) 
    local discordID = GetPlayerIdentifierByType(playerID, "discord") 
    
    if clues[clueID] then
        local deletedClue = clues[clueID] 
        clues[clueID] = nil 
        saveCluesToFile() 
        notifyClientsAboutClues() 

        sendToDiscord(deletedClue.text, playerName, playerID, discordID, "deleted") 
    end
end)

RegisterNetEvent("clue:logMessage")
AddEventHandler("clue:logMessage", function(clue, playerID)
    local playerName = GetPlayerName(source) 
    local discordID = GetPlayerIdentifierByType(source, "discord") 

    sendToDiscord(clue, playerName, playerID, discordID)  
end)