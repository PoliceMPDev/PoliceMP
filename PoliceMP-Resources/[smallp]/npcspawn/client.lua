local spawnedNPCs = {}

RegisterNetEvent('networked_npcs:spawn')
AddEventHandler('networked_npcs:spawn', function(coords)
    local pedModels = {
        -- Male pedestrians
        "a_m_m_farmer_01",
        "a_m_m_business_01",
        "a_m_m_beach_01",
        "a_m_m_fatlatin_01",
        "a_m_m_genfat_01",
        "a_m_m_skater_01",
        "a_m_y_skater_02",
        "a_m_y_business_01",
        "a_m_y_hipster_02",
        "a_m_y_genstreet_01",
        "a_m_y_hiker_01",
        "a_m_y_musclbeac_01",
        "a_m_y_roadcyc_01",
        "a_m_y_runner_01",
        "a_m_y_soucent_01",
        "a_m_y_stbla_01",
        "a_m_y_stwhi_01",
        "a_m_y_vinewood_01",
        "a_m_y_beachvesp_01",
    
        -- Female pedestrians
        "a_f_m_bevhills_01",
        "a_f_m_fatbla_01",
        "a_f_m_genfat_01",
        "a_f_m_skidrow_01",
        "a_f_m_soucentmc_01",
        "a_f_y_beach_01",
        "a_f_y_bevhills_01",
        "a_f_y_business_01",
        "a_f_y_fitness_01",
        "a_f_y_genhot_01",
        "a_f_y_hipster_01",
        "a_f_y_hiker_01",
        "a_f_y_runner_01",
        "a_f_y_tourist_01",
        "a_f_y_vinewood_01",
        "a_f_y_soucent_01",
    
        -- Criminal and gang models
        "g_m_y_ballaeast_01",
        "g_m_y_ballaorig_01",
        "g_m_y_famca_01",
        "g_m_y_famdnf_01",
        "g_m_y_famfor_01",
        "g_m_y_lost_01",
        "g_m_y_lost_02",
        "g_m_y_mexgoon_01",
        "g_m_y_pologoon_01",
        "g_m_y_salvagoon_01",
        "g_m_y_salvagoon_02",
    
        -- Other pedestrians
        "u_m_m_prolsec_01",
        "csb_car3guy1",
        "csb_car3guy2",
        "csb_chef",
        "csb_cop",
        "csb_ramp_hic",
        "csb_stripper_01",
        "u_m_y_baygor",
        "u_m_y_militarybum",
        "u_f_y_comjane",
        "u_f_y_dancer_01",
        "u_f_y_hotposh_01",
        "u_f_y_mistress",
        "u_f_y_poppymich"
    }
    

    for i = 1, 10 do
        Citizen.CreateThread(function()
            local modelHash = GetHashKey(pedModels[math.random(1, #pedModels)])
            RequestModel(modelHash)

            while not HasModelLoaded(modelHash) do
                Citizen.Wait(0)
            end

            local offsetX = math.random(-10, 10)
            local offsetY = math.random(-10, 10)
            local spawnCoords = vector3(coords.x + offsetX, coords.y + offsetY, coords.z)

            local npc = CreatePed(4, modelHash, spawnCoords.x, spawnCoords.y, spawnCoords.z, math.random(0, 360), true, true)

            SetPedCanRagdoll(npc, true)
            SetEntityAsMissionEntity(npc, true, true)
            SetNetworkIdExistsOnAllMachines(NetworkGetNetworkIdFromEntity(npc), true)
            TaskWanderStandard(npc, 10.0, 10)
            table.insert(spawnedNPCs, npc)
            SetModelAsNoLongerNeeded(modelHash)
        end)
    end
end)

RegisterNetEvent('networked_npcs:clear')
AddEventHandler('networked_npcs:clear', function()
    for _, npc in ipairs(spawnedNPCs) do
        if DoesEntityExist(npc) then
            DeleteEntity(npc)
        end
    end
    spawnedNPCs = {}
end)

RegisterNetEvent('networked_npcs:notify')
AddEventHandler('networked_npcs:notify', function(message)
    ShowNotification(message)
end)

function ShowNotification(text)
    SetNotificationTextEntry('STRING')
    AddTextComponentString(text)
    DrawNotification(false, true)
end

RegisterNetEvent('networked_npcs:spawnPartying')
AddEventHandler('networked_npcs:spawnPartying', function(coords)
    local pedModels = {
        -- Male pedestrians
        "a_m_m_farmer_01",
        "a_m_m_business_01",
        "a_m_m_beach_01",
        "a_m_m_fatlatin_01",
        "a_m_m_genfat_01",
        "a_m_m_skater_01",
        "a_m_y_skater_02",
        "a_m_y_business_01",
        "a_m_y_hipster_02",
        "a_m_y_genstreet_01",
        "a_m_y_hiker_01",
        "a_m_y_musclbeac_01",
        "a_m_y_roadcyc_01",
        "a_m_y_runner_01",
        "a_m_y_soucent_01",
        "a_m_y_stbla_01",
        "a_m_y_stwhi_01",
        "a_m_y_vinewood_01",
        "a_m_y_beachvesp_01",
    
        -- Female pedestrians
        "a_f_m_bevhills_01",
        "a_f_m_fatbla_01",
        "a_f_m_genfat_01",
        "a_f_m_skidrow_01",
        "a_f_m_soucentmc_01",
        "a_f_y_beach_01",
        "a_f_y_bevhills_01",
        "a_f_y_business_01",
        "a_f_y_fitness_01",
        "a_f_y_genhot_01",
        "a_f_y_hipster_01",
        "a_f_y_hiker_01",
        "a_f_y_runner_01",
        "a_f_y_tourist_01",
        "a_f_y_vinewood_01",
        "a_f_y_soucent_01",
    
        -- Criminal and gang models
        "g_m_y_ballaeast_01",
        "g_m_y_ballaorig_01",
        "g_m_y_famca_01",
        "g_m_y_famdnf_01",
        "g_m_y_famfor_01",
        "g_m_y_lost_01",
        "g_m_y_lost_02",
        "g_m_y_mexgoon_01",
        "g_m_y_pologoon_01",
        "g_m_y_salvagoon_01",
        "g_m_y_salvagoon_02",
    
        -- Other pedestrians
        "u_m_m_prolsec_01",
        "csb_car3guy1",
        "csb_car3guy2",
        "csb_chef",
        "csb_cop",
        "csb_ramp_hic",
        "csb_stripper_01",
        "u_m_y_baygor",
        "u_m_y_militarybum",
        "u_f_y_comjane",
        "u_f_y_dancer_01",
        "u_f_y_hotposh_01",
        "u_f_y_mistress",
        "u_f_y_poppymich"
    }

    for i = 1, 10 do
        Citizen.CreateThread(function()
            local modelHash = GetHashKey(pedModels[math.random(1, #pedModels)])
            RequestModel(modelHash)

            while not HasModelLoaded(modelHash) do
                Citizen.Wait(0)
            end

            local offsetX = math.random(-10, 10)
            local offsetY = math.random(-10, 10)
            local spawnCoords = vector3(coords.x + offsetX, coords.y + offsetY, coords.z)

            local npc = CreatePed(4, modelHash, spawnCoords.x, spawnCoords.y, spawnCoords.z, math.random(0, 360), true, true)

            SetPedCanRagdoll(npc, true)
            SetEntityAsMissionEntity(npc, true, true)
            SetNetworkIdExistsOnAllMachines(NetworkGetNetworkIdFromEntity(npc), true)
            TaskStartScenarioInPlace(npc, "WORLD_HUMAN_PARTYING", 0, true)

            table.insert(spawnedNPCs, npc)
            SetModelAsNoLongerNeeded(modelHash)
        end)
    end
end)

RegisterNetEvent('networked_npcs:spawnAndEmote')
AddEventHandler('networked_npcs:spawnAndEmote', function(coords, emoteName)
    local pedModels = {
        -- Male pedestrians
        "a_m_m_farmer_01",
        "a_m_m_business_01",
        "a_m_m_beach_01",
        "a_m_m_fatlatin_01",
        "a_m_m_genfat_01",
        "a_m_m_skater_01",
        "a_m_y_skater_02",
        "a_m_y_business_01",
        "a_m_y_hipster_02",
        "a_m_y_genstreet_01",
        "a_m_y_hiker_01",
        "a_m_y_musclbeac_01",
        "a_m_y_roadcyc_01",
        "a_m_y_runner_01",
        "a_m_y_soucent_01",
        "a_m_y_stbla_01",
        "a_m_y_stwhi_01",
        "a_m_y_vinewood_01",
        "a_m_y_beachvesp_01",
    
        -- Female pedestrians
        "a_f_m_bevhills_01",
        "a_f_m_fatbla_01",
        "a_f_m_genfat_01",
        "a_f_m_skidrow_01",
        "a_f_m_soucentmc_01",
        "a_f_y_beach_01",
        "a_f_y_bevhills_01",
        "a_f_y_business_01",
        "a_f_y_fitness_01",
        "a_f_y_genhot_01",
        "a_f_y_hipster_01",
        "a_f_y_hiker_01",
        "a_f_y_runner_01",
        "a_f_y_tourist_01",
        "a_f_y_vinewood_01",
        "a_f_y_soucent_01",
    
        -- Criminal and gang models
        "g_m_y_ballaeast_01",
        "g_m_y_ballaorig_01",
        "g_m_y_famca_01",
        "g_m_y_famdnf_01",
        "g_m_y_famfor_01",
        "g_m_y_lost_01",
        "g_m_y_lost_02",
        "g_m_y_mexgoon_01",
        "g_m_y_pologoon_01",
        "g_m_y_salvagoon_01",
        "g_m_y_salvagoon_02",
    
        -- Other pedestrians
        "u_m_m_prolsec_01",
        "csb_car3guy1",
        "csb_car3guy2",
        "csb_chef",
        "csb_cop",
        "csb_ramp_hic",
        "csb_stripper_01",
        "u_m_y_baygor",
        "u_m_y_militarybum",
        "u_f_y_comjane",
        "u_f_y_dancer_01",
        "u_f_y_hotposh_01",
        "u_f_y_mistress",
        "u_f_y_poppymich"
    }

    for i = 1, 10 do
        Citizen.CreateThread(function()
            local modelHash = GetHashKey(pedModels[math.random(1, #pedModels)])
            RequestModel(modelHash)

            while not HasModelLoaded(modelHash) do
                Citizen.Wait(0)
            end

            local offsetX = math.random(-10, 10)
            local offsetY = math.random(-10, 10)
            local spawnCoords = vector3(coords.x + offsetX, coords.y + offsetY, coords.z)

            local npc = CreatePed(4, modelHash, spawnCoords.x, spawnCoords.y, spawnCoords.z, math.random(0, 360), true, true)

            SetPedCanRagdoll(npc, true)
            SetEntityAsMissionEntity(npc, true, true)
            SetNetworkIdExistsOnAllMachines(NetworkGetNetworkIdFromEntity(npc), true)
            table.insert(spawnedNPCs, npc)

            -- Trigger the emote on the NPC
            Citizen.Wait(100)
            TriggerEvent('rpemotes:playEmoteForNPC', npc, emoteName)

            SetModelAsNoLongerNeeded(modelHash)
        end)
    end
end)

RegisterNetEvent('rpemotes:playEmoteForNPC')
AddEventHandler('rpemotes:playEmoteForNPC', function(npc, emoteName)
    if npc and DoesEntityExist(npc) then
        local npcNetId = NetworkGetNetworkIdFromEntity(npc)
        -- Use the EmoteCommandStart export to apply the emote to the NPC
        exports["rpemotes-reborn"]:EmoteCommandStartForNPC(npcNetId, emoteName)
    end
end)

RegisterNetEvent('networked_npcs:spawnAndSitAnim')
AddEventHandler('networked_npcs:spawnAndSitAnim', function(coords)
    local pedModels = {
        -- Male pedestrians
        "a_m_m_farmer_01",
        "a_m_m_business_01",
        "a_m_m_beach_01",
        "a_m_m_fatlatin_01",
        "a_m_m_genfat_01",
        "a_m_m_skater_01",
        "a_m_y_skater_02",
        "a_m_y_business_01",
        "a_m_y_hipster_02",
        "a_m_y_genstreet_01",
        "a_m_y_hiker_01",
        "a_m_y_musclbeac_01",
        "a_m_y_roadcyc_01",
        "a_m_y_runner_01",
        "a_m_y_soucent_01",
        "a_m_y_stbla_01",
        "a_m_y_stwhi_01",
        "a_m_y_vinewood_01",
        "a_m_y_beachvesp_01",
    
        -- Female pedestrians
        "a_f_m_bevhills_01",
        "a_f_m_fatbla_01",
        "a_f_m_genfat_01",
        "a_f_m_skidrow_01",
        "a_f_m_soucentmc_01",
        "a_f_y_beach_01",
        "a_f_y_bevhills_01",
        "a_f_y_business_01",
        "a_f_y_fitness_01",
        "a_f_y_genhot_01",
        "a_f_y_hipster_01",
        "a_f_y_hiker_01",
        "a_f_y_runner_01",
        "a_f_y_tourist_01",
        "a_f_y_vinewood_01",
        "a_f_y_soucent_01",
    
        -- Criminal and gang models
        "g_m_y_ballaeast_01",
        "g_m_y_ballaorig_01",
        "g_m_y_famca_01",
        "g_m_y_famdnf_01",
        "g_m_y_famfor_01",
        "g_m_y_lost_01",
        "g_m_y_lost_02",
        "g_m_y_mexgoon_01",
        "g_m_y_pologoon_01",
        "g_m_y_salvagoon_01",
        "g_m_y_salvagoon_02",
    
        -- Other pedestrians
        "u_m_m_prolsec_01",
        "csb_car3guy1",
        "csb_car3guy2",
        "csb_chef",
        "csb_cop",
        "csb_ramp_hic",
        "csb_stripper_01",
        "u_m_y_baygor",
        "u_m_y_militarybum",
        "u_f_y_comjane",
        "u_f_y_dancer_01",
        "u_f_y_hotposh_01",
        "u_f_y_mistress",
        "u_f_y_poppymich"
    }

    -- Animation data
    local animDict = "anim@amb@business@bgen@bgen_no_work@"
    local animName = "sit_phone_phoneputdown_idle_nowork"

    -- Load the animation dictionary
    RequestAnimDict(animDict)
    while not HasAnimDictLoaded(animDict) do
        Citizen.Wait(0)
    end

    for i = 1, 5 do
        Citizen.CreateThread(function()
            local modelHash = GetHashKey(pedModels[math.random(1, #pedModels)])
            RequestModel(modelHash)

            while not HasModelLoaded(modelHash) do
                Citizen.Wait(0)
            end

            local offsetX = math.random(-5, 5)
            local offsetY = math.random(-5, 5)
            local spawnCoords = vector3(coords.x + offsetX, coords.y + offsetY, coords.z)

            local npc = CreatePed(4, modelHash, spawnCoords.x, spawnCoords.y, spawnCoords.z, math.random(0, 360), true, true)

            SetPedCanRagdoll(npc, true)
            SetEntityAsMissionEntity(npc, true, true)
            SetNetworkIdExistsOnAllMachines(NetworkGetNetworkIdFromEntity(npc), true)
            table.insert(spawnedNPCs, npc)

            -- Make the NPC play the sitting animation
            TaskPlayAnim(npc, animDict, animName, 8.0, -8.0, -1, 1, 0, false, false, false)

            SetModelAsNoLongerNeeded(modelHash)
        end)
    end
end)

RegisterNetEvent('networked_npcs:courtspawn')
AddEventHandler('networked_npcs:courtspawn', function()
    local seatingPositions = {
        {x = -516.195, y = -201.9091, z = 37.85882, heading = 28.52},
        {x = -517.0638, y = -202.3476, z = 37.85882, heading = 34.47},
        {x = -512.2285, y = -199.5844, z = 37.85882, heading = 30.34},
        {x = -511.472,  y = -199.0264, z = 37.85882, heading = 39.38},
        {x = -510.6573, y = -198.6953, z = 37.85882, heading = 34.09},
        {x = -517.8552, y = -202.7680, z = 37.85882, heading = 38.84},
        {x = -518.8497, y = -200.8890, z = 37.85882, heading = 31.46},
        {x = -518.0554, y = -200.4260, z = 37.85882, heading = 36.22},
        {x = -517.2493, y = -200.1047, z = 37.85882, heading = 30.26},
        {x = -513.4590, y = -197.8143, z = 37.85882, heading = 29.31},
        {x = -512.4936, y = -197.2941, z = 37.85882, heading = 31.99},
        {x = -511.6848, y = -196.9379, z = 37.85882, heading = 34.43},
        {x = -512.7816, y = -195.0866, z = 37.85882, heading = 34.42},
        {x = -513.6214, y = -195.5143, z = 37.85882, heading = 33.20},
        {x = -514.4000, y = -195.9250, z = 37.85882, heading = 29.62},
        {x = -518.2411, y = -198.2264, z = 37.85882, heading = 33.20},
        {x = -519.2205, y = -198.6178, z = 37.85882, heading = 31.20},
        {x = -520.0359, y = -199.1329, z = 37.85882, heading = 24.27},
        {x = -520.8115, y = -197.5287, z = 37.85882, heading = 28.24},
        {x = -520.0045, y = -197.1051, z = 37.85882, heading = 32.20},
        {x = -519.2986, y = -196.5854, z = 37.85882, heading = 35.32},
        {x = -515.2490, y = -194.3734, z = 37.85882, heading = 32.80},
        {x = -514.5964, y = -193.8269, z = 37.85882, heading = 26.95},
        {x = -513.7156, y = -193.3937, z = 37.85882, heading = 33.98},
        {x = -514.6614, y = -191.7726, z = 37.85882, heading = 35.63},
        {x = -515.4675, y = -192.1806, z = 37.85882, heading = 31.02},
        {x = -516.3942, y = -192.7793, z = 37.85882, heading = 32.75},
        {x = -520.2003, y = -194.9377, z = 37.85882, heading = 34.11},
        {x = -521.0604, y = -195.4140, z = 37.85882, heading = 36.52},
        {x = -521.8768, y = -195.6976, z = 37.85882, heading = 31.97}
    }

    local pedModels = {
        -- Male pedestrians
        "a_m_m_farmer_01",
        "a_m_m_business_01",
        "a_m_m_beach_01",
        "a_m_m_fatlatin_01",
        "a_m_m_genfat_01",
        "a_m_m_skater_01",
        "a_m_y_skater_02",
        "a_m_y_business_01",
        "a_m_y_hipster_02",
        "a_m_y_genstreet_01",
        "a_m_y_hiker_01",
        "a_m_y_musclbeac_01",
        "a_m_y_roadcyc_01",
        "a_m_y_runner_01",
        "a_m_y_soucent_01",
        "a_m_y_stbla_01",
        "a_m_y_stwhi_01",
        "a_m_y_vinewood_01",
        "a_m_y_beachvesp_01",

        -- Female pedestrians
        "a_f_m_bevhills_01",
        "a_f_m_fatbla_01",
        "a_f_m_genfat_01",
        "a_f_m_skidrow_01",
        "a_f_m_soucentmc_01",
        "a_f_y_beach_01",
        "a_f_y_bevhills_01",
        "a_f_y_business_01",
        "a_f_y_fitness_01",
        "a_f_y_genhot_01",
        "a_f_y_hipster_01",
        "a_f_y_hiker_01",
        "a_f_y_runner_01",
        "a_f_y_tourist_01",
        "a_f_y_vinewood_01",
        "a_f_y_soucent_01",

        -- Criminal and gang models
        "g_m_y_ballaeast_01",
        "g_m_y_ballaorig_01",
        "g_m_y_famca_01",
        "g_m_y_famdnf_01",
        "g_m_y_famfor_01",
        "g_m_y_lost_01",
        "g_m_y_lost_02",
        "g_m_y_mexgoon_01",
        "g_m_y_pologoon_01",
        "g_m_y_salvagoon_01",
        "g_m_y_salvagoon_02",

        -- Other pedestrians
        "u_m_m_prolsec_01",
        "csb_car3guy1",
        "csb_car3guy2",
        "csb_chef",
        "csb_cop",
        "csb_ramp_hic",
        "csb_stripper_01",
        "u_m_y_baygor",
        "u_m_y_militarybum",
        "u_f_y_comjane",
        "u_f_y_dancer_01",
        "u_f_y_hotposh_01",
        "u_f_y_mistress",
        "u_f_y_poppymich"
    }

    for _, pos in ipairs(seatingPositions) do
        Citizen.CreateThread(function()
            local modelHash = GetHashKey(pedModels[math.random(1, #pedModels)])
            RequestModel(modelHash)
            while not HasModelLoaded(modelHash) do
                Citizen.Wait(0)
            end

            local npc = CreatePed(4, modelHash, pos.x, pos.y, pos.z, pos.heading, true, true)

            SetPedCanRagdoll(npc, true)
            SetEntityAsMissionEntity(npc, true, true)
            SetNetworkIdExistsOnAllMachines(NetworkGetNetworkIdFromEntity(npc), true)

            -- Start the seating scenario at the given position and heading
            TaskStartScenarioAtPosition(npc, "PROP_HUMAN_SEAT_CHAIR", pos.x, pos.y, pos.z, pos.heading, -1, false, true)

            table.insert(spawnedNPCs, npc)
            SetModelAsNoLongerNeeded(modelHash)
        end)
    end
end)

