local weapons = {"weapon_spithood"}
local spitHoodObject = nil

Citizen.CreateThread(function()
    while true do
        local PlayerPed = GetPlayerPed(-1)
        local PlayerWeapon = GetSelectedPedWeapon(PlayerPed)
        for k,v in pairs(weapons) do
            local Weapon = GetHashKey(v)
            if PlayerWeapon ~= Weapon then goto skip end
            DisableControlAction(0, 24, true)
            DisableControlAction(0, 142, true)
            ::skip::
        end
        Citizen.Wait(0)
    end
end)

local isHoldingSpitHood = false
local nearestPlayer = nil

-- Function to get the nearest player
function GetNearestPlayer()
    local players = GetActivePlayers()
    local closestDistance = -1
    local closestPlayer = -1
    local playerPed = PlayerPedId()
    local playerCoords = GetEntityCoords(playerPed)

    for _, playerId in ipairs(players) do
        local targetPed = GetPlayerPed(playerId)
        if targetPed ~= PlayerPedId() then
            local targetCoords = GetEntityCoords(targetPed)
            local distance = #(playerCoords - targetCoords)
            if closestDistance == -1 or distance < closestDistance then
                closestPlayer = playerId
                closestDistance = distance
            end
        end
    end

    return closestPlayer, closestDistance
end


Citizen.CreateThread(function()
    while true do
        Citizen.Wait(0)
        
        local playerPed = PlayerPedId()
        local playerCoords = GetEntityCoords(playerPed)
        
        local weaponHash = GetSelectedPedWeapon(playerPed)
        local isHoldingSpitHood = (weaponHash == GetHashKey("weapon_spithood"))
        
        if isHoldingSpitHood and IsControlJustPressed(0, 38) then -- E key
            local nearestPlayer, _ = GetNearestPlayer()
            if nearestPlayer ~= -1 then
                local targetPed = GetPlayerPed(nearestPlayer)
                local targetCoords = GetEntityCoords(targetPed)
                local distance = #(playerCoords - targetCoords)
                if distance < 3.0 then
                    TriggerServerEvent("placeSpitHood", GetPlayerServerId(nearestPlayer))
                else
                    -- Notify the player that they are not close enough
                    TriggerEvent("chatMessage", "^1You are not close enough to the player.")
                end
            end
        end
    end
end)



-- Event to add Spit Hood prop to the player's head
RegisterNetEvent("addSpitHood")
AddEventHandler("addSpitHood", function()
    local playerPed = PlayerPedId()
    local spitHoodModel = GetHashKey("mdx_spithood")

    -- Load the Spit Hood model
    RequestModel(spitHoodModel)
    while not HasModelLoaded(spitHoodModel) do
        Wait(500)
    end

    -- Attach the Spit Hood prop to the player's head
    local boneIndex = GetPedBoneIndex(playerPed, 12844) -- SKEL_Head
    spitHoodObject = CreateObject(spitHoodModel, 0.0, 0.0, 0.0, true, true, true)
    AttachEntityToEntity(spitHoodObject, playerPed, boneIndex, 0.07, 0.03, 0.0, 185.0, -90.0, 0.0, true, true, false, true, 1, true)
end)

-- BKINGLOVESCODING

RegisterCommand("removespithood", function(source, args, rawCommand)
    local playerPed = PlayerPedId()
    local playerCoords = GetEntityCoords(playerPed)
    nearestPlayer, _ = GetNearestPlayer()
    if nearestPlayer ~= -1 then
        local targetPed = GetPlayerPed(nearestPlayer)
        local targetCoords = GetEntityCoords(targetPed)
        local distance = #(playerCoords - targetCoords)
        if distance < 3.0 then
            TriggerServerEvent("removeSpitHood", GetPlayerServerId(nearestPlayer))
        else
            -- Notify the player that they are not close enough
            TriggerEvent("chatMessage", "^1You are not close enough to the player.")
        end
    end
end, false)

-- Event to remove Spit Hood prop from the player's head
RegisterNetEvent("removeSpitHood")
AddEventHandler("removeSpitHood", function()
    if spitHoodObject ~= nil then 
        DeleteEntity(spitHoodObject)
        spitHoodObject = nil
    end
end)