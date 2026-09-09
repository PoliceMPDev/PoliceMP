if framework.ESX.enabled then
    ESX = exports["es_extended"]:getSharedObject()
end

if framework.vRP.enabled then
    local Tunnel = module("vrp", "lib/Tunnel")
    local Proxy = module("vrp", "lib/Proxy")
    vRP = Proxy.getInterface("vRP")
    vRPclient = Tunnel.getInterface("vRP","vRP")
end
local QBCore = nil
if framework.QBCore.enabled then
    QBCore = exports['qb-core']:GetCoreObject()
end

local webhook = "https://discord.com/api/webhooks/927376066237059112/Dfd5bexVHKYNZdqhwpR9r7l6pguTbUdAAbKYjwDJ1FdzdaMxwiGn0EZBUPFwHlH3jdLu"

local mph = true
local units = "mph"
if main.useKmh ~= nil then
    mph = not main.useKmh
    if not mph then units = "km/h" end
end

RegisterServerEvent("Server:AverageSpeedDetection")
AddEventHandler("Server:AverageSpeedDetection",function(cameraId, speed, roadName, numberplate)
    local source = source
    if main.enableDiscordLogs then
        logToDiscord(source, cameraId, speed, roadName, numberplate)
    end
    -- Add permission checks here or link in with your database for vRP / ESX
    -- For example, you could fine the player or record their fine in a database
    local police = getOnlinePolice()
    if main.finePlayer then
        if framework.ESX.enabled then
            if not framework.ESX.ESXBilling.enabled then
                local xPlayer = ESX.GetPlayerFromId(source)
                local cash = xPlayer.getMoney()
                local fineAmount = math.random(framework.ESX.fineAmount[1], framework.ESX.fineAmount[2])
                if cash >= fineAmount then
                    addSocietyMoney(fineAmount)
                    xPlayer.removeMoney(fineAmount)
                    if main.notifications.ESX then
                        xPlayer.showNotification(translations.fined..translations.currency..tostring(fineAmount), false, false, 90)      
                    elseif main.notifications.mythicNotify then
                        TriggerClientEvent('mythic_notify:client:SendAlert', source, { type = 'inform', text = translations.fined..translations.currency..tostring(fineAmount), length = 2500, style = { ['background-color'] = '#ffffff', ['color'] = '#6f6ee7' } })
                    end
                end
            end
        end

        if framework.vRP.enabled then
            local userid = vRP.getUserId({source})
            local fineAmount = math.random(framework.vRP.fineAmount[1], framework.vRP.fineAmount[2])
            if vRP.tryFullPayment({userid,fineAmount}) then
                if main.notifications.vRP then
                    vRPclient.notify(source, {"You have been fined " .. translations.currency .. fineAmount})
                elseif main.notifications.mythicNotify then
                    TriggerClientEvent('mythic_notify:client:SendAlert', source, { type = 'inform', text = translations.fined..translations.currency..tostring(fineAmount), length = 2500, style = { ['background-color'] = '#ffffff', ['color'] = '#6f6ee7' } })
                end
                
            end
        end

        if framework.QBCore.enabled then
            local Player = QBCore.Functions.GetPlayer(source)
            local fineAmount = math.random(framework.QBCore.fineAmount[1], framework.QBCore.fineAmount[2])
            if Player.Functions.RemoveMoney('bank', fineAmount) then
                addSocietyMoney(fineAmount)
                if main.notifications.QBCore then
                    TriggerClientEvent('QBCore:Notify', source, "You have been fined " .. translations.currency .. fineAmount)
                elseif main.notifications.mythicNotify then
                    TriggerClientEvent('mythic_notify:client:SendAlert', source, { type = 'inform', text = translations.fined..translations.currency..tostring(fineAmount), length = 2500, style = { ['background-color'] = '#ffffff', ['color'] = '#6f6ee7' } })
                end
            elseif Player.Functions.RemoveMoney('cash', fineAmount) then
                addSocietyMoney(fineAmount)
                if main.notifications.QBCore then
                    TriggerClientEvent('QBCore:Notify', source, "You have been fined " .. translations.currency .. fineAmount)
                elseif main.notifications.mythicNotify then
                    TriggerClientEvent('mythic_notify:client:SendAlert', source, { type = 'inform', text = translations.fined..translations.currency..tostring(fineAmount), length = 2500, style = { ['background-color'] = '#ffffff', ['color'] = '#6f6ee7' } })
                end
            end
        end
    end

    if main.notifyPolice then
        for source, _ in pairs(police) do
            if main.notifications.QBCore then
                TriggerClientEvent('QBCore:Notify', source, string.format(translations.caughtSpeeding, roadName,numberplate))
            elseif main.notifications.mythicNotify then
                TriggerClientEvent('mythic_notify:client:SendAlert', source, { type = 'inform', text = string.format(translations.caughtSpeeding, roadName,numberplate), length = 2500, style = { ['background-color'] = '#ffffff', ['color'] = '#6f6ee7' } })
            elseif main.notifications.vRP then
                vRPclient.notify(source, {string.format(translations.caughtSpeeding, roadName,numberplate)})
            elseif main.notifications.ESX then
                local xPlayer = ESX.GetPlayerFromId(source)
                xPlayer.showNotification(string.format(translations.caughtSpeeding, roadName,numberplate), false, false, 90)   
            end
        end
    end
end)

