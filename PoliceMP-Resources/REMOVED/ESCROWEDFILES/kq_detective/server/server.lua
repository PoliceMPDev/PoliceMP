RegisterNetEvent('kq_detective:saveInfo')
AddEventHandler('kq_detective:saveInfo', function(data)
    local _source = source
    
    for k, info in pairs(data) do
        if k == 'timeOfDeath' then
            info = GetGameTimer()
        end
        Entity(GetPlayerPed(_source)).state['kq_detective_' .. k] = info
    end
end)

Citizen.CreateThread(function()
    while true do
        Citizen.Wait(30000)
        SetConvarReplicated('kq_server_time' , GetGameTimer())
    end
end)
