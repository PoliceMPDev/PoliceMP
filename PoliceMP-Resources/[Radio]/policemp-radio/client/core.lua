RegisterNetEvent('pma-voice:police-mp-user-toggle-talking')

AddEventHandler('pma-voice:police-mp-user-toggle-talking', function(radioUser, talking, channel, name)
    SendNUIMessage({
        type = 'radio-voice-toggle',
        user = radioUser,
        talking = talking,
        channel = channel,
        name = name,
    })
end)

RegisterNUICallback('close-modification', function(_, callback)
    SetNuiFocus(false, false)

    TriggerEvent('policemp-radio:stop-modification')

    callback()
end)

AddEventHandler('rp-radio:police-mp-toggle-modification', function(status)
    if status == true then
        SetNuiFocus(true, true)
    else
        SetNuiFocus(false, false)
    end

    SendNUIMessage({
        type = 'toggle-modification',
        status = status,
    })
end)

RegisterNUICallback('power-on', function(_, callback)
    TriggerEvent('policemp-radio:power-on')

    callback()
end)

RegisterNUICallback('power-off', function(_, callback)
    TriggerEvent('policemp-radio:power-off')

    callback()
end)

RegisterNUICallback('change-channel', function(data, callback)
    TriggerEvent('policemp-radio:change-channel', data.channel)

    callback()
end)

RegisterNUICallback('change-channel', function(data, callback)
    TriggerEvent('policemp-radio:change-channel', data.channel)

    callback()
end)

Citizen.CreateThread(function()
  while true do
    local radioChannel = Player(source).state['radioChannel']
    
    print(radiochannel)

    SendNUIMessage({
      type = 'update-radiochannel',
      channel = radioChannel,
    })
    Citizen.Wait(3000)
  end
end)
