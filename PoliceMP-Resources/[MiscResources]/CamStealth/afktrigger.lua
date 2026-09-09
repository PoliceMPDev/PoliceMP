local isAfk = false
local afkLoopThread = nil

-- Show AFK warning every 10 seconds until cleared
RegisterNetEvent("Playtime:ShowAfkNotification", function()
    if isAfk then return end
    isAfk = true

    afkLoopThread = CreateThread(function()
        while isAfk do
            lib.notify({
                id = 'afk_status',
                title = 'AFK DETECTED!',
                description = 'You are marked as AFK. Move to resume.',
                type = 'error',
                position = 'top-right',
                iconAnimation = 'pulse',
                icon = 'ban',
                showDuration = false
            })
            Wait(5000) -- 10 seconds
        end
    end)
end)

-- Stop showing the AFK warning
RegisterNetEvent("Playtime:AfkClearedNotification", function()
    if not isAfk then return end
    isAfk = false

    -- Show confirmation
    lib.notify({
        title = 'AFK CLEARED',
        description = 'You are no longer marked as AFK.',
        type = 'success',
        position = 'top-right',
        duration = 8000,
        iconAnimation = 'bounce',
        showDuration = false,
    })
end)
