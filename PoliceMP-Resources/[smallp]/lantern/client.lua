local holdingLantern = false
local prop = nil
local nuiOpen = false

RegisterCommand("lantern", function()
    if not holdingLantern then
        holdingLantern = true
        local ped = PlayerPedId()

        RequestAnimDict("amb@world_human_mobile_film_shocking@male@base")
        while not HasAnimDictLoaded("amb@world_human_mobile_film_shocking@male@base") do
            Wait(100)
        end
        TaskPlayAnim(ped, "amb@world_human_mobile_film_shocking@male@base", "base", 8.0, 8.0, -1, 49, 0, false, false, false)

        local x, y, z = table.unpack(GetEntityCoords(ped))
        prop = CreateObject(GetHashKey("prop_phone_ing_02_lod"), x, y, z + 0.2, true, true, true)
        AttachEntityToEntity(prop, ped, GetPedBoneIndex(ped, 57005), 0.14, 0.08, -0.05, 75.0, 45.0, 150.0, true, true, false, true, 1, true)

        Notify("Press [E] near a player to scan their fingerprint. Press [ESC] to cancel.")
    else
        CancelLanternAction()
    end
end)

CreateThread(function()
    while true do
        Wait(0)

        if holdingLantern then
            if IsControlJustPressed(0, 38) then -- E key
                local ped = PlayerPedId()
                local playerId = GetNearestPlayer()
                if playerId then
                    TriggerServerEvent("lantern:showNUI", GetPlayerServerId(playerId))
                else
                    Notify("No player nearby!")
                end
            end

            if IsControlJustPressed(0, 177) then -- ESC key
                CancelLanternAction()
            end
        end
    end
end)

function CancelLanternAction()
    holdingLantern = false
    local ped = PlayerPedId()
    ClearPedTasks(ped)
    if DoesEntityExist(prop) then
        DeleteObject(prop)
        prop = nil
    end
    Notify("Lantern scanning canceled.")
end

RegisterNUICallback("close", function(data, cb)
    nuiOpen = false
    SetNuiFocus(false, false)
    cb("ok")
end)

RegisterNetEvent("lantern:openUI")
AddEventHandler("lantern:openUI", function()
    SetNuiFocus(true, true)
    SendNUIMessage({ action = "open" })
end)

RegisterNUICallback("submitDetails", function(data, cb)
    SetNuiFocus(false, false)
    TriggerServerEvent("lantern:submitDetails", data) -- Send the data to the server
    cb("ok")
end)


function GetNearestPlayer()
    local players = GetActivePlayers()
    local closestPlayer, closestDistance = nil, 5.0
    local ped = PlayerPedId()
    local pedCoords = GetEntityCoords(ped)

    for _, player in ipairs(players) do
        if player ~= PlayerId() then
            local targetPed = GetPlayerPed(player)
            local targetCoords = GetEntityCoords(targetPed)
            local distance = #(pedCoords - targetCoords)
            if distance < closestDistance then
                closestPlayer = player
                closestDistance = distance
            end
        end
    end

    return closestPlayer
end

function Notify(message)
    SetNotificationTextEntry("STRING")
    AddTextComponentString(message)
    DrawNotification(false, true)
end
