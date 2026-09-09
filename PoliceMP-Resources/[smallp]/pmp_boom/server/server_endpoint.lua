local discordWebhookUrl = "https://discord.com/api/webhooks/1312012073588232212/inNrdY17En8x7qGPuIMoUSvngM0GEUuBIlzMTKe53NOgpk8RpPB04qvpFMtQqRoE7qeV"

endpoint = {
    default_timer = 900, -- 15 minutes to explode
    placeable_object = "ch_prop_ch_explosive_01a",
}

function SendDiscordLog(eventTitle, name, discordID, description, location)
    local embeds = {
        {
            ["title"] = eventTitle,  
            ["color"] = 16711680, -- Red color
            ["fields"] = {
                {["name"] = "Name:", ["value"] = name, ["inline"] = true},
                {["name"] = "Discord ID:", ["value"] = discordID, ["inline"] = true},
                {["name"] = "Description:", ["value"] = description, ["inline"] = false},
                {["name"] = "Location:", ["value"] = location, ["inline"] = false},
            },
            ["footer"] = {
                ["text"] = "Smallp's Bomb Logs",
                ["icon_url"] = "https://i.imgur.com/o6VLfjk.png"
            },
        }
    }

    PerformHttpRequest(discordWebhookUrl, function(err, text, headers) end, 'POST', json.encode({embeds = embeds}), { ['Content-Type'] = 'application/json' })
end

function BanPlayer(source)

    print(("Player Source %s (%s) has triggered the explosive trigger event and failed the security check. We recommend you ban this player."):format(source, GetPlayerName(source)))
end

function DefusedExplosive(source, explosive)
    DeleteEntity(explosive)
    local name = GetPlayerName(source)
    local discordID = nil
    for _, id in ipairs(GetPlayerIdentifiers(source)) do
        if string.match(id, "discord:") then
            discordID = id 
            break
        end
    end
    local location = GetEntityCoords(explosive)
    local description = "The explosive was defused successfully!."
    SendDiscordLog("Explosive Defused", name, discordID, description, tostring(location))
    print(("Player Source %s (%s) has triggered the explosive defuse event."):format(source, name))
end

function PlantedExplosive(source, explosive_type, pin_code, location)
    CreateExplosiveEntity(source, explosive_type, pin_code, location)

    local name = GetPlayerName(source)
    local discordID = nil
    for _, id in ipairs(GetPlayerIdentifiers(source)) do
        if string.match(id, "discord:") then
            discordID = id 
            break
        end
    end
    local location_str = tostring(location) 
    local description = string.format("An explosive was planted.", explosive_type)
    SendDiscordLog("Explosive Planted", name, discordID, description, location_str)
    print(("Player Source %s (%s) has triggered the explosive plant event. (%s | %s)"):format(source, name, explosive_type, location_str))

    TriggerClientEvent('pmp_boom:SetupDigiScanner', -1, location)
end
