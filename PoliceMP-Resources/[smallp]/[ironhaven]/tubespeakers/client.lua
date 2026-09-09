local soundRadius = 100.0 -- Radius in feet
local soundVolume = 0.5 -- Maximum volume

RegisterNetEvent("myresource:playSoundAtLocationForEveryone")
AddEventHandler("myresource:playSoundAtLocationForEveryone", function(soundName, soundFile, soundCoords)
    local playerPed = PlayerPedId()
    local playerCoords = GetEntityCoords(playerPed)

    -- Calculate distance to the sound source
    local distance = #(playerCoords - soundCoords)

    if distance <= soundRadius then
        local volume = math.max(0.0, soundVolume * (1.0 - (distance / soundRadius))) -- Adjust volume based on distance
        local resourceSoundPath = 'nui://' .. GetCurrentResourceName() .. '/sounds/' .. soundFile
        exports.xsound:PlayUrlPos(soundName, resourceSoundPath, volume, soundCoords)
        exports.xsound:Distance(soundName, soundRadius) -- Set the sound radius
    end
end)

RegisterNetEvent("myresource:drawNotification")
AddEventHandler("myresource:drawNotification", function(message)
    SetNotificationTextEntry("STRING")
    AddTextComponentString(message)
    DrawNotification(false, true)
end)
