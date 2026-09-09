-- List of hydrant models to freeze
local hydrantModels = {
    GetHashKey("prop_fire_hydrant_1"),
    GetHashKey("prop_fire_hydrant_2"),
    GetHashKey("prop_fire_hydrant_4")
}

-- Function to check and freeze hydrants
local function freezeHydrants()
    local hydrants = GetGamePool("CObject") -- Get all objects in the game pool
    for _, hydrant in ipairs(hydrants) do
        local model = GetEntityModel(hydrant)
        -- Check if the object's model is in the list of hydrant models
        if table.contains(hydrantModels, model) then
            FreezeEntityPosition(hydrant, true) -- Freeze the entity in place
        end
    end
end

-- Register the event to freeze hydrants on command from server
RegisterNetEvent('client:freezeHydrants')
AddEventHandler('client:freezeHydrants', freezeHydrants)

-- Freeze hydrants when the player spawns
AddEventHandler('playerSpawned', function()
    freezeHydrants()
end)

-- Periodic loop to freeze hydrants in newly loaded areas
Citizen.CreateThread(function()
    while true do
        Citizen.Wait(1000) -- Check every 2 minutes (adjust as needed)
        freezeHydrants() -- Reapply freezing to hydrants in the area
    end
end)

-- Helper function to check if a table contains a value
table.contains = function(table, element)
    for _, value in pairs(table) do
        if value == element then
            return true
        end
    end
    return false
end
