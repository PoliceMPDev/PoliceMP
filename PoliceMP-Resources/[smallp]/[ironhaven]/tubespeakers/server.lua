local trainSoundLocations = {
    {coords = vector3(283.0484, -1204.3373, 38.9021)}, -- Add your train detection points here
    {coords = vector3(-297.5240, -307.6686, 10.0631)},
    {coords = vector3(-808.4702, -136.1023, 19.9503)},
    {coords = vector3(-1346.7855, -471.4881, 15.0454)},
    {coords = vector3(-481.3943, -671.3796, 11.8090)},
    {coords = vector3(-214.5347, -1038.5742, 30.1382)},
    {coords = vector3(112.8925, -1724.6261, 30.1116)},
    {coords = vector3(-541.9816, -1288.5806, 26.8981)},
    {coords = vector3(-881.9276, -2318.6865, -11.7328)},
    {coords = vector3(-1079.8005, -2714.7732, -7.4101)}

}

local trainRadius = 50.0 -- Radius to detect train proximity
local soundCooldown = 30000 -- Cooldown time in milliseconds

Citizen.CreateThread(function()
    while true do
        local vehicles = GetGamePool('CVehicle')
        local currentTime = GetGameTimer()

        for _, vehicle in ipairs(vehicles) do
            if IsTrainModel(GetEntityModel(vehicle)) then
                local trainCoords = GetEntityCoords(vehicle)
                for _, location in ipairs(trainSoundLocations) do
                    local distance = #(trainCoords - location.coords)
                    if distance <= trainRadius and not location.soundPlayed then
                        TriggerClientEvent("myresource:playSoundAtLocationForEveryone", -1, "seeit_sound", "seeit.wav", location.coords)
                        location.soundPlayed = true

                        -- Reset soundPlayed after the cooldown
                        Citizen.SetTimeout(soundCooldown, function()
                            location.soundPlayed = false
                        end)
                    end
                end
            end
        end

        Citizen.Wait(500)
    end
end)

function IsTrainModel(model)
    local trainModels = {
        GetHashKey("freight"),
        GetHashKey("freightcar"),
        GetHashKey("freightcont1"),
        GetHashKey("freightcont2"),
        GetHashKey("freightgrain"),
        GetHashKey("freighttrailer"),
        GetHashKey("metrotrain")
    }
    for _, trainModel in ipairs(trainModels) do
        if model == trainModel then
            return true
        end
    end
    return false
end

RegisterCommand("+tubefire", function(source, args, rawCommand)
    local playerPed = GetPlayerPed(source)
    local playerCoords = GetEntityCoords(playerPed)

    local nearestLocation = nil
    local nearestDistance = math.huge -- Set to a large initial value

    -- Find the nearest train station
    for _, location in ipairs(trainSoundLocations) do
        local distance = #(playerCoords - location.coords)
        if distance < nearestDistance then
            nearestDistance = distance
            nearestLocation = location.coords
        end
    end

    -- Trigger the sound at the nearest station if found
    if nearestLocation then
        TriggerClientEvent("myresource:playSoundAtLocationForEveryone", -1, "tubefire_sound", "tubefire.wav", nearestLocation)
        TriggerClientEvent("myresource:drawNotification", source, "~r~Fire Warning Activated!")
    else
        TriggerClientEvent("myresource:drawNotification", source, "~r~No nearby station found!")
    end
end, false)

RegisterCommand("+armedevac", function(source, args, rawCommand)
    local playerPed = GetPlayerPed(source)
    local playerCoords = GetEntityCoords(playerPed)

    local nearestLocation = nil
    local nearestDistance = math.huge -- Set to a large initial value

    -- Find the nearest train station
    for _, location in ipairs(trainSoundLocations) do
        local distance = #(playerCoords - location.coords)
        if distance < nearestDistance then
            nearestDistance = distance
            nearestLocation = location.coords
        end
    end

    -- Trigger the sound at the nearest station if found
    if nearestLocation then
        TriggerClientEvent("myresource:playSoundAtLocationForEveryone", -1, "armedevac_sound", "armedevac.wav", nearestLocation)
        TriggerClientEvent("myresource:drawNotification", source, "~r~Firearms Evac Activated!")
    else
        TriggerClientEvent("myresource:drawNotification", source, "~r~No nearby station found!")
    end
end, false)