local typingPlayers = {}
local typingAnimation = { "Typing", "Typing.", "Typing..", "Typing..." }
local currentFrame = 1
local isNoclip = false

local typingTimeout = 60 -- seconds
local typingStartTime = 0
local isCurrentlyTyping = false

RegisterNetEvent('chat:opened')
AddEventHandler('chat:opened', function()
    if not isNoclip then
        TriggerServerEvent('chatstatus:updateTyping', true)
        typingStartTime = GetGameTimer()
        isCurrentlyTyping = true
    end
end)

RegisterNetEvent('chat:closed')
AddEventHandler('chat:closed', function()
    TriggerServerEvent('chatstatus:updateTyping', false)
    isCurrentlyTyping = false
    typingStartTime = 0
end)

CreateThread(function()
    while true do
        Wait(5000)
        if isCurrentlyTyping and typingStartTime > 0 then
            local elapsed = (GetGameTimer() - typingStartTime) / 1000
            if elapsed >= typingTimeout then
                TriggerServerEvent('chatstatus:updateTyping', false)
                isCurrentlyTyping = false
                typingStartTime = 0
            end
        end
    end
end)

CreateThread(function()
    while true do
        Wait(1000)
        local playerPed = PlayerPedId()
        local isInVehicle = IsPedInAnyVehicle(playerPed, false)

        if not IsEntityVisible(playerPed) or 
           (not isInVehicle and not IsPedOnFoot(playerPed)) or 
           GetEntityCollisionDisabled(playerPed) then 
            isNoclip = true
        else
            isNoclip = false
        end
    end
end)

CreateThread(function()
    while true do
        Wait(500)
        currentFrame = currentFrame + 1
        if currentFrame > #typingAnimation then
            currentFrame = 1
        end
    end
end)

CreateThread(function()
    while true do
        Wait(0)
        local myServerId = GetPlayerServerId(PlayerId())
        local myCoords = GetEntityCoords(PlayerPedId())
        for _, player in ipairs(GetActivePlayers()) do
            local serverId = GetPlayerServerId(player)

            if serverId ~= myServerId and typingPlayers[serverId] then
                local ped = GetPlayerPed(player)
                local coords = GetEntityCoords(ped)
                local distance = #(myCoords - coords)

                if distance < 8.0 and not IsPedInAnyVehicle(ped, false) then
                    Draw3DText(coords.x, coords.y, coords.z + 1.05, typingAnimation[currentFrame])
                end
            end
        end
    end
end)

RegisterNetEvent('chatstatus:syncTyping')
AddEventHandler('chatstatus:syncTyping', function(typingPlayersUpdate)
    typingPlayers = typingPlayersUpdate
end)

function Draw3DText(x, y, z, text)
    SetDrawOrigin(x, y, z, 0)

    SetTextFont(6)
    SetTextProportional(1)
    SetTextScale(0.27, 0.27)
    SetTextColour(255, 255, 255, 255)
    SetTextOutline()
    SetTextDropShadow(1, 0, 0, 0, 255)
    SetTextEdge(1, 0, 0, 0, 255)
    SetTextCentre(true)

    BeginTextCommandDisplayText("STRING")
    AddTextComponentSubstringPlayerName(text)
    EndTextCommandDisplayText(0.0, 0.0)

    ClearDrawOrigin()
end
