local locations = {}

-- Please place your Discord webhook here
config.main.logging.webhook = "https://discord.com/api/webhooks/927376928460128306/CoG4y_5HQHMzVITJ1BJiUQmrif-fcGsxrdhz1LFtjR4CX3s6dk9q1EgVAILrNo7VRfEV" 

if config.main.vRP.enabled then
    Proxy = module("vrp", "lib/Proxy")
    vRP = Proxy.getInterface("vRP")
end

if config.main.ESX.enabled then
    ESX = nil
    TriggerEvent('esx:getSharedObject', function(obj) ESX = obj end)
end

if config.main.QBCore.enabled then
    QBCore = exports["qb-core"]:GetCoreObject()
end

Citizen.CreateThread(function()
    local jsonFile = LoadResourceFile(GetCurrentResourceName(), "locations.json")
    locations = json.decode(jsonFile)
    for k, v in pairs(locations) do
        if v.defaultText.firstLine ~= nil then
            for k2, v2 in pairs(v.defaultText) do
                if v2 ~= nil and string.len(v2) < 15 then
                    if k2 == "firstLine" then
                        locations[k].defaultText.firstLine = string.lower(v2)
                    elseif k2 == "secondLine" then
                        locations[k].defaultText.secondLine = string.lower(v2)
                    elseif k2 == "thirdLine" then
                        locations[k].defaultText.thirdLine = string.lower(v2)
                    end
                else
                    if k2 == "firstLine" then
                        locations[k].defaultText.firstLine = ""
                    elseif k2 == "secondLine" then
                        locations[k].defaultText.secondLine = ""
                    elseif k2 == "thirdLine" then
                        locations[k].defaultText.thirdLine = ""
                    end
                end
            end
        end
    end
end)

function updateSign(source, signId, text, streetName)
    local source = source
    local permission = not(config.main.vRP.enabled or config.main.ESX.enabled or config.main.QBCore.enabled or config.main.acePermissions.enabled)
    if source == nil then
        permission = true
    end
    -- Add any additional permission checks here.
    -- We've pre-configured Ace Permissions, vRP, ESX and QBCore

    -- vRP Permission Integration (if enabled in config)
    if config.main.vRP.enabled then
        local user_id = vRP.getUserId({source})
        if config.main.vRP.checkPermission.enabled and not permission then
            -- Permission Check (if enabled in config)
            for k, v in pairs(config.main.vRP.checkPermission.permissions) do
                if vRP.hasPermission({user_id,v}) then
                    permission = true
                    break
                end
            end
        end
            -- Group Check (if enabled in config)
        if config.main.vRP.checkGroup.enabled and not permission then
            for k, v in pairs(config.main.vRP.checkGroup.groups) do
                if vRP.hasGroup({user_id,v}) then
                    permission = true
                    break
                end
            end
        end
    end

    -- ESX Permission Integration (if enabled in config)
    if config.main.ESX.enabled then
        local xPlayer = ESX.GetPlayerFromId(source)
        if config.main.ESX.checkJob.enabled then
            for k, v in pairs(config.main.ESX.checkJob.jobs) do
                if xPlayer.getJob().name == v then
                    permission = true
                    break
                end
            end
        end
    end

    -- Ace Permission Integration (if enabled in config)
    if config.main.acePermissions.enabled then
        if IsPlayerAceAllowed(source, config.main.acePermissions.permission) then
            permission = true
        end
    end

    -- QBCore Permission
    if config.main.QBCore.enabled then
        local player = QBCore.Functions.GetPlayer(source)
        if config.main.QBCore.checkJob.enabled and not permission then
            for k, v in pairs(config.main.QBCore.checkJob.jobs) do
                if player.PlayerData.job.name == v then
                    permission = true
                    break
                end
            end
        end
        if config.main.QBCore.checkPermission.enabled and not permission then
            for k, v in pairs(config.main.QBCore.checkPermission.permissions) do
                if QBCore.Functions.HasPermission(source, v) then
                    permission = true
                    break
                end
            end
        end
    end
    
    if permission then processSign(source, signId, text, streetName) end
end
RegisterNetEvent("SmartSigns:updateSign")
AddEventHandler("SmartSigns:updateSign", function(signId, text, streetName)
    local source = source
    updateSign(source, signId, text, streetName)
end)

