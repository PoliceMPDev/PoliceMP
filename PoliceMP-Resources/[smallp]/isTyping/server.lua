local typingPlayers = {}

RegisterNetEvent('chatstatus:updateTyping')
AddEventHandler('chatstatus:updateTyping', function(isTyping)
    local source = source
    typingPlayers[source] = isTyping

    TriggerClientEvent('chatstatus:syncTyping', -1, typingPlayers)
end)

AddEventHandler('playerDropped', function()
    local source = source
    typingPlayers[source] = nil
    TriggerClientEvent('chatstatus:syncTyping', -1, typingPlayers)
end)
