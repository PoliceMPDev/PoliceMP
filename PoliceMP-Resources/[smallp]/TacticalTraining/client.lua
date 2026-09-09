local spawnedNPCs = {}

local spawnPoints3 = {
    vector4(-2021.61, 3205.602, 32.81028, -139.2728),
    vector4(-2009.855, 3183.317, 32.81049, 65.82011),
    vector4(-2007.334, 3199.44, 32.81028, 75.88328),
    vector4(-1990.393, 3234.467, 32.81027, 148.4792),
    vector4(-2002.595, 3223.844, 32.86728, 151.7188),
    vector4(-1994.44, 3214.572, 33.16415, 56.72921),
    vector4(-2015.8673, 3213.5923, 32.8103, 245.3226),
    vector4(-2007.7675, 3229.7471, 33.1734, 282.1349),
    vector4(-1996.1919, 3234.2834, 33.3175, 145.0117),
    vector4(-1997.3350, 3200.3167, 32.8103, 61.8282),
    vector4(-2017.6196, 3205.0928, 32.8103, 319.6143)
}

local spawnPoints4 = {
    vector4(-1855.7518, 2982.9390, 32.8103, 31.7760),
    vector4(-1854.2090, 2985.4414, 32.8103, 244.1814),
    vector4(-1848.2693, 2979.4978, 32.8101, 134.1882),
    vector4(-1844.1669, 2998.7332, 32.8102, 197.8410),
    vector4(-1839.8010, 2982.9663, 32.8100, 266.6373)
}

local spawnPoints5 = {
    vector4(-1821.3737, 2985.2156, 32.8098, 186.2774),
    vector4(-1817.8601, 2975.5771, 32.8099, 18.8708),
    vector4(-1826.1499, 2976.5869, 32.8099, 232.2208),
    vector4(-1828.2323, 2960.1013, 35.8438, 315.7725),
    vector4(-1831.9358, 2961.9822, 35.8438, 267.1209),
    vector4(-1833.6691, 2963.4043, 35.8438, 326.1852),
    vector4(-1821.5732, 2968.6804, 35.8399, 331.1713),
    vector4(-1816.1532, 2981.3853, 35.8449, 133.3853),
    vector4(-1824.2267, 2980.9023, 35.8449, 308.7357),
    vector4(-1807.4282, 2976.5532, 35.8478, 134.4991),
    vector4(-1815.7894, 2977.5769, 35.8438, 321.0198)
}

local spawnPoints6 = {
    vector4(-1806.4426, -1203.2173, 14.3059, 328.1486),
    vector4(-1811.2584, -1221.8555, 19.1657, 309.9269),
    vector4(-1826.9990, -1210.2664, 13.0182, 307.8233),
    vector4(-1850.4194, -1197.2411, 13.0180, 342.8906),
    vector4(-1865.6399, -1209.2256, 13.0180, 337.0388),
    vector4(-1849.5638, -1198.2578, 19.1778, 340.7043),
    vector4(-1845.6553, -1237.1471, 13.0182, 312.6486),
    vector4(-1828.4739, -1250.4481, 13.0182, 322.0798),
    vector4(-1816.3927, -1204.7711, 19.1690, 315.8515)
    
}

local pedModels = {
    's_m_y_blackops_01',
    's_m_y_blackops_02',
    's_m_y_blackops_03',
    'mp_m_bogdangoon',
    's_m_y_swat_01',
    'csb_mweather',
    'mp_m_weapexp_01'
}

local weaponList = {
    "WEAPON_PISTOL",
    "WEAPON_COMBATPISTOL",
    "WEAPON_ASSAULTRIFLE",
    "WEAPON_CARBINERIFLE"
}

-- Create a hostile relationship group
local hostileGroup = GetHashKey("HATES_PLAYER")
AddRelationshipGroup("HOSTILE_PEDS")
SetRelationshipBetweenGroups(5, hostileGroup, GetHashKey("PLAYER"))
SetRelationshipBetweenGroups(5, GetHashKey("PLAYER"), hostileGroup)

local function getRandomSpawnPoints(spawnPoints, count)
    local shuffled = {}
    for i = 1, #spawnPoints do
        table.insert(shuffled, spawnPoints[i])
    end

    for i = #shuffled, 2, -1 do
        local j = math.random(i)
        shuffled[i], shuffled[j] = shuffled[j], shuffled[i]
    end

    local result = {}
    for i = 1, count do
        table.insert(result, shuffled[i])
    end
    return result
end

local function spawnNPCs(spawnPoints)
    local spawnLocations = getRandomSpawnPoints(spawnPoints, 5) -- Pick 5 random locations

    for _, spawn in pairs(spawnLocations) do
        Citizen.CreateThread(function()
            local modelHash = GetHashKey(pedModels[math.random(1, #pedModels)])
            RequestModel(modelHash)

            while not HasModelLoaded(modelHash) do
                Citizen.Wait(0)
            end

            local npc = CreatePed(4, modelHash, spawn.x, spawn.y, spawn.z, spawn.w, true, true)

            if DoesEntityExist(npc) then
                GiveWeaponToPed(npc, GetHashKey(weaponList[math.random(1, #weaponList)]), 9999, false, true)
                SetPedCanRagdoll(npc, true)
                SetNetworkIdExistsOnAllMachines(NetworkGetNetworkIdFromEntity(npc), true)

                -- Set combat behavior
                SetPedCombatAttributes(npc, 44, true) -- Always fight to death
                SetPedCombatAttributes(npc, 11, true)
                SetPedCombatAttributes(npc, 12, true)
                SetPedCombatAbility(npc, 1) -- Professional combat ability
                SetPedCombatMovement(npc, 1) -- Coordinated attack
                SetPedCombatRange(npc, 1) -- Medium range
                SetPedAlertness(npc, 1) -- High alertness
                SetPedRelationshipGroupHash(npc, hostileGroup)
                SetEntityAsMissionEntity(npc, true, true)

                -- Attack all players
                local players = GetActivePlayers()
                for _, player in pairs(players) do
                    local playerPed = GetPlayerPed(player)
                    if playerPed and playerPed ~= 0 then
                        TaskCombatPed(npc, playerPed, 0, 16) -- Aggressive attack
                    end
                end

                table.insert(spawnedNPCs, npc)
            end

            SetModelAsNoLongerNeeded(modelHash)
        end)
    end
end

RegisterNetEvent('gunTraining:spawn')
AddEventHandler('gunTraining:spawn', function()
    spawnNPCs(spawnPoints3)
end)

RegisterNetEvent('gunTraining:spawn2')
AddEventHandler('gunTraining:spawn2', function()
    spawnNPCs(spawnPoints4)
end)

RegisterNetEvent('gunTraining:spawn3')
AddEventHandler('gunTraining:spawn3', function()
    spawnNPCs(spawnPoints5)
end)

RegisterNetEvent('gunTraining:spawn4')
AddEventHandler('gunTraining:spawn4', function()
    spawnNPCs(spawnPoints6)
end)

RegisterNetEvent('gunTraining:clear')
AddEventHandler('gunTraining:clear', function()
    for _, npc in ipairs(spawnedNPCs) do
        if DoesEntityExist(npc) then
            DeleteEntity(npc)
        end
    end
    spawnedNPCs = {}
end)