function processSign(source, signId, text, streetName)
    local text = text
    local streetName = streetName
    for i = 1, config.letterPositions.numberOfLines do
        if text[i] == nil then text[i] = "" end
    end

    for i = 1, config.letterPositions.numberOfLines do -- each level of text
        if config.main.developerMode then
            print("[Smart Signs] - Formatted string before: "..defaultText[i].." for Sign ID: "..signId)
        end
        local formattedLine = text[i]

        for bannedWord = 1, #config.main.bannedWords do
            if string.find(formattedLine, config.main.bannedWords[bannedWord]) then
                if config.main.logging.bannedWordLogs.enabled then
                    if streetName == nil then
                        if config.main.developerMode then
                            print("[Smart Signs] - Street Name Not Received - Undefined")
                        end
                        local streetName = "" 
                    end
                    bannedWordDetection(source, signId, config.main.bannedWords[bannedWord].." (line "..i..")", formattedLine, streetName)
                end
            end
            formattedLine = formattedLine:gsub(config.main.bannedWords[bannedWord], " ")
            if config.main.developerMode then
                print("[Smart Signs] - Formatted string after banned word removal: "..formattedLine)
            end
        end
        for s = 1, #text[i] do
            local c = text[i]:sub(s,s)
            if config.main.developerMode then
                print("[Smart Signs] - Checking character: "..c.." for Sign ID: "..signId)
            end

            if c == "." or config.letterModels[c] == nil then
                if config.main.developerMode then
                    print("[Smart Signs] - Invalid character found: "..c.." Message before: "..formattedLine)
                end
                formattedLine = string.gsub(formattedLine, "%"..c, " ")
                if config.main.developerMode then
                    print("[Smart Signs] - Invalid character removed. Message after: "..formattedLine)
                end
            end
        end
        if config.main.developerMode then
            print("[Smart Signs] - Formatted string for Sign ID: "..signId.." Line: "..i.." - Formatted: "..formattedLine)
        end
        if i == 1 then
            locations[signId].defaultText.firstLine = formattedLine
        elseif i == 2 then
            locations[signId].defaultText.secondLine = formattedLine
        elseif i == 3 then
            locations[signId].defaultText.thirdLine = formattedLine
        end
        if config.main.saveChangesToJson then
            SaveResourceFile(GetCurrentResourceName(), "locations.json", json.encode(locations, {indent = true}), -1)
        end
    end
    if config.main.logging.enabled then
        if streetName == nil then
            if config.main.developerMode then
                print("[Smart Signs] - Street Name Not Received - Undefined")
            end
            local streetName = "" 
        end
        if config.main.developerMode then
            print("[Smart Signs] - Discord Webhook Submitted")
        end
        normalLog(source, signId, locations[signId], streetName)
    end
    TriggerClientEvent("SmartSigns:syncSignsClient", -1, signId, locations[signId].defaultText)
end

RegisterServerEvent("SmartSigns:requestDataServer")
AddEventHandler("SmartSigns:requestDataServer", function()
    local source = source
    TriggerClientEvent("SmartSigns:signDataClient", source, locations)
    TriggerClientEvent("SmartSigns:DataReceived", source)
    if config.main.developerMode then
        print("[Smart Signs] - Source: "..source.." - Requesting Sign Data Upon Joining")
    end
end)

function normalLog(source, signId, text, streetName)
    local value = ""
    for line = 1, config.letterPositions.numberOfLines do
        if line == 1 then
            line = "firstLine"
        elseif line == 2 then
            line = "secondLine"
        elseif line == 3 then
            line = "thirdLine"
        end
        value = value .. "\n" .. text.defaultText[line]
    end

    local embed = {
          {
              ["fields"] = {
                {
                    ["name"] = "**Player:**",
                    ["value"] = GetPlayerName(source),
                    ["inline"] = true
                },
                {
                    ["name"] = "**Sign ID:**",
                    ["value"] = signId,
                    ["inline"] = true
                },
                {
                    ["name"] = "**Street Name:**",
                    ["value"] = tostring(streetName),
                    ["inline"] = false
                },
                {
                    ["name"] = "**Sign Message:**",
                    ["value"] = value,
                    ["inline"] = false
                },
              },
              ["color"] = config.main.logging.colour,
              ["title"] = config.main.logging.title,
              ["description"] = "",
              ["footer"] = {
                  ["text"] = "Timestamp: "..os.date(config.main.logging.dateFormat),
                  ["icon_url"] = config.main.logging.footerIcon,
              },
              ["thumbnail"] = {
                  ["url"] = config.main.logging.icon,
              },
          }
      }

    PerformHttpRequest(config.main.logging.webhook, function(err, text, headers) end, 'POST', json.encode({username = config.main.logging.displayName, embeds = embed}), { ['Content-Type'] = 'application/json' })
end

function bannedWordDetection(source, signId, word, fullMessage, streetName)
    local embed = {
          {
              ["fields"] = {
                {
                    ["name"] = "**Player:**",
                    ["value"] = GetPlayerName(source),
                    ["inline"] = true
                },
                {
                    ["name"] = "**Sign ID:**",
                    ["value"] = signId,
                    ["inline"] = true
                },
                {
                    ["name"] = "**Street Name:**",
                    ["value"] = tostring(streetName),
                    ["inline"] = false
                },
                {
                    ["name"] = "**Banned Word:**",
                    ["value"] = word,
                    ["inline"] = false
                },
                {
                    ["name"] = "**Full Line:**",
                    ["value"] = fullMessage,
                    ["inline"] = false
                },
              },
              ["color"] = config.main.logging.bannedWordLogs.colour,
              ["title"] = "Banned Word Detection",
              ["description"] = "",
              ["footer"] = {
                  ["text"] = "Timestamp: "..os.date(config.main.logging.dateFormat),
                  ["icon_url"] = config.main.logging.footerIcon,
              },
              ["thumbnail"] = {
                  ["url"] = config.main.logging.icon,
              },
          }
      }

    PerformHttpRequest(config.main.logging.webhook, function(err, text, headers) end, 'POST', json.encode({username = config.main.logging.displayName, embeds = embed}), { ['Content-Type'] = 'application/json' })
end

exports('SmartSigns:GetSigns', function()
    local exportSigns = {};
    for i,v in ipairs(locations) do 
        exportSigns[i] = {
            ["id"] = i,
            ["x"] = v.coords.z,
            ["y"] = v.coords.y, 
            ["z"] = v.coords.z,
            ["defaultText"] = v.defaultText 
        }
    end
    return exportSigns
end)

RegisterNetEvent("SmartSigns:apiUpdateSign")
AddEventHandler("SmartSigns:apiUpdateSign", function(signId, text)
    if source == nil then
        updateSign(nil, signId, text, nil)
    end
end)

