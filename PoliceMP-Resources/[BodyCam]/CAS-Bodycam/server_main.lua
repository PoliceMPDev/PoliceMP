local Videos = {}

CreateThread(function()
    local file = LoadResourceFile(GetCurrentResourceName(), "videopatch.json")
    if file then
        Videos = json.decode(file)
    end
end)

local function getPlayerName(source)
    return GetPlayerName(source) or "Unknown Player"
end

RegisterCommand("+zrecords", function(source)
    if IsPlayerAceAllowed(source, "Police.chiefinspector") then
        TriggerClientEvent("cas-bodycam:action", source, "records", Videos)
    else
        TriggerClientEvent("cas-bodycam:notify", source, "You do not have permissions!")
    end
end)

RegisterCommand("+zbodycam", function(source)
    if IsPlayerAceAllowed(source, "Police.afoTrained") then
        local info = {
            name = getPlayerName(source),
            grade = "" -- grade removed
        }
        TriggerClientEvent("cas-bodycam:action", source, "bodycam", info)
    else
        TriggerClientEvent("cas-bodycam:notify", source, "Access denied: You do not have permissios!")
    end
end)


RegisterServerEvent("sendFileData")
AddEventHandler("sendFileData", function(videoURL, recordName, videoDesc)
    local src = source
    if videoURL ~= nil then
        local newVideo = {
            date = os.date("%Y-%m-%d"),
            hms = os.date("%H:%M:%S"),
            recordName = recordName,
            recordDetails = videoDesc,
            recorder = getPlayerName(src),
            videoLink = videoURL
        }
        table.insert(Videos, newVideo)
        SaveResourceFile(GetCurrentResourceName(), "videopatch.json", json.encode(Videos, { indent = true }), -1)
    end
end)
