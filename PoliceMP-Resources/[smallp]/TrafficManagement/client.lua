Citizen.CreateThread(function()
    local player = GetPlayerPed(-1)
    local currentItemIndex = 1
    local selectedItemIndex = 1
    WarMenu.CreateMenu('menu:main', 'Traffic Management')
    WarMenu.SetTitleBackgroundColor('menu:main', 0, 122, 255, 255)
    WarMenu.SetTitleColor('menu:main', 255, 255, 255, 255)

    while true do
        Citizen.Wait(0)

        if WarMenu.IsMenuOpened('menu:main') then
            DisableControlAction(0, 37, true) -- Disable weapon wheel
            DisableControlAction(0, 24, true) -- Disable attack
            DisableControlAction(0, 25, true) -- Disable aim
            DisableControlAction(0, 44, true) -- Disable cover

            if WarMenu.MenuButton('Stop Traffic', 'menu:main') then
                TriggerEvent("stoptraffic")
            elseif WarMenu.Button('Slow Traffic') then
                TriggerEvent("slowtraffic")
            elseif WarMenu.Button('Resume Traffic') then
                TriggerEvent("resumetraffic")
            elseif WarMenu.Button('Re-route Traffic') then
                ExecuteCommand("+zreroute")
                WarMenu.CloseMenu()
            elseif WarMenu.Button('Re-route Clear') then
                ExecuteCommand("+zunroute")
            end

            WarMenu.Display()
        end
    end
end)

RegisterCommand("traffic", function() 
    TriggerServerEvent('traffic:openMenu') 
end)

function ShowNotification(text)
    SetNotificationTextEntry("STRING")
    AddTextComponentSubstringPlayerName(text)
    DrawNotification(false, false)
end

local sz = nil
local tcblip = nil 

RegisterNetEvent('traffic:menuOpened')
AddEventHandler('traffic:menuOpened', function()
    WarMenu.OpenMenu('menu:main')
end)

RegisterNetEvent('slowtraffic')
AddEventHandler('slowtraffic', function()
    local playerPed = PlayerPedId()
    local coords = GetEntityCoords(playerPed)
    local postal = exports['nearest-postal']:getPostal()

    TriggerServerEvent('traffic:slowLog', coords, postal)
    TriggerServerEvent('traffic:slow') 
end)

RegisterNetEvent('stoptraffic')
AddEventHandler('stoptraffic', function()
    local playerPed = PlayerPedId()
    local coords = GetEntityCoords(playerPed)
    local postal = exports['nearest-postal']:getPostal()

    TriggerServerEvent('traffic:stopLog', coords, postal)
    TriggerServerEvent('traffic:stop') 
end)

RegisterNetEvent('resumetraffic')
AddEventHandler('resumetraffic', function()
    local playerPed = PlayerPedId()
    local coords = GetEntityCoords(playerPed)
    local postal = exports['nearest-postal']:getPostal()

    TriggerServerEvent('traffic:resumeLog', coords, postal)
    TriggerServerEvent('traffic:resume') 
end)

local trafficZones = {}

RegisterNetEvent('traffic:update')
AddEventHandler('traffic:update', function(action, coords, playerId)
    if not trafficZones[playerId] then
        trafficZones[playerId] = {sz = nil, tcblip = nil}
    end

    if action == "slow" then
        if trafficZones[playerId].sz == nil then
            trafficZones[playerId].tcblip = AddBlipForRadius(coords, 10.0)
            SetBlipAlpha(trafficZones[playerId].tcblip, 80)
            SetBlipColour(trafficZones[playerId].tcblip, 5)
            trafficZones[playerId].sz = AddSpeedZoneForCoord(coords, 10.0, 5.0, false)
        end
    elseif action == "stop" then
        if trafficZones[playerId].sz == nil then
            trafficZones[playerId].tcblip = AddBlipForRadius(coords, 10.0)
            trafficZones[playerId].sz = AddSpeedZoneForCoord(coords, 10.0, 0.0, false)
            SetBlipAlpha(trafficZones[playerId].tcblip, 80)
            SetBlipColour(trafficZones[playerId].tcblip, 1)
        end
    elseif action == "resume" then
        if trafficZones[playerId].sz ~= nil then
            RemoveSpeedZone(trafficZones[playerId].sz)
            RemoveBlip(trafficZones[playerId].tcblip)
            trafficZones[playerId] = nil
        end
    end
end)

RegisterNetEvent('traffic:notify')
AddEventHandler('traffic:notify', function(message)
    ShowNotification(message)
end)

RegisterNetEvent('traffic:removeZone')
AddEventHandler('traffic:removeZone', function(playerId)
    if trafficZones[playerId] ~= nil then
        if trafficZones[playerId].sz ~= nil then
            RemoveSpeedZone(trafficZones[playerId].sz)
        end
        if trafficZones[playerId].tcblip ~= nil then
            RemoveBlip(trafficZones[playerId].tcblip)
        end
        trafficZones[playerId] = nil
    end
end)

AddSpeedZoneForCoord(236.2, 6565.1, 31.5, 40.0, 20.0, false)
AddSpeedZoneForCoord(161.2, 6544.5, 31.8, 40.0, 10.0, false)
