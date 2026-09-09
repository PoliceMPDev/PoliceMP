Citizen.CreateThread(function()
    local playerPed = PlayerPedId()
    local playerGroup = GetPedRelationshipGroupDefaultHash(playerPed)

    -- List of gang groups
    local npcGroups = {
        "AMBIENT_GANG_LOST",
        "AMBIENT_GANG_SALVA",
        "AMBIENT_GANG_HILLBILLY",
        "AMBIENT_GANG_BALLAS",
        "AMBIENT_GANG_MEXICAN",
        "AMBIENT_GANG_FAMILY",
        "AMBIENT_GANG_MARABUNTE",
        "GANG_1",
        "GANG_2",
        "GANG_9",
        "GANG_10",
        "ARMY"
    }

    while true do
        Citizen.Wait(5000) -- Check every 5 seconds

        -- Ensure NPCs respect the player
        for _, group in ipairs(npcGroups) do
            local groupHash = GetHashKey(group)
            SetRelationshipBetweenGroups(1, groupHash, playerGroup) -- NPCs respect player
            SetRelationshipBetweenGroups(1, playerGroup, groupHash) -- Player respects NPCs
        end

        -- Find and update hostile NPCs
        for ped in EnumeratePeds() do
            if DoesEntityExist(ped) and not IsPedAPlayer(ped) then
                local pedGroup = GetPedRelationshipGroupHash(ped)

                -- If ped belongs to a hostile group, modify their behavior
                for _, group in ipairs(npcGroups) do
                    if pedGroup == GetHashKey(group) then
                        -- Force NPCs to be passive
                        SetPedRelationshipGroupHash(ped, GetHashKey("CIVMALE"))
                        SetPedCombatAttributes(ped, 46, false) -- Disable combat AI
                        SetPedCombatAbility(ped, 0) -- Set lowest combat ability
                        SetPedFleeAttributes(ped, 2, false) -- Prevent them from attacking

                        -- Remove their weapons if needed
                        RemoveAllPedWeapons(ped, true)

                        -- Reset tasks (stop their aggression)
                        -- ClearPedTasksImmediately(ped)
                    end
                end
            end
        end
    end
end)

-- List of blacklisted ped models
local blacklistedPeds = {
    "a_m_m_acult_01",
    "a_m_o_acult_01",
    "a_m_y_acult_01",
    "a_m_y_acult_02",
    "a_m_o_acult_02"
}

-- Function to convert model names to hash keys
local function getModelHashes()
    local hashes = {}
    for _, model in pairs(blacklistedPeds) do
        table.insert(hashes, GetHashKey(model))
    end
    return hashes
end

local pedHashes = getModelHashes()

-- Suppress ped models (Prevents them from spawning naturally)
Citizen.CreateThread(function()
    while true do
        Citizen.Wait(30)
        for _, hash in ipairs(pedHashes) do
            SetPedModelIsSuppressed(hash, true)
        end
    end
end)

-- Delete blacklisted peds if they spawn
Citizen.CreateThread(function()
    while true do
        Citizen.Wait(1000) -- Check every half-second
        for ped in EnumeratePeds() do
            if DoesEntityExist(ped) then
                local model = GetEntityModel(ped)
                if isPedBlacklisted(model) then
                    DeleteEntity(ped)
                end
            end
        end
    end
end)

-- Hook into CreatePed to prevent script-spawned peds
local originalCreatePed = CreatePed
CreatePed = function(pedType, modelHash, x, y, z, heading, isNetwork, thisScriptCheck)
    if isPedBlacklisted(modelHash) then
        --print("[Anti-Ped] Blocked spawning of ped:", modelHash)
        return 0 -- Prevents the ped from being created
    end
    return originalCreatePed(pedType, modelHash, x, y, z, heading, isNetwork, thisScriptCheck)
end

-- Helper function to check if a ped model is blacklisted
function isPedBlacklisted(modelHash)
    for _, hash in ipairs(pedHashes) do
        if modelHash == hash then
            return true
        end
    end
    return false
end

-- Function to enumerate peds safely
function EnumeratePeds()
    return coroutine.wrap(function()
        local handle, ped = FindFirstPed()
        if handle and handle ~= -1 then
            local success
            repeat
                coroutine.yield(ped)
                success, ped = FindNextPed(handle)
            until not success
            EndFindPed(handle)
        end
    end)
end

local blacklistedHelicopters = {
    "frogger",
    "buzzard",
    "buzzard2",
    "supervolito",
    "supervolito2",
    "swift",
    "swift2",
    "valkyrie",
    "valkyrie2"
}

-- Convert model names to hashes once
local helicopterHashes = {}
for _, model in pairs(blacklistedHelicopters) do
    helicopterHashes[GetHashKey(model)] = true
end

local playerSpawnedHelicopters = {}

-- Suppress helicopter models (Prevents NPCs from spawning them)
Citizen.CreateThread(function()
    for hash in pairs(helicopterHashes) do
        SetVehicleModelIsSuppressed(hash, true)
    end
end)

-- Detect if a vehicle is spawned or used by a player
function isVehicleSpawnedByPlayer(vehicle)
    if playerSpawnedHelicopters[vehicle] then return true end
    local driver = GetPedInVehicleSeat(vehicle, -1)
    if driver and IsPedAPlayer(driver) then
        playerSpawnedHelicopters[vehicle] = true
        return true
    end
    return false
end

-- Improved vehicle enumeration function
function EnumerateVehicles()
    return coroutine.wrap(function()
        local handle, vehicle = FindFirstVehicle()
        if handle and handle ~= -1 then
            local success
            repeat
                coroutine.yield(vehicle)
                success, vehicle = FindNextVehicle(handle)
            until not success
            EndFindVehicle(handle)
        end
    end)
end

-- Periodically delete NPC-spawned helicopters
Citizen.CreateThread(function()
    while true do
        Citizen.Wait(500) -- Reduced load by checking every 5 seconds
        for vehicle in EnumerateVehicles() do
            if DoesEntityExist(vehicle) then
                local model = GetEntityModel(vehicle)
                if helicopterHashes[model] and not isVehicleSpawnedByPlayer(vehicle) then
                    DeleteEntity(vehicle)
                end
            end
        end
    end
end)

-- Hook into CreateVehicle to track player-spawned helicopters
local originalCreateVehicle = CreateVehicle
CreateVehicle = function(modelHash, x, y, z, heading, isNetwork, thisScriptCheck)
    if helicopterHashes[modelHash] then
        return 0 -- Prevent NPCs from spawning blacklisted helicopters
    end

    local vehicle = originalCreateVehicle(modelHash, x, y, z, heading, isNetwork, thisScriptCheck)
    if DoesEntityExist(vehicle) then
        playerSpawnedHelicopters[vehicle] = true
    end

    return vehicle
end
