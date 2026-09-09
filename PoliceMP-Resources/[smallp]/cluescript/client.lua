local clues = {} 
local clueID = 0
local isNuiOpen = false
local activeMarkers = {}
local openedClues = {}

Citizen.CreateThread(function()
    SetNuiFocus(false, false) 
    SendNUIMessage({ action = "hide" }) 
end)

RegisterCommand("+xclue", function()
    TriggerServerEvent("clue:request") 
end)

RegisterNUICallback("submitMessage", function(data, cb)
    local message = data.text
    local playerPed = PlayerPedId()
    local playerCoords = GetEntityCoords(playerPed)
    local playerID = GetPlayerServerId(PlayerId()) 

    local forwardVector = GetEntityForwardVector(playerPed)
    local cluePosition = playerCoords + forwardVector * 1.5

    clueID = clueID + 1
    clues[clueID] = { text = message, coords = cluePosition, creator = playerID }

    TriggerServerEvent("clue:add", clues[clueID])

    TriggerServerEvent("clue:logMessage", message, playerID, discordID)

    SendNUIMessage({ action = "clearInput" })
    SendNUIMessage({ action = "hide" }) 
    SetNuiFocus(false, false)
    isNuiOpen = false

    cb("ok")
end)

RegisterNUICallback("closeNUI", function(data, cb)
    SetNuiFocus(false, false)
    isNuiOpen = false
    cb("ok")
end)

RegisterNUICallback("deleteClue", function(data, cb)
    local clueIDToDelete = data.clueID

    if clues[clueIDToDelete] then
        clues[clueIDToDelete] = nil
        
        TriggerServerEvent("clue:delete", clueIDToDelete)

        SetNuiFocus(false, false)
        isNuiOpen = false
        SendNUIMessage({ action = "hide" })

        cb("ok")
    else
        cb("error")
    end
end)

Citizen.CreateThread(function()
    while true do
        local playerPed = PlayerPedId()
        local playerCoords = GetEntityCoords(playerPed)
        local isNearClue = false  -- Flag to track if the player is near any clue

        for id, clue in pairs(clues) do
            local distance = #(playerCoords - clue.coords)
            if distance < 0.75 then
                isNearClue = true
                if not openedClues[id] then
                    local playerID = GetPlayerServerId(PlayerId())

                    SetNuiFocus(true, true)
                    SendNUIMessage({
                        action = "showMessage",
                        text = clue.text,
                        clueID = id,
                        canDelete = true
                    })
                    openedClues[id] = true
                end
            else
                if openedClues[id] then
                    openedClues[id] = false -- Reset the opened flag when moving away
                end
            end
        end

        if not isNearClue and isNuiOpen then
            SendNUIMessage({ action = "hideMessage" })
        end

        Citizen.Wait(1000)
    end
end)

Citizen.CreateThread(function()
    while true do
        for id, clue in pairs(clues) do
            if not activeMarkers[id] then
                activeMarkers[id] = clue
            end
            
            DrawMarker(32, clue.coords.x, clue.coords.y, clue.coords.z, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.2, 0.2, 0.2, 255, 255, 255, 255, true, true, 2, nil, nil, true)
        end

        for id in pairs(activeMarkers) do
            if not clues[id] then
                activeMarkers[id] = nil -- Remove the marker
            end
        end
        
        Citizen.Wait(0)
    end
end)

AddEventHandler("onResourceStart", function(resourceName)
    if resourceName == GetCurrentResourceName() then
        TriggerServerEvent("clue:load")
    end
end)

RegisterNetEvent("clue:loadClues")
AddEventHandler("clue:loadClues", function(loadedClues)
    clues = loadedClues 
end)

RegisterNetEvent("clue:removeMarker")
AddEventHandler("clue:removeMarker", function(clueID)
    if activeMarkers[clueID] then
        activeMarkers[clueID] = nil 
    end
end)

RegisterNetEvent("clue:addClient")
AddEventHandler("clue:addClient", function(clueID, clueData)
    clues[clueID] = clueData
    activeMarkers[clueID] = clueData 
end)

RegisterNetEvent("clue:openNUI")
AddEventHandler("clue:openNUI", function()
    if not isNuiOpen then 
        SetNuiFocus(true, true) 
        SendNUIMessage({ action = "open" }) 
        isNuiOpen = true 
    end
end)