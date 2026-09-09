-- Put your webhook link here
-- This is due to the config being a shared script
logging.webhook = "https://discord.com/api/webhooks/969161157002809374/yQR5hg8JvvxH7fPEM1kAOm_3EU-uboK3NTKkH1SWa8Y5A3nOGGbqR_bmPTvgrk_SZy8B"

if permissions.vRP.enabled then
    Proxy = module("vrp", "lib/Proxy")
    vRP = Proxy.getInterface("vRP")
end

if permissions.ESX.enabled then
    ESX = nil
    TriggerEvent('esx:getSharedObject', function(obj) ESX = obj end)
end

if permissions.QBCore.enabled then
    QBCore = exports["qb-core"]:GetCoreObject()
end

-- Command registered server side below:
RegisterCommand("eod", function(source, args, rawCommand)
    local source = source
    local permission = false
    if not (permissions.vRP.enabled or permissions.ESX.enabled or permissions.acePermissions.enabled or permissions.QBCore.enabled) then
        permission = true
    end

    if source > 0 then
        if permissions.vRP.enabled then
            local user_id = vRP.getUserId({source})
            if permissions.vRP.checkPermission.enabled then
                -- Permission Check (if enabled in config)
                for k, v in pairs(pconermissions.vRP.checkPermission.permissions) do
                    if vRP.hasPermission({user_id,v}) then
                        permission = true
                    end
                end
            end
                -- Group Check (if enabled in config)
            if permissions.vRP.checkGroup.enabled then
                for k, v in pairs(permissions.vRP.checkGroup.groups) do
                    if vRP.hasGroup({user_id,v}) then
                        permission = true
                    end
                end
            end
        end
    
        -- ESX Permission Integration (if enabled in config)
        if permissions.ESX.enabled then
            local xPlayer = ESX.GetPlayerFromId(source)
            for k, v in pairs(permissions.ESX.checkJob.jobs) do
                if xPlayer.job.name == v then
                    permission = true
                end
            end
        end
    
        -- Ace Permission Integration (if enabled in config)
        if permissions.acePermissions.enabled then
            if IsPlayerAceAllowed(source, permissions.acePermissions.permission) then
                permission = true
            end
        end

        -- QBCore Permission
        if permissions.QBCore.enabled then
            local player = QBCore.Functions.GetPlayer(source)
            if permissions.QBCore.checkJob.enabled then
                for k, v in pairs(permissions.QBCore.checkJob.jobs) do
                    if player.PlayerData.job.name == v then
                        permission = true
                    end
                end
            end
            if permissions.QBCore.checkPermission.enabled then
                for k, v in pairs(permissions.QBCore.checkPermission.permissions) do
                    if QBCore.Functions.HasPermission(source, v) then
                        permission = true
                    end
                end
            end
        end
        
        if permission then 
            TriggerClientEvent("toggleEOD", source)
            normalLog(source)
        end
    end
end, false)

function normalLog(source)
    local embed = {
          {
              ["fields"] = {
                {
                    ["name"] = "**Player:**",
                    ["value"] = GetPlayerName(source),
                    ["inline"] = true
                },
              },
              ["color"] = logging.colour,
              ["title"] = logging.title,
              ["description"] = "",
              ["footer"] = {
                  ["text"] = "Timestamp: "..os.date(logging.dateFormat),
                  ["icon_url"] = logging.footerIcon,
              },
              ["thumbnail"] = {
                  ["url"] = logging.icon,
              },
          }
      }

    PerformHttpRequest(logging.webhook, function(err, text, headers) end, 'POST', json.encode({username = logging.displayName, embeds = embed}), { ['Content-Type'] = 'application/json' })
end

-- We do not recommend editing below this point

RegisterNetEvent("activateHoseServer")
AddEventHandler("activateHoseServer", function(vehicle)
    TriggerClientEvent("activateHose", -1, vehicle)
end)

RegisterNetEvent("deactivateHoseServer")
AddEventHandler("deactivateHoseServer", function(vehicle)
    TriggerClientEvent("deactivateHose", -1, vehicle)
end)