local f = false

function openpedmenu()
    f = true

    SendNUIMessage({
        type = "ui",
        status = true,
    })
end

RegisterCommand('+charmenu', function()
    if not f then
        openpedmenu()
    else
        closepedmenu()
    end
end, false)

function closepedmenu()
    f = false

    SetNuiFocus(0, 0)
    SendNUIMessage({
        type = "ui",
        status = false,
    })
end

RegisterNUICallback("showCursor", function(data)
    SetNuiFocus(1, 1)
end)

RegisterNUICallback("exit", function(data)
    closepedmenu()
end)

RegisterNUICallback("spawn", function(data, cb)
    closepedmenu()
    local model = data.ped

    local j1 = PlayerId()
    local p1 = model

    RequestModel(p1)
    while not HasModelLoaded(p1) do
        Wait(100)
    end

    SetPlayerModel(j1, p1)
    local playerPed = GetPlayerPed(j1)
    SetEntityHealth(playerPed, GetEntityMaxHealth(playerPed))
    ClearPedTasksImmediately(playerPed)
    SetModelAsNoLongerNeeded(p1)

    cb('ok')  
end)

function refreshPedModel()
    local playerPed = PlayerPedId()  
    local model = GetEntityModel(playerPed) 

    RequestModel(model)
    while not HasModelLoaded(model) do
        Wait(100) 
    end

    SetPlayerModel(PlayerId(), model)
    Wait(100)
    ClearPedTasksImmediately(playerPed)

    SetModelAsNoLongerNeeded(model)
end

function closePedMenu()
    SetNuiFocus(0, 0)
    SendNUIMessage({
        type = "ui",
        status = false,
    })
end

RegisterNUICallback('exit', function(data, cb)
    closePedMenu()
    cb('ok')
end)

RegisterNUICallback('refreshped', function(data, cb)
    refreshPedModel()  
    cb('ok')  
end)

