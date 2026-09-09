RegisterNetEvent('customchat:sendGlobalMessage')
AddEventHandler('customchat:sendGlobalMessage', function(message)
    -- Broadcast the message globally
    TriggerClientEvent('chat:addMessage', -1, {
        multiline = true,
        args = {message}
    })
end)
