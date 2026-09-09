local tokens = {}

local char_set = {}
for i = 48,  57 do table.insert(char_set, string.char(i)) end
for i = 65,  90 do table.insert(char_set, string.char(i)) end
for i = 97, 122 do table.insert(char_set, string.char(i)) end

local function GetRandomToken(length)
    local str = ''

    for i = 1, length do
        str = str .. char_set[math.random(1, #char_set)]
    end

    return str
end

local function ValidateToken(source, passed_token)
	if tokens[source] and tokens[source] == passed_token then
		return true
	else
		return false
	end
end

local created_explosives = {}

RegisterNetEvent('PoliceMP:Boom:Plant')
AddEventHandler('PoliceMP:Boom:Plant', function(sec_table)
	if ValidateToken(source, sec_table.P1) then
		PlantedExplosive(source, sec_table.explosive_type, sec_table.pin_code, sec_table.explosive_location)
	else
		BanPlayer(source)
	end
end)

local function GetClosestExplosiveById(id)
	for k, v in pairs(created_explosives) do
		if v.id == id then
			return { key = k, entity = v.entity, value = v }
		end
	end

	return nil
end

RegisterNetEvent('PoliceMP:Boom:Defuse')
AddEventHandler('PoliceMP:Boom:Defuse', function(sec_table)
	if ValidateToken(source, sec_table.P1) then
		local explosive = GetClosestExplosiveById(sec_table.explosive_id)
		if explosive and explosive.entity then
			table.remove(created_explosives, explosive.key)
			DefusedExplosive(source, explosive.entity)
		end
	else
		BanPlayer(source)
	end
end)

AddEventHandler('playerJoining', function()
	local assigned_token = GetRandomToken(16)
	tokens[source] = assigned_token

	TriggerClientEvent('PoliceMP:Boom:Login', source, assigned_token)
end)

AddEventHandler('onServerResourceStart', function(_resource)
	if _resource == GetCurrentResourceName() then
		Citizen.Wait(1000)

		for _, _id in ipairs(GetPlayers()) do
			local source = tonumber(_id)
			local assigned_token = GetRandomToken(16)
			tokens[source] = assigned_token

			TriggerClientEvent('PoliceMP:Boom:Login', source, assigned_token)
		end
	end
end)

local function TriggerExplosive(explosive)
	if DoesEntityExist(explosive.entity) then
		local entity_coords = GetEntityCoords(explosive.entity)
		TriggerClientEvent('PoliceMP:Boom:ExplodeAtCoordinate', -1, explosive.explosive_type, entity_coords)
		DeleteEntity(explosive.entity)
	end
end

function CreateExplosiveEntity(source, explosive_type, pin_code, location)
    local explosive_object = CreateObject(GetHashKey(endpoint.placeable_object), location.x, location.y, location.z - 1.0, true, true, true)

    while not DoesEntityExist(explosive_object) do
        Citizen.Wait(100)
    end

    local id = GetRandomToken(7)

    local entity = Entity(explosive_object)
    entity.state.explosive_timer = endpoint.default_timer
    entity.state.explosive_code = pin_code
    entity.state.explosive_id = id

    table.insert(created_explosives, {
        entity = explosive_object,
        location = location,
        pin_code = pin_code,
        explosive_type = explosive_type,
        timer = endpoint.default_timer,
        id = id,
        frozen = false -- Initialize frozen state
    })
end

Citizen.CreateThread(function()
    while true do
        Citizen.Wait(1000)

        for k, v in pairs(created_explosives) do
            if DoesEntityExist(v.entity) then
                if not v.frozen then -- Skip countdown if the timer is frozen
                    v.timer = v.timer - 1
                    local entity = Entity(v.entity)
                    entity.state.explosive_timer = v.timer

                    if v.timer <= 0 then
                        TriggerExplosive(v)
                        local coords = GetEntityCoords(v.entity)
                        local location = string.format("vec3(%.6f, %.6f, %.6f)", coords.x, coords.y, coords.z)
                        local description = "An explosive has detonated."
                        SendDiscordLog("Explosive Detonated", "Explosive Tracker", "N/A", description, location)
                        table.remove(created_explosives, k)
                    end
                end
            else
                table.remove(created_explosives, k)
            end
        end
    end
end)

RegisterNetEvent('PoliceMP:Boom:Explode')
AddEventHandler('PoliceMP:Boom:Explode', function(sec_table)
    local explosive = GetClosestExplosiveById(sec_table.explosive_id)
    if explosive and explosive.entity then
        local name = GetPlayerName(source)
        local discordID = nil
        for _, id in ipairs(GetPlayerIdentifiers(source)) do
            if string.match(id, "discord:") then
                discordID = id 
                break
            end
        end
        local location = GetEntityCoords(explosive.entity)
        local description = "The explosive has exploded."
        SendDiscordLog("Explosive Triggered", name, discordID, description, tostring(location))
        print(("Player Source %s (%s) has triggered the explosive detonation."):format(source, name))
        TriggerExplosive(explosive.value)
        table.remove(created_explosives, explosive.key)
    else
        print("Explosive not found.")
    end
end)

-- Server-Side Ace Perms
local bombPlanted = false -- Flag to track if a bomb is planted
RegisterCommand('+xbplant', function(source, args, rawCommand)
    local player = source
    if IsPlayerAceAllowed(player, "SeniorCiv.Trained") then
        if not bombPlanted then
            bombPlanted = true -- Set the flag to true
            TriggerClientEvent('pmp_boom:plant', player)

            CreateThread(function()
                Wait(600000) -- 10 mins
                bombPlanted = false
            end)
        else
            TriggerClientEvent('basicNotification', player, "~r~A bomb is already planted!")
        end
    else
        TriggerClientEvent('basicNotification', player, "~r~Nice Try! Dr. Samurai will yeet you!")
    end
end, false)

RegisterCommand('+xbdefuse', function(source, args, rawCommand)
    local player = source
    if IsPlayerAceAllowed(player, "SeniorCiv.Trained") or IsPlayerAceAllowed(player, "Police.afoTrained") then
        TriggerClientEvent('pmp_boom:defuse', player)
    else
        TriggerClientEvent('basicNotification', player, "~r~You are not EOD Trained!")
    end
end, false)

RegisterCommand('+xbtimer', function(source, args, rawCommand)
    if not IsPlayerAceAllowed(source, "SeniorCiv.Trained") then
        TriggerClientEvent('basicNotification', source, "~r~You do not have permission to use this command!")
        return
    end

    local additionalTime = tonumber(args[1])
    if not additionalTime then
        TriggerClientEvent('basicNotification', source, "~r~Usage: /btimer <additional_time>")
        return
    end

    local playerPed = GetPlayerPed(source)
    local playerCoords = GetEntityCoords(playerPed)

    local closestExplosive = nil
    local closestDistance = math.huge

    for _, explosive in pairs(created_explosives) do
        if DoesEntityExist(explosive.entity) then
            local explosiveCoords = GetEntityCoords(explosive.entity)
            local distance = #(playerCoords - explosiveCoords)

            if distance < closestDistance then
                closestDistance = distance
                closestExplosive = explosive
            end
        end
    end

    if not closestExplosive or closestDistance > 500.0 then -- Adjust range as needed
        TriggerClientEvent('basicNotification', source, "~r~No explosives found nearby!")
        return
    end

    closestExplosive.timer = closestExplosive.timer + additionalTime

    local entity = Entity(closestExplosive.entity)
    entity.state.explosive_timer = closestExplosive.timer

    local playerName = GetPlayerName(source)
    local discordID = nil
    for _, identifier in ipairs(GetPlayerIdentifiers(source)) do
        if string.match(identifier, "discord:") then
            discordID = identifier:gsub("discord:", "") -- Extract Discord ID
            break
        end
    end

    local location = GetEntityCoords(closestExplosive.entity)
    local description = ("The timer for the explosive was increased by %d seconds!"):format(additionalTime)
    SendDiscordLog("Explosive Timer Adjusted", playerName, discordID, description, tostring(location))

    TriggerClientEvent('basicNotification', source, ("~y~Timer for the explosive increased by %d seconds."):format(additionalTime))
end, false)

RegisterCommand('+xbfreeze', function(source, args, rawCommand)
    if not IsPlayerAceAllowed(source, "SeniorCiv.Trained") then
        TriggerClientEvent('basicNotification', source, "~r~You do not have permission to use this command!")
        return
    end

    local playerPed = GetPlayerPed(source)
    local playerCoords = GetEntityCoords(playerPed)

    local closestExplosive = nil
    local closestDistance = math.huge

    for _, explosive in pairs(created_explosives) do
        if DoesEntityExist(explosive.entity) then
            local explosiveCoords = GetEntityCoords(explosive.entity)
            local distance = #(playerCoords - explosiveCoords)

            if distance < closestDistance then
                closestDistance = distance
                closestExplosive = explosive
            end
        end
    end

    if not closestExplosive or closestDistance > 500.0 then -- Adjust range as needed
        TriggerClientEvent('basicNotification', source, "~r~No explosives found nearby!")
        return
    end

    closestExplosive.frozen = not closestExplosive.frozen

    local stateMessage = closestExplosive.frozen and "FROZEN" or "UNFROZEN"
    TriggerClientEvent('basicNotification', source, ("~y~The explosive's timer is now %s."):format(stateMessage))

    local playerName = GetPlayerName(source)
    local coords = GetEntityCoords(closestExplosive.entity)
    local location = string.format("vec3(%.6f, %.6f, %.6f)", coords.x, coords.y, coords.z)
    local description = ("The explosive's timer was %s."):format(stateMessage)
    local discordID = nil
    for _, identifier in ipairs(GetPlayerIdentifiers(source)) do
        if string.match(identifier, "discord:") then
            discordID = identifier:gsub("discord:", "") -- Extract Discord ID
            break
        end
    end
    SendDiscordLog("Explosive Freeze Timer", playerName, discordID, description, location)
end, false)


RegisterCommand('+xbremote', function(source, args, rawCommand)
    if not IsPlayerAceAllowed(source, "SeniorCiv.Trained") then
        TriggerClientEvent('basicNotification', source, "~r~You do not have permission to use this command!")
        return
    end

    local playerPed = GetPlayerPed(source)
    local playerCoords = GetEntityCoords(playerPed)

    local closestExplosive = nil
    local closestDistance = math.huge

    for _, explosive in pairs(created_explosives) do
        if DoesEntityExist(explosive.entity) then
            local explosiveCoords = GetEntityCoords(explosive.entity)
            local distance = #(playerCoords - explosiveCoords)

            if distance < closestDistance then
                closestDistance = distance
                closestExplosive = explosive
            end
        end
    end

    if not closestExplosive or closestDistance > 500.0 then -- Adjust range as needed
        TriggerClientEvent('basicNotification', source, "~r~No explosives found nearby!")
        return
    end

    TriggerExplosive(closestExplosive)

    local playerName = GetPlayerName(source)
    local discordID = nil
    for _, identifier in ipairs(GetPlayerIdentifiers(source)) do
        if string.match(identifier, "discord:") then
            discordID = identifier:gsub("discord:", "") -- Extract Discord ID
            break
        end
    end

    local location = GetEntityCoords(closestExplosive.entity)
    local description = "The explosive was triggered remotely!"
    SendDiscordLog("Explosive Triggered Remotely", playerName, discordID, description, tostring(location))

    table.remove(created_explosives, closestExplosive.key)

    TriggerClientEvent('basicNotification', source, "~y~Explosive has been remote triggered!")
end, false)