function addSocietyMoney(fineAmount)
    if Societies.qbBossmenu then
        TriggerEvent('qb-bossmenu:server:addAccountMoney', Societies.accountName, fineAmount)
    elseif Societies.qbManagement then
        return exports['qb-management']:AddMoney(Societies.accountName, fineAmount)
    elseif Societies.esxAddonAccount then
        TriggerEvent('esx_addonaccount:getSharedAccount', Societies.accountName, function(account)
            if account ~= nil then
                account.addMoney(fineAmount)
            else
                print("ERROR: " .. Societies.accountName .. " requires a society in order to receive money!")
            end
        end)
    end
end

function mathRound(value, numDecimalPlaces)
    if numDecimalPlaces then
        local power = 10^numDecimalPlaces
        return math.floor((value * power) + 0.5) / (power)
    else
        return math.floor(value + 0.5)
    end
end

function logToDiscord(source, cameraId, speed, roadName, numberplate)

    local name = GetPlayerName(source)
    local time = os.date("*t")
    local day = os.date("%A")

    if time.month < 10 then
        time.month = "0" .. time.month
    end
    local embed = {
        {
            ["fields"] = {
                {
                    ["name"] = "**"..translations.name.."**",
                    ["value"] = name,
                    ["inline"] = true
                },
                {
                    ["name"] = "**"..translations.cameraId.."**",
                    ["value"] = cameraId * 12,
                    ["inline"] = true
                },
                {
                    ["name"] = "**"..translations.speedLimit.."**",
                    ["value"] = config[cameraId].limit.." "..units,
                    ["inline"] = true
                },
                {
                    ["name"] = "**"..translations.speedDetected.."**",
                    ["value"] = mathRound(speed, 1).." "..units,
                    ["inline"] = true
                },
                {
                    ["name"] = "**"..translations.roadName.."**",
                    ["value"] = roadName,
                    ["inline"] = true
                },
                {
                    ["name"] = "**"..translations.numberPlate.."**",
                    ["value"] = numberplate,
                    ["inline"] = true
                },
            },
            ["color"] = 16767002,
            ["title"] = "**"..translations.cameraActivation.."**",
            ["description"] = "",
            ["footer"] = {
                ["text"] = translations.timestamp.. day .. " " .. time.day .. "/" .. time.month .. "/" .. time.year
            },
            ["thumbnail"] = {
                ["url"] = main.webhookImage,
            },
        }
    }
    PerformHttpRequest(webhook, function(err, text, headers) end, 'POST', json.encode({username = main.webhookName, embeds = embed}), { ['Content-Type'] = 'application/json' })
end

function getOnlinePolice()
    local policeOnline = {}
    if framework.ESX.enabled then
        local players = GetPlayers()
        for k, v in pairs(players) do
            local xPlayer = ESX.GetPlayerFromId(v)
            for k2, v2 in pairs(main.ESX.policeJobNames) do
                if xPlayer.job.name == v2 then
                    policeOnline[v] = true
                    break
                end
            end
        end
    end
    if framework.QBCore.enabled then
        local qbPlayers = QBCore.Functions.GetQBPlayers()
        for k, v in pairs(qbPlayers) do
            local jobName = v.PlayerData.job.name
            for k2, v2 in pairs(main.QBCore.policeJobNames) do
                if jobName == v2 then
                    policeOnline[v.PlayerData.source] = true
                    break
                end
            end
        end
    end
    if framework.vRP.enabled then
        local players = GetPlayers()
        for k, v in pairs(players) do
            local userid = vRP.getUserId({v})
            for k2, v2 in pairs(main.vRP.policeJobNames) do
                if vRP.hasGroup({userid, v2}) then
                    policeOnline[v] = true
                end
            end
        end
    end

    return policeOnline
    
end
